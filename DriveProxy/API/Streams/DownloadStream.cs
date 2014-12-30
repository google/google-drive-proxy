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
using Google.Apis.Download;

namespace DriveProxy.API
{
  partial class DriveService
  {
    public class DownloadStream : Stream
    {
      protected string _DownloadFilePath = "";
      protected long _FileSize = 0;
      protected DateTime _LastWriteTime = default(DateTime);
      protected Parameters _Parameters = null;

      public override StreamType Type
      {
        get { return StreamType.Download; }
      }

      public DateTime ModifiedDate
      {
        get
        {
          if (_FileInfo != null)
          {
            return _FileInfo.ModifiedDate;
          }

          return default(DateTime);
        }
      }

      public int ChunkSize
      {
        get
        {
          if (_Parameters == null || !_Parameters.ChunkSize.HasValue)
          {
            return 0;
          }

          return _Parameters.ChunkSize.Value;
        }
      }

      public bool CheckIfAlreadyDownloaded
      {
        get
        {
          if (_Parameters == null || !_Parameters.CheckIfAlreadyDownloaded.HasValue)
          {
            return true;
          }

          return _Parameters.CheckIfAlreadyDownloaded.Value;
        }
      }

      public long FileSize
      {
        get { return _FileSize; }
      }

      public DateTime LastWriteTime
      {
        get { return _LastWriteTime; }
      }

      public void Init(FileInfo fileInfo, Parameters parameters = null)
      {
        try
        {
          if (Status == StatusType.Queued)
          {
            throw new Exception("Stream has already been queued.");
          }

          if (Status != StatusType.NotStarted)
          {
            throw new Exception("Stream has already been started.");
          }

          if (fileInfo == null)
          {
            throw new Exception("File is null.");
          }

          if (fileInfo.IsFolder)
          {
            throw new Exception("Cannot download folder.");
          }

          _FileId = fileInfo.Id;

          _FileInfo = fileInfo;
          _Parameters = parameters ?? new Parameters();

          base.Init();
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      protected override void Start()
      {
        try
        {
          try
          {
            if (Status != StatusType.Queued)
            {
              throw new Exception("Stream has not been queued.");
            }

            base.Start();

            _FileInfo = _DriveService.GetFile(FileId);

            if (_FileInfo == null)
            {
              throw new Exception("File '" + FileId + "' no longer exists.");
            }

            if (String.IsNullOrEmpty(_FileInfo.FilePath))
            {
              throw new Exception("FileInfo.FilePath is blank.");
            }
          }
          catch (Exception exception)
          {
            Log.Error(exception);
          }

          try
          {
            if (_FileInfo.IsGoogleDoc)
            {
              try
              {
                API.DriveService.WriteGoogleDoc(_FileInfo);

                DriveService_ProgressChanged(DownloadStatus.Completed, 0, null);

                return;
              }
              catch (Exception exception)
              {
                Log.Error(exception);
              }
            }
            else if (String.IsNullOrEmpty(_FileInfo.DownloadUrl))
            {
              try
              {
                DriveService_ProgressChanged(DownloadStatus.Completed, 0, null);

                return;
              }
              catch (Exception exception)
              {
                Log.Error(exception);
              }
            }
            else if (_FileInfo.FileSize == 0)
            {
              try
              {
                API.DriveService.CreateFile(FileInfo);

                DriveService_ProgressChanged(DownloadStatus.Completed, 0, null);

                return;
              }
              catch (Exception exception)
              {
                Log.Error(exception);
              }
            }
            else
            {
              FileInfoStatus fileInfoStatus = API.DriveService.GetFileInfoStatus(_FileInfo);

              if (fileInfoStatus == FileInfoStatus.ModifiedOnDisk ||
                  fileInfoStatus == FileInfoStatus.OnDisk)
              {
                if (CheckIfAlreadyDownloaded)
                {
                  try
                  {
                    DriveService_ProgressChanged(DownloadStatus.Completed, 0, null);

                    return;
                  }
                  catch (Exception exception)
                  {
                    Log.Error(exception);
                  }
                }
              }
            }
          }
          catch (Exception exception)
          {
            Log.Error(exception);
          }

          try
          {
            API.DriveService.DeleteFile(_FileInfo.FilePath);

            _DownloadFilePath = _FileInfo.FilePath + ".download";

            Lock(_DownloadFilePath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
          }
          catch (Exception exception)
          {
            Log.Error(exception);
          }

          try
          {
            _FileSize = API.DriveService.GetLong(_FileInfo.FileSize);
            _LastWriteTime = API.DriveService.GetFileLastWriteTime(_DownloadFilePath);
          }
          catch (Exception exception)
          {
            Log.Error(exception);
          }

          try
          {
            using (API.DriveService.Connection connection = API.DriveService.Connection.Create())
            {
              var request = new MediaDownloader(connection.Service);

              request.ProgressChanged += DriveService_ProgressChanged;

              if (ChunkSize <= 0)
              {
                request.ChunkSize = API.DriveService.Settings.DownloadFileChunkSize;
              }
              else if (ChunkSize > MediaDownloader.MaximumChunkSize)
              {
                request.ChunkSize = MediaDownloader.MaximumChunkSize;
              }
              else
              {
                request.ChunkSize = ChunkSize;
              }

              _CancellationTokenSource = new System.Threading.CancellationTokenSource();

              System.Threading.Tasks.Task<IDownloadProgress> task = request.DownloadAsync(_FileInfo.DownloadUrl,
                                                                                          _FileStream,
                                                                                          _CancellationTokenSource.Token);
            }
          }
          catch (Exception exception)
          {
            Log.Error(exception);
          }
        }
        catch (Exception exception)
        {
          try
          {
            _Status = StatusType.Failed;
            _ExceptionMessage = exception.Message;

            DriveService_ProgressChanged(DownloadStatus.Failed, 0, exception);
          }
          catch
          {
            Debugger.Break();
          }

          Log.Error(exception);
        }
      }

      protected override void Dispose()
      {
        try
        {
          CloseFileStream();

          if (!String.IsNullOrEmpty(_DownloadFilePath))
          {
            API.DriveService.DeleteFile(_DownloadFilePath);

            _DownloadFilePath = "";
          }

          base.Dispose();
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      protected void DriveService_ProgressChanged(DownloadStatus status, long bytesDownloaded, Exception exception)
      {
        try
        {
          DriveService_ProgressChanged(new DownloadProgress(status, bytesDownloaded, exception));
        }
        catch (Exception exception2)
        {
          Log.Error(exception2);
        }
      }

      protected void DriveService_ProgressChanged(IDownloadProgress downloadProgress)
      {
        try
        {
          var status = StatusType.NotStarted;
          long bytesProcessed = downloadProgress.BytesDownloaded;
          long totalBytes = FileSize;
          Exception processException = downloadProgress.Exception;

          if (downloadProgress.Status == DownloadStatus.Completed)
          {
            status = StatusType.Completed;
          }
          else if (downloadProgress.Status == DownloadStatus.Downloading)
          {
            status = StatusType.Processing;
          }
          else if (downloadProgress.Status == DownloadStatus.Failed)
          {
            status = StatusType.Failed;
          }
          else
          {
            status = StatusType.Starting;
          }

          UpdateProgress(status, bytesProcessed, totalBytes, processException);

          if (_Status == StatusType.Failed && String.IsNullOrEmpty(_ExceptionMessage))
          {
            Debugger.Break();
          }

          if (Processed)
          {
            try
            {
              if (Completed)
              {
                if (!String.IsNullOrEmpty(_DownloadFilePath))
                {
                  if (!String.Equals(_DownloadFilePath, _FileInfo.FilePath, StringComparison.CurrentCultureIgnoreCase))
                  {
                    API.DriveService.MoveFile(_DownloadFilePath, _FileInfo.FilePath);

                    _DownloadFilePath = "";
                  }
                }
              }
              else
              {
                if (!String.IsNullOrEmpty(_DownloadFilePath))
                {
                  API.DriveService.DeleteFile(_DownloadFilePath);

                  _DownloadFilePath = "";
                }

                _DriveService.CleanupFile(_FileInfo, true);
              }
            }
            catch (Exception exception)
            {
              Log.Error(exception, false);

              if (!Failed)
              {
                _ExceptionMessage = exception.Message;
                _Status = StatusType.Failed;
              }
            }

            try
            {
              if (Completed)
              {
                _LastWriteTime = API.DriveService.GetFileLastWriteTime(FilePath);

                if (!API.DriveService.IsDateTimeEqual(_LastWriteTime, _FileInfo.ModifiedDate))
                {
                  _LastWriteTime = API.DriveService.SetFileLastWriteTime(FilePath, _FileInfo.ModifiedDate);
                }
              }
            }
            catch (Exception exception)
            {
              Log.Error(exception, false);

              if (!Failed)
              {
                _ExceptionMessage = exception.Message;
                _Status = StatusType.Failed;
              }
            }
          }

          if (_Status == StatusType.Failed && String.IsNullOrEmpty(_ExceptionMessage))
          {
            Debugger.Break();
          }

          InvokeOnProgressEvent();
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private class DownloadProgress : IDownloadProgress
      {
        private readonly long _bytesDownloaded;

        private readonly Exception _exception;

        private readonly DownloadStatus _status = DownloadStatus.NotStarted;

        public DownloadProgress(DownloadStatus status, long bytesDownloaded, Exception exception)
        {
          _status = status;
          _bytesDownloaded = bytesDownloaded;
          _exception = exception;
        }

        public long BytesDownloaded
        {
          get { return _bytesDownloaded; }
        }

        public Exception Exception
        {
          get { return _exception; }
        }

        public DownloadStatus Status
        {
          get { return _status; }
        }
      }

      public class Parameters
      {
        public bool? CheckIfAlreadyDownloaded;
        public int? ChunkSize;
      }
    }
  }
}
