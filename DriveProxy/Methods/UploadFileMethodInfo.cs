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
  internal class UploadFileMethodInfo : MethodInfo
  {
    public UploadFileMethodInfo()
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
      get { return MethodType.UploadFile; }
    }

    public override string Invoke(string[] args)
    {
      try
      {
        if (args == null)
        {
          throw new Exception("Input parameter 'args' is null.");
        }

        if (args.Length != 1 && args.Length != 2) // old call includes filePath
        {
          throw new Exception("Input parameter 'args' length does not equal 2.");
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
          throw new Exception("Input parameter 'fileId' is null or empty (Cannot upload root directory).");
        }

        using (DriveService driveService = DriveService.Create())
        {
          FileInfo fileInfo = driveService.GetFile(fileId);

          if (fileInfo == null)
          {
            throw new Exception("File no longer exists.");
          }

          var uploadStream = new API.DriveService.UploadStream();

          uploadStream.Init(fileInfo, null);

          driveService.UploadFile(uploadStream);

          while (!uploadStream.Finished)
          {
            System.Threading.Thread.Sleep(250);
          }

          if (uploadStream.Cancelled)
          {
            return null;
          }
          if (uploadStream.Failed)
          {
            throw new LogException("Upload could not complete - " + uploadStream.ExceptionMessage, true, true);
          }

          fileInfo = uploadStream.FileInfo;

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
