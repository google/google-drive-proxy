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
  internal class UntrashFileMethodInfo : MethodInfo
  {
    public UntrashFileMethodInfo()
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
      get { return MethodType.UntrashFiles; }
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

        List<string> untrashedIds = Invoke(ids);

        if (untrashedIds == null)
        {
          return "";
        }

        string result = GetFileIdsAsXml(untrashedIds);

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
          var untrashStreams = new List<API.DriveService.UntrashStream>();

          foreach (string fileId in fileIds)
          {
            FileInfo fileInfo = driveService.GetFile(fileId);

            if (fileInfo == null)
            {
              throw new Exception("File no longer exists.");
            }

            var untrashStream = new API.DriveService.UntrashStream();

            untrashStream.Init(fileInfo);

            untrashStream.Visible = false;

            untrashStreams.Add(untrashStream);
          }

          driveService.UntrashFiles(untrashStreams);

          while (true)
          {
            bool finished = true;

            for (int i = 0; i < untrashStreams.Count; i++)
            {
              API.DriveService.UntrashStream untrashStream = untrashStreams[i];

              if (!untrashStream.Finished)
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

          var untrashedFileIds = new List<string>();

          foreach (DriveService.UntrashStream untrashStream in untrashStreams)
          {
            if (untrashStream.Completed)
            {
              untrashedFileIds.Add(untrashStream.FileId);
            }
            else if (untrashStream.Cancelled)
            {
            }
            else if (untrashStream.Failed)
            {
              throw new LogException("Untrash could not complete - " + untrashStream.ExceptionMessage, true, true);
            }
          }

          return untrashedFileIds;
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
