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

namespace DriveProxy.API
{
  partial class DriveService
  {
    public class MoveStream : Stream
    {
      protected string _ParentId = null;

      public string ParentId
      {
        get { return _ParentId; }
      }

      public override StreamType Type
      {
        get { return StreamType.Move; }
      }

      public void Init(string parentId, FileInfo fileInfo)
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

          if (fileInfo == null)
          {
            throw new Exception("File is null.");
          }

          if (fileInfo.IsRoot)
          {
            throw new Exception("Cannot move root folder.");
          }

          if (fileInfo.Id == parentId)
          {
            throw new Exception("Cannot move file to itself.");
          }

          _FileId = fileInfo.Id;

          _ParentId = parentId;
          _FileInfo = fileInfo;

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

          Lock();

          if (_FileInfo.ParentId == ParentId)
          {
            DriveService_ProgressChanged(StatusType.Completed, null);
          }
          else
          {
            FileInfoStatus currentStatus = API.DriveService.GetFileInfoStatus(_FileInfo);

            Google.Apis.Drive.v2.Data.File file = _FileInfo._file;

            using (API.DriveService.Connection connection = API.DriveService.Connection.Create())
            {
              file = connection.Service.Files.Get(file.Id).Execute();

              if (file.Parents.Count > 0)
              {
                file.Parents.RemoveAt(0);
              }

              var parentReference = new ParentReference {Id = _ParentId};

              file.Parents.Insert(0, parentReference);

              FilesResource.UpdateRequest request = connection.Service.Files.Update(file, FileId);

              file = request.Execute();

              _FileInfo = GetFileInfo(file);

              FileInfoStatus newStatus = API.DriveService.GetFileInfoStatus(_FileInfo);

              if (currentStatus == FileInfoStatus.OnDisk)
              {
                DateTime modifiedDate = API.DriveService.GetDateTime(file.ModifiedDate);

                try
                {
                  API.DriveService.SetFileLastWriteTime(_FileInfo.FilePath, modifiedDate);
                }
                catch
                {
                }

                _FileInfo = GetFileInfo(file);

                newStatus = API.DriveService.GetFileInfoStatus(_FileInfo);
              }

              DriveService_ProgressChanged(StatusType.Completed, null);
            }
          }
        }
        catch (Exception exception)
        {
          try
          {
            _Status = StatusType.Failed;
            _ExceptionMessage = exception.Message;

            DriveService_ProgressChanged(StatusType.Failed, exception);
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

      protected void DriveService_ProgressChanged(System.Threading.Tasks.Task<File> task)
      {
        try
        {
          if (task.IsCanceled)
          {
            DriveService_ProgressChanged(StatusType.Cancelled, null);
          }
          else if (task.IsFaulted)
          {
            DriveService_ProgressChanged(StatusType.Failed, task.Exception);
          }
          else if (task.IsCompleted)
          {
            _FileInfo = API.DriveService.GetFileInfo(task.Result);

            DriveService_ProgressChanged(StatusType.Completed, null);
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      protected void DriveService_ProgressChanged(StatusType status, Exception processException)
      {
        try
        {
          UpdateProgress(status, BytesProcessed, TotalBytes, processException);

          InvokeOnProgressEvent();
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }
    }
  }
}
