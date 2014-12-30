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
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Upload;

namespace DriveProxy.API
{
  partial class DriveService
  {
    public class UploadStream : Stream
    {
      protected long _FileSize = 0;
      protected DateTime _LastWriteTime = default(DateTime);
      protected Parameters _Parameters = null;

      public override StreamType Type
      {
        get { return StreamType.Upload; }
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

      public bool CheckIfAlreadyUploaded
      {
        get
        {
          if (_Parameters == null || !_Parameters.CheckIfAlreadyUploaded.HasValue)
          {
            return true;
          }

          return _Parameters.CheckIfAlreadyUploaded.Value;
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
            throw new Exception("Cannot upload folder.");
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

          if (!System.IO.File.Exists(_FileInfo.FilePath))
          {
            throw new Exception("FileInfo.FilePath '" + _FileInfo.FilePath + "' does not exist.");
          }

          Lock(_FileInfo.FilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);

          _FileSize = _FileStream.Length;
          _LastWriteTime = API.DriveService.GetFileLastWriteTime(_FileInfo.FilePath);

          if (_FileInfo.IsGoogleDoc)
          {
            DriveService_ProgressChanged(UploadStatus.Completed, FileSize, null);
          }
          else
          {
            FileInfoStatus fileInfoStatus = API.DriveService.GetFileInfoStatus(_FileInfo);

            if (fileInfoStatus != FileInfoStatus.ModifiedOnDisk)
            {
              if (CheckIfAlreadyUploaded)
              {
                try
                {
                  DriveService_ProgressChanged(UploadStatus.Completed, 0, null);

                  return;
                }
                catch (Exception exception)
                {
                  Log.Error(exception);
                }
              }
            }

            using (API.DriveService.Connection connection = API.DriveService.Connection.Create())
            {
              Google.Apis.Drive.v2.Data.File file = _FileInfo._file;

              file.ModifiedDate = _LastWriteTime.ToUniversalTime();

              FilesResource.UpdateMediaUpload request = connection.Service.Files.Update(file,
                                                                                        file.Id,
                                                                                        _FileStream,
                                                                                        file.MimeType);

              request.ProgressChanged += DriveService_ProgressChanged;
              request.ResponseReceived += DriveService_ResponseReceived;

              request.Fields = API.DriveService.RequestFields.FileFields;

              if (ChunkSize <= 0)
              {
                request.ChunkSize = FilesResource.UpdateMediaUpload.DefaultChunkSize;
              }
              else if (ChunkSize < FilesResource.UpdateMediaUpload.MinimumChunkSize)
              {
                request.ChunkSize = FilesResource.UpdateMediaUpload.MinimumChunkSize;
              }
              else
              {
                request.ChunkSize = ChunkSize;
              }

              request.Convert = _Parameters.Convert;
              request.NewRevision = true;
              request.Ocr = _Parameters.Ocr;
              request.OcrLanguage = _Parameters.OcrLanguage;
              request.Pinned = _Parameters.Pinned;
              request.SetModifiedDate = true;
              request.TimedTextLanguage = _Parameters.TimedTextLanguage;
              request.TimedTextTrackName = _Parameters.TimedTextTrackName;
              request.UpdateViewedDate = _Parameters.UpdateViewedDate;
              request.UseContentAsIndexableText = _Parameters.UseContentAsIndexableText;

              _CancellationTokenSource = new System.Threading.CancellationTokenSource();

              System.Threading.Tasks.Task<IUploadProgress> task = request.UploadAsync(_CancellationTokenSource.Token);
            }
          }
        }
        catch (Exception exception)
        {
          try
          {
            _Status = StatusType.Failed;
            _ExceptionMessage = exception.Message;

            DriveService_ProgressChanged(UploadStatus.Failed, 0, exception);
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
          base.Dispose();
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      protected void DriveService_ProgressChanged(UploadStatus status, long bytesSent, Exception exception)
      {
        try
        {
          DriveService_ProgressChanged(new UploadProgress(status, bytesSent, exception));
        }
        catch (Exception exception2)
        {
          Log.Error(exception2);
        }
      }

      protected void DriveService_ProgressChanged(IUploadProgress uploadProgress)
      {
        try
        {
          var status = StatusType.NotStarted;
          long bytesProcessed = uploadProgress.BytesSent;
          long totalBytes = FileSize;
          Exception processException = uploadProgress.Exception;

          if (uploadProgress.Status == UploadStatus.Completed)
          {
            status = StatusType.Completed;
          }
          else if (uploadProgress.Status == UploadStatus.Uploading)
          {
            status = StatusType.Processing;
          }
          else if (uploadProgress.Status == UploadStatus.Failed)
          {
            status = StatusType.Failed;
          }
          else
          {
            status = StatusType.Starting;
          }

          UpdateProgress(status, bytesProcessed, totalBytes, processException);

          if (Processed)
          {
            try
            {
              if (Completed)
              {
                _LastWriteTime = API.DriveService.GetFileLastWriteTime(FilePath);

                DateTime modifiedDate = _FileInfo.ModifiedDate;

                if (!API.DriveService.IsDateTimeEqual(_LastWriteTime, modifiedDate))
                {
                  try
                  {
                    _LastWriteTime = API.DriveService.SetFileLastWriteTime(FilePath, modifiedDate);
                  }
                  catch (Exception exception)
                  {
                    Log.Error(exception, false, false);

                    System.Threading.Thread.Sleep(500);
                  }
                }

                if (!API.DriveService.IsDateTimeEqual(_LastWriteTime, modifiedDate))
                {
                  try
                  {
                    using (API.DriveService.Connection connection = API.DriveService.Connection.Create())
                    {
                      File file = _FileInfo._file;

                      file.ModifiedDate = _LastWriteTime.ToUniversalTime();

                      Google.Apis.Drive.v2.FilesResource.UpdateRequest updateRequest =
                        connection.Service.Files.Update(file, file.Id);

                      updateRequest.SetModifiedDate = true;

                      _FileInfo._file = updateRequest.Execute();

                      modifiedDate = _FileInfo.ModifiedDate;
                    }
                  }
                  catch (Exception exception)
                  {
                    Log.Error(exception, false, false);

                    System.Threading.Thread.Sleep(500);
                  }
                }

                if (!API.DriveService.IsDateTimeEqual(_LastWriteTime, modifiedDate))
                {
                  try
                  {
                    using (API.DriveService.Connection connection = API.DriveService.Connection.Create())
                    {
                      File file = _FileInfo._file;

                      file.ModifiedDate = _LastWriteTime.ToUniversalTime();

                      Google.Apis.Drive.v2.FilesResource.PatchRequest patchRequest = connection.Service.Files.Patch(
                                                                                                                    file,
                                                                                                                    file
                                                                                                                      .Id);

                      patchRequest.SetModifiedDate = true;

                      _FileInfo._file = patchRequest.Execute();

                      modifiedDate = _FileInfo.ModifiedDate;
                    }
                  }
                  catch (Exception exception)
                  {
                    Log.Error(exception, false, false);

                    System.Threading.Thread.Sleep(500);
                  }
                }

                if (!API.DriveService.IsDateTimeEqual(_LastWriteTime, modifiedDate))
                {
                  // hopefully at least this will keep the upload process from firing off
                  _FileInfo._file.ModifiedDate = _LastWriteTime.ToUniversalTime();
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

          InvokeOnProgressEvent();
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      protected void DriveService_ResponseReceived(File file)
      {
        try
        {
          _FileInfo = API.DriveService.GetFileInfo(file);
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      public class Parameters
      {
        public bool? CheckIfAlreadyUploaded;
        public int? ChunkSize;
        public bool? Convert = null;
        public bool? Ocr = null;
        public string OcrLanguage = null;
        public bool? Pinned = null;
        public string TimedTextLanguage = null;
        public string TimedTextTrackName = null;
        public bool? UpdateViewedDate = null;
        public bool? UseContentAsIndexableText = null;
      }

      private class UploadProgress : IUploadProgress
      {
        private readonly long _bytesSent;

        private readonly Exception _exception;

        private readonly UploadStatus _status = UploadStatus.NotStarted;

        public UploadProgress(UploadStatus status, long bytesSent, Exception exception)
        {
          _status = status;
          _bytesSent = bytesSent;
          _exception = exception;
        }

        public long BytesSent
        {
          get { return _bytesSent; }
        }

        public Exception Exception
        {
          get { return _exception; }
        }

        public UploadStatus Status
        {
          get { return _status; }
        }
      }
    }
  }
}
