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
using DriveProxy.Utils;

namespace DriveProxy.Service
{
  internal class ServicePipeServer : IDisposable
  {
    public enum ExecuteResult
    {
      Ok,
      Exception,
      DisconnectClient,
      DisconnectServer
    }

    protected bool _Enabled = false;
    protected Exception _LastException = null;
    private Pipe.Server _pipe;
    protected int _ProcessCount = 0;

    public bool Enabled
    {
      get { return _Enabled; }
      set { _Enabled = value; }
    }

    public bool Processing
    {
      get
      {
        if (ProcessCount > 0)
        {
          return true;
        }

        return false;
      }
    }

    public int ProcessCount
    {
      get { return _ProcessCount; }
    }

    public Exception LastException
    {
      get { return _LastException; }
      set
      {
        _LastException = value;

        Log.Error(_LastException, false);
      }
    }

    public string LastExceptionMessage
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

    public bool HasException
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

    public void Dispose()
    {
      try
      {
        if (_pipe == null)
        {
          return;
        }

        _pipe.Dispose();

        _pipe = null;
      }
      catch (Exception exception)
      {
        LastException = exception;
      }
    }

    public bool Process()
    {
      try
      {
        return Process(ServicePipe.PipeName, true);
      }
      catch (Exception exception)
      {
        LastException = exception;

        return false;
      }
    }

    protected bool Process(string pipeName, bool waitForDisconnectMessage)
    {
      try
      {
        _ProcessCount++;

        try
        {
          _Enabled = true;

          DateTime started = DateTime.Now;

          while (Enabled)
          {
            if (IsCreated() || Create(pipeName))
            {
              if (Connect())
              {
                ServicePipeServer.ExecuteResult executeResult = Execute();

                if (!waitForDisconnectMessage)
                {
                  break;
                }

                if (executeResult == ServicePipeServer.ExecuteResult.DisconnectServer)
                {
                  break;
                }
              }
              else
              {
                Log.Warning("Could not connect to pipe " + pipeName);

                System.Threading.Thread.Sleep(10);
              }
            }
            else
            {
              Log.Warning("Could not create pipe " + pipeName);

              System.Threading.Thread.Sleep(10);
            }

            if (!waitForDisconnectMessage)
            {
              TimeSpan timeSpan = DateTime.Now.Subtract(started);

              if (timeSpan.TotalSeconds >= 15)
              {
                Log.Error("Closing pipe " + _pipe.Name + " (client did not connect)");

                break;
              }
            }
          }

          Close();
        }
        finally
        {
          _ProcessCount--;
        }

        return true;
      }
      catch (Exception exception)
      {
        LastException = exception;

        return false;
      }
    }

    protected void Process_Thread(object args)
    {
      try
      {
        var pipeName = (string)args;

        var server = new ServicePipeServer();

        server.Process(pipeName, false);
      }
      catch (Exception exception)
      {
        LastException = exception;
      }
    }

    public bool IsCreated()
    {
      try
      {
        if (_pipe == null)
        {
          return false;
        }

        return _pipe.IsCreated;
      }
      catch (Exception exception)
      {
        LastException = exception;

        return false;
      }
    }

    protected bool Create(string pipeName)
    {
      try
      {
        Close();

        Log.Information("Attempting to create pipe '" + pipeName + "'");

        _pipe = Pipe.Server.Create(pipeName, System.IO.FileAccess.ReadWrite);

        Log.Information("Successfully created pipe '" + pipeName + "'");

        return true;
      }
      catch (Exception exception)
      {
        LastException = exception;

        return false;
      }
    }

    public bool IsConnected()
    {
      try
      {
        if (_pipe == null)
        {
          return false;
        }

        return _pipe.IsConnected;
      }
      catch (Exception exception)
      {
        LastException = exception;

        return false;
      }
    }

    protected bool Connect()
    {
      try
      {
        if (_pipe == null)
        {
          throw new Exception("Pipe has not been created.");
        }

        if (!_pipe.Connect())
        {
          return false;
        }

        return true;
      }
      catch (Exception exception)
      {
        LastException = exception;

        return false;
      }
    }

    public void Disconnect()
    {
      try
      {
        if (_pipe == null)
        {
          return;
        }

        _pipe.Disconnect();
      }
      catch (Exception exception)
      {
        LastException = exception;
      }
    }

    public void Close()
    {
      try
      {
        if (_pipe == null)
        {
          return;
        }

        _pipe.Close();
      }
      catch (Exception exception)
      {
        LastException = exception;
      }
    }

    protected ExecuteResult Execute()
    {
      try
      {
        if (_pipe == null)
        {
          throw new Exception("Pipe has not been created.");
        }

        Log.Debug("Attempting to handle Service.Execute");

        string result = null;
        var executeResult = ExecuteResult.Ok;

        string message = Read();

        if (message == "ping")
        {
          Log.Information("Received 'ping' message");

          result = "hello";
        }
        else if (message == "redirect")
        {
          Log.Information("Received 'redirect' message");

          Guid guid = Guid.NewGuid();

          result = guid.ToString().Replace("-", "");

          string pipeName = "DriveProxy.Service_" + result;

          Log.Information("Redirecting to new pipe " + pipeName);

          new System.Threading.Thread(Process_Thread).Start(pipeName);
        }
        else if (message == "process count")
        {
          Log.Information("Received 'process count' message");

          result = _ProcessCount.ToString();
        }
        else if (message == "close")
        {
          Log.Information("Received 'close' message");

          executeResult = ExecuteResult.DisconnectClient;
        }
        else if (message == "disconnect")
        {
          Log.Information("Received 'disconnect' message");

          result = "disconnecting";
          executeResult = ExecuteResult.DisconnectServer;
        }
        else
        {
          Log.Debug("Received 'execute' message");

          string[] args = message.Split(',');

          result = Service.Execute(args);
        }

        if (result != null)
        {
          Write(result);
        }

        Log.Debug("Successfully handled Service.Execute");

        return executeResult;
      }
      catch (Exception exception)
      {
        LastException = exception;

        return ExecuteResult.Exception;
      }
      finally
      {
        Disconnect();
      }
    }

    protected string Read()
    {
      try
      {
        Log.Debug("Attempting to read message from ServicePipe process");

        string result = ServicePipe.Read(_pipe);

        Log.Debug(String.Format("Successfully read message: \"{0}\" from ServicePipe process", result));

        return result;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    protected void Write(string message)
    {
      try
      {
        if (message.Length > 100)
        {
          Log.Information(String.Format("Sending: \"{0}\" to ServicePipe process",
                                        message.Substring(0, 100) + "...TRUNCATED"));
        }
        else
        {
          Log.Information(String.Format("Sending: \"{0}\" to ServicePipe process", message));
        }

        ServicePipe.Write(_pipe, message);

        Log.Debug("Successfully wrote message to ServicePipe process");
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }
  }
}
