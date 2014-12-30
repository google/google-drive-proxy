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
using System.Xml.Linq;
using DriveProxy.Utils;

namespace DriveProxy.Service
{
  internal abstract class MethodInfo : XElementObject
  {
    public delegate void ReceivedHwndHandler(IntPtr hwnd);

    protected List<ParameterInfo> _parameters = null;
    protected ResultInfo _result = null;

    public virtual int ID
    {
      get { return (int)Type; }
    }

    public abstract MethodType Type { get; }

    public virtual string Name
    {
      get { return Type.ToString(); }
    }

    protected virtual List<ParameterInfo> Parameters
    {
      get { return _parameters ?? (_parameters = new List<ParameterInfo>()); }
    }

    protected virtual ResultInfo Result
    {
      get { return _result ?? (_result = new ResultInfo()); }
    }

    protected virtual void AddParameter(string name, Type type)
    {
      try
      {
        Parameters.Add(new ParameterInfo(name, type));
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public abstract string Invoke(string[] args);

    protected virtual void SetResult(Type type)
    {
      try
      {
        Result.Type = type;
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public override XElement ToXElement()
    {
      try
      {
        var element = new XElement("Method",
                                   new XAttribute("ID", ID),
                                   new XAttribute("Name", Name)
          );

        foreach (ParameterInfo parameter in Parameters)
        {
          XElement childElement = parameter.ToXElement();

          element.Add(childElement);
        }

        XElement childElement2 = Result.ToXElement();

        element.Add(childElement2);

        return element;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public event ReceivedHwndHandler OnReceivedHwnd = null;

    protected IntPtr ReceivedHwnd(string hwnd)
    {
      try
      {
        if (String.IsNullOrEmpty(hwnd))
        {
          return IntPtr.Zero;
        }

        long value = 0;

        if (!long.TryParse(hwnd, out value))
        {
          return IntPtr.Zero;
        }

        ReceivedHwnd(value);

        return new IntPtr(value);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return IntPtr.Zero;
      }
    }

    protected void ReceivedHwnd(long hwnd)
    {
      try
      {
        if (hwnd == 0)
        {
          return;
        }

        ReceivedHwnd(new IntPtr(hwnd));
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    protected void ReceivedHwnd(IntPtr hwnd)
    {
      try
      {
        if (hwnd == IntPtr.Zero)
        {
          return;
        }

        if (OnReceivedHwnd != null)
        {
          OnReceivedHwnd(hwnd);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }
  }
}
