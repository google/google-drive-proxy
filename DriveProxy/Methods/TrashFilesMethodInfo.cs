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
  internal class TrashFileMethodInfo : MethodInfo
  {
    public TrashFileMethodInfo()
    {
      try
      {
        AddParameter("Count", typeof (string));
        AddParameter("Id(1, 2, etc.)", typeof (string));
        SetResult(typeof (string[]));
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public override MethodType Type
    {
      get { return MethodType.TrashFiles; }
    }

    public override string Invoke(string[] args)
    {
      try
      {
        if (args == null)
        {
          throw new Exception("Input parameter 'args' is null.");
        }

        if (args.Length < 2)
        {
          throw new Exception("Input parameter 'args' length is less than 2.");
        }

        List<string> ids = GetFileIds(args, 0);

        List<string> trashedIds = Invoke(ids);

        if (trashedIds == null)
        {
          return "";
        }

        string result = GetFileIdsAsXml(trashedIds);

        return result;
      }
      catch (Exception exception)
      {
        StatusForm.Exception(exception);

        Log.Error(exception);

        return null;
      }
    }

    public List<string> Invoke(List<string> fileIds)
    {
      try
      {
        using (DriveService driveService = DriveService.Create())
        {
          var trashedFileIds = new List<string>();
          var trashStreams = new List<API.DriveService.TrashStream>();

          foreach (string fileId in fileIds)
          {
            FileInfo fileInfo = driveService.GetFile(fileId);

            if (fileInfo != null)
            {
              var trashStream = new API.DriveService.TrashStream();

              trashStream.Init(fileInfo);

              trashStream.Visible = false;

              trashStreams.Add(trashStream);
            }

            trashedFileIds.Add(fileId);
          }

          driveService.TrashFiles(trashStreams);

          while (true)
          {
            bool finished = true;

            for (int i = 0; i < trashStreams.Count; i++)
            {
              API.DriveService.TrashStream trashStream = trashStreams[i];

              if (!trashStream.Finished)
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

          foreach (DriveService.TrashStream trashStream in trashStreams)
          {
            if (!trashStream.Completed)
            {
              trashedFileIds.Remove(trashStream.FileId);
            }
            else if (trashStream.Cancelled)
            {
            }
            else if (trashStream.Failed)
            {
              throw new LogException("Trash could not complete - " + trashStream.ExceptionMessage, true, true);
            }
          }

          return trashedFileIds;
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
