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
using System.Collections.Generic;
using DriveProxy.Utils;

namespace DriveProxy.API
{
  partial class DriveService
  {
    private class UploadFileWatcher : IDisposable
    {
      protected System.Threading.Thread LockedItemThread;
      protected System.IO.FileSystemWatcher _FileSystemWatcher;

      protected string _FolderPath = "";
      protected List<Item> _Items;
      protected Mutex _LockedItemMutex;
      protected List<Item> _LockedItems;
      protected Mutex _Mutex;
      protected int _ProcessCount;

      protected UploadFileWatcher()
      {
      }

      public string FolderPath
      {
        get { return _FolderPath; }
      }

      public bool Enabled
      {
        get
        {
          if (_FileSystemWatcher != null)
          {
            return _FileSystemWatcher.EnableRaisingEvents;
          }

          return false;
        }
      }

      public bool Processing
      {
        get
        {
          if (ProcessCount > 0)
          {
            return true;
          }

          return false;
        }
      }

      public int ProcessCount
      {
        get { return _ProcessCount; }
      }

      protected List<Item> Items
      {
        get { return _Items ?? (_Items = new List<Item>()); }
      }

      protected Mutex Mutex
      {
        get { return _Mutex ?? (_Mutex = new Mutex()); }
      }

      protected List<Item> LockedItems
      {
        get { return _LockedItems ?? (_LockedItems = new List<Item>()); }
      }

      protected Mutex LockedItemMutex
      {
        get
        {
          if (_LockedItemMutex == null)
          {
            _LockedItemMutex = new Mutex();
          }

          return Mutex;
        }
      }

      public void Dispose()
      {
        try
        {
          Log.Information("Disposing UploadFileWatcher");
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      public static UploadFileWatcher Create()
      {
        return Factory.GetInstance();
      }

      public void Start()
      {
        try
        {
          Log.Information("Attempting to start UploadFileWatcher");

          if (_FileSystemWatcher == null)
          {
            Log.Information("Creating UploadFileWatcher");

            _FileSystemWatcher = new System.IO.FileSystemWatcher();

            _FileSystemWatcher.IncludeSubdirectories = true;

            _FileSystemWatcher.Changed += _FileSystemWatcher_Changed;
            _FileSystemWatcher.Created += _FileSystemWatcher_Created;
            _FileSystemWatcher.Deleted += _FileSystemWatcher_Deleted;
            _FileSystemWatcher.Error += _FileSystemWatcher_Error;
            _FileSystemWatcher.Renamed += _FileSystemWatcher_Renamed;
          }

          if (Enabled)
          {
            if (String.Equals(_FolderPath, Settings.LocalGoogleDriveData, StringComparison.CurrentCultureIgnoreCase))
            {
              Log.Information("UploadFileWatcher is already watching folder " + _FolderPath);
              return;
            }

            Stop();
          }

          _FolderPath = Settings.LocalGoogleDriveData;

          _FileSystemWatcher.Path = _FolderPath;

          _FileSystemWatcher.EnableRaisingEvents = true;

          Log.Information("UploadFileWatcher has started watching folder " + _FolderPath);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void Stop()
      {
        try
        {
          Log.Information("Stopping UploadFileWatcher");

          if (_FileSystemWatcher != null)
          {
            _FileSystemWatcher.EnableRaisingEvents = false;
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private void _FileSystemWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
      {
        try
        {
          ProcessFile(e.FullPath, true);
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private void _FileSystemWatcher_Created(object sender, System.IO.FileSystemEventArgs e)
      {
        try
        {
          _FileSystemWatcher_Changed(sender, e);
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private void _FileSystemWatcher_Deleted(object sender, System.IO.FileSystemEventArgs e)
      {
        try
        {
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private void _FileSystemWatcher_Error(object sender, System.IO.ErrorEventArgs e)
      {
        try
        {
          Debugger.Break();

          Exception exception = e.GetException();

          if (exception == null || String.IsNullOrEmpty(exception.Message))
          {
            Log.Information("Error processing UploadFileWatcher - Unknown");
          }
          else
          {
            Log.Information("Error processing UploadFileWatcher - " + exception.Message);
          }

          Start();
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private void _FileSystemWatcher_Renamed(object sender, System.IO.RenamedEventArgs e)
      {
        try
        {
          _FileSystemWatcher_Changed(sender, e);
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      protected bool ProcessFile(string fullPath, bool handleLockedItems)
      {
        try
        {
          Mutex.Wait();

          try
          {
            string folderPath = System.IO.Path.GetDirectoryName(fullPath);
            string rootPath = System.IO.Path.GetDirectoryName(folderPath);

            if (!String.Equals(rootPath, FolderPath, StringComparison.CurrentCultureIgnoreCase))
            {
              return false;
            }

            Log.Information("UploadFileWatcher attempting to process file " + fullPath);

            string filePath = fullPath;
            string fileExtension = System.IO.Path.GetExtension(filePath);

            if (String.Equals(".download", fileExtension, StringComparison.CurrentCultureIgnoreCase))
            {
              Log.Information("File " + fullPath + " is downloading.");
              return false;
            }
            int itemIndex = -1;

            foreach (Item item in Items)
            {
              itemIndex++;

              if (String.Equals(item.FilePath, filePath, StringComparison.CurrentCultureIgnoreCase))
              {
                DateTime lastWriteTime = GetFileLastWriteTime(filePath);

                if (IsDateTimeEqual(item.LastWriteTime, lastWriteTime))
                {
                  Log.Information("File " + fullPath + " last write time " + lastWriteTime +
                                  " has not changed since the last time it was processed.");
                  return false;
                }
                else if (item.Status == DriveService.Stream.StatusType.Starting)
                {
                  Log.Warning("File " + fullPath + " is starting uploading.");
                  return false;
                }
                else if (item.Status == DriveService.Stream.StatusType.Processing)
                {
                  Log.Warning("File " + fullPath + " is uploading " + item.PercentCompleted + "% completed.");
                  return false;
                }
                else if (item.Status == DriveService.Stream.StatusType.Cancelling)
                {
                  Log.Warning("File " + fullPath + " is cancelling uploading.");
                  return false;
                }

                item.Status = DriveService.Stream.StatusType.NotStarted;
                item.PercentCompleted = 0;

                break;
              }
            }

            if (!DriveService.IsSignedIn)
            {
              Log.Warning("Drive service is not signed in.");
              return false;
            }

            using (DriveService driveService = DriveService.Create())
            {
              string fileId = System.IO.Path.GetFileName(folderPath);
              FileInfo fileInfo = driveService.GetFile(fileId);

              if (fileInfo != null)
              {
                FileInfoStatus fileInfoStatus = GetFileInfoStatus(fileInfo);
                DriveService.Stream stream = null;

                filePath = fileInfo.FilePath;

                if (fileInfoStatus != FileInfoStatus.ModifiedOnDisk)
                {
                  Log.Information("File " + fileId + " is not modified on disk");
                  return true;
                }
                else if (fileInfo.IsGoogleDoc)
                {
                  Log.Information("File " + fileId + " is a google doc");
                  return true;
                }
                else if (IsStreamLocked(fileInfo.Id, fileInfo.FilePath, ref stream))
                {
                  Log.Warning("Stream " + fileId + " is locked (type=" + stream.Type + ")");
                }
                else if (IsFileLocked(fileInfo.Id, fileInfo.FilePath))
                {
                  Log.Warning("File " + fileId + " is locked");
                }
                else if (!CanOpenFile(filePath, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite, 1))
                {
                  Log.Warning("File " + fullPath + " is being used by another application.");

                  if (handleLockedItems)
                  {
                    AddLockedItem(filePath);
                  }
                }
                else
                {
                  Log.Information("Attempting to upload file " + fileId);

                  if (handleLockedItems)
                  {
                    RemoveLockedItem(filePath);
                  }

                  Item item = null;

                  if (itemIndex >= 0)
                  {
                    item = Items[itemIndex];
                  }
                  else
                  {
                    item = new Item(this);

                    item.FilePath = filePath;

                    Items.Add(item);
                  }

                  item.UploadStream = new DriveService.UploadStream();

                  item.UploadStream.OnProgressStarted += item.UploadStream_ProgressStarted;
                  item.UploadStream.OnProgressChanged += item.UploadStream_ProgressChanged;
                  item.UploadStream.OnProgressFinished += item.UploadStream_ProgressFinished;

                  item.Status = DriveService.Stream.StatusType.Starting;

                  try
                  {
                    item.UploadStream.Init(fileInfo, null);

                    driveService.UploadFile(item.UploadStream);

                    return true;
                  }
                  catch (Exception exception)
                  {
                    if (item.Status == DriveService.Stream.StatusType.Starting)
                    {
                      item.Status = DriveService.Stream.StatusType.Failed;
                    }

                    throw exception;
                  }
                }
              }
              else
              {
                Log.Information("File " + fileId + " no longer exists");
                return true;
              }
            }

            return false;
          }
          finally
          {
            Mutex.Release();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);

          return false;
        }
      }

      protected void UploadStream_ProgressStarted(DriveService.UploadStream stream, Item item)
      {
        try
        {
          _ProcessCount++;

          item.Status = stream.Status;
          item.PercentCompleted = stream.PercentCompleted;

          Log.Information("Started uploading file " + stream.FileId);
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      protected void UploadStream_ProgressChanged(DriveService.UploadStream stream, Item item)
      {
        try
        {
          item.Status = stream.Status;
          item.PercentCompleted = stream.PercentCompleted;

          Log.Information("Uploading file " + stream.FileId + " - " + stream.PercentCompleted + "% completed");
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      protected void UploadStream_ProgressFinished(DriveService.UploadStream stream, Item item)
      {
        try
        {
          Mutex.Wait();

          try
          {
            _ProcessCount--;

            item.Status = stream.Status;
            item.PercentCompleted = stream.PercentCompleted;

            if (stream.Completed)
            {
              DriveService.UploadStream uploadStream = stream;

              item.ModifiedDate = uploadStream.ModifiedDate;
              item.LastWriteTime = uploadStream.LastWriteTime;

              Log.Information("Completed uploading file " + stream.FileId);
            }
            else if (stream.Cancelled)
            {
              Log.Warning("Cancelled uploading file " + stream.FileId);
            }
            else if (stream.Failed)
            {
              Log.Error("Failed uploading file " + stream.FileId + " - " + stream.ExceptionMessage);
            }

            Items.Remove(item);
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

      protected void AddLockedItem(string filePath)
      {
        try
        {
          LockedItemMutex.Wait();

          try
          {
            int lockedItemIndex = -1;

            for (int i = 0; i < LockedItems.Count; i++)
            {
              lockedItemIndex++;

              Item lockedItem = LockedItems[i];

              if (String.Equals(lockedItem.FilePath, filePath, StringComparison.CurrentCultureIgnoreCase))
              {
                break;
              }
            }

            if (lockedItemIndex == -1)
            {
              var lockedItem = new Item(this);

              lockedItem.FilePath = filePath;

              LockedItems.Add(lockedItem);
            }

            if (LockedItemThread == null)
            {
              LockedItemThread = new System.Threading.Thread(ProcessLockedItems);

              LockedItemThread.Start();
            }
          }
          finally
          {
            LockedItemMutex.Release();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      protected void ProcessLockedItems()
      {
        try
        {
          System.Threading.Thread.Sleep(1000);

          while (LockedItems.Count > 0)
          {
            LockedItemMutex.Wait();

            bool enabled = Enabled;

            if (!enabled)
            {
              break;
            }

            try
            {
              for (int i = 0; i < LockedItems.Count; i++)
              {
                Item lockedItem = LockedItems[i];

                if (CanOpenFile(lockedItem.FilePath, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite, 1))
                {
                  DateTime lastWriteTime = GetFileLastWriteTime(lockedItem.FilePath);

                  if (lastWriteTime > lockedItem.LastWriteTime)
                  {
                    if (ProcessFile(lockedItem.FilePath, false))
                    {
                      LockedItems.RemoveAt(i);

                      i--;

                      break;
                    }
                  }
                }

                System.Threading.Thread.Sleep(100);
              }
            }
            finally
            {
              LockedItemMutex.Release();
            }

            System.Threading.Thread.Sleep(1000);
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
        finally
        {
          LockedItemThread = null;
        }
      }

      protected void RemoveLockedItem(string filePath)
      {
        try
        {
          LockedItemMutex.Wait();

          try
          {
            for (int i = 0; i < LockedItems.Count; i++)
            {
              Item lockedItem = LockedItems[i];

              if (String.Equals(lockedItem.FilePath, filePath, StringComparison.CurrentCultureIgnoreCase))
              {
                LockedItems.RemoveAt(i);

                break;
              }
            }
          }
          finally
          {
            LockedItemMutex.Release();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private class Factory
      {
        private static UploadFileWatcher _UploadFileWatcher;

        public static UploadFileWatcher GetInstance()
        {
          try
          {
            return _UploadFileWatcher ?? (_UploadFileWatcher = new UploadFileWatcher());
          }
          catch (Exception exception)
          {
            Log.Error(exception);

            return null;
          }
        }
      }

      protected class Item
      {
        public readonly UploadFileWatcher UploadFileWatcher;
        public string FilePath = "";
        public DateTime LastWriteTime = default(DateTime);
        public DateTime ModifiedDate = default(DateTime);
        public int PercentCompleted;
        public DriveService.Stream.StatusType Status = DriveService.Stream.StatusType.NotStarted;
        public DriveService.UploadStream UploadStream;

        public Item(UploadFileWatcher uploadFileWatcher)
        {
          UploadFileWatcher = uploadFileWatcher;
        }

        public void UploadStream_ProgressStarted(DriveService.Stream stream)
        {
          UploadFileWatcher.UploadStream_ProgressStarted(UploadStream, this);
        }

        public void UploadStream_ProgressChanged(DriveService.Stream stream)
        {
          UploadFileWatcher.UploadStream_ProgressChanged(UploadStream, this);
        }

        public void UploadStream_ProgressFinished(DriveService.Stream stream)
        {
          UploadFileWatcher.UploadStream_ProgressFinished(UploadStream, this);
        }
      }
    }
  }
}
