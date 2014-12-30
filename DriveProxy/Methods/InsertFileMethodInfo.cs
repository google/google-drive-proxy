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
  internal class InsertFileMethodInfo : MethodInfo
  {
    public InsertFileMethodInfo()
    {
      try
      {
        AddParameter("ParentId", typeof (string));
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
      get { return MethodType.InsertFile; }
    }

    public override string Invoke(string[] args)
    {
      try
      {
        if (args == null)
        {
          throw new Exception("Input parameter 'args' is null.");
        }

        if (args.Length != 3)
        {
          throw new Exception("Input parameter 'args' length does not equal 3.");
        }

        string parentId = args[0];
        string title = args[1];
        string mimeType = args[2];

        if (title.IndexOf("Google Drive") == 0)
        {
          Debugger.Break();
        }

        FileInfo fileInfo = Invoke(parentId, title, mimeType);

        if (fileInfo == null)
        {
          return "";
        }

        return fileInfo.ToString();
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public FileInfo Invoke(string parentId, string title, bool isFolder)
    {
      try
      {
        if (isFolder)
        {
          return Invoke(parentId, title, DriveService.MimeType.Folder);
        }
        return Invoke(parentId, title, null);
      }
      catch (Exception exception)
      {
        StatusForm.Exception(exception);

        Log.Error(exception);

        return null;
      }
    }

    public FileInfo Invoke(string parentId, string title, string mimeType = null)
    {
      try
      {
        if (String.IsNullOrEmpty(title))
        {
          throw new Exception("Input parameter 'title' is null or empty.");
        }

        using (DriveService driveService = DriveService.Create())
        {
          FileInfo parentInfo = driveService.GetFile(parentId);

          if (parentInfo == null)
          {
            throw new Exception("Parent no longer exists.");
          }

          var insertStream = new API.DriveService.InsertStream();

          insertStream.Init(parentInfo.Id, title, mimeType);

          insertStream.Visible = false;

          driveService.InsertFile(insertStream);

          while (!insertStream.Finished)
          {
            System.Threading.Thread.Sleep(250);
          }

          if (insertStream.Cancelled)
          {
            return null;
          }
          if (insertStream.Failed)
          {
            throw new LogException("Insert file could not complete - " + insertStream.ExceptionMessage, true, true);
          }

          FileInfo fileInfo = insertStream.FileInfo;

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
