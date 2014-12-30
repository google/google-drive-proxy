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
using System.Linq;
using DriveProxy.Utils;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;

namespace DriveProxy.API
{
  partial class DriveService
  {
    public class CopyStream : Stream
    {
      protected string _ParentId = null;

      protected long TotalFiles
      {
        get { return _TotalBytes; }
        set { _TotalBytes = value; }
      }

      protected long FilesProcessed
      {
        get { return _BytesProcessed; }
        set { _BytesProcessed = value; }
      }

      public string ParentId
      {
        get { return _ParentId; }
      }

      public override StreamType Type
      {
        get { return StreamType.Copy; }
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
            throw new Exception("Cannot copy root folder.");
          }

          if (fileInfo.Id == parentId)
          {
            throw new Exception("Cannot copy file to itself.");
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

          TotalFiles = 1;
          FilesProcessed = 0;

          if (FileInfo.IsFolder)
          {
            GetFiles(ref _FileInfo);

            _FileInfo = CopyFolder(_ParentId, _FileInfo);
          }
          else
          {
            _FileInfo = CopyFile(_ParentId, _FileInfo);
          }

          DriveService_ProgressChanged(StatusType.Completed, null);
        }
        catch (Exception exception)
        {
          try
          {
            _Status = IsCancellationRequested ? StatusType.Cancelled : StatusType.Failed;

            _ExceptionMessage = exception.Message;

            DriveService_ProgressChanged(_Status, exception);
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

      protected void GetFiles(ref FileInfo fileInfo)
      {
        try
        {
          IEnumerable<FileInfo> children = _DriveService.GetFiles(fileInfo.Id);

          fileInfo.Files.AddRange(children);

          TotalFiles += children.Count();

          for (int i = 0; i < children.Count(); i++)
          {
            if (IsCancellationRequested)
            {
              DriveService_ProgressChanged(StatusType.Cancelled, null);

              return;
            }

            FileInfo child = children.ElementAt(i);

            if (child.IsFolder)
            {
              GetFiles(ref child);
            }
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      protected FileInfo CopyFolder(string parentId, FileInfo fileInfo)
      {
        try
        {
          FileInfo folder = CreateFolder(parentId, fileInfo);

          FilesProcessed++;

          foreach (FileInfo childInfo in fileInfo.Files)
          {
            if (IsCancellationRequested)
            {
              throw new Exception("Process cancelled by user.");
            }

            if (childInfo.IsFolder)
            {
              CopyFolder(folder.Id, childInfo);
            }
            else
            {
              var copyStream = new CopyStream();

              copyStream.Init(folder.Id, childInfo);

              copyStream.Visible = false;

              _DriveService.CopyFile(copyStream);

              while (!copyStream.Processed)
              {
                if (IsCancellationRequested)
                {
                  copyStream.Cancel();
                }

                System.Threading.Thread.Sleep(250);
              }

              if (IsCancellationRequested)
              {
                throw new Exception("Process cancelled by user.");
              }

              if (!copyStream.Completed)
              {
                throw new Exception("Could not create new folder");
              }

              FilesProcessed++;
            }
          }

          return folder;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      protected FileInfo CreateFolder(string parentId, FileInfo fileInfo)
      {
        try
        {
          var insertStream = new InsertStream();

          insertStream.Init(parentId, fileInfo.Title, API.DriveService.MimeType.Folder);

          insertStream.Visible = false;

          _DriveService.InsertFile(insertStream);

          while (!insertStream.Finished)
          {
            if (IsCancellationRequested)
            {
              insertStream.Cancel();
            }

            System.Threading.Thread.Sleep(250);
          }

          if (IsCancellationRequested)
          {
            throw new Exception("Process cancelled by user.");
          }

          if (!insertStream.Completed)
          {
            throw new Exception("Could not create new folder");
          }

          FileInfo folder = insertStream.FileInfo;

          return folder;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      protected FileInfo CopyFile(string parentId, FileInfo fileInfo)
      {
        try
        {
          FileInfoStatus currentStatus = API.DriveService.GetFileInfoStatus(fileInfo);

          File file = fileInfo._file;

          if (file.Parents.Count > 0)
          {
            file.Parents.RemoveAt(0);
          }

          var parentReference = new ParentReference {Id = parentId};

          if (file.Parents == null)
          {
            file.Parents = new List<ParentReference>();
          }

          file.Parents.Insert(0, parentReference);

          using (API.DriveService.Connection connection = API.DriveService.Connection.Create())
          {
            FilesResource.CopyRequest request = connection.Service.Files.Copy(file, file.Id);

            request.Fields = API.DriveService.RequestFields.FileFields;

            _CancellationTokenSource = new System.Threading.CancellationTokenSource();

            file = request.Execute();

            fileInfo = GetFileInfo(file);

            FileInfoStatus newStatus = API.DriveService.GetFileInfoStatus(fileInfo);

            if (currentStatus == FileInfoStatus.OnDisk)
            {
              DateTime modifiedDate = API.DriveService.GetDateTime(file.ModifiedDate);

              try
              {
                API.DriveService.SetFileLastWriteTime(fileInfo.FilePath, modifiedDate);
              }
              catch
              {
              }

              fileInfo = GetFileInfo(file);

              newStatus = API.DriveService.GetFileInfoStatus(fileInfo);
            }

            return fileInfo;
          }
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
