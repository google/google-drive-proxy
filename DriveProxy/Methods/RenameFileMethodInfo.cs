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
  internal class RenameFileMethodInfo : MethodInfo
  {
    public RenameFileMethodInfo()
    {
      try
      {
        AddParameter("FileId", typeof (string));
        AddParameter("Title", typeof (string));
        AddParameter("MimeType", typeof (string));
        SetResult(typeof (FileInfo));
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public override MethodType Type
    {
      get { return MethodType.RenameFile; }
    }

    public override string Invoke(string[] args)
    {
      try
      {
        if (args == null)
        {
          throw new Exception("Input parameter 'args' is null.");
        }

        if (args.Length != 2 && args.Length != 3)
        {
          throw new Exception("Input parameter 'args' length does not equal 2 or 3.");
        }

        string fileId = args[0];
        string title = args[1];
        string mimeType = null;

        if (args.Length > 2)
        {
          mimeType = args[2];
        }

        FileInfo fileInfo = Invoke(fileId, title, mimeType);

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

    public FileInfo Invoke(string fileId, string title, string mimeType = null)
    {
      try
      {
        if (String.IsNullOrEmpty(fileId))
        {
          throw new Exception("Input parameter 'fileId' is null or empty (Cannot rename root directory).");
        }

        if (String.IsNullOrEmpty(title))
        {
          throw new Exception("Input parameter 'title' is null or empty.");
        }

        using (DriveService driveService = DriveService.Create())
        {
          FileInfo fileInfo = driveService.GetFile(fileId);

          if (fileInfo == null)
          {
            throw new Exception("File no longer exists.");
          }

          var renameStream = new API.DriveService.RenameStream();

          renameStream.Init(fileInfo, title, mimeType);

          renameStream.Visible = false;

          driveService.RenameFile(renameStream);

          while (!renameStream.Finished)
          {
            System.Threading.Thread.Sleep(250);
          }

          if (renameStream.Cancelled)
          {
            return null;
          }
          if (renameStream.Failed)
          {
            throw new LogException("Rename file could not complete - " + renameStream.ExceptionMessage, true, true);
          }

          fileInfo = renameStream.FileInfo;

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
