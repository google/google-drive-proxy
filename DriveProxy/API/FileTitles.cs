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
using DriveProxy.Utils;

namespace DriveProxy.API
{
  partial class DriveService
  {
    private class FileTitles
    {
      private static List<Item> _items;

      private static Mutex _mutex;

      private static bool _loading;

      private static bool _loaded;

      private static bool _changed;

      private static bool _saving;
      private static System.Threading.Thread _changedThread;

      private static List<Item> _Items
      {
        get { return _items ?? (_items = new List<Item>()); }
      }

      private static Mutex _Mutex
      {
        get { return _mutex ?? (_mutex = new Mutex()); }
      }

      public static bool Loading
      {
        get { return _loading; }
      }

      public static bool Loaded
      {
        get { return _loaded; }
      }

      public static bool Changed
      {
        get { return _changed; }
      }

      public static bool Saving
      {
        get { return _saving; }
      }

      public static void Load()
      {
        try
        {
          if (_loaded || _loading)
          {
            return;
          }

          _Mutex.Wait();

          try
          {
            if (_loaded || _loading)
            {
              return;
            }

            _loading = true;

            if (FileExists(Settings.FileTitlesFilePath))
            {
              using (
                System.IO.FileStream fileStream = OpenFile(Settings.FileTitlesFilePath,
                                                           System.IO.FileMode.Open,
                                                           System.IO.FileAccess.Read,
                                                           System.IO.FileShare.Read))
              {
                using (var streamReader = new System.IO.StreamReader(fileStream))
                {
                  while (true)
                  {
                    string line = streamReader.ReadLine();

                    if (line == null)
                    {
                      break;
                    }

                    string[] values = line.Split('=');

                    if (values.Length != 2)
                    {
                      _Items.Clear();

                      throw new Exception("Unexpected data in file.");
                    }

                    var item = new Item {Id = values[0], Title = values[1]};

                    _Items.Add(item);
                  }
                }
              }
            }

            _loaded = true;
          }
          finally
          {
            _Mutex.Release();

            _loading = false;
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      public static void Save()
      {
        try
        {
          if (!_changed)
          {
            return;
          }

          _Mutex.Wait();

          try
          {
            _saving = true;

            using (
              System.IO.FileStream fileStream = OpenFile(Settings.FileTitlesFilePath,
                                                         System.IO.FileMode.Create,
                                                         System.IO.FileAccess.Write,
                                                         System.IO.FileShare.Write))
            {
              using (var streamWriter = new System.IO.StreamWriter(fileStream))
              {
                foreach (Item item in _Items)
                {
                  string line = item.Id + "=" + item.Title;

                  streamWriter.WriteLine(line);
                }
              }
            }

            _changed = false;
          }
          finally
          {
            _Mutex.Release();

            _saving = false;
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      public static void Update(Google.Apis.Drive.v2.Data.File file)
      {
        try
        {
          Load();

          if (file == null)
          {
            return;
          }

          _Mutex.Wait();

          try
          {
            int index = -1;

            Update(file, ref index);
          }
          finally
          {
            _Mutex.Release();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      protected static void Update(Google.Apis.Drive.v2.Data.File file, ref int index)
      {
        try
        {
          MakeFileNameWindowsSafe(file);

          index = IndexOf(file);

          if (index > -1)
          {
            Item item = _Items[index];

            file.Title = item.Title;
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public static void Update(ref List<Google.Apis.Drive.v2.Data.File> files)
      {
        try
        {
          Load();

          if (files == null)
          {
            return;
          }

          _Mutex.Wait();

          try
          {
            SortFiles(ref files, false);

            var indices = new List<int>();

            int fileCount = files.Count();

            for (int i = 0; i < fileCount; i++)
            {
              Google.Apis.Drive.v2.Data.File file = files.ElementAt(i);

              int index = -1;

              Update(file, ref index);

              indices.Add(index);
            }

            for (int i = 0; i < fileCount - 1; i++)
            {
              Google.Apis.Drive.v2.Data.File file = files.ElementAt(i);
              Google.Apis.Drive.v2.Data.File file2 = files.ElementAt(i + 1);
              string title = file2.Title;
              string fileName = System.IO.Path.GetFileNameWithoutExtension(title);
              string fileExtension = System.IO.Path.GetExtension(title);
              int foundCount = 0;

              while (true)
              {
                bool found = false;

                for (int j = 0; j <= i; j++)
                {
                  Google.Apis.Drive.v2.Data.File tempFile = files.ElementAt(j);

                  if (String.Equals(tempFile.Title, title, StringComparison.CurrentCultureIgnoreCase))
                  {
                    found = true;

                    break;
                  }
                }

                if (!found)
                {
                  break;
                }

                foundCount++;

                title = fileName + " (" + foundCount + ")" + fileExtension;
              }

              if (file2.Title != title)
              {
                file2.Title = title;

                int index = indices[i];

                if (index == -1)
                {
                  var item = new Item {Id = file2.Id, Title = file2.Title};

                  _Items.Add(item);

                  _changed = true;
                }
                else
                {
                  _Items[index].Title = file2.Title;

                  _changed = true;
                }
              }
            }

            SortFiles(ref files, true);
          }
          finally
          {
            _Mutex.Release();
          }

          if (_changed && _changedThread == null)
          {
            _changedThread = new System.Threading.Thread(ChangedThread_Start);

            _changedThread.Start();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      protected static int IndexOf(Google.Apis.Drive.v2.Data.File file)
      {
        try
        {
          int index = -1;

          for (int i = 0; i < _Items.Count; i++)
          {
            Item item = _Items[i];

            if (file.Id == item.Id)
            {
              index = i;

              break;
            }
          }

          return index;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return -1;
        }
      }

      public static void Remove(Google.Apis.Drive.v2.Data.File file)
      {
        try
        {
          Load();

          if (file == null)
          {
            return;
          }

          _Mutex.Wait();

          try
          {
            int index = IndexOf(file);

            if (index > -1)
            {
              _Items.RemoveAt(index);
            }
          }
          finally
          {
            _Mutex.Release();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      protected static void ChangedThread_Start()
      {
        try
        {
          while (true)
          {
            System.Threading.Thread.Sleep(1000);

            if (Loading || Saving)
            {
              continue;
            }

            Save();

            break;
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
        finally
        {
          _changedThread = null;
        }
      }

      private class Item
      {
        public string Id = "";
        public string Title = "";
      }
    }
  }
}
