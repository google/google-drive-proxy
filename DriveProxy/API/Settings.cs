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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DriveProxy.Utils;

namespace DriveProxy.API
{
  partial class DriveService
  {
    public class Settings
    {
      public static readonly string GUID_MyComputer = "20d04fe0-3aea-1069-a2d8-08002b30309d";
      public static readonly string GUID_GDriveShlExt = "30323693-0E1E-4365-99FB-5074A5C6F273";
      public static readonly int KB = 0x400;
      public static readonly int DownloadFileChunkSize = 256 * KB;
      public static readonly int UploadFileChunkSize = 256 * KB;

      public static int AuthenticationTimeout = 120;
      public static readonly int CleanupFileTimeout = 5;
      public static readonly int FileOnDiskTimeout = 5;

      private static string _googleDriveFolder = "";

      private static readonly string _FileDataStoreFolder = String.Format("{0}.MyLibrary", Title);
      private static readonly string _ImageFolder = "Images";

      private static readonly string _RootIdFileName = "root.key";
      private static readonly string _LastChangeIdFileName = "history.key";
      private static readonly string _JournalFileName = "journal.dat";
      private static readonly string _DesktopIniFileName = "Desktop.ini";
      private static readonly string _RefreshTokenFileName = "refreshtoken.dat";
      private static readonly string _FileTitlesFileName = "filetitles.dat";
      private static readonly string _FileName = "settings.dat";
      private static readonly string _LogFileName = "log.dat";
      private static readonly string _ShellLogFileName = "log.shell.dat";
      private static readonly string _PathRegistryStartup = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
      protected static string _LocalApplicationData = null;
      protected static string _LocalGoogleDriveData = null;
      protected static string _LocalGoogleDriveDataUser = null;
      protected static string _GoogleDriveDataUser = null;
      protected static string _ApplicationData = null;
      protected static string _GoogleDriveData = null;
      private static string _imageFolderPath = "";
      private static string _desktopOldIniFilePath;
      private static string _desktopNewIniFilePath;
      private static string _tokenOldFilePath;
      private static string _tokenNewFilePath;
      private static string _rootIdFilePath;
      private static string _lastChangeIdPath;
      private static string _journalFolderPath;
      private static string _fileTitlesFilePath;
      private static string _filePath;
      private static string _logFilePath;
      private static string _shellLogFilePath;
      protected static string _UserName = "";
      protected static string _userLogged = "";
      protected static string _Location = "";
      protected static string _LocationFileName = "";
      protected static FileReturnType _FileReturnType = FileReturnType.FilterGoogleFiles;
      public static int _UseCaching = 1;

      public static string GoogleDriveFolder
      {
        get
        {
          if (String.IsNullOrEmpty(_googleDriveFolder))
          {
            _googleDriveFolder = CompanyPath + "\\" + Product;
          }

          return _googleDriveFolder;
        }
        set
        {
          if (!String.IsNullOrEmpty(value) && value.IndexOf(CompanyPath + "\\") != 0)
          {
            _googleDriveFolder = CompanyPath + "\\" + value;
          }
          else
          {
            _googleDriveFolder = "";
          }
        }
      }

      public static string Version
      {
        get
        {
          FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
          return fvi.FileVersion;
        }
      }

      public static string Title
      {
        get
        {
          AssemblyTitleAttribute titleAttribute =
            Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyTitleAttribute), false)
                    .OfType<AssemblyTitleAttribute>()
                    .FirstOrDefault();
          return titleAttribute == null
            ? Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().CodeBase)
            : titleAttribute.Title;
        }
      }

      public static string Description
      {
        get
        {
          AssemblyDescriptionAttribute descriptionAttribute =
            Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyDescriptionAttribute), false)
                    .OfType<AssemblyDescriptionAttribute>()
                    .FirstOrDefault();
          if (descriptionAttribute != null)
          {
            return descriptionAttribute.Description;
          }
          Log.Error("Couldn't resolve description");
          return "Unknown";
        }
      }

      public static string Company
      {
        get
        {
          AssemblyCompanyAttribute companyAttribute =
            Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyCompanyAttribute), false)
                    .OfType<AssemblyCompanyAttribute>()
                    .FirstOrDefault();
          if (companyAttribute != null)
          {
            return companyAttribute.Company;
          }
          Log.Error("Couldn't resolve company");
          return "Unknown";
        }
      }

      public static string Product
      {
        get
        {
          AssemblyProductAttribute productAttribute =
            Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyProductAttribute), false)
                    .OfType<AssemblyProductAttribute>()
                    .FirstOrDefault();
          if (productAttribute != null)
          {
            return productAttribute.Product;
          }
          Log.Error("Couldn't resolve product");
          return "Unknown";
        }
      }

      public static string Copyright
      {
        get
        {
          AssemblyCopyrightAttribute copyrightAttribute =
            Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (AssemblyCopyrightAttribute), false)
                    .OfType<AssemblyCopyrightAttribute>()
                    .FirstOrDefault();
          if (copyrightAttribute != null)
          {
            return copyrightAttribute.Copyright;
          }
          Log.Error("Couldn't resolve copyright");
          return "Unknown";
        }
      }

      public static string CompanyPath
      {
        get { return GetCustomAttribute<CompanyPathAttr>(); }
      }

      public static string ServicePipeName
      {
        get { return GetCustomAttribute<ServicePipeNameAttr>(); }
      }

      public static string ClientID
      {
        get { return GetCustomAttribute<ClientIDAttr>(); }
      }

      public static string ClientSecret
      {
        get { return GetCustomAttribute<ClientSecretAttr>(); }
      }

      public static string LocalApplicationData
      {
        get
        {
          if (String.IsNullOrEmpty(_LocalApplicationData))
          {
            _LocalApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
          }

          return _LocalApplicationData;
        }
      }

      public static string LocalGoogleDriveData
      {
        get
        {
          if (String.IsNullOrEmpty(_LocalGoogleDriveData) || !_LocalGoogleDriveData.Equals(LocalGoogleDriveDataUser))
          {
            _LocalGoogleDriveData = System.IO.Path.Combine(LocalApplicationData, GoogleDriveFolder, LoggedUser);
          }

          CreateDirectory(_LocalGoogleDriveData);

          return _LocalGoogleDriveData;
        }
      }

      public static string LocalGoogleDriveDataUser
      {
        get
        {
          if (IsSignedIn)
          {
            _LocalGoogleDriveDataUser = System.IO.Path.Combine(LocalApplicationData, GoogleDriveFolder, LoggedUser);
          }

          CreateDirectory(_LocalGoogleDriveDataUser);

          return _LocalGoogleDriveDataUser;
        }
      }

      public static string ApplicationData
      {
        get
        {
          if (String.IsNullOrEmpty(_ApplicationData))
          {
            _ApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
          }

          return _ApplicationData;
        }
      }

      public static string GoogleDriveData
      {
        get
        {
          if (String.IsNullOrEmpty(_GoogleDriveData))
          {
            _GoogleDriveData = System.IO.Path.Combine(ApplicationData, GoogleDriveFolder);
          }

          CreateDirectory(_GoogleDriveData);

          return _GoogleDriveData;
        }
      }

      public static string ImageFolderPath
      {
        get
        {
          if (String.IsNullOrEmpty(_imageFolderPath))
          {
            _imageFolderPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            _imageFolderPath = System.IO.Path.GetDirectoryName(_imageFolderPath);
            _imageFolderPath = System.IO.Path.Combine(_imageFolderPath, _ImageFolder);
          }

          return _imageFolderPath;
        }
      }

      public static string DesktopOldIniFilePath
      {
        get
        {
          if (String.IsNullOrEmpty(_desktopOldIniFilePath))
          {
            _desktopOldIniFilePath = System.IO.Path.Combine(LocalGoogleDriveData, _DesktopIniFileName);
          }

          return _desktopOldIniFilePath;
        }
      }

      public static string DesktopNewIniFilePath
      {
        get
        {
          if (String.IsNullOrEmpty(_desktopNewIniFilePath))
          {
            string myDocuments = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            _desktopNewIniFilePath = System.IO.Path.Combine(myDocuments, "My Google Drive", _DesktopIniFileName);
          }

          return _desktopNewIniFilePath;
        }
      }

      public static string TokenOldFilePath
      {
        get
        {
          if (String.IsNullOrEmpty(_tokenOldFilePath))
          {
            _tokenOldFilePath = System.IO.Path.Combine(ApplicationData,
                                                       string.Format("{0}\\{1}\\refreshtoken.dat", CompanyPath, Product));
          }

          return _tokenOldFilePath;
        }
      }

      public static string TokenNewFilePath
      {
        get
        {
          if (String.IsNullOrEmpty(_tokenNewFilePath))
          {
            _tokenNewFilePath = System.IO.Path.Combine(GoogleDriveData, "Token");
          }

          return _tokenNewFilePath;
        }
      }

      public static string RootIdFilePath
      {
        get
        {
          if (String.IsNullOrEmpty(_rootIdFilePath))
          {
            _rootIdFilePath = System.IO.Path.Combine(GoogleDriveData, LoggedUser, _RootIdFileName);
          }

          return _rootIdFilePath;
        }
      }

      public static string LastChangeIdPath
      {
        get
        {
          if (String.IsNullOrEmpty(_lastChangeIdPath))
          {
            _lastChangeIdPath = System.IO.Path.Combine(GoogleDriveData, LoggedUser, _LastChangeIdFileName);
          }

          return _lastChangeIdPath;
        }
      }

      public static string JournalFolderPath
      {
        get
        {
          if (String.IsNullOrEmpty(_journalFolderPath))
          {
            _journalFolderPath = System.IO.Path.Combine(GoogleDriveData, "Journal");
          }

          return _journalFolderPath;
        }
      }

      public static string FileTitlesFilePath
      {
        get
        {
          if (String.IsNullOrEmpty(_fileTitlesFilePath) || !FileExists(_fileTitlesFilePath))
          {
            _fileTitlesFilePath = System.IO.Path.Combine(GoogleDriveData, _FileTitlesFileName);
          }

          return _fileTitlesFilePath;
        }
      }

      public static string FilePath
      {
        get
        {
          if (String.IsNullOrEmpty(_filePath) || !FileExists(_filePath))
          {
            _filePath = Win32.MakeFileNameWindowsSafe(UserName) + "." + _FileName;
            _filePath = System.IO.Path.Combine(GoogleDriveData, _filePath);
          }

          return _filePath;
        }
      }

      public static string LogFilePath
      {
        get
        {
          if (String.IsNullOrEmpty(_logFilePath) || !FileExists(_logFilePath))
          {
            _logFilePath = System.IO.Path.Combine(GoogleDriveData, _LogFileName);
          }

          return _logFilePath;
        }
        set { _logFilePath = value; }
      }

      public static string ShellLogFilePath
      {
        get
        {
          if (String.IsNullOrEmpty(_shellLogFilePath) || !FileExists(_ShellLogFileName))
          {
            _shellLogFilePath = System.IO.Path.Combine(GoogleDriveData, _ShellLogFileName);
          }

          return _shellLogFilePath;
        }
      }

      public static string UserName
      {
        get
        {
          try
          {
            if (String.IsNullOrEmpty(_UserName))
            {
              _UserName = Environment.UserName;

              if (!String.IsNullOrEmpty(Environment.UserDomainName))
              {
                _UserName += "@" + Environment.UserDomainName;
              }
            }

            return _UserName;
          }
          catch (Exception exception)
          {
            Log.Error(exception);

            return null;
          }
        }
      }

      public static string Location
      {
        get
        {
          if (String.IsNullOrEmpty(_Location))
          {
            _Location = System.Reflection.Assembly.GetEntryAssembly().Location;
          }

          return _Location;
        }
      }

      public static string LocationFileName
      {
        get
        {
          if (String.IsNullOrEmpty(_LocationFileName))
          {
            _LocationFileName = System.IO.Path.GetFileName(Location);
          }

          return _LocationFileName;
        }
      }

      public static LogType LogLevel
      {
        get { return Log.Level; }
        set { Log.Level = value; }
      }

      public static FileReturnType FileReturnType
      {
        get
        {
          if (_FileReturnType == FileReturnType.Unknown)
          {
            _FileReturnType = FileReturnType.FilterGoogleFiles;
          }

          return _FileReturnType;
        }
        set { _FileReturnType = value; }
      }

      public static bool UseCaching
      {
        get
        {
          if (_UseCaching == 0)
          {
            return false;
          }

          return true;
        }
        set { _UseCaching = value ? 1 : 0; }
      }

      public static bool IsStartingStartOnStartup
      {
        get { return IsRegisterOnStartup(); }
        set
        {
          if (value)
          {
            RegisterApplicationOnStartup();
          }
          else
          {
            UnRegisterApplicationOnStartup();
          }
        }
      }

      public static string LoggedUser
      {
        get
        {
          if (IsSignedIn && String.IsNullOrEmpty(_userLogged))
          {
            AboutInfo aboutInfo = DriveService.GetAbout();
            _userLogged = aboutInfo.Name;
          }
          return _userLogged;
        }
        set { _userLogged = String.IsNullOrEmpty(value) ? value : String.Empty; }
      }

      public static string GetCustomAttribute<T>() where T : DPSAttribute
      {
        T attr = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (T), false)
                         .OfType<T>()
                         .FirstOrDefault();
        if (attr != null)
        {
          return attr.attr_;
        }
        Log.Error("Couldn't resolve " + typeof (T).Name);
        return "Unknown";
      }

      public static string GetJournalFilePath(string fileId, bool createDirectory = true)
      {
        string journalFilePath = System.IO.Path.Combine(GoogleDriveData, "Journal", LoggedUser);

        if (createDirectory)
        {
          CreateDirectory(journalFilePath);
        }

        journalFilePath = System.IO.Path.Combine(journalFilePath, fileId);

        return journalFilePath;
      }

      public static string[] GetLocalGoogleDriveDataSubFolders()
      {
        try
        {
          if (!System.IO.Directory.Exists(LocalGoogleDriveData))
          {
            return new string[0];
          }

          string[] folderPaths = System.IO.Directory.GetDirectories(LocalGoogleDriveData);

          return folderPaths;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      public static void Load()
      {
        try
        {
          if (!FileExists(FilePath))
          {
            Save();
          }
          else
          {
            var iniFile = new IniFile(FilePath);
            string value = "";

            LogLevel = LogType.None;
            FileReturnType = FileReturnType.FilterGoogleFiles;

            value = iniFile.GetValue("Settings", "UseCaching");
            UseCaching = ToBoolean(value);
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      private static bool ToBoolean(string value)
      {
        try
        {
          return (value == "1");
        }
        catch
        {
          return true;
        }
      }

      public static void Save()
      {
        try
        {
          var iniFile = new IniFile(FilePath);
          iniFile.SetValue("Settings", "UseCaching", _UseCaching.ToString());
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      private static bool IsRegisterOnStartup()
      {
        bool result = false;
        try
        {
          using (var registryKey = new Registry())
          {
            if (registryKey.Open(RegistryKeyType.CurrentUser, _PathRegistryStartup, true))
            {
              object value = registryKey.GetValue(Title);
              if (value != null)
              {
                result = !String.IsNullOrEmpty(value.ToString());
              }
            }
          }
        }
        catch (Exception exception)
        {
          result = false;
          Log.Error(exception);
        }
        return result;
      }

      private static void RegisterApplicationOnStartup()
      {
        try
        {
          using (var registryKey = new Registry())
          {
            if (registryKey.Open(RegistryKeyType.CurrentUser, _PathRegistryStartup, true))
            {
              registryKey.SetValue(Title,
                                   Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) +
                                   String.Format("\\{0}\\{1}\\DriveProxy.Service.exe", CompanyPath, Product));
            }
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      private static void UnRegisterApplicationOnStartup()
      {
        try
        {
          using (var registryKey = new Registry())
          {
            if (registryKey.Open(RegistryKeyType.CurrentUser, _PathRegistryStartup, true))
            {
              registryKey.DeleteValue(Title);
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
}
