/*
Copyright 2014 Google Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DriveProxy.Utils;
using Google.Apis.Drive.v2.Data;

namespace DriveProxy.API
{
  partial class DriveService
  {
    private class Journal : IDisposable
    {
      private readonly string _lastLargestChangeId = null;
      private string _lastSyncedRootFolderId;
      private bool _loadedFromDisk;
      private bool _loadedFromWeb;
      private long _loadedLargestChangeId;
      private string _loadedRootFolderId = "";
      private bool _loadingFromDisk;
      private bool _loadingFromWeb;
      private int _processCount;
      private DateTime _syncChangesFromWebDateTime = default(DateTime);
      private SyncState _syncState = SyncState.Unknown;
      private bool _syncingChangesFromWeb;
      private bool _syncingFromWeb;
      private bool _syncingToDisk;
      private ConcurrentDictionary<string, Item> _files;

      private ConcurrentDictionary<string, ConcurrentDictionary<string, Item>> _filesByParent;
      private bool _multiThreaded = true;

      private Mutex _mutex;
      private Mutex _mutexSyncToDisk;

      private ConcurrentDictionary<string, Item> Files
      {
        get { return _files ?? (_files = new ConcurrentDictionary<string, Item>()); }
        set { _files = value; }
      }

      private ConcurrentDictionary<string, ConcurrentDictionary<string, Item>> FilesByParent
      {
        get {
          return _filesByParent ??
                 (_filesByParent = new ConcurrentDictionary<string, ConcurrentDictionary<string, Item>>());
        }
        set { _filesByParent = value; }
      }

      private Mutex Mutex
      {
        get { return _mutex ?? (_mutex = new Mutex()); }
      }

      private bool MultiThreaded
      {
        get { return _multiThreaded; }
        set { _multiThreaded = value; }
      }

      public bool Loading
      {
        get
        {
          if (LoadingFromDisk || LoadingFromWeb)
          {
            return true;
          }

          return false;
        }
      }

      public bool Loaded
      {
        get
        {
          if (LoadedFromDisk && LoadedFromWeb)
          {
            return true;
          }

          return false;
        }
      }

      public bool Syncing
      {
        get
        {
          if (SyncingFromWeb || SyncingChangesFromWeb || SyncingToDisk)
          {
            return true;
          }

          return false;
        }
      }

      public bool Processing
      {
        get
        {
          if (Loading || Syncing)
          {
            return true;
          }

          return false;
        }
      }

      public bool LoadingFromDisk
      {
        get { return _loadingFromDisk; }
      }

      public bool LoadedFromDisk
      {
        get { return _loadedFromDisk; }
      }

      public bool LoadingFromWeb
      {
        get { return _loadingFromWeb; }
      }

      public bool LoadedFromWeb
      {
        get { return _loadedFromWeb; }
      }

      public bool SyncingFromWeb
      {
        get { return _syncingFromWeb; }
      }

      public bool SyncingChangesFromWeb
      {
        get { return _syncingChangesFromWeb; }
      }

      public bool SyncingToDisk
      {
        get { return _syncingToDisk; }
      }

      private Mutex MutexSyncToDisk
      {
        get { return _mutexSyncToDisk ?? (_mutexSyncToDisk = new Mutex()); }
      }

      public void Dispose()
      {
        try
        {
          Factory.Dispose(this);
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      public static Journal Create()
      {
        return Factory.GetInstance();
      }

      public static void Load()
      {
        Factory.Load();
      }

      public static void Refresh()
      {
        Factory.Refresh();
      }

      public static bool HasSynced(bool startSync)
      {
        return Factory.HasSynced(startSync);
      }

      public static bool RemoveItem(string fileId)
      {
        return Factory.RemoveItem(fileId);
      }

      public static void Stop()
      {
        Factory.Stop();
      }

      public static void Cache(File file, List<File> children)
      {
        Factory.Cache(file, children);
      }

      private bool _LoadFromDisk()
      {
        try
        {
          if (LoadedFromDisk)
          {
            return true;
          }

          Mutex.Wait();

          try
          {
            _UpdateProcessCount("_LoadFromDisk", 1);

            if (LoadedFromDisk)
            {
              return true;
            }

            _loadingFromDisk = true;

            try
            {
              if (FileExists(Settings.RootIdFilePath))
              {
                _loadedRootFolderId = System.IO.File.ReadAllText(Settings.RootIdFilePath);
              }

              if (FileExists(Settings.LastChangeIdPath))
              {
                string loadedLargestChangeId = System.IO.File.ReadAllText(Settings.LastChangeIdPath);

                _loadedLargestChangeId = GetLong(loadedLargestChangeId);
              }

              Files = new ConcurrentDictionary<string, Item>();
              FilesByParent = new ConcurrentDictionary<string, ConcurrentDictionary<string, Item>>();

              if (String.IsNullOrEmpty(_loadedRootFolderId) || _loadedLargestChangeId == 0)
              {
                try
                {
                  _loadedRootFolderId = "";
                  _loadedLargestChangeId = 0;
                }
                catch
                {
                }
              }
            }
            finally
            {
              _loadingFromDisk = false;

              _UpdateProcessCount("_LoadFromDisk", -1);
            }

            _loadedFromDisk = true;
          }
          finally
          {
            Mutex.Release();
          }

          return true;
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);

          return false;
        }
      }

      private Item _LoadFromDisk(string fileId)
      {
        try
        {
          string journalFilePath = "";
          Item item = null;

          try
          {
            journalFilePath = Settings.GetJournalFilePath(fileId + ".item", false);

            if (!FileExists(journalFilePath))
            {
              return null;
            }

            DateTime lastWriteTime = GetFileLastWriteTime(journalFilePath);

            using (
              System.IO.FileStream fileStream = OpenFile(journalFilePath,
                                                         System.IO.FileMode.Open,
                                                         System.IO.FileAccess.Read,
                                                         System.IO.FileShare.Read))
            {
              using (var streamReader = new System.IO.StreamReader(fileStream))
              {
                using (Newtonsoft.Json.JsonReader jsonReader = new Newtonsoft.Json.JsonTextReader(streamReader))
                {
                  var jsonSerializer = new Newtonsoft.Json.JsonSerializer();

                  item = (Item)jsonSerializer.Deserialize(jsonReader, typeof (Item));
                }
              }
            }

            if (item == null || item.Status != ItemStatus.Cached)
            {
              throw new Exception("Json parsing failed");
            }

            _AddItem(item);
          }
          catch (Exception exception)
          {
            Log.Error(exception, false, false);

            try
            {
              DeleteFile(journalFilePath);
            }
            catch (Exception exception2)
            {
              Log.Error(exception2, false, false);
            }

            return null;
          }

          Item[] children = null;

          try
          {
            journalFilePath = Settings.GetJournalFilePath(fileId + ".children", false);

            if (!FileExists(journalFilePath))
            {
              return null;
            }

            using (
              System.IO.FileStream fileStream = OpenFile(journalFilePath,
                                                         System.IO.FileMode.Open,
                                                         System.IO.FileAccess.Read,
                                                         System.IO.FileShare.Read))
            {
              using (var streamReader = new System.IO.StreamReader(fileStream))
              {
                using (Newtonsoft.Json.JsonReader jsonReader = new Newtonsoft.Json.JsonTextReader(streamReader))
                {
                  var jsonSerializer = new Newtonsoft.Json.JsonSerializer();

                  children = (Item[])jsonSerializer.Deserialize(jsonReader, typeof (Item[]));
                }
              }
            }

            if (children == null)
            {
              throw new Exception("Json parsing failed");
            }

            foreach (Item child in children)
            {
              _AddItem(child);
            }
          }
          catch (Exception exception)
          {
            Log.Error(exception, false, false);

            try
            {
              DeleteFile(journalFilePath);
            }
            catch (Exception exception2)
            {
              Log.Error(exception2, false, false);
            }

            return null;
          }

          return item;
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);

          return null;
        }
      }

      private void _LoadFromDiskAsync()
      {
        try
        {
          if (MultiThreaded)
          {
            new System.Threading.Thread(_LoadFromDisk_Async).Start();
          }
          else
          {
            _LoadFromDisk_Async();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private void _LoadFromDisk_Async()
      {
        try
        {
          _LoadFromDisk();
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private SyncState _CheckSyncState()
      {
        try
        {
          if (_syncState != SyncState.Unknown)
          {
            return _syncState;
          }

          Mutex.Wait();

          try
          {
            _UpdateProcessCount("_CheckSyncState", 1);

            try
            {
              if (_loadedRootFolderId != RootFolderId)
              {
                _loadedRootFolderId = RootFolderId;
                _loadedLargestChangeId = LargestChangeId;

                Files.Clear();
                FilesByParent.Clear();

                _syncState = SyncState.SyncFromWeb;
              }
              else if (_loadedLargestChangeId != LargestChangeId)
              {
                // used as StartChangeId in SyncChangesFromWeb
                _largestChangeId = _loadedLargestChangeId;

                _syncState = SyncState.SyncChangesFromWeb;
              }
              else
              {
                _syncState = SyncState.SyncedToWeb;
              }
            }
            finally
            {
              _UpdateProcessCount("_CheckSyncState", -1);
            }
          }
          finally
          {
            Mutex.Release();
          }

          return _syncState;
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);

          return _syncState;
        }
      }

      private bool _LoadFromWeb()
      {
        try
        {
          if (LoadedFromWeb)
          {
            return true;
          }

          if (!_LoadFromDisk())
          {
            Log.Error("Cannot load Journal from web - Could not load Journal from disk.");
            return false;
          }

          if (!IsSignedIn)
          {
            Log.Error("Cannot load Journal from web - Not signed in.");
            return false;
          }

          if (LoadingFromWeb)
          {
            Log.Error("Cannot load Journal from web - Already loading from web.");
            return false;
          }

          Mutex.Wait();

          try
          {
            _UpdateProcessCount("_LoadFromWeb", 1);

            _loadingFromWeb = true;

            try
            {
              SyncState syncState = _CheckSyncState();

              if (syncState == SyncState.SyncFromWeb)
              {
                //if (_SyncFromWeb(RootFolderId, true) == null)
                //{
                //    Log.Error("Cannot load Journal from web - Could not sync from web.");
                //    return false;
                //}
              }
              else if (syncState == SyncState.SyncChangesFromWeb)
              {
                //if (!_SyncChangesFromWeb())
                //{
                //    Log.Error("Cannot load Journal from web - Could not sync changes from web.");
                //    return false;
                //}
              }
            }
            finally
            {
              _loadingFromWeb = false;

              _UpdateProcessCount("_LoadFromWeb", -1);
            }

            _loadedFromWeb = true;
            _syncState = SyncState.SyncedToWeb;
          }
          finally
          {
            Mutex.Release();
          }

          return true;
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);

          return false;
        }
      }

      private void _LoadFromWebAsync()
      {
        try
        {
          if (MultiThreaded)
          {
            new System.Threading.Thread(_LoadFromWeb_Async).Start();
          }
          else
          {
            _LoadFromWeb_Async();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private void _LoadFromWeb_Async()
      {
        try
        {
          _LoadFromWeb();
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private Item _SyncFromWeb(string fileId, bool getChildren)
      {
        try
        {
          if (!IsSignedIn)
          {
            Log.Error("Cannot sync Journal from web - Not signed in.");
            return null;
          }

          Mutex.Wait();

          try
          {
            _UpdateProcessCount("_SyncFromWeb", 1);

            _syncingFromWeb = true;

            Item item = null;

            try
            {
              fileId = GetFileId(fileId);

              File file = _GetFile(fileId);

              if (file == null)
              {
                Log.Error("Cannot sync Journal from web - GetFile returned null.");
                return null;
              }

              List<File> children = null;

              if (getChildren)
              {
                children = _ListFiles(fileId, true);
              }

              item = _SyncFromData(file, children);
            }
            finally
            {
              _syncingFromWeb = false;

              _UpdateProcessCount("_SyncFromWeb", -1);
            }

            return item;
          }
          finally
          {
            Mutex.Release();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);

          return null;
        }
      }

      private Item _SyncFromData(File file, List<File> children = null)
      {
        try
        {
          Mutex.Wait();

          try
          {
            _UpdateProcessCount("_SyncFromFile", 1);

            Item item = null;

            try
            {
              _RemoveItem(file.Id);

              item = Files.GetOrAdd(file.Id, new Item());

              item.Status = ItemStatus.Pending;

              _BindToItem(item, file);

              if (!IsFolder(item.File))
              {
                item.Status = ItemStatus.Cached;
              }
              else if (children != null)
              {
                foreach (File child in children)
                {
                  _AddItem(child);
                }

                item.Status = ItemStatus.Cached;

                List<Item> itemChildren = GetChildrenItems(file.Id);

                _SyncToDiskAsync(item, itemChildren);
              }
            }
            finally
            {
              _UpdateProcessCount("_SyncFromFile", -1);
            }

            _loadedFromWeb = true;
            _syncState = SyncState.SyncedToWeb;

            return item;
          }
          finally
          {
            Mutex.Release();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);

          return null;
        }
      }

      private void _SyncFromDataAsync(Item item, List<Item> children)
      {
        try
        {
          if (MultiThreaded)
          {
            new System.Threading.Thread(_SyncFromData_Async).Start(new object[] {item, children});
          }
          else
          {
            _SyncFromData_Async(new object[] {item, children});
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private void _SyncFromData_Async(object parameter)
      {
        try
        {
          var args = (object[])parameter;
          var file = (File)args[0];
          var children = (List<File>)args[1];

          _SyncFromData(file, children);
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private bool _SyncChangesFromWeb()
      {
        try
        {
          if (!LoadingFromWeb && !LoadedFromWeb)
          {
            Log.Error("Cannot sync Journal web changes - Cannot load from web.");
            return false;
          }

          if (!IsSignedIn)
          {
            Log.Error("Cannot sync Journal web changes - Not signed in.");
            return false;
          }

          Mutex.Wait();

          try
          {
            _UpdateProcessCount("_SyncChangesFromWeb", 1);

            _syncingChangesFromWeb = true;

            try
            {
              TimeSpan timeSpan = DateTime.Now.Subtract(_syncChangesFromWebDateTime);

              if (timeSpan.TotalSeconds < 15)
              {
                Log.Information("Waiting to sync Journal (" + timeSpan.TotalSeconds + " seconds since last update)");
                return true;
              }

              long largestChangeId = 0;

              List<Change> changes = _ListChanges(_largestChangeId, out largestChangeId);

              var changedItems = new Dictionary<string, Item>();
              var changedParentIds = new List<string>();

              foreach (Change change in changes)
              {
                if (change.Deleted.HasValue && change.Deleted.Value)
                {
                  _RemoveItem(change.FileId);
                  _RemoveFile(change.FileId);
                }

                string parentId = GetParentId(change.File);

                if (!Files.ContainsKey(parentId))
                {
                  continue;
                }
                if (!IsTrashed(change.File))
                {
                  Item item = _AddItem(change.File);

                  if (IsFolder(item.File) && !changedItems.ContainsKey(item.File.Id))
                  {
                    changedItems.Add(item.File.Id, item);
                  }
                }
                else
                {
                  _RemoveItem(change.FileId);
                  _RemoveFile(change.FileId);
                }

                if (!String.IsNullOrEmpty(parentId) && !changedParentIds.Contains(parentId))
                {
                  changedParentIds.Add(parentId);
                }
              }

              foreach (string changedParentId in changedParentIds)
              {
                if (!changedItems.ContainsKey(changedParentId))
                {
                  _UpdateFile(changedParentId);
                }
              }

              foreach (Item changedItem in changedItems.Values)
              {
                _UpdateFile(changedItem);
              }

              _largestChangeId = largestChangeId;
              _syncChangesFromWebDateTime = DateTime.Now;
            }
            finally
            {
              _syncingChangesFromWeb = false;

              _UpdateProcessCount("_SyncChangesFromWeb", -1);
            }
          }
          finally
          {
            Mutex.Release();
          }

          return true;
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);

          return false;
        }
      }

      private void _UpdateFile(string fileId)
      {
        try
        {
          Item item = null;

          if (Files.TryGetValue(fileId, out item))
          {
            _UpdateFile(item);
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      private void _UpdateFile(Item item)
      {
        try
        {
          if (item.Status == ItemStatus.Pending)
          {
            return;
          }

          List<Item> children = GetChildrenItems(item.File.Id);

          _SyncToDiskAsync(item, children);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      private bool _SyncToDisk(Item item, List<Item> children)
      {
        try
        {
          if (_cancelProcessing)
          {
            return false;
          }

          if (String.IsNullOrEmpty(RootFolderId))
          {
            Log.Error("Cannot sync Journal to disk - RootFolderId is blank.");
            return false;
          }

          MutexSyncToDisk.Wait();

          try
          {
            _UpdateProcessCount("_SyncToDisk", 1);

            _syncingToDisk = true;

            try
            {
              if (_lastSyncedRootFolderId == null || _lastSyncedRootFolderId != RootFolderId)
              {
                System.IO.File.WriteAllText(Settings.RootIdFilePath, RootFolderId);

                _lastSyncedRootFolderId = RootFolderId;
              }

              if (_lastLargestChangeId == null || _lastLargestChangeId != RootFolderId)
              {
                System.IO.File.WriteAllText(Settings.LastChangeIdPath, LargestChangeId.ToString());

                _lastSyncedRootFolderId = RootFolderId;
              }

              string journalFilePath = Settings.GetJournalFilePath(item.File.Id + ".item");

              using (
                System.IO.FileStream fileStream = OpenFile(journalFilePath,
                                                           System.IO.FileMode.Create,
                                                           System.IO.FileAccess.Write,
                                                           System.IO.FileShare.Write))
              {
                using (var streamWriter = new System.IO.StreamWriter(fileStream))
                {
                  using (Newtonsoft.Json.JsonWriter jsonWriter = new Newtonsoft.Json.JsonTextWriter(streamWriter))
                  {
                    jsonWriter.Formatting = Newtonsoft.Json.Formatting.None;

                    var jsonSerializer = new Newtonsoft.Json.JsonSerializer();

                    jsonSerializer.Serialize(jsonWriter, item);
                  }
                }
              }

              System.IO.File.SetCreationTime(journalFilePath, DateTime.Now);

              journalFilePath = Settings.GetJournalFilePath(item.File.Id + ".children");

              using (
                System.IO.FileStream fileStream = OpenFile(journalFilePath,
                                                           System.IO.FileMode.Create,
                                                           System.IO.FileAccess.Write,
                                                           System.IO.FileShare.Write))
              {
                using (var streamWriter = new System.IO.StreamWriter(fileStream))
                {
                  using (Newtonsoft.Json.JsonWriter jsonWriter = new Newtonsoft.Json.JsonTextWriter(streamWriter))
                  {
                    jsonWriter.Formatting = Newtonsoft.Json.Formatting.None;

                    var jsonSerializer = new Newtonsoft.Json.JsonSerializer();

                    jsonSerializer.Serialize(jsonWriter, children);
                  }
                }
              }

              System.IO.File.SetCreationTime(journalFilePath, DateTime.Now);
            }
            finally
            {
              _syncingToDisk = false;

              _UpdateProcessCount("_SyncToDisk", -1);
            }
          }
          finally
          {
            MutexSyncToDisk.Release();
          }

          return true;
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);

          return false;
        }
      }

      private void _SyncToDiskAsync(Item item, List<Item> children)
      {
        try
        {
          if (_cancelProcessing)
          {
            return;
          }

          if (MultiThreaded)
          {
            new System.Threading.Thread(_SyncToDisk_Async).Start(new object[] {item, children});
          }
          else
          {
            _SyncToDisk_Async(new object[] {item, children});
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private void _SyncToDisk_Async(object parameter)
      {
        try
        {
          if (_cancelProcessing)
          {
            return;
          }

          var args = (object[])parameter;
          var item = (Item)args[0];
          var children = (List<Item>)args[1];

          _SyncToDisk(item, children);
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      public void UpdateCachedData()
      {
        try
        {
          _syncChangesFromWebDateTime = default(DateTime);
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      public File GetCachedFile(string fileId, bool getChildren, out List<File> children)
      {
        children = null;

        try
        {
          if (!_LoadFromWeb())
          {
            Log.Error("Cannot get Journal cached file - Cannot load from web.");
            return null;
          }

          Mutex.Wait();

          try
          {
            _UpdateProcessCount("GetCachedFile", 1);

            try
            {
              fileId = GetFileId(fileId);

              Item item = null;

              Files.TryGetValue(fileId, out item);

              bool isLoaded = false;

              if (item == null || item.Status == ItemStatus.Pending)
              {
                item = _LoadFromDisk(fileId);

                if (item != null && item.Status == ItemStatus.Cached)
                {
                  if (!_SyncChangesFromWeb())
                  {
                    Log.Error("Cannot get Journal cached file - Cannot sync web changes.");
                    return null;
                  }

                  isLoaded = true;
                }
              }

              if (!isLoaded)
              {
                if (item == null)
                {
                  item = _SyncFromWeb(fileId, getChildren);

                  if (item == null)
                  {
                    Log.Error("Cannot get Journal cached file - Cannot sync from web.");
                    return null;
                  }
                }
                else if (item.Status == ItemStatus.Pending)
                {
                  item = _SyncFromWeb(fileId, getChildren);

                  if (item == null)
                  {
                    Log.Error("Cannot get Journal cached file - Cannot sync from web.");
                    return null;
                  }
                }
                else if (item.Status == ItemStatus.Cached)
                {
                  if (!_SyncChangesFromWeb())
                  {
                    Log.Error("Cannot get Journal cached file - Cannot sync web changes.");
                    return null;
                  }
                }
              }

              if (IsFolder(item.File) && getChildren)
              {
                children = GetChildrenFiles(fileId);
              }

              return item.File;
            }
            finally
            {
              _UpdateProcessCount("GetCachedFile", -1);
            }
          }
          finally
          {
            Mutex.Release();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);

          return null;
        }
      }

      private Item _AddItem(File file)
      {
        try
        {
          Item item = Files.GetOrAdd(file.Id, new Item());

          _BindToItem(item, file);

          return item;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      private void _AddItem(Item item)
      {
        try
        {
          Files.GetOrAdd(item.File.Id, item);

          _BindToItem(item, item.File);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      private void _BindToItem(Item item, File file)
      {
        try
        {
          if (item.File != null && item.File.Parents != null && file.Parents != null)
          {
            if (item.File.Parents.Count > 0)
            {
              if (file.Parents.Count == 0 || file.Parents[0].Id != item.File.Parents[0].Id)
              {
                _RemoveFromParent(item);
              }
            }
          }

          item.Value = file;

          _AddToParent(item);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      private bool _AddToParent(Item item)
      {
        try
        {
          if (item.File == null || item.File.Parents == null)
          {
            return false;
          }

          if (item.File.Parents.Count > 0)
          {
            ParentReference parent = item.File.Parents[0];

            ConcurrentDictionary<string, Item> itemParent = FilesByParent.GetOrAdd(parent.Id,
                                                                                    new ConcurrentDictionary
                                                                                      <string, Item>());

            itemParent.GetOrAdd(item.File.Id, item);
          }

          return true;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return false;
        }
      }

      private static bool _RemoveFile(string fileId)
      {
        try
        {
          if (!String.IsNullOrEmpty(fileId))
          {
            string filePath = Settings.GetJournalFilePath(fileId + ".item", false);

            DeleteFile(filePath);

            filePath = Settings.GetJournalFilePath(fileId + ".children", false);

            DeleteFile(filePath);
          }

          return true;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return false;
        }
      }

      private bool _RemoveItem(string fileId)
      {
        string parentId = null;

        return _RemoveItem(fileId, ref parentId);
      }

      private bool _RemoveItem(string fileId, ref string parentId)
      {
        try
        {
          parentId = null;

          Item item = null;

          if (Files.TryRemove(fileId, out item))
          {
            _RemoveFromParent(item, ref parentId);
          }

          return true;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return false;
        }
      }

      private bool _RemoveFromParent(Item item)
      {
        string parentId = null;

        return _RemoveFromParent(item, ref parentId);
      }

      private bool _RemoveFromParent(Item item, ref string parentId)
      {
        try
        {
          parentId = null;

          if (item.File == null)
          {
            return false;
          }

          if (item.File.Parents.Count > 0)
          {
            ParentReference parent = item.File.Parents[0];
            ConcurrentDictionary<string, Item> itemParent = null;

            if (FilesByParent.TryGetValue(parent.Id, out itemParent))
            {
              Item itemTemp = null;

              itemParent.TryRemove(item.File.Id, out itemTemp);

              parentId = parent.Id;
            }
          }

          return true;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return false;
        }
      }

      private ConcurrentDictionary<string, Item> GetFilesByParent(string fileId)
      {
        try
        {
          ConcurrentDictionary<string, Item> itemChildren = null;

          FilesByParent.TryGetValue(fileId, out itemChildren);

          return itemChildren;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      private List<Item> GetChildrenItems(string fileId)
      {
        try
        {
          ConcurrentDictionary<string, Item> itemChildren = GetFilesByParent(fileId);

          var children = new List<Item>();

          if (itemChildren != null)
          {
            foreach (Item itemChild in itemChildren.Values)
            {
              if (!IsTrashed(itemChild.File))
              {
                children.Add(itemChild);
              }
            }
          }

          return children;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      private List<File> GetChildrenFiles(string fileId)
      {
        try
        {
          List<Item> itemChildren = GetChildrenItems(fileId);

          var children = new List<File>();

          if (itemChildren != null)
          {
            foreach (Item itemChild in itemChildren)
            {
              children.Add(itemChild.File);
            }
          }

          return children;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      private void _UpdateProcessCount(string functionName, int processCount)
      {
        try
        {
          if (Settings.LogLevel == LogType.All)
          {
            _processCount += processCount;

            if (_processCount < 0)
            {
              Debugger.Break();

              Log.Error("PROCESS !ERROR! " + functionName + " - ProcessCount + " + processCount + " = " + _processCount);
            }
            else
            {
              Log.Information("Process " + functionName + " - ProcessCount + " + processCount + " = " + _processCount);
            }
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private class Factory
      {
        private static Journal _journal;

        private static Mutex _mutex;

        private static Journal Journal
        {
          get { return _journal ?? (_journal = new Journal()); }
          set { _journal = value; }
        }

        private static Mutex Mutex
        {
          get { return _mutex ?? (_mutex = new Mutex()); }
        }

        public static Journal GetInstance()
        {
          try
          {
            return Journal;
          }
          catch (Exception exception)
          {
            Log.Error(exception);

            return null;
          }
        }

        public static void Load()
        {
          try
          {
            Journal._LoadFromDiskAsync();
          }
          catch (Exception exception)
          {
            Log.Error(exception, false);
          }
        }

        public static void Dispose(Journal instance)
        {
        }

        public static void Refresh()
        {
          try
          {
            Mutex.Wait();

            try
            {
              _rootFolderId = "";
              _largestChangeId = 0;

              if (Journal != null)
              {
                Journal.Dispose();
                Journal = null;
              }
            }
            finally
            {
              Mutex.Release();
            }
          }
          catch (Exception exception)
          {
            Log.Error(exception, false);
          }
        }

        public static bool HasSynced(bool startSync)
        {
          try
          {
            Log.Information("Checking HasSynced");

            SyncState syncState = Journal._CheckSyncState();

            if (syncState == SyncState.SyncFromWeb)
            {
              Log.Information("Starting Sync From Web");

              if (startSync)
              {
                Journal._LoadFromWebAsync();
              }
              else
              {
                Log.Information("Bypassing Sync (sync not requested)");
              }
            }
            else if (syncState == SyncState.SyncChangesFromWeb)
            {
              Log.Information("Starting Sync Changes From Web");

              Journal._LoadFromWeb();

              return true;
            }
            else if (syncState == SyncState.SyncedToWeb)
            {
              Log.Information("Already Synced To Web");

              return true;
            }

            return false;
          }
          catch (Exception exception)
          {
            Log.Error(exception, false);

            return false;
          }
        }

        public static bool RemoveItem(string fileId)
        {
          try
          {
            string parentId = null;

            if (!Journal._RemoveItem(fileId, ref parentId))
            {
              return false;
            }

            _RemoveFile(fileId);

            Journal._UpdateFile(parentId);

            return true;
          }
          catch (Exception exception)
          {
            Log.Error(exception, false);

            return false;
          }
        }

        public static void Stop()
        {
          try
          {
            _journal = new Journal();
          }
          catch (Exception exception)
          {
            Log.Error(exception, false, false);
          }
        }

        public static void Cache(File file, List<File> children)
        {
          try
          {
            Journal._SyncFromData(file, children);
          }
          catch (Exception exception)
          {
            Log.Error(exception, false);
          }
        }
      }

      [Serializable]
      private class Item
      {
        public Item()
        {
          Status = ItemStatus.Pending;
          Value = null;
        }

        public ItemStatus Status { get; set; }
        public File Value { get; set; }

        public File File
        {
          get { return Value; }
        }

        public override string ToString()
        {
          if (File != null)
          {
            return File.Title;
          }

          return "null";
        }
      }

      [Flags]
      private enum ItemStatus
      {
        Unknown = 0,
        Pending = 1,
        Cached = 2,
        DoesNotExist = 3
      }

      private enum SyncState
      {
        Unknown,
        SyncFromWeb,
        SyncChangesFromWeb,
        SyncedToWeb
      }
    }
  }
}
