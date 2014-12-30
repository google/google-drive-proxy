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
  internal class StopWatch
  {
    private DateTime _startDateTime = default(DateTime);
    private bool _isRunning;

    private string _processTime = "";
    private TimeSpan _timeSpan;
    private double _totalSeconds;

    public StopWatch()
    {
      try
      {
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public TimeSpan TimeSpan
    {
      get { return _timeSpan; }
    }

    public string ProcessTime
    {
      get { return _processTime; }
    }

    public double TotalSeconds
    {
      get { return _totalSeconds; }
    }

    public bool IsRunning
    {
      get { return _isRunning; }
    }

    public string RemainingProcessTime(double part, double whole)
    {
      try
      {
        TimeSpan timeSpan = RemainingTimeSpan(part, whole);

        string processTime = timeSpan.ToString();

        int index = processTime.LastIndexOf(":");

        if (index > -1)
        {
          string seconds = processTime.Substring(index + 1, processTime.Length - (index + 1));

          double totalSeconds = 0;

          Double.TryParse(seconds, out totalSeconds);

          int tempSeconds = Convert.ToInt32(totalSeconds);

          if (tempSeconds == 60)
          {
            tempSeconds = 59;
          }

          seconds = tempSeconds.ToString().PadLeft(2, '0');

          processTime = processTime.Substring(0, index) + ":" + seconds;
        }

        return processTime;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public TimeSpan RemainingTimeSpan(double part, double whole)
    {
      try
      {
        Update();

        double percent = 0;

        if (part > 0)
        {
          percent = Math.IsWhatPercentOf(part, whole) / 100;
        }
        else
        {
          percent = 1;
        }

        double totalTicks = 0;

        if (percent > 0)
        {
          totalTicks = TimeSpan.Ticks / percent;
        }

        var timeSpan2 = new TimeSpan(Convert.ToInt64(totalTicks));

        totalTicks = totalTicks - TimeSpan.Ticks;

        var timeSpan = new TimeSpan(Convert.ToInt64(totalTicks));

        return timeSpan;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return default(TimeSpan);
      }
    }

    public bool Start()
    {
      try
      {
        if (IsRunning)
        {
          Log.Error("Object is already running.");

          return false;
        }

        _processTime = "";
        _totalSeconds = 0;
        _isRunning = true;

        _startDateTime = DateTime.Now;

        return true;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    public bool Stop()
    {
      try
      {
        if (!IsRunning)
        {
          return true;
        }

        bool result = Update();

        _isRunning = false;

        return result;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    public bool Update()
    {
      try
      {
        if (!IsRunning)
        {
          return false;
        }

        _timeSpan = DateTime.Now - _startDateTime;

        _totalSeconds = TimeSpan.TotalSeconds;
        _processTime = TimeSpan.ToString();

        int index = _processTime.LastIndexOf(":");

        if (index > -1)
        {
          string seconds = _processTime.Substring(index + 1, _processTime.Length - (index + 1));

          double totalSeconds = 0;

          Double.TryParse(seconds, out totalSeconds);

          int tempSeconds = Convert.ToInt32(totalSeconds);

          if (tempSeconds == 60)
          {
            tempSeconds = 59;
          }

          seconds = tempSeconds.ToString().PadLeft(2, '0');

          _processTime = _processTime.Substring(0, index) + ":" + seconds;
        }

        return true;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }
  }
}
