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
using System.IO;

namespace DriveProxy.Utils
{
  internal class IniFile
  {
    private string _filePath = "";

    public IniFile()
    {
    }

    public IniFile(string filePath)
    {
      try
      {
        Open(filePath);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public string FilePath
    {
      get { return _filePath; }
    }

    public bool IsOpen
    {
      get
      {
        if (_filePath != "")
        {
          return true;
        }

        return false;
      }
    }

    public void Close()
    {
      try
      {
        _filePath = "";
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public string GetValue(string section, string key)
    {
      try
      {
        if (String.IsNullOrEmpty(FilePath))
        {
          throw new Exception("FilePath is blank.");
        }

        if (!System.IO.File.Exists(FilePath))
        {
          throw new Exception("FilePath " + FilePath + " does not exist.");
        }

        if (String.IsNullOrEmpty(section))
        {
          throw new Exception("Section is blank.");
        }

        if (String.IsNullOrEmpty(key))
        {
          throw new Exception("Key is blank.");
        }

        string[] lines = System.IO.File.ReadAllLines(FilePath);

        bool foundSection = false;

        foreach (string line in lines)
        {
          if (!foundSection)
          {
            if (String.Equals(line, "[" + section + "]", StringComparison.CurrentCultureIgnoreCase))
            {
              foundSection = true;
            }
          }
          else
          {
            string[] values = line.Split('=');

            if (values.Length != 2)
            {
              return "";
            }

            if (String.Equals(values[0], key, StringComparison.CurrentCultureIgnoreCase))
            {
              return values[1];
            }
          }
        }

        return "";
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public void Open(string filePath)
    {
      try
      {
        if (String.IsNullOrEmpty(filePath))
        {
          throw new Exception("FilePath is blank.");
        }

        this._filePath = filePath;
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public void SetValue(string section, string key, string value)
    {
      try
      {
        if (String.IsNullOrEmpty(FilePath))
        {
          throw new Exception("FilePath is blank.");
        }

        if (String.IsNullOrEmpty(section))
        {
          throw new Exception("Section is blank.");
        }

        if (String.IsNullOrEmpty(key))
        {
          throw new Exception("Key is blank.");
        }

        if (value == null)
        {
          value = "";
        }

        if (!System.IO.File.Exists(FilePath))
        {
          System.IO.FileStream fileStream = null;

          try
          {
            fileStream = new System.IO.FileStream(FilePath, FileMode.CreateNew, FileAccess.Write, FileShare.Write);
          }
          catch (Exception exception)
          {
            throw exception;
          }
          finally
          {
            if (fileStream != null)
            {
              fileStream.Close();
              fileStream.Dispose();
              fileStream = null;
            }
          }
        }

        string[] lines = System.IO.File.ReadAllLines(FilePath);
        var lineList = new List<string>();

        bool foundSection = false;
        bool foundKey = false;

        foreach (string t in lines)
        {
          string line = t;

          if (!foundSection)
          {
            if (String.Equals(line, "[" + section + "]", StringComparison.CurrentCultureIgnoreCase))
            {
              foundSection = true;
            }
          }
          else if (!foundKey)
          {
            string[] values = line.Split('=');

            if (values.Length != 2)
            {
              string newLine = key + "=" + value;

              lineList.Add(newLine);

              foundKey = true;
            }
            else if (String.Equals(values[0], key, StringComparison.CurrentCultureIgnoreCase))
            {
              line = values[0] + "=" + value;

              foundKey = true;
            }
          }

          lineList.Add(line);
        }

        if (!foundSection)
        {
          lineList.Add("[" + section + "]");
          lineList.Add(key + "=" + value);
        }
        else if (!foundKey)
        {
          lineList.Add(key + "=" + value);
        }

        System.IO.File.WriteAllLines(FilePath, lineList.ToArray());
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }
  }
}
