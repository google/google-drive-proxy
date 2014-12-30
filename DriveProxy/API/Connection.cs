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
using DriveProxy.Utils;

namespace DriveProxy.API
{
  partial class DriveService
  {
    private class Connection : IDisposable
    {
      private static bool _isConnectionValid = true;

      private static Mutex _mutex;

      private static Authenticator _authenticator;

      private static bool _isSignedIn;

      private static DateTime _refreshDateTime;
      private bool _aquired;
      private Google.Apis.Drive.v2.DriveService _service;

      private Connection()
      {
        try
        {
          Aquire();
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      protected static Mutex Mutex
      {
        get { return _mutex ?? (_mutex = new Mutex()); }
      }

      public static Authenticator Authenticator
      {
        get { return _authenticator ?? (_authenticator = Authenticator.Create()); }
        set { _authenticator = value; }
      }

      public static bool IsSignedIn
      {
        get { return _isSignedIn; }
        protected set { _isSignedIn = value; }
      }

      public static bool SignedOut { get; protected set; }

      public Google.Apis.Drive.v2.DriveService Service
      {
        get { return _service; }
      }

      public bool Aquired
      {
        get { return _aquired; }
      }

      public void Dispose()
      {
        try
        {
          if (Aquired)
          {
            Factory.Dispose(this);

            _aquired = false;
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public static void Reset()
      {
        _isConnectionValid = true;
      }

      public static Connection Create(bool startBackgroundProcesses = true)
      {
        try
        {
          return Factory.GetInstance(startBackgroundProcesses);
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      public static void Authenticate(bool startBackgroundProcesses = true, bool openInExplorer = false)
      {
        try
        {
          Mutex.Wait();

          try
          {
            using (Connection connection = Create(startBackgroundProcesses))
            {
            }

            if (openInExplorer)
            {
              string folderPath = "::{" + Settings.GUID_MyComputer + "}\\::{" + Settings.GUID_GDriveShlExt + "}";

              System.Diagnostics.Process.Start("explorer.exe", folderPath);
            }
          }
          finally
          {
            Mutex.Release();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public static void Signout()
      {
        try
        {
          Mutex.Wait();

          try
          {
            DriveService.StopBackgroundProcesses(true);

            try
            {
              Win32.CloseFolders(Settings.Product);
            }
            catch (Exception exception)
            {
              Log.Error(exception, false);
            }

            Authenticator.RevokeRefreshToken();
            Authenticator.ClearRefreshToken();
            Journal.Refresh();
            Factory.Refresh();

            IsSignedIn = false;
            SignedOut = true;
          }
          finally
          {
            Mutex.Release();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      public static void SignoutAndAuthenticate(bool startBackgroundProcesses = true, bool openInExplorer = false)
      {
        try
        {
          Mutex.Wait();

          try
          {
            Signout();
            Authenticate(startBackgroundProcesses, openInExplorer);
          }
          finally
          {
            Mutex.Release();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      public bool Aquire()
      {
        try
        {
          if (Aquired)
          {
            return false;
          }
          _aquired = true;

          return true;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return false;
        }
      }

      public bool RefreshIfTimedOut(bool startBackgroundProcesses)
      {
        try
        {
          var stopWatch = new Log.Stopwatch();

          if (IsSignedIn)
          {
            try
            {
              if (_refreshDateTime > DateTime.Now)
              {
                return true;
              }

              Log.StartPerformance(ref stopWatch, "Begin - Google.Apis.Authentication.RefreshToken");

              bool timedOut = Authenticator.HasTimedOut();

              Log.EndPerformance(ref stopWatch, "End - Google.Apis.Authentication.RefreshToken");

              if (!timedOut)
              {
                _refreshDateTime = DateTime.Now.AddSeconds(15);

                return true;
              }
            }
            catch (Exception exception)
            {
              Log.Error(exception, false);
            }
          }

          IsSignedIn = false;

          _service = null;

          Log.StartPerformance(ref stopWatch, "Begin - Google.Apis.Authentication.Auth");

          string tokenFilePath = GetRegistryEntryTokenFilePath();

          if (String.IsNullOrEmpty(tokenFilePath))
          {
            tokenFilePath = Settings.TokenNewFilePath;
          }

          _service = Authenticator.Init(
                                        Settings.Product,
                                        Settings.ClientID,
                                        Settings.ClientSecret,
                                        new[] {Google.Apis.Drive.v2.DriveService.Scope.Drive},
                                        SignedOut,
                                        tokenFilePath);

          Log.EndPerformance(ref stopWatch, "End - Google.Apis.Authentication.Auth");

          if (_service == null)
          {
            return false;
          }

          _refreshDateTime = DateTime.Now.AddSeconds(15);

          IsSignedIn = true;
          SignedOut = false;

          UpdateRegistryEntries(tokenFilePath);

          if (startBackgroundProcesses || AutoStartBackgroundProcesses)
          {
            DriveService.StartBackgroundProcesses();
          }

          return true;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return false;
        }
      }

      public bool UpdateRegistryEntries(string tokenFilePath)
      {
        try
        {
          try
          {
            DeleteFile(Settings.TokenOldFilePath);
          }
          catch
          {
          }

          string path = string.Format(@"SOFTWARE\{0}\{1}", Settings.CompanyPath, Settings.Product);

          using (var registryKey = new Registry())
          {
            if (registryKey.Open(RegistryKeyType.CurrentUser, path, true))
            {
              registryKey.SetValue("DataPath", Settings.GoogleDriveData);
              registryKey.SetValue("CachePath", Settings.LocalGoogleDriveData);
              registryKey.SetValue("TokenFilePath", tokenFilePath);
            }
          }

          return true;
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);

          return false;
        }
      }

      public string GetRegistryEntryTokenFilePath()
      {
        try
        {
          string path = string.Format(@"SOFTWARE\{0}\{1}", Settings.CompanyPath, Settings.Product);
          object value = null;

          using (var registryKey = new Registry())
          {
            if (registryKey.Open(RegistryKeyType.CurrentUser, path, true))
            {
              value = registryKey.GetValue("TokenFilePath");
            }
          }

          if (value == null)
          {
            return null;
          }

          return value.ToString();
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);

          return null;
        }
      }

      private class Factory
      {
        public static Connection _Connection;

        public static Connection GetInstance(bool startBackgroundProcesses)
        {
          try
          {
            Mutex.Wait();

            try
            {
              if (_Connection == null)
              {
                _Connection = new Connection();
              }

              if (!_Connection.RefreshIfTimedOut(startBackgroundProcesses))
              {
                throw new LogException("Could not connect to Google Drive", true, false);
              }

              return _Connection;
            }
            catch (Exception exception)
            {
              _isConnectionValid = false;

              throw exception;
            }
            finally
            {
              Mutex.Release();
            }
          }
          catch (Exception exception)
          {
            Log.Error(exception);

            return null;
          }
        }

        public static void Dispose(Connection connection)
        {
          try
          {
            if (connection == null || !connection.Aquired)
            {
              return;
            }

            Mutex.Wait();

            try
            {
              // TODO: shut down unneeded instances
            }
            finally
            {
              Mutex.Release();
            }
          }
          catch (Exception exception)
          {
            Log.Error(exception);
          }
        }

        public static void Refresh()
        {
          try
          {
            Mutex.Wait();

            try
            {
              if (_Connection != null)
              {
                _Connection.Dispose();
                _Connection = null;
              }
            }
            finally
            {
              Mutex.Release();
            }
          }
          catch (Exception exception)
          {
            Log.Error(exception);
          }
        }
      }
    }
  }
}
