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
using System.Threading;
using DriveProxy.Forms;
using DriveProxy.Service;
using DriveProxy.Utils;
using Google.Apis.Services;

namespace DriveProxy.API
{
  partial class DriveService
  {
    private class Authenticator
    {
      private Google.Apis.Drive.v2.DriveService _driveService;
      private string _refreshTokenFilePath;
      private Google.Apis.Auth.OAuth2.UserCredential _userCredential;
      private EncryptedFileDataStore _fileDataStore;

      private Authenticator()
      {
      }

      public static Authenticator Create()
      {
        return Factory.Create();
      }

      private static void ThrowException(Exception exception)
      {
        string message = String.Empty;

        if (!IsNetworkAvailable())
        {
          message = "Not currently connected to a network";
        }
        else if (!CanConnectToGoogleDrive())
        {
          message = "Cannot connect to https://drive.google.com";
        }
        else
        {
          message = exception.Message + " (user " + Settings.UserName + ")";
        }

        Log.Error(message, false);

        throw new AuthenticationException(message);
      }

      private static bool IsNetworkAvailable()
      {
        try
        {
          if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
          {
            return false;
          }

          return true;
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);

          return false;
        }
      }

      private static bool CanConnectToGoogleDrive()
      {
        try
        {
          if (!IsNetworkAvailable())
          {
            return false;
          }

          System.Net.WebRequest webRequest = System.Net.WebRequest.Create("https://drive.google.com/");

          System.Net.WebResponse webResponse = webRequest.GetResponse();

          return true;
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);

          return false;
        }
      }

      public Google.Apis.Drive.v2.DriveService Init(string applicationName,
                                                    string clientIdentifier,
                                                    string clientSecret,
                                                    string[] scope,
                                                    bool logout,
                                                    string refreshTokenFolder)
      {
        StatusForm.ShowDialog();

        try
        {
          _refreshTokenFilePath = refreshTokenFolder;

          string fileDataStorePath = _refreshTokenFilePath;
          if (FileExists(fileDataStorePath))
          {
            DateTime lastFileWriteTime = GetFileLastWriteTime(fileDataStorePath);

            if (lastFileWriteTime < new DateTime(2014, 8, 22))
            {
              DeleteFile(fileDataStorePath);
            }
          }

          if (logout)
          {
            try
            {
              if (DirectoryExists(fileDataStorePath))
              {
                string[] fileDataStoreFiles = System.IO.Directory.GetFiles(fileDataStorePath,
                                                                           "Google.Apis.Auth.OAuth2.Responses.*");

                foreach (string fileDataStoreFile in fileDataStoreFiles)
                {
                  DeleteFile(fileDataStoreFile);
                }
              }

              LoginForm.Logout();
            }
            catch (Exception exception)
            {
              Log.Error(String.Format("Authenticator.Logout - Deleting token file. {0}",exception.Message));
            }
          }

          var clientSecrets = new Google.Apis.Auth.OAuth2.ClientSecrets
          {
            ClientId = clientIdentifier,
            ClientSecret = clientSecret
          };

          _fileDataStore = new EncryptedFileDataStore(refreshTokenFolder);

          System.Threading.Tasks.Task<Google.Apis.Auth.OAuth2.UserCredential> task = null;

          try
          {
            task = Google.Apis.Auth.OAuth2.GoogleWebAuthorizationBroker.AuthorizeAsync(
                                                                                       clientSecrets,
                                                                                       scope,
                                                                                       System.Environment.UserName,
                                                                                       System.Threading.CancellationToken.None,
                                                                                       _fileDataStore);

            task.Wait();
          }
          catch (Exception exception)
          {
            throw new LogException("GoogleWebAuthorizationBroker.AuthorizeAsync - " + exception.Message, false, false);
          }

          _userCredential = task.Result;

          var initializer = new BaseClientService.Initializer
          {
            HttpClientInitializer = _userCredential,
            ApplicationName = applicationName
          };

          _driveService = new Google.Apis.Drive.v2.DriveService(initializer);

          UpdateAboutData(_driveService);

          return _driveService;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
        finally
        {
          StatusForm.CloseDialog();
        }
      }

      public bool HasTimedOut()
      {
        try
        {
          if (_userCredential == null || _userCredential.Token == null)
          {
            return true;
          }

          if (_userCredential.Token.IsExpired(Google.Apis.Util.SystemClock.Default))
          {
            return true;
          }

          System.Threading.Tasks.Task<bool> task =
            _userCredential.RefreshTokenAsync(System.Threading.CancellationToken.None);

          if (task == null)
          {
            return true;
          }

          task.Wait();

          if (task.Status != System.Threading.Tasks.TaskStatus.RanToCompletion)
          {
            return true;
          }

          if (!task.Result)
          {
            return true;
          }

          return false;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return true;
        }
      }

      public void ClearRefreshToken()
      {
        try
        {
          //Delete Browser cookies
          DeleteDirectoryContents(Environment.GetFolderPath(Environment.SpecialFolder.Cookies));
          //Delete Token
          DeleteDirectoryContents(_fileDataStore.FolderPath);
          //Clean User Credentials
          _userCredential = null;
          //Clean LoggedUser
          Settings.LoggedUser = String.Empty;
        }
        catch (Exception exception)
        {
          Log.Error(exception, false, false);
        }
      }

      public void RevokeRefreshToken()
      {
        try
        {
          //Revokes the user token.
          _userCredential.RevokeTokenAsync(CancellationToken.None);
        }
        catch (Exception exception)
        {
          Log.Error(exception, false, false);
        }
      }

      private static class Factory
      {
        public static Authenticator Create()
        {
          return new Authenticator();
        }
      }
    }
  }
}
