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
using DriveProxy.API;
using DriveProxy.Forms;
using DriveProxy.Service;
using DriveProxy.Utils;

namespace DriveProxy.Methods
{
  internal class DownloadFileMethodInfo : MethodInfo
  {
    public DownloadFileMethodInfo()
    {
      try
      {
        AddParameter("Id", typeof (string));
        SetResult(typeof (FileInfo));
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public override MethodType Type
    {
      get { return MethodType.DownloadFile; }
    }

    public override string Invoke(string[] args)
    {
      try
      {
        if (args == null)
        {
          throw new Exception("Input parameter 'args' is null.");
        }

        if (args.Length != 1)
        {
          throw new Exception("Input parameter 'args' length does not equal 1.");
        }

        string fileId = args[0];

        FileInfo fileInfo = Invoke(fileId);

        if (fileInfo == null)
        {
          return "";
        }

        return fileInfo.ToString();
      }
      catch (Exception exception)
      {
        StatusForm.Exception(exception);

        Log.Error(exception);

        return null;
      }
    }

    public FileInfo Invoke(string fileId)
    {
      try
      {
        if (String.IsNullOrEmpty(fileId))
        {
          throw new Exception("Input parameter 'fileId' is null or empty (Cannot download root directory).");
        }

        using (API.DriveService driveService = DriveProxy.API.DriveService.Create())
        {
          FileInfo fileInfo = driveService.GetFile(fileId);

          if (fileInfo == null)
          {
            throw new Exception("File no longer exists.");
          }

          FileInfoStatus fileInfoStatus = DriveProxy.API.DriveService.GetFileInfoStatus(fileInfo);

          if (fileInfoStatus == FileInfoStatus.ModifiedOnDisk ||
              fileInfoStatus == FileInfoStatus.OnDisk)
          {
            return fileInfo;
          }

          var downloadStream = new DriveProxy.API.DriveService.DownloadStream();

          downloadStream.Init(fileInfo);

          driveService.DownloadFile(downloadStream);

          while (!downloadStream.Finished)
          {
            System.Threading.Thread.Sleep(250);
          }

          if (downloadStream.Cancelled)
          {
            return null;
          }
          if (downloadStream.Failed)
          {
            throw new LogException("Download could not complete - " + downloadStream.ExceptionMessage, true, true);
          }

          fileInfo = downloadStream.FileInfo;

          return fileInfo;
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, breakWithDebugger: false);

        return null;
      }
    }
  }
}
