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
  internal enum ErrorInfoType
  {
    None = 0,
    Authentication = 1,
  }

  internal class ErrorInfo : XElementObject
  {
    protected List<DetailInfo> _details = null;
    protected string _message = "";
    protected ErrorInfoType _type = ErrorInfoType.None;

    public ErrorInfo()
    {
    }

    public ErrorInfo(string message)
    {
      try
      {
        _message = message;
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public ErrorInfo(Exception exception)
    {
      try
      {
        if (exception is AuthenticationException)
        {
          Copy(((AuthenticationException)exception).ErrorInfo);
        }

        _message = exception.Message;
      }
      catch (Exception exception2)
      {
        Log.Error(exception2);
      }
    }

    public ErrorInfo(ErrorInfoType type)
    {
      try
      {
        _type = type;
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public ErrorInfo(ErrorInfoType type, string message)
    {
      try
      {
        _type = type;
        _message = message;
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public ErrorInfo(ErrorInfoType type, Exception exception)
    {
      try
      {
        if (exception is AuthenticationException)
        {
          Copy(((AuthenticationException)exception).ErrorInfo);
        }

        _type = type;
        _message = exception.Message;
      }
      catch (Exception exception2)
      {
        Log.Error(exception2);
      }
    }

    public ErrorInfoType Type
    {
      get { return _type; }
      set { _type = value; }
    }

    public string Message
    {
      get { return _message; }
      set { _message = value; }
    }

    public List<DetailInfo> Details
    {
      get { return _details ?? (_details = new List<DetailInfo>()); }
    }

    public void AddDetail(string message)
    {
      try
      {
        Details.Add(new DetailInfo(message));
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public void Copy(ErrorInfo errorInfo)
    {
      try
      {
        _type = errorInfo.Type;
        _message = errorInfo.Message;

        for (int i = 0; i < errorInfo.Details.Count; i++)
        {
          DetailInfo detailInfo = errorInfo.Details[i].Copy();

          Details.Add(detailInfo);
        }
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
        var element = new XElement("Error",
                                   new XAttribute("Type", (int)Type),
                                   new XAttribute("Message", Message)
          );

        foreach (DetailInfo detail in Details)
        {
          XElement childElement = detail.ToXElement();

          element.Add(childElement);
        }

        return element;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }
  }

  internal class ErrorInfoException : Exception
  {
    public ErrorInfo ErrorInfo = null;

    public ErrorInfoException(ErrorInfo errorInfo)
    {
      try
      {
        ErrorInfo = errorInfo;
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public override string Message
    {
      get { return ErrorInfo.Message; }
    }

    public override string ToString()
    {
      try
      {
        return ErrorInfo.ToString();
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }
  }

  internal class AuthenticationException : ErrorInfoException
  {
    public AuthenticationException(string message)
      : base(new ErrorInfo(ErrorInfoType.Authentication, message))
    {
    }
  }
}
