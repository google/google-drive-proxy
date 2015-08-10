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
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Upload;

namespace DriveProxy.API
{
  partial class DriveService
  {
    public class InsertStream : Stream
    {
      private string _fileName = "";
      private string _filePath = "";
      protected long _FileSize = 0;
      private bool _isFolder;
      private DateTime _lastWriteTime = default(DateTime);
      private string _mimeType = "";
      private string _originalTitle = "";
      private Parameters _parameters;

      private string _parentId;
      private string _title = "";

      public string ParentId
      {
        get { return _parentId; }
      }

      public override StreamType Type
      {
        get { return StreamType.Insert; }
      }

      public override string FilePath
      {
        get { return _filePath; }
      }

      public override string FileName
      {
        get { return _fileName; }
      }

      public override string Title
      {
        get { return _title; }
      }

      public override string MimeType
      {
        get { return _mimeType; }
      }

      public override bool IsFolder
      {
        get { return _isFolder; }
      }

      public long FileSize
      {
        get { return _FileSize; }
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

      public DateTime LastWriteTime
      {
        get { return _lastWriteTime; }
      }

      public void Init(string parentId, string title, string mimeType = null, Parameters parameters = null)
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

          if (String.IsNullOrEmpty(parentId))
          {
            parentId = API.DriveService.GetFileId(parentId);
          }

          if (String.IsNullOrEmpty(parentId))
          {
            throw new Exception("ParentId is blank.");
          }

          if (String.IsNullOrEmpty(title))
          {
            throw new Exception("Title is blank.");
          }

          if (String.IsNullOrEmpty(mimeType))
          {
            mimeType = API.DriveService.GetMimeType(title);
          }

          _parentId = parentId;
          _title = title;
          _originalTitle = title;
          _mimeType = mimeType;

          if (String.Equals(MimeType, API.DriveService.MimeType.Folder))
          {
            _isFolder = true;
          }
          else
          {
            _isFolder = false;
            _filePath = System.IO.Path.GetTempFileName();
          }

          _fileName = System.IO.Path.GetFileName(_title);
          _lastWriteTime = DateTime.Now;
          _parameters = parameters ?? new Parameters();

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

          List<File> children = null;

          File parent = API.DriveService._GetCachedFile(_parentId, true, false, ref children);

          string fileName = System.IO.Path.GetFileNameWithoutExtension(_originalTitle);
          string fileExtenstion = System.IO.Path.GetExtension(_originalTitle);

          string title = fileName;

          if (!String.IsNullOrEmpty(fileExtenstion))
          {
            title += fileExtenstion;
          }

          int index = 0;

          while (true)
          {
            bool found = false;

            foreach (File child in children)
            {
              if (child.Title == title)
              {
                FileInfoType fileInfoType = API.DriveService.GetFileInfoType(child);

                if (IsFolder && fileInfoType == FileInfoType.Folder)
                {
                  found = true;
                }
                else if (!IsFolder && fileInfoType != FileInfoType.Folder)
                {
                  found = true;
                }
              }
            }

            if (!found)
            {
              break;
            }

            index++;

            title = fileName + " (" + index + ")";

            if (!String.IsNullOrEmpty(fileExtenstion))
            {
              title += fileExtenstion;
            }
          }

          _title = title;

          var file = new File();

          var parentReference = new ParentReference {Id = _parentId};

          file.Parents = new List<ParentReference> {parentReference};

          file.Title = _title;
          file.Description = "Created by Google Drive Proxy.";
          file.FileExtension = System.IO.Path.GetExtension(_title);
          file.MimeType = _mimeType;
          file.ModifiedDate = _lastWriteTime.ToUniversalTime();

          _FileInfo = API.DriveService.GetFileInfo(file);

          if (_FileInfo.IsFolder)
          {
            Lock();

            using (API.DriveService.Connection connection = API.DriveService.Connection.Create())
            {
              FilesResource.InsertRequest request = connection.Service.Files.Insert(file);

              request.Fields = API.DriveService.RequestFields.FileFields;

              file = request.Execute();

              _FileId = file.Id;
              _FileInfo = API.DriveService.GetFileInfo(file);

              DriveService_ProgressChanged(UploadStatus.Completed, 0, null);
            }
          }
          else
          {
            Lock(_filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);

            using (API.DriveService.Connection connection = API.DriveService.Connection.Create())
            {
              FilesResource.InsertMediaUpload request = connection.Service.Files.Insert(file, _FileStream, _mimeType);

              request.Fields = API.DriveService.RequestFields.FileFields;

              request.ProgressChanged += DriveService_ProgressChanged;
              request.ResponseReceived += DriveService_ResponseReceived;

              request.ChunkSize = FilesResource.InsertMediaUpload.DefaultChunkSize;

              request.Pinned = _parameters.Pinned;
              request.UseContentAsIndexableText = _parameters.UseContentAsIndexableText;

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

      private void DriveService_ProgressChanged(UploadStatus status, long bytesSent, Exception exception)
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

      private void DriveService_ProgressChanged(IUploadProgress uploadProgress)
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
              API.DriveService.DeleteFile(_filePath);

              if (Completed)
              {
                if (!FileInfo.IsFolder)
                {
                  _filePath = _FileInfo.FilePath;
                }
              }
              else
              {
                _filePath = "";
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

      private void DriveService_ResponseReceived(File file)
      {
        try
        {
          _FileId = file.Id;
          _FileInfo = API.DriveService.GetFileInfo(file);
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      public class Parameters
      {
        public bool? Pinned = null;
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
