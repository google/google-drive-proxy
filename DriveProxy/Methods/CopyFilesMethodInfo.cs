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
using DriveProxy.API;
using DriveProxy.Forms;
using DriveProxy.Service;
using DriveProxy.Utils;

namespace DriveProxy.Methods
{
  internal class CopyFileMethodInfo : MethodInfo
  {
    public CopyFileMethodInfo()
    {
      try
      {
        AddParameter("ParentId", typeof(string));
        AddParameter("Count", typeof(string));
        AddParameter("Id(1, 2, etc.)", typeof(string));
        SetResult(typeof(string[]));
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public override MethodType Type
    {
      get { return MethodType.CopyFiles; }
    }

    public override string Invoke(string[] args)
    {
      try
      {
        if (args == null)
        {
          throw new Exception("Input parameter 'args' is null.");
        }

        if (args.Length < 1)
        {
          throw new Exception("Input parameter 'args' length is less than 1.");
        }

        string parentId = args[0];
        List<string> ids = GetFileIds(args, 1);

        List<string> copiedIds = Invoke(parentId, ids);

        if (copiedIds == null)
        {
          return "";
        }

        string result = GetFileIdsAsXml(copiedIds);

        return result;
      }
      catch (Exception exception)
      {
        StatusForm.Exception(exception);

        Log.Error(exception);

        return null;
      }
    }

    public List<string> Invoke(string parentId, List<string> fileIds)
    {
      try
      {
        using (DriveService driveService = DriveService.Create())
        {
          FileInfo parent = driveService.GetFile(parentId);

          if (parent == null)
          {
            throw new Exception("Parent no longer exists.");
          }

          var copyStreams = new List<DriveProxy.API.DriveService.CopyStream>();

          foreach (string fileId in fileIds)
          {
            FileInfo fileInfo = driveService.GetFile(fileId);

            if (fileInfo == null)
            {
              throw new Exception("File no longer exists.");
            }

            var copyStream = new DriveProxy.API.DriveService.CopyStream();

            copyStream.Init(parent.Id, fileInfo);

            copyStreams.Add(copyStream);
          }

          driveService.CopyFiles(copyStreams);

          while (true)
          {
            bool finished = true;

            for (int i = 0; i < copyStreams.Count; i++)
            {
              DriveProxy.API.DriveService.CopyStream copyStream = copyStreams[i];

              if (!copyStream.Finished)
              {
                finished = false;
                break;
              }
            }

            if (finished)
            {
              break;
            }

            System.Threading.Thread.Sleep(250);
          }

          var copiedFileIds = new List<string>();

          foreach (DriveService.CopyStream copyStream in copyStreams)
          {
            if (copyStream.Completed)
            {
              copiedFileIds.Add(copyStream.FileId);
            }
            else if (copyStream.Cancelled)
            {
            }
            else if (copyStream.Failed)
            {
              throw new LogException("Copy could not complete - " + copyStream.ExceptionMessage, true, true);
            }
          }

          return copiedFileIds;
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
