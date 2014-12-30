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
using DriveProxy.Utils;

namespace DriveProxy.API
{
  partial class DriveService
  {
    private class CleanupFileWatcher : IDisposable
    {
      private static System.Threading.Thread _thread;
      private bool _enabled;

      private string _folderPath = "";
      private int _processCount;

      private CleanupFileWatcher()
      {
      }

      public string FolderPath
      {
        get { return _folderPath; }
      }

      public bool Enabled
      {
        get { return _enabled; }
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
        get { return _processCount; }
      }

      public void Dispose()
      {
        try
        {
          Log.Information("Disposing CleanupFileWatcher");
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      public static CleanupFileWatcher Create()
      {
        return Factory.GetInstance();
      }

      public void Start()
      {
        try
        {
          Log.Information("Attempting to start CleanupFileWatcher");

          if (Enabled)
          {
            if (String.Equals(_folderPath,
                              DriveService.Settings.LocalGoogleDriveData,
                              StringComparison.CurrentCultureIgnoreCase))
            {
              Log.Information("CleanupFileWatcher is already watching folder " + _folderPath);
              return;
            }

            Stop();
          }

          while (true)
          {
            if (_thread == null)
            {
              break;
            }

            System.Threading.Thread.Sleep(250);
          }

          Log.Information("Creating CleanupFileWatcher");

          _thread = new System.Threading.Thread(Process);

          _folderPath = DriveService.Settings.LocalGoogleDriveData;
          _enabled = true;

          _thread.Start();

          Log.Information("CleanupFileWatcher has started watching folder " + _folderPath);
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
          Log.Information("Stopping CleanupFileWatcher");

          _enabled = false;
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      protected void Process()
      {
        try
        {
          _processCount++;

          DateTime lastProcessTime = default(DateTime);

          while (true)
          {
            try
            {
              if (!_enabled)
              {
                break;
              }

              TimeSpan timeSpan = DateTime.Now.Subtract(lastProcessTime);

              if (timeSpan.TotalMinutes < DriveService.Settings.CleanupFileTimeout)
              {
                for (int i = 0; i < 120; i++)
                {
                  if (!_enabled)
                  {
                    break;
                  }

                  System.Threading.Thread.Sleep(500);
                }

                if (!_enabled)
                {
                  break;
                }

                continue;
              }

              if (!DriveService.IsSignedIn)
              {
                continue;
              }

              using (DriveService driveService = DriveService.Create())
              {
                if (!_enabled)
                {
                  break;
                }

                string[] folderPaths = DriveService.Settings.GetLocalGoogleDriveDataSubFolders();

                foreach (string folderPath in folderPaths)
                {
                  if (!_enabled)
                  {
                    break;
                  }

                  ProcessFolder(driveService, folderPath);
                }

                if (!_enabled)
                {
                  break;
                }
              }

              lastProcessTime = DateTime.Now;
            }
            catch (Exception exception)
            {
              Log.Error(exception, false);
            }
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
        finally
        {
          _processCount--;
          _thread = null;
        }
      }

      private void ProcessFolder(DriveService driveService, string folderPath)
      {
        try
        {
          Log.Information("CleanupFileWatcher attempting to process folder " + folderPath);

          string fileId = System.IO.Path.GetFileName(folderPath);
          FileInfo fileInfo = null;

          try
          {
            fileInfo = driveService.GetFile(fileId);
          }
          catch (Exception exception)
          {
            Log.Error(exception, false, false);
          }

          if (fileInfo != null)
          {
            DriveService.Stream stream = null;

            if (IsStreamLocked(fileInfo.Id, fileInfo.FilePath, ref stream))
            {
              Log.Warning("Stream " + fileId + " is locked (type=" + stream.Type + ")");
            }
            else if (IsFileLocked(fileInfo.Id, fileInfo.FilePath))
            {
              Log.Warning("File " + fileId + " is locked");
            }
            else if (!CanOpenFile(fileInfo.FilePath, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite, 1))
            {
              Log.Warning("File " + fileInfo.FilePath + " is being used by another application.");
            }
            else if (!HasFileOnDiskTimedout(fileInfo.FilePath))
            {
              Log.Warning("File " + fileId + " has not timed out on disk");
            }
            else
            {
              FileInfoStatus fileInfoStatus = GetFileInfoStatus(fileInfo);

              if (fileInfoStatus == FileInfoStatus.ModifiedOnDisk)
              {
                Log.Information("Attempting to upload file " + fileId);

                var uploadStream = new DriveService.UploadStream();

                uploadStream.OnProgressStarted += uploadStream_ProgressStarted;
                uploadStream.OnProgressChanged += uploadStream_ProgressChanged;
                uploadStream.OnProgressFinished += uploadStream_ProgressFinished;

                uploadStream.Init(fileInfo, null);

                driveService.UploadFile(uploadStream);
              }
              else
              {
                Log.Information("Attempting to clean-up file " + fileId);

                driveService.CleanupFile(fileInfo, fileInfoStatus, false);
              }
            }
          }
          else
          {
            Log.Information("Attempting to clean-up directory " + folderPath + ": File was not found in google drive");

            System.IO.Directory.Delete(folderPath, true);
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private void uploadStream_ProgressStarted(DriveService.Stream stream)
      {
        try
        {
          _processCount++;

          Log.Information("Started uploading file " + stream.FileId);
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private void uploadStream_ProgressChanged(DriveService.Stream stream)
      {
        try
        {
          Log.Information("Uploading file " + stream.FileId + " - " + stream.PercentCompleted + "% completed");
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private void uploadStream_ProgressFinished(DriveService.Stream stream)
      {
        try
        {
          _processCount--;

          if (stream.Completed)
          {
            Log.Information("Completed uploading file " + stream.FileId);
          }
          else if (stream.Cancelled)
          {
            Log.Information("Cancelled uploading file " + stream.FileId);
          }
          else if (stream.Failed)
          {
            Log.Information("Failed uploading file " + stream.FileId + " - " + stream.ExceptionMessage);
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private class Factory
      {
        private static CleanupFileWatcher _cleanupFileWatcher;

        public static CleanupFileWatcher GetInstance()
        {
          try
          {
            return _cleanupFileWatcher ?? (_cleanupFileWatcher = new CleanupFileWatcher());
          }
          catch (Exception exception)
          {
            Log.Error(exception);

            return null;
          }
        }
      }
    }
  }
}
