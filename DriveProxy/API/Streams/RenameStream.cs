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
    public class RenameStream : Stream
    {
      protected string _MimeType = "";
      protected string _TempFilePath;

      protected string _Title = "";

      public override StreamType Type
      {
        get { return StreamType.Rename; }
      }

      public override string Title
      {
        get { return _Title; }
      }

      public override string MimeType
      {
        get { return _MimeType; }
      }

      public void Init(FileInfo fileInfo, string title, string mimeType = null)
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

          if (fileInfo.IsRoot)
          {
            throw new Exception("Cannot trash root folder.");
          }

          if (String.IsNullOrEmpty(title))
          {
            throw new Exception("Title is blank.");
          }

          if (fileInfo.IsFolder)
          {
            mimeType = fileInfo.MimeType;
          }
          else if (String.IsNullOrEmpty(mimeType))
          {
            mimeType = API.DriveService.GetMimeType(title);
          }

          _FileId = fileInfo.Id;

          _FileInfo = fileInfo;
          _Title = title;
          _MimeType = mimeType;
          _TempFilePath = _FileInfo.FilePath;

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

          using (API.DriveService.Connection connection = API.DriveService.Connection.Create())
          {
            Google.Apis.Drive.v2.Data.File file = _FileInfo._file;

            file = connection.Service.Files.Get(file.Id).Execute();

            file.Title = Title;

            if (!String.IsNullOrEmpty(MimeType))
            {
              file.MimeType = MimeType;
            }

            FilesResource.UpdateRequest request = connection.Service.Files.Update(file, FileId);

            file = request.Execute();

            _FileInfo = GetFileInfo(file);

            DriveService_ProgressChanged(StatusType.Completed, null);
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

          if (Processed)
          {
            try
            {
              if (Completed)
              {
                if (!String.Equals(_FileInfo.OriginalFilename, Title, StringComparison.CurrentCultureIgnoreCase))
                {
                  throw new Exception("File in cloud was not renamed.");
                }

                API.DriveService.FileTitles.Remove(_FileInfo._file);

                if (API.DriveService.FileExists(_TempFilePath))
                {
                  if (!String.Equals(_TempFilePath, FilePath, StringComparison.CurrentCultureIgnoreCase))
                  {
                    API.DriveService.MoveFile(_TempFilePath, FilePath);

                    API.DriveService.SetFileLastWriteTime(FilePath, _FileInfo.ModifiedDate);
                  }
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
    }
  }
}
