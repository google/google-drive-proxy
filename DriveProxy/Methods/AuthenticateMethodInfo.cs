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
  internal class AuthenticateMethodInfo : MethodInfo
  {
    public AuthenticateMethodInfo()
    {
      try
      {
        SetResult(typeof(void));
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public override MethodType Type
    {
      get { return MethodType.Authenticate; }
    }

    public override string Invoke(string[] args)
    {
      try
      {
        Invoke();

        return "";
      }
      catch (Exception exception)
      {
        StatusForm.Exception(exception);

        Log.Error(exception);

        return null;
      }
    }

    public void Invoke(bool signout = false)
    {
      try
      {
        if (signout)
        {
          DriveService.Signout();
        }

        DriveService.Authenticate(false);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }
  }
}
