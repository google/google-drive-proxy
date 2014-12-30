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

namespace DriveProxy.Utils
{
  internal class Mutex : IDisposable
  {
    protected System.Threading.Mutex _Mutex = null;

    public void Dispose()
    {
      try
      {
        try
        {
          Release();
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
        finally
        {
          _Mutex = null;
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public void Wait()
    {
      try
      {
        if (_Mutex == null)
        {
          _Mutex = new System.Threading.Mutex();
        }

        if (!_Mutex.WaitOne())
        {
          throw new Exception("Mutex wait timed out.");
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public void Release()
    {
      try
      {
        if (_Mutex != null)
        {
          _Mutex.ReleaseMutex();
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }
  }
}
