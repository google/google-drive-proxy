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
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace DriveProxy.Utils
{
  internal enum RegistryKeyType
  {
    ClassesRoot,
    CurrentConfig,
    CurrentUser,
    LocalMachine,
    PerformanceData,
    Users,
  }

  internal enum RegistryNodeType
  {
    HKEY_CLASSES_ROOT,
    HKEY_CURRENT_CONFIG,
    HKEY_CURRENT_USER,
    HKEY_LOCAL_MACHINE,
    HKEY_PERFORMANCE_DATA,
    HKEY_USERS,
  }

  internal class Registry : IDisposable
  {
    private RegistryKey _key;
    private RegistryKeyType _keyType = RegistryKeyType.LocalMachine;

    private string _subKey = "";

    public Registry()
    {
    }

    public Registry(RegistryKeyType keyType, bool createIfDoesNotExist = false)
    {
      Open(keyType, createIfDoesNotExist);
    }

    public Registry(RegistryKeyType keyType, string subKey, bool createIfDoesNotExist = false)
    {
      Open(keyType, subKey, createIfDoesNotExist);
    }

    public RegistryKeyType KeyType
    {
      get { return _keyType; }
    }

    public RegistryKey Key
    {
      get { return _key; }
    }

    public string SubKey
    {
      get { return _subKey; }
    }

    public bool IsOpen
    {
      get
      {
        if (_key != null)
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
        Close();
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    public void Close()
    {
      try
      {
        if (_key != null)
        {
          _key.Close();
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
      finally
      {
        _key = null;
      }
    }

    public void Delete()
    {
      try
      {
        ValidateIsOpen();

        Key.DeleteSubKey(SubKey);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public void DeleteValue(string name)
    {
      try
      {
        ValidateIsOpen();

        if (name == null)
        {
          throw new Exception("Input parameter 'name' is blank.");
        }

        Key.DeleteValue(name);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public object GetDefaultValue(object defaultValue = null)
    {
      try
      {
        return GetValue("", defaultValue);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public object GetValue(string name, object defaultValue = null)
    {
      try
      {
        ValidateIsOpen();

        if (name == null)
        {
          throw new Exception("Input parameter 'name' is blank.");
        }

        object value = Key.GetValue(name, defaultValue);

        if (value == null)
        {
          return defaultValue;
        }

        return value;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public string[] GetSubKeyNames()
    {
      try
      {
        ValidateIsOpen();

        string[] subKeyNames = _key.GetSubKeyNames();

        return subKeyNames;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public string[] GetValueNames()
    {
      try
      {
        ValidateIsOpen();

        string[] valueNames = _key.GetValueNames();

        return valueNames;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public bool Open(bool createIfDoesNotExist = false)
    {
      try
      {
        return Open(KeyType, SubKey, createIfDoesNotExist);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    public bool Open(RegistryKeyType keyType, bool createIfDoesNotExist = false)
    {
      try
      {
        return Open(keyType, SubKey, createIfDoesNotExist);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    public bool Open(string subKey, bool createIfDoesNotExist = false)
    {
      try
      {
        return Open(KeyType, subKey, createIfDoesNotExist);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    public bool Open(RegistryKeyType keyType, string subKey, bool createIfDoesNotExist = false)
    {
      try
      {
        Close();

        this._keyType = keyType;

        _key = createIfDoesNotExist ? CreateSubKey(keyType, subKey) : OpenSubKey(keyType, subKey);

        this._subKey = subKey;

        return IsOpen;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    public void SetDefaultValue(object value)
    {
      try
      {
        SetValue("", value);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public void SetDefaultValue(object value, RegistryValueKind valueKind)
    {
      try
      {
        SetValue("", value, valueKind);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public void SetValue(string name, object value)
    {
      try
      {
        SetValue(name, value, RegistryValueKind.Unknown);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public void SetValue(string name, object value, RegistryValueKind valueKind)
    {
      try
      {
        ValidateIsOpen();

        if (name == null)
        {
          throw new Exception("Input parameter 'name' is blank.");
        }

        if (value == null)
        {
          throw new Exception("Input parameter 'value' is null.");
        }

        if (valueKind == RegistryValueKind.Unknown)
        {
          Key.SetValue(name, value);
        }
        else
        {
          Key.SetValue(name, value, valueKind);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public void ValidateIsOpen()
    {
      try
      {
        if (!IsOpen)
        {
          throw new Exception("The Registry object has not been opened.");
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    protected RegistryKey CreateSubKey(RegistryKeyType keyType, string subKey)
    {
      try
      {
        if (keyType == RegistryKeyType.ClassesRoot)
        {
          return Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(subKey);
        }
        if (keyType == RegistryKeyType.CurrentConfig)
        {
          return Microsoft.Win32.Registry.CurrentConfig.CreateSubKey(subKey);
        }
        if (keyType == RegistryKeyType.CurrentUser)
        {
          return Microsoft.Win32.Registry.CurrentUser.CreateSubKey(subKey);
        }
        if (keyType == RegistryKeyType.LocalMachine)
        {
          return Microsoft.Win32.Registry.LocalMachine.CreateSubKey(subKey);
        }
        if (keyType == RegistryKeyType.PerformanceData)
        {
          return Microsoft.Win32.Registry.PerformanceData.CreateSubKey(subKey);
        }
        if (keyType == RegistryKeyType.Users)
        {
          return Microsoft.Win32.Registry.Users.CreateSubKey(subKey);
        }
        throw new Exception("Input parameter 'keyType' is not implemented.");
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    protected RegistryKey OpenSubKey(RegistryKeyType keyType, string subKey)
    {
      try
      {
        if (subKey == null)
        {
          throw new Exception("Input parameter 'subKey' is blank.");
        }

        if (keyType == RegistryKeyType.ClassesRoot)
        {
          return Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(subKey);
        }
        if (keyType == RegistryKeyType.CurrentConfig)
        {
          return Microsoft.Win32.Registry.CurrentConfig.OpenSubKey(subKey);
        }
        if (keyType == RegistryKeyType.CurrentUser)
        {
          return Microsoft.Win32.Registry.CurrentUser.OpenSubKey(subKey);
        }
        if (keyType == RegistryKeyType.LocalMachine)
        {
          return Microsoft.Win32.Registry.LocalMachine.OpenSubKey(subKey);
        }
        if (keyType == RegistryKeyType.PerformanceData)
        {
          return Microsoft.Win32.Registry.PerformanceData.OpenSubKey(subKey);
        }
        if (keyType == RegistryKeyType.Users)
        {
          return Microsoft.Win32.Registry.Users.OpenSubKey(subKey);
        }
        throw new Exception("Input parameter 'keyType' is not implemented.");
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public static void Create(RegistryKeyType keyType, string subKey)
    {
      try
      {
        if (Exists(keyType, subKey))
        {
          return;
        }

        using (var registry = new Registry(keyType, subKey, true))
        {
          if (!registry.IsOpen)
          {
            throw new Exception("Could not create registry key " + keyType + "\\" + subKey);
          }
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public static void Delete(RegistryKeyType keyType, string subKey)
    {
      try
      {
        if (!Exists(keyType, subKey))
        {
          return;
        }

        using (var registry = new Registry(keyType, subKey))
        {
          registry.Delete();
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public static bool Exists(RegistryKeyType keyType, string subKey)
    {
      try
      {
        using (var registry = new Registry(keyType, subKey, false))
        {
          if (registry.IsOpen)
          {
            return true;
          }

          return false;
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    public static void WriteToFile(string filePath)
    {
      try
      {
        using (FileStream fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Write))
        {
          using (var streamWriter = new StreamWriter(fileStream, Encoding.Unicode))
          {
            streamWriter.WriteLine("Windows Registry Editor Version 5.00");
            streamWriter.WriteLine("");

            var registry = new Registry();

            WriteToFile(streamWriter, registry, RegistryKeyType.ClassesRoot, "");
            WriteToFile(streamWriter, registry, RegistryKeyType.CurrentUser, "");
            WriteToFile(streamWriter, registry, RegistryKeyType.LocalMachine, "");
            WriteToFile(streamWriter, registry, RegistryKeyType.Users, "");
            WriteToFile(streamWriter, registry, RegistryKeyType.CurrentConfig, "");
          }
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    protected static void WriteToFile(StreamWriter streamWriter,
                                      Registry registry,
                                      RegistryKeyType keyType,
                                      string subKey)
    {
      try
      {
        string line = "";

        try
        {
          line = "[" + ((RegistryNodeType)keyType);

          if (!String.IsNullOrEmpty(subKey))
          {
            line += "\\" + subKey;
          }

          line += "]";

          streamWriter.WriteLine(line);

          if (!registry.Open(keyType, subKey))
          {
            line = "ERROR - COULD NOT OPEN";

            streamWriter.WriteLine(line);

            return;
          }
        }
        catch (Exception exception)
        {
          line = "ERROR - " + exception.Message.ToUpper();

          streamWriter.WriteLine(line);

          return;
        }

        string[] valueNames = registry.GetValueNames();

        if (valueNames == null)
        {
          line = "ERROR - COULD NOT GET VALUE NAMES";

          streamWriter.WriteLine(line);
        }
        else
        {
          foreach (string valueName in valueNames)
          {
            if (String.IsNullOrEmpty(valueName))
            {
              line = "@=";
            }
            else
            {
              line = "\"" + valueName + "\"=";
            }

            object value = registry.GetValue(valueName);

            if (value == null)
            {
              line += "NULL";
            }
            else
            {
              line += "\"" + value + "\"";
            }

            try
            {
              streamWriter.WriteLine(line);
            }
            catch (Exception exception)
            {
              line = "ERROR - " + exception.Message.ToUpper();

              streamWriter.WriteLine(line);
            }
          }
        }

        streamWriter.WriteLine("");

        string[] subKeyNames = registry.GetSubKeyNames();

        if (subKeyNames == null)
        {
          line = "ERROR - COULD NOT GET SUBKEY NAMES";

          streamWriter.WriteLine(line);
        }
        else
        {
          foreach (string subKeyName in subKeyNames)
          {
            string subKeyPath = "";

            if (String.IsNullOrEmpty(subKey))
            {
              subKeyPath = subKeyName;
            }
            else
            {
              subKeyPath = subKey + "\\" + subKeyName;
            }

            WriteToFile(streamWriter, registry, keyType, subKeyPath);
          }
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }
  }
}
