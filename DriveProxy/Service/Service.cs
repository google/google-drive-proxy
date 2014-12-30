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
using System.Linq;
using System.Text;
using DriveProxy.Utils;

namespace DriveProxy.Service
{
  internal class Service
  {
    public delegate void ExecuteEventHanlder();

    public static ExecuteEventHanlder OnExecute = null;

    protected static List<MethodInfo> _methods = null;

    public static List<MethodInfo> Methods
    {
      get
      {
        try
        {
          if (_methods == null)
          {
            _methods = new List<MethodInfo>();

            Type methodInfoType = typeof (MethodInfo);
            Type[] types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();

            foreach (Type type in types)
            {
              if (type.BaseType != null && type.BaseType.Equals(methodInfoType))
              {
                System.Reflection.ConstructorInfo constructorInfo = type.GetConstructor(new Type[0]);

                if (constructorInfo == null)
                {
                  throw new Exception("Default constructor does not exist for type '" + type.FullName + "'.");
                }

                object instance = null;

                try
                {
                  instance = type.Assembly.CreateInstance(type.FullName);
                }
                catch (Exception exception)
                {
                  throw new Exception("Could not create instance of '" + type.FullName + "' - " + exception.Message,
                                      exception);
                }

                if (instance == null)
                {
                  throw new Exception("Could not create instance of '" + type.FullName + "'.");
                }

                var methodInfo = instance as MethodInfo;

                if (methodInfo == null)
                {
                  throw new Exception("Could not create MethodInfo '" + type.FullName + "'.");
                }

                foreach (MethodInfo method in _methods)
                {
                  if (method.Type == methodInfo.Type)
                  {
                    throw new Exception("There is more than one MethodType of '" + method.Type + "' defined.");
                  }
                }

                _methods.Add(methodInfo);
              }
            }
          }

          return _methods;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }
    }

    protected static string GetMethodsAsString()
    {
      try
      {
        return XElementObject.ToString(Methods.ToArray());
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public override string ToString()
    {
      try
      {
        return GetMethodsAsString();
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public static string Execute(string serviceAccountUser, string[] args)
    {
      try
      {
        return Execute(args);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public static string Execute(string[] args)
    {
      try
      {
        var parameters = new List<string>();

        if (args != null)
        {
          for (int i = 0; i < args.Length; i++)
          {
            string arg = args[i];

            if (arg != null)
            {
              parameters.Add(arg);
            }
          }
        }

        return Execute(parameters);
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);

        var errorInfo = new ErrorInfo(exception);

        return errorInfo.ToString();
      }
    }

    public static string Execute(List<string> parameters)
    {
      try
      {
        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

        try
        {
          MethodInfo method = null;

          try
          {
            if (parameters == null || parameters.Count == 0)
            {
              throw new Exception("No arguments where found.");
            }

            if (parameters.Any(t => t == null))
            {
              throw new Exception("Input parameter 'parameters' contains null values.");
            }

            if (String.Equals(parameters[0], "Debug", StringComparison.CurrentCultureIgnoreCase))
            {
              System.Diagnostics.Debugger.Launch();

              parameters.RemoveAt(0);

              if (parameters.Count == 0)
              {
                throw new Exception("No arguments where found besides debug.");
              }
            }

            var methodType = MethodType.Unknown;

            if (!Enum.TryParse(parameters[0], out methodType))
            {
              throw new Exception("Input parameter 'args' does not contain a valid method ID {" + parameters[0] +
                                  "} at first position.");
            }

            foreach (MethodInfo t in Methods)
            {
              method = t;

              if (method.Type == methodType)
              {
                break;
              }

              method = null;
            }

            if (method == null)
            {
              throw new Exception("Input parameter 'args' specified method ID '" + (int)methodType +
                                  "' which was not found.");
            }

            parameters.RemoveAt(0);
          }
          catch (Exception exception)
          {
            Log.Error(exception, false);

            var errorInfo = new ErrorInfo(exception);

            errorInfo.AddDetail("Expected format:  MethodID  Parameter1  Parameter2  etc...");

            var builder = new StringBuilder();

            builder.Append(errorInfo);
            builder.AppendLine();
            builder.Append(GetMethodsAsString());

            return builder.ToString();
          }

          var stopWatch = new Log.Stopwatch();

          Log.StartPerformance(ref stopWatch, "Begin - Invoking method ID " + method.ID + " (" + method.Name + ")...");

          Log.Information("Received: " + parameters.Aggregate(method.Name, (current, t) => current + (" " + t)));
          try
          {
            if (OnExecute != null)
            {
              try
              {
                OnExecute();
              }
              catch
              {
              }
            }

            return method.Invoke(parameters.ToArray());
          }
          catch (Exception exception)
          {
            Log.Error(exception, false);

            var errorInfo = new ErrorInfo(exception);

            errorInfo.AddDetail("Attempted to Invoke method ID " + method.ID + " (" + method.Name + ")");

            var builder = new StringBuilder();

            builder.Append(errorInfo);

            return builder.ToString();
          }
          finally
          {
            Log.EndPerformance(ref stopWatch,
                               "End - Invoking method ID " + method.ID + " (" + method.Name + ")",
                               LogType.Warning);
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);

          var errorInfo = new ErrorInfo(exception);

          var builder = new StringBuilder();

          builder.Append(errorInfo);
          builder.AppendLine();
          builder.Append(GetMethodsAsString());

          return builder.ToString();
        }
        finally
        {
          System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);

        var errorInfo = new ErrorInfo(exception);

        return errorInfo.ToString();
      }
    }
  }
}
