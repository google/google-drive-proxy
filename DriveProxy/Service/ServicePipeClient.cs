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
using System.Text;
using DriveProxy.Utils;

namespace DriveProxy.Service
{
  internal class ServicePipeClient
  {
    protected static Pipe.Client _Pipe = null;

    protected static Exception _LastException = null;

    public static Exception LastException
    {
      get { return _LastException; }
      set
      {
        _LastException = value;

        Log.Error(_LastException, false);
      }
    }

    public static string LastExceptionMessage
    {
      get
      {
        if (HasException)
        {
          return LastException.Message;
        }

        return "";
      }
    }

    public static bool HasException
    {
      get
      {
        if (LastException != null)
        {
          return true;
        }

        return false;
      }
    }

    protected static bool Open()
    {
      try
      {
        return Open(false);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    protected static bool Open(string pipeName)
    {
      try
      {
        return Open(pipeName, false);
      }
      catch (Exception exception)
      {
        LastException = exception;

        return false;
      }
    }

    protected static bool Open(bool shortRetry)
    {
      try
      {
        return Open(ServicePipe.PipeName, shortRetry);
      }
      catch (Exception exception)
      {
        LastException = exception;

        return false;
      }
    }

    protected static bool Open(string pipeName, bool shortRetry)
    {
      try
      {
        Close();

        Log.Warning("Attempting to open pipe '" + pipeName + "'");

        _Pipe = Pipe.Client.Open(pipeName, System.IO.FileAccess.ReadWrite, shortRetry);

        if (_Pipe == null)
        {
          return false;
        }

        Log.Information("Successfully opened pipe '" + pipeName + "'");

        return true;
      }
      catch (Exception exception)
      {
        LastException = exception;

        return false;
      }
    }

    protected static void Close()
    {
      try
      {
        if (_Pipe == null)
        {
          return;
        }

        _Pipe.Close();
      }
      catch (Exception exception)
      {
        LastException = exception;
      }
    }

    public static void Dispose()
    {
      try
      {
        if (_Pipe == null)
        {
          return;
        }

        _Pipe.Dispose();
      }
      catch (Exception exception)
      {
        LastException = exception;
      }
      finally
      {
        _Pipe = null;
      }
    }

    public static bool CloseServer()
    {
      try
      {
        Log.Warning("Attempting to send ServicePipe process disconnect message");

        if (!Open())
        {
          throw LastException;
        }

        string message = "disconnect";

        Write(message);

        string result = Read();

        Close();

        if (result == null || result != "disconnecting")
        {
          throw new Exception("Received unexpected response from server.");
        }

        Log.Information("Successfully sent ServicePipe process disconnect message");

        return true;
      }
      catch (Exception exception)
      {
        LastException = exception;

        return false;
      }
      finally
      {
        Close();
      }
    }

    public static bool Ping()
    {
      try
      {
        return Ping(true);
      }
      catch (Exception exception)
      {
        LastException = exception;

        return false;
      }
    }

    protected static bool Ping(bool doRetry)
    {
      try
      {
        Log.Warning("Attempting to ping ServicePipe process");

        if (!Open(doRetry))
        {
          throw LastException;
        }

        string message = "ping";

        Write(message);

        string result = Read();

        Close();

        if (result == null || result != "hello")
        {
          throw new Exception("Received unexpected response from server.");
        }

        Log.Information("Successfully pinged ServicePipe process");

        return true;
      }
      catch (Exception exception)
      {
        LastException = exception;

        return false;
      }
      finally
      {
        Close();
      }
    }

    public static long GetProcessCount()
    {
      try
      {
        if (!Open())
        {
          throw LastException;
        }

        string message = "process count";

        Write(message);

        string result = Read();

        Close();

        long processCount = -1;

        if (result == null || !long.TryParse(result, out processCount))
        {
          throw new Exception("Received unexpected response from server.");
        }

        return processCount;
      }
      catch (Exception exception)
      {
        LastException = exception;

        return -1;
      }
      finally
      {
        Close();
      }
    }

    public static string Execute(string[] args)
    {
      try
      {
        Log.Warning("Attempting to invoke Execute against ServicePipe process");

        if (!Open())
        {
          throw LastException;
        }

        Write("redirect");

        string pipeName = Read();

        Close();

        if (String.IsNullOrEmpty(pipeName))
        {
          throw new Exception("Requested redirect did not return a pipe name.");
        }

        pipeName = "DriveProxy.Service_" + pipeName;

        if (!Open(pipeName))
        {
          return null;
        }

        var message = new StringBuilder();

        foreach (string t in args)
        {
          if (message.Length > 0)
          {
            message.Append(",");
          }

          message.Append(t);
        }

        Write(message.ToString());

        string result = Read();

        Close();

        Log.Information("Successfully invoked Execute against ServicePipe process");

        return result;
      }
      catch (Exception exception)
      {
        LastException = exception;

        throw exception;
      }
      finally
      {
        Close();
      }
    }

    protected static string Read()
    {
      try
      {
        Log.Information("Attempting to read message from ServicePipe process");

        string result = ServicePipe.Read(_Pipe);

        Log.Information("Successfully read message from ServicePipe process");

        return result;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    protected static void Write(string message)
    {
      try
      {
        Log.Warning("Attempting to write messsage to ServicePipe process");

        ServicePipe.Write(_Pipe, message);

        Log.Information("Successfully wrote messsage to ServicePipe process");
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }
  }
}
