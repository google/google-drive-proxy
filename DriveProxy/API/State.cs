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
using System.IO;
using DriveProxy.Utils;

namespace DriveProxy.API
{
  partial class DriveService
  {
    internal class FileInfoLock : IDisposable
    {
      private readonly string _fileId;

      private readonly string _filePath;
      private readonly State _state;

      public FileInfoLock(State state, string fileId, string filePath, System.IO.FileStream fileStream = null)
      {
        try
        {
          _state = state;
          _fileId = fileId;
          _filePath = filePath;
          FileStream = fileStream;
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public string FileId
      {
        get { return _fileId; }
      }

      public string FilePath
      {
        get { return _filePath; }
      }

      public FileStream FileStream { get; set; }

      public void Dispose()
      {
        try
        {
          _state.UnlockFile(this);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }
    }

    internal class State : IDisposable
    {
      public delegate void EventHandler(State sender);

      public delegate void FileLockHandler(State sender, FileInfoLock fileInfoLock);

      public delegate void StreamHandler(State sender, DriveService.Stream stream);

      protected bool _CalledOnStartedProcessing = false;

      protected List<Item> _Items = null;

      protected Mutex _Mutex = null;
      protected Item _ProcessItem = null;

      protected bool _Processing = false;

      protected State()
      {
      }

      protected List<Item> Items
      {
        get { return _Items ?? (_Items = new List<Item>()); }
      }

      protected Mutex Mutex
      {
        get { return _Mutex ?? (_Mutex = new Mutex()); }
      }

      public bool Processing
      {
        get { return _Processing; }
      }

      public void Dispose()
      {
        try
        {
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      public static State Create()
      {
        return Factory.GetInstance();
      }

      public static event EventHandler OnStartedProcessing = null;
      public static event EventHandler OnFinishedProcessing = null;

      public event FileLockHandler OnLockFile = null;
      public event FileLockHandler OnUnlockFile = null;

      public bool IsFileLocked(string fileId, string filePath)
      {
        try
        {
          Item lockedItem = null;

          return IsFileLocked(fileId, filePath, ref lockedItem);
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return false;
        }
      }

      protected bool IsFileLocked(string fileId, string filePath, ref Item lockedItem)
      {
        try
        {
          Mutex.Wait();

          try
          {
            foreach (Item item in Items)
            {
              if (item.Stream != null)
              {
                continue;
              }

              if (item.IsLocked(fileId, filePath))
              {
                lockedItem = item;

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
          Log.Error(exception);

          return false;
        }
      }

      public FileInfoLock LockFile(string fileId, string filePath)
      {
        try
        {
          return LockFile(null, fileId, filePath);
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      protected FileInfoLock LockFile(DriveService.Stream stream, string fileId, string filePath)
      {
        try
        {
          Mutex.Wait();

          try
          {
            var fileInfoLock = new FileInfoLock(this, fileId, filePath);

            Items.Add(new Item(fileInfoLock, stream));

            if (OnLockFile != null)
            {
              OnLockFile(this, fileInfoLock);
            }

            return fileInfoLock;
          }
          finally
          {
            Mutex.Release();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, true, false);

          return null;
        }
      }

      public FileInfoLock LockFile(string fileId,
                                   string filePath,
                                   System.IO.FileMode fileMode,
                                   System.IO.FileAccess fileAccess,
                                   System.IO.FileShare fileShare)
      {
        try
        {
          return LockFile(null, fileId, filePath, fileMode, fileAccess, fileShare);
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      protected FileInfoLock LockFile(DriveService.Stream stream,
                                      string fileId,
                                      string filePath,
                                      System.IO.FileMode fileMode,
                                      System.IO.FileAccess fileAccess,
                                      System.IO.FileShare fileShare)
      {
        try
        {
          Mutex.Wait();

          try
          {
            System.IO.FileStream fileStream = null;

            Exception lastException = null;

            for (int i = 0; i < 10; i++)
            {
              try
              {
                // Open file will try and execute for 1 sec,
                // for this function we will wait up to 10 sec
                fileStream = OpenFile(filePath, fileMode, fileAccess, fileShare);

                lastException = null;

                break;
              }
              catch (Exception exception)
              {
                lastException = exception;
              }

              System.Threading.Thread.Sleep(100);
            }

            if (lastException != null)
            {
              throw lastException;
            }

            FileInfoLock fileInfoLock = null;

            try
            {
              fileInfoLock = LockFile(stream, fileId, filePath);

              fileInfoLock.FileStream = fileStream;
            }
            catch (Exception exception)
            {
              if (fileStream != null)
              {
                fileStream.Dispose();
                fileStream = null;
              }

              throw exception;
            }

            return fileInfoLock;
          }
          finally
          {
            Mutex.Release();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, true, false);

          return null;
        }
      }

      public void UnlockFile(FileInfoLock fileInfoLock)
      {
        try
        {
          Mutex.Wait();

          try
          {
            if (fileInfoLock.FileStream != null)
            {
              fileInfoLock.FileStream.Dispose();
              fileInfoLock.FileStream = null;
            }

            int index = 0;
            bool found = false;

            foreach (Item item in Items)
            {
              if (item.FileInfoLock == fileInfoLock)
              {
                found = true;

                Items.RemoveAt(index);

                break;
              }

              index++;
            }

            if (!found)
            {
              return;
            }

            if (OnUnlockFile != null)
            {
              OnUnlockFile(this, fileInfoLock);
            }
          }
          finally
          {
            Mutex.Release();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public static event StreamHandler OnQueuedStream = null;
      public static event StreamHandler OnFinishedStream = null;

      public bool IsStreamLocked(string fileId, string filePath, ref DriveService.Stream stream)
      {
        try
        {
          Item lockedItem = null;

          bool result = IsStreamLocked(fileId, filePath, ref lockedItem);

          if (result)
          {
            stream = lockedItem.Stream;
          }

          return result;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return false;
        }
      }

      protected bool IsStreamLocked(string fileId, string filePath, ref Item lockedItem)
      {
        try
        {
          Mutex.Wait();

          try
          {
            foreach (Item item in Items)
            {
              if (item.Stream == null)
              {
                continue;
              }

              if (item.IsLocked(fileId, filePath))
              {
                lockedItem = item;

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
          Log.Error(exception);

          return false;
        }
      }

      public void QueueStream(DriveService.Stream stream)
      {
        try
        {
          QueueStreams(new[] {stream});
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void QueueStreams(IEnumerable<DriveService.Stream> streams)
      {
        try
        {
          Mutex.Wait();

          try
          {
            foreach (DriveService.Stream stream in streams)
            {
              bool found = false;

              foreach (Item item in Items)
              {
                if (item.Stream == stream)
                {
                  found = true;

                  break;
                }
              }

              if (!found)
              {
                DriveService.Stream.Factory.Queue(stream);

                Items.Add(new Item(stream));

                if (stream.Visible)
                {
                  try
                  {
                    if (OnQueuedStream != null)
                    {
                      OnQueuedStream(this, stream);
                    }
                  }
                  catch
                  {
                  }
                }
              }
            }

            TryStartProcessing();
          }
          finally
          {
            Mutex.Release();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public FileInfoLock LockStream(DriveService.Stream stream, string filePath)
      {
        try
        {
          return LockFile(stream, stream.FileId, filePath);
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      public FileInfoLock LockStream(DriveService.Stream stream,
                                     string filePath,
                                     System.IO.FileMode fileMode,
                                     System.IO.FileAccess fileAccess,
                                     System.IO.FileShare fileShare)
      {
        try
        {
          return LockFile(stream, stream.FileId, filePath, fileMode, fileAccess, fileShare);
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      public void FinishStream(DriveService.Stream stream)
      {
        try
        {
          Mutex.Wait();

          try
          {
            int index = 0;
            bool found = false;

            foreach (Item item in Items)
            {
              if (item.Stream == stream)
              {
                found = true;

                Items.RemoveAt(index);

                break;
              }

              index++;
            }

            if (!found)
            {
              return;
            }

            DriveService.Stream.Factory.Dispose(stream);

            if (stream.Visible)
            {
              try
              {
                if (OnFinishedStream != null)
                {
                  OnFinishedStream(this, stream);
                }
              }
              catch
              {
              }
            }

            TryFinishProcessing();
          }
          finally
          {
            Mutex.Release();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      private bool TryStartProcessing()
      {
        try
        {
          foreach (Item item in Items)
          {
            if (TryStartProcessing(item))
            {
              return true;
            }
          }

          return false;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return false;
        }
      }

      private bool TryStartProcessing(Item item)
      {
        try
        {
          System.Windows.Forms.Application.DoEvents();

          if (item.Stream != null)
          {
            if (item.Stream.Cancelled)
            {
              DriveService.Stream.Factory.Dispose(item.Stream);

              QueueStream(item.Stream);

              return false;
            }
            if (item.Stream.Queued)
            {
              if (_ProcessItem == item)
              {
                Debugger.Break();
              }

              _Processing = true;
              _ProcessItem = item;

              if (!_CalledOnStartedProcessing && item.Stream.Visible)
              {
                if (OnStartedProcessing != null)
                {
                  _CalledOnStartedProcessing = true;

                  try
                  {
                    OnStartedProcessing(this);
                  }
                  catch (Exception exception)
                  {
                    Log.Error(exception, false);
                  }
                }
              }

              DriveService.Stream.Factory.Start(item.Stream);

              return true;
            }
          }

          return false;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return false;
        }
      }

      private bool TryFinishProcessing()
      {
        try
        {
          if (TryStartProcessing())
          {
            return false;
          }

          _Processing = false;

          if (_CalledOnStartedProcessing)
          {
            if (OnFinishedProcessing != null)
            {
              try
              {
                OnFinishedProcessing(this);
              }
              catch (Exception exception)
              {
                Log.Error(exception, false);
              }
            }
          }

          _ProcessItem = null;
          _CalledOnStartedProcessing = false;

          return true;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return false;
        }
      }

      private class Factory
      {
        private static State _State;

        public static State GetInstance()
        {
          try
          {
            return _State ?? (_State = new State());
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
        public FileInfoLock FileInfoLock = null;
        public DriveService.Stream Stream = null;

        public Item(FileInfoLock fileInfoLock, DriveService.Stream stream)
        {
          FileInfoLock = fileInfoLock;
          Stream = stream;
        }

        public Item(DriveService.Stream stream)
        {
          Stream = stream;
        }

        public bool IsLocked(string fileId, string filePath)
        {
          if (FileInfoLock != null)
          {
            if (!String.IsNullOrEmpty(fileId))
            {
              if (FileInfoLock.FileId == fileId)
              {
                return true;
              }
            }
            else
            {
              if (String.Equals(FileInfoLock.FilePath, filePath, StringComparison.CurrentCultureIgnoreCase))
              {
                return true;
              }
            }
          }

          return false;
        }
      }
    }
  }
}
