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
    public class UntrashStream : Stream
    {
      public override StreamType Type
      {
        get { return StreamType.Untrash; }
      }

      public void Init(FileInfo fileInfo)
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
            throw new Exception("Cannot untrash root folder.");
          }

          _FileId = fileInfo.Id;
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

          using (API.DriveService.Connection connection = API.DriveService.Connection.Create())
          {
            FilesResource.UntrashRequest request = connection.Service.Files.Untrash(FileId);

            request.Fields = API.DriveService.RequestFields.FileFields;

            _CancellationTokenSource = new System.Threading.CancellationTokenSource();

            System.Threading.Tasks.Task<File> task = request.ExecuteAsync(_CancellationTokenSource.Token);

            task.ContinueWith(thisTask => DriveService_ProgressChanged(thisTask));
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
