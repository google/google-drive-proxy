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
  internal class GetFilesMethodInfo : MethodInfo
  {
    public GetFilesMethodInfo()
    {
      try
      {
        AddParameter("Id", typeof (string));
        AddParameter("IgnoreTrash", typeof (bool));
        AddParameter("UpdateCachedData", typeof (bool));
        AddParameter("GetChildren", typeof (bool));
        SetResult(typeof (FileInfo));
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public override MethodType Type
    {
      get { return MethodType.GetFiles; }
    }

    public override string Invoke(string[] args)
    {
      try
      {
        if (args == null)
        {
          throw new Exception("Input parameter 'args' is null.");
        }

        if (args.Length != 5)
        {
          throw new Exception("Input parameter 'args' length does not equal 5.");
        }

        string fileId = args[0];
        bool ignoreTrash = true;
        bool updateCachedData = false;
        bool getChildren = true;
        bool ignoreError = true;

        if (args[1] == "0" || args[1].ToLower() == "false")
        {
          ignoreTrash = false;
        }

        if (args[2] == "1" || args[2].ToLower() == "true")
        {
          updateCachedData = true;
        }

        if (args[3] == "0" || args[3].ToLower() == "false")
        {
          getChildren = false;
        }

        if (args[4] == "0" || args[4].ToLower() == "false")
        {
          ignoreError = false;
        }

        FileInfo fileInfo = null;

        try
        {
          fileInfo = Invoke(fileId, ignoreTrash, updateCachedData, getChildren);

          if (fileInfo == null)
          {
            return "";
          }
        }
        catch (Exception exception)
        {
          if (!ignoreError)
          {
            StatusForm.Exception(exception);
          }

          throw exception;
        }

        return fileInfo.ToString();
      }
      catch (Exception exception)
      {
        Log.Error(exception);
        return null;
      }
    }

    public FileInfo Invoke(string fileId, bool ignoreTrash, bool updateCachedData, bool getChildren)
    {
      try
      {
        using (DriveService driveService = DriveService.Create())
        {
          FileInfo fileInfo = driveService.GetCachedFile(fileId, getChildren, updateCachedData);

          if (fileInfo == null)
          {
            throw new Exception("File no longer exists.");
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
