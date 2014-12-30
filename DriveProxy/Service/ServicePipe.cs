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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using DriveProxy.API;
using DriveProxy.Utils;

namespace DriveProxy.Service
{
  public class ServicePipe
  {
    public const string ProcessName = "DriveProxy.Service";
    public static System.Text.Encoding PipeEncoding = System.Text.Encoding.Unicode;
    public static int PipeEncodingSize = 2;

    protected static uint _SessionId = 0;

    protected static string _PipeName = DriveService.Settings.ServicePipeName;

    protected static uint SessionId
    {
      get
      {
        if (_SessionId == 0)
        {
          uint processId = Dll.GetCurrentProcessId();

          if (!Dll.ProcessIdToSessionId(processId, out _SessionId))
          {
            _SessionId = 0;
          }
        }

        return _SessionId;
      }
    }

    public static string PipeName
    {
      get { return _PipeName + "_" + UserName + "." + UserDomainName + "." + SessionId; }
    }

    public static string UserName
    {
      get { return Environment.UserName; }
    }

    public static string UserDomainName
    {
      get { return Environment.UserDomainName; }
    }

    protected static string GetPipeName(string pipeName)
    {
      return PipeName + "." + pipeName;
    }

    public static System.Diagnostics.Process[] GetServerProcesses(bool forCurrentUserOnly)
    {
      try
      {
        return UserProcess.GetProcessesByName(ProcessName, forCurrentUserOnly);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public static bool IsServerRunning(bool forCurrentUserOnly)
    {
      try
      {
        return UserProcess.IsRunning(ProcessName, forCurrentUserOnly);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    public static bool StopServer(bool forCurrentUserOnly)
    {
      try
      {
        System.Diagnostics.Process[] processes = GetServerProcesses(forCurrentUserOnly);

        if (processes != null && processes.Length > 0)
        {
          foreach (Process t in processes)
          {
            try
            {
              t.Kill();
            }
            catch (Exception exception)
            {
              Log.Error(exception, false);
            }
          }

          System.Threading.Thread.Sleep(500);
        }

        if (ServicePipe.IsServerRunning(forCurrentUserOnly))
        {
          return false;
        }

        return true;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    public static string Read(Pipe pipe)
    {
      try
      {
        Exception lastException = null;

        for (int i = 0; i < 10; i++)
        {
          try
          {
            lastException = null;

            int length = 8;

            byte[] bytes = pipe.Read(length * PipeEncodingSize);

            if (bytes == null)
            {
              throw new Exception("No data was read.");
            }

            string header = PipeEncoding.GetString(bytes);

            if (!int.TryParse(header.Trim(), out length) || length < 0)
            {
              throw new Exception("Data read contained an unexpected header.");
            }

            if (length == 0)
            {
              return "";
            }

            bytes = pipe.Read(length * PipeEncodingSize);

            if (bytes == null)
            {
              throw new Exception("No messsage following header was read.");
            }

            string message = PipeEncoding.GetString(bytes);

            if (message == null)
            {
              throw new Exception("Received null value from Pipe read.");
            }

            return message;
          }
          catch (Exception exception)
          {
            lastException = exception;
          }

          System.Threading.Thread.Sleep(150);
        }

        throw lastException;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public static void Write(Pipe pipe, string message)
    {
      try
      {
        Exception lastException = null;

        for (int i = 0; i < 10; i++)
        {
          try
          {
            lastException = null;

            string header = message.Length.ToString().PadRight(8);

            var stringBuilder = new StringBuilder();

            stringBuilder.Append(header);
            stringBuilder.Append(message);

            byte[] bytes = PipeEncoding.GetBytes(stringBuilder.ToString());

            pipe.Write(bytes);

            return;
          }
          catch (Exception exception)
          {
            lastException = exception;
          }

          System.Threading.Thread.Sleep(150);
        }

        throw lastException;
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    private class Dll
    {
      [DllImport("kernel32.dll")]
      public static extern uint GetCurrentProcessId();

      [DllImport("kernel32.dll")]
      public static extern bool ProcessIdToSessionId(uint dwProcessId, out uint pSessionId);
    }
  }
}
