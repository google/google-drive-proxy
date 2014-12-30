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
using System.Text;
using System.Xml.Linq;
using DriveProxy.Utils;

namespace DriveProxy.Service
{
  public abstract class XElementObject
  {
    public override string ToString()
    {
      try
      {
        XElement element = ToXElement();

        return element.ToString();
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public abstract XElement ToXElement();

    public static string ToString(Array xElementObjects, bool addRoot = false)
    {
      try
      {
        var result = new StringBuilder();

        if (addRoot)
        {
          result.AppendLine("<root>");
        }

        if (xElementObjects != null)
        {
          for (int i = 0; i < xElementObjects.Length; i++)
          {
            var xElementObject = xElementObjects.GetValue(i) as XElementObject;

            if (xElementObject != null)
            {
              if (result.Length > 0)
              {
                result.AppendLine();
              }

              result.Append(xElementObject);
            }
          }
        }

        if (addRoot)
        {
          result.AppendLine("</root>");
        }

        return result.ToString();
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public static List<string> GetFileIds(string[] args, int startIndex)
    {
      try
      {
        if (args == null)
        {
          throw new Exception("Input parameter 'args' is null.");
        }

        if (args.Length <= startIndex)
        {
          throw new Exception("Input parameter 'args' length is less than " + startIndex + ".");
        }

        int index = startIndex;
        int count = 0;

        if (!int.TryParse(args[index], out count))
        {
          throw new Exception("Input parameter 'args[0]' does not contain a valid int value.");
        }

        index++;

        if (count != args.Length - index)
        {
          throw new Exception("Input parameter 'args' contains unexpected values (count=" + count + ", args.Length=" +
                              args.Length + ")");
        }

        var ids = new List<string>();

        for (int i = index; i < args.Length; i++)
        {
          string id = args[i];

          ids.Add(id);
        }

        return ids;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public static string GetFileIdsAsXml(List<string> ids)
    {
      try
      {
        var output = new StringBuilder();

        foreach (string id in ids)
        {
          var element = new System.Xml.Linq.XElement("Id", id);

          if (output.Length > 0)
          {
            output.Append("  ");
          }

          output.AppendLine(element.ToString());
        }

        return output.ToString();
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }
  }
}
