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
using System.Threading.Tasks;
using Google.Apis.Json;
using Google.Apis.Util.Store;

namespace DriveProxy.Utils
{
  public class EncryptedFileDataStore : IDataStore
  {
    private readonly string _folderPath;

    public EncryptedFileDataStore(string folder, bool fullPath = false)
    {
      _folderPath = fullPath
        ? folder
        : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), folder);
      if (!Directory.Exists(_folderPath))
      {
        Directory.CreateDirectory(_folderPath);
      }
    }

    /// <summary>Gets the full folder path.</summary>
    public string FolderPath
    {
      get { return _folderPath; }
    }

    public Task ClearAsync()
    {
      if (Directory.Exists(_folderPath))
      {
        Directory.Delete(_folderPath, true);
        Directory.CreateDirectory(_folderPath);
      }

      return TaskEx.Delay(0);
    }

    public Task DeleteAsync<T>(string key)
    {
      if (string.IsNullOrEmpty(key))
      {
        throw new ArgumentException("Key MUST have a value");
      }

      string filePath = Path.Combine(_folderPath, GenerateStoredKey(key, typeof (T)));
      if (File.Exists(filePath))
      {
        File.Delete(filePath);
      }
      return TaskEx.Delay(0);
    }

    public Task<T> GetAsync<T>(string key)
    {
      if (string.IsNullOrEmpty(key))
      {
        throw new ArgumentException("Key MUST have a value");
      }

      var tcs = new TaskCompletionSource<T>();
      string filePath = Path.Combine(_folderPath, GenerateStoredKey(key, typeof (T)));
      if (File.Exists(filePath))
      {
        try
        {
          string obj = File.ReadAllText(filePath);
          tcs.SetResult(NewtonsoftJsonSerializer.Instance.Deserialize<T>(DataProtection.Unprotect(obj)));
        }
        catch (Exception ex)
        {
          tcs.SetException(ex);
        }
      }
      else
      {
        tcs.SetResult(default(T));
      }
      return tcs.Task;
    }

    public Task StoreAsync<T>(string key, T value)
    {
      if (string.IsNullOrEmpty(key))
      {
        throw new ArgumentException("Key MUST have a value");
      }

      string serialized = NewtonsoftJsonSerializer.Instance.Serialize(value);
      string filePath = Path.Combine(_folderPath, GenerateStoredKey(key, typeof (T)));
      File.WriteAllText(filePath, DataProtection.Protect(serialized));
      return TaskEx.Delay(0);
    }

    public static string GenerateStoredKey(string key, Type t)
    {
      return string.Format("{0}-{1}", t.FullName, key);
    }
  }
}
