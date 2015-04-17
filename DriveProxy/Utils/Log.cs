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
  [Flags]
  public enum LogType
  {
    None = 0,
    Information = 1<<0,
    Warning = 1<<1,
    Error = 1<<2,
    Performance = 1<<3,
    Debug = 1 << 4,
    All = Information | Warning | Error | Performance | Debug,
  }

  public class Log
  {
    public delegate void WriteEntryEventHandler(string dateTime, string message, LogType type);

    private static LogType _level = LogType.None;

    private static string _createdSource;

    private static string _source;

    private static string _lastError;

    private static DateTime _lastErrorTime = default(DateTime);

    public static LogType Level
    {
      get { return _level; }
      set { _level = value; }
    }

    public static string Source
    {
      get
      {
        if (String.IsNullOrEmpty(_source))
        {
          _source = System.Reflection.Assembly.GetEntryAssembly().Location;
          _source = System.IO.Path.GetFileNameWithoutExtension(_source);
        }

        return _source;
      }
      set { _source = value; }
    }

    public static string LastError
    {
      get { return _lastError; }
      protected set
      {
        _lastError = value;

        if (!String.IsNullOrEmpty(_lastError))
        {
          _lastErrorTime = DateTime.Now;

          WriteEntry(value, LogType.Error);
        }
      }
    }

    public static DateTime LastErrorTime
    {
      get { return _lastErrorTime; }
      protected set { _lastErrorTime = value; }
    }

    public static event WriteEntryEventHandler OnWriteEntry = null;

    public static void Error(Exception exception, bool continueThrow = true, bool breakWithDebugger = true)
    {
      try
      {
        var logException = exception as LogException;

        if (logException == null)
        {
          logException = new LogException(exception.Message, continueThrow, breakWithDebugger, exception);
        }
        else
        {
          if (!logException.ContinueThrow)
          {
            continueThrow = false;
          }

          if (!logException.BreakWithDebugger)
          {
            breakWithDebugger = false;
          }

          if (logException.TriedToBreak)
          {
            breakWithDebugger = false;
          }
        }

        if (!logException.AlreadyLogged)
        {
          Error(exception.Message, false);

          logException.AlreadyLogged = true;
        }
#if DEBUG
        if (breakWithDebugger)
        {
          logException.TriedToBreak = true;
        }
#endif
        exception = logException;
      }
      catch
      {
        Debugger.Break();
      }
#if DEBUG
      if (breakWithDebugger)
      {
        if (!Debugger.IsAttached)
        {
          System.Diagnostics.Debugger.Launch();
        }
        else
        {
          System.Diagnostics.Debugger.Break();
        }
      }
#endif

      if (continueThrow)
      {
        throw exception;
      }
    }

    public static void Error(string message, bool breakWithDebugger = false)
    {
      try
      {
        double totalSeconds = 0;
        bool bypass = false;

        if (!String.IsNullOrEmpty(message) &&
            !String.IsNullOrEmpty(LastError) &&
            message == LastError)
        {
          totalSeconds = DateTime.Now.Subtract(LastErrorTime).TotalSeconds;

          if (totalSeconds <= 1)
          {
            bypass = true;
            breakWithDebugger = false;
          }
        }

        if (!bypass)
        {
          LastError = message;
        }

#if DEBUG
        if (breakWithDebugger)
        {
          Debugger.Break();

          LastErrorTime = DateTime.Now;
        }
#endif
      }
      catch
      {
        Debugger.Break();
      }
    }

    public static void Information(string message)
    {
      try
      {
        WriteEntry(message, LogType.Information);
      }
      catch
      {
        Debugger.Break();
      }
    }

    public static void Debug(string message)
    {
      try
      {
        WriteEntry(message, LogType.Debug);
      }
      catch
      {
        Debugger.Break();
      }
    }

    public static void Performance(string message)
    {
      try
      {
        WriteEntry(message, LogType.Performance);
      }
      catch
      {
        Debugger.Break();
      }
    }

    public static void StartPerformance(ref Stopwatch stopWatch, string message, LogType logType = LogType.Warning)
    {
      try
      {
        WriteEntry(message, LogType.Performance);

        stopWatch.Restart();
      }
      catch
      {
        Debugger.Break();
      }
    }

    public static void EndPerformance(ref Stopwatch stopWatch, string message, LogType logType = LogType.Information)
    {
      try
      {
        stopWatch.Stop();

        WriteEntry(message + " (milliseconds " + stopWatch.ElapsedMilliseconds + ")", LogType.Performance);
      }
      catch
      {
        Debugger.Break();
      }
    }

    public static void Warning(string message)
    {
      try
      {
        WriteEntry(message, LogType.Warning);
      }
      catch
      {
        Debugger.Break();
      }
    }

    public static void WriteEntry(string message, LogType logType)
    {
      try
      {
        if ((_level & logType) == 0)
        {
          return;
        }

        try
        {
          if (_createdSource != Source)
          {
            if (!System.Diagnostics.EventLog.SourceExists(Source))
            {
              var sourceData =
                new System.Diagnostics.EventSourceCreationData(Source, "Application");

              System.Diagnostics.EventLog.CreateEventSource(sourceData);
            }

            _createdSource = Source;
          }

          var eventLogEntryType = System.Diagnostics.EventLogEntryType.Error;

          if (logType == LogType.Information)
          {
            eventLogEntryType = System.Diagnostics.EventLogEntryType.Information;
          }
          else if (logType == LogType.Warning)
          {
            eventLogEntryType = System.Diagnostics.EventLogEntryType.Warning;
          }

          System.Diagnostics.EventLog.WriteEntry(Source, message, eventLogEntryType);
        }
        catch (Exception e)
        {
          Log.Information(e.Message);
        }

        DateTime now = DateTime.Now;

        string nowValue = now.ToShortDateString() + " " +
                          now.Hour + ":" +
                          now.Minute.ToString().PadLeft(2, '0') + ":" +
                          now.Second.ToString().PadLeft(2, '0') + ":" +
                          now.Millisecond.ToString().PadLeft(3, '0');

#if DEBUG
        try
        {
          string consoleMessage = Source + " - " + nowValue + " - " + logType.ToString().ToUpper() + " - " + message;

          System.Diagnostics.Debug.WriteLine(consoleMessage);
          System.Diagnostics.Debug.Flush();
        }
        catch
        {
        }
#endif

        try
        {
          if (OnWriteEntry != null)
          {
            OnWriteEntry(nowValue, message, logType);
          }
        }
        catch
        {
        }
      }
      catch
      {
        Debugger.Break();
      }
    }

    public class Stopwatch : System.Diagnostics.Stopwatch
    {
    }
  }

  public class LogException : Exception
  {
    public bool AlreadyLogged = false;
    public bool BreakWithDebugger = true;
    public bool ContinueThrow = true;
    public bool TriedToBreak = false;

    public LogException(string message,
                        bool continueThrow = true,
                        bool breakWithDebugger = true,
                        Exception innerException = null) :
                          base(message, innerException)
    {
      ContinueThrow = continueThrow;
      BreakWithDebugger = breakWithDebugger;
    }
  }
}
