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
using DriveProxy.Forms;
using DriveProxy.Utils;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Math = DriveProxy.Utils.Math;

namespace DriveProxy.API
{
  /// <summary>Drive Service Class. </summary>
  public partial class DriveService : IDisposable
  {
    private static bool _isInitialized = false;
    private static bool _useFeedbackForm = true;
    private static bool _autoStartBackgroundProcesses = true;
    private static bool _startedBackgroundProcesses = false;
    private static bool _startingBackgroundProcesses = false;
    private static bool _stoppingBackgroundProcesses = false;
    private static Mutex _backgroundProcessesMutex = null;
    private static DateTime _settingsCheckedTime = default(DateTime);
    private static DateTime _settingsLastWriteTime = default(DateTime);
    private static System.IO.Stream _logStream = null;
    private static Mutex _logMutex = new Mutex();
    private static bool _loggingError = false;
    private static bool _loggingEnabled = true;
    private static string _rootFolderId = "";
    private static long _largestChangeId = 0;
    private static bool _cancelProcessing = false;
    private static Mutex _listFilesMutex = null;

    private static System.Collections.Concurrent.ConcurrentDictionary<string, FileInfoAssociation> _associations =
      new System.Collections.Concurrent.ConcurrentDictionary<string, FileInfoAssociation>();

    private Mutex _cleanupFileMutex = null;

    private DriveService()
    {
    }

    /// <summary> Gets a value indicating whether this object is signed in. </summary>
    /// <value> true if this object is signed in, false if not. </value>
    public static bool IsSignedIn
    {
      get { return Connection.IsSignedIn; }
    }

    /// <summary> Gets the full pathname of the folder file. </summary>
    /// <value> The full pathname of the folder file. </value>
    public static string FolderPath
    {
      get { return Settings.LocalGoogleDriveData; }
    }

    /// <summary> Gets a value indicating whether the processing. </summary>
    /// <value> true if processing, false if not. </value>
    public static bool Processing
    {
      get
      {
        try
        {
          UploadFileWatcher uploadFileWatcher = UploadFileWatcher.Create();

          if (uploadFileWatcher.Processing)
          {
            return true;
          }

          CleanupFileWatcher cleanupFileWatcher = CleanupFileWatcher.Create();

          if (cleanupFileWatcher.Processing)
          {
            return true;
          }

          State state = State.Create();

          if (state.Processing)
          {
            return true;
          }

          Journal journal = Journal.Create();

          if (journal.Processing)
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
    }

    /// <summary> Gets a value indicating whether this object is initialized. </summary>
    /// <value> true if this object is initialized, false if not. </value>
    public static bool IsInitialized
    {
      get { return _isInitialized; }
    }

    /// <summary> Gets a value indicating whether this object use feedback form. </summary>
    /// <value> true if use feedback form, false if not. </value>
    public static bool UseFeedbackForm
    {
      get { return _useFeedbackForm; }
    }

    /// <summary>
    ///   Gets a value indicating whether the automatic start background processes.
    /// </summary>
    /// <value> true if automatic start background processes, false if not. </value>
    public static bool AutoStartBackgroundProcesses
    {
      get { return _autoStartBackgroundProcesses; }
    }

    /// <summary> Gets a value indicating whether the started background processes. </summary>
    /// <value> true if started background processes, false if not. </value>
    public static bool StartedBackgroundProcesses
    {
      get { return _startedBackgroundProcesses; }
    }

    /// <summary> Gets a value indicating whether the starting background processes. </summary>
    /// <value> true if starting background processes, false if not. </value>
    public static bool StartingBackgroundProcesses
    {
      get { return _startingBackgroundProcesses; }
    }

    /// <summary> Gets a value indicating whether the stopping background processes. </summary>
    /// <value> true if stopping background processes, false if not. </value>
    public static bool StoppingBackgroundProcesses
    {
      get { return _stoppingBackgroundProcesses; }
    }

    private static Mutex BackgroundProcessesMutex
    {
      get { return _backgroundProcessesMutex ?? (_backgroundProcessesMutex = new Mutex()); }
    }

    /// <summary> Gets the identifier of the root folder. </summary>
    /// <value> The identifier of the root folder. </value>
    public static string RootFolderId
    {
      get
      {
        UpdateAboutData();

        return _rootFolderId;
      }
    }

    /// <summary> Gets the identifier of the largest change. </summary>
    /// <value> The identifier of the largest change. </value>
    public static long LargestChangeId
    {
      get
      {
        UpdateAboutData();

        return _largestChangeId;
      }
    }

    private Mutex CleanupFileMutex
    {
      get { return _cleanupFileMutex ?? (_cleanupFileMutex = new Mutex()); }
    }

    private static Mutex ListFilesMutex
    {
      get { return _listFilesMutex ?? (_listFilesMutex = new Mutex()); }
    }

    /// <summary>
    ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
    ///   resources.
    /// </summary>
    public void Dispose()
    {
      try
      {
        Factory.Dispose(this);
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    /// <summary> Initialises this object. </summary>
    /// <param name="autoStartBackgroundProcesses"> true to automatically start background processes. </param>
    /// <param name="googleDriveFolder">            Pathname of the google drive folder. </param>
    /// <param name="useFeedbackForm">              true to use feedback form. </param>
    public static void Init(bool autoStartBackgroundProcesses = true,
                            string googleDriveFolder = null,
                            bool useFeedbackForm = true)
    {
      try
      {
        if (IsInitialized)
        {
          return;
        }

        if (!String.IsNullOrEmpty(googleDriveFolder))
        {
          Debugger.Launch();

          Settings.GoogleDriveFolder = googleDriveFolder;
        }

        _useFeedbackForm = useFeedbackForm;

        Log.OnWriteEntry += new Log.WriteEntryEventHandler(Log_WriteEntry);

        Log.Information(string.Format("Initializing {0}...", Settings.Product));

        DriveProxy.Service.Service.OnExecute += new DriveProxy.Service.Service.ExecuteEventHanlder(Service_Execute);

        _autoStartBackgroundProcesses = autoStartBackgroundProcesses;

        CreateDesktopIni(false);
        CreateDesktopIni(true);

        Settings.Load();
        Journal.Load();

        _isInitialized = true;
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    private static void Service_Execute()
    {
      try
      {
        ResetConnection();
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Creates a new DriveService. </summary>
    /// <returns> A DriveService. </returns>
    public static DriveService Create()
    {
      try
      {
        return Factory.GetInstance();
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    /// <summary> Authenticates. </summary>
    /// <param name="startBackgroundProcesses"> true to start background processes. </param>
    /// <param name="openInExplorer">           true to open in explorer. </param>
    public static void Authenticate(bool startBackgroundProcesses = true, bool openInExplorer = false)
    {
      try
      {
        Connection.Authenticate(startBackgroundProcesses, openInExplorer);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Signouts this object. </summary>
    public static void Signout()
    {
      try
      {
        Connection.Signout();
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Signout and authenticate. </summary>
    /// <param name="startBackgroundProcesses"> true to start background processes. </param>
    /// <param name="openInExplorer">           true to open in explorer. </param>
    public static void SignoutAndAuthenticate(bool startBackgroundProcesses = true, bool openInExplorer = false)
    {
      try
      {
        Connection.SignoutAndAuthenticate(startBackgroundProcesses, openInExplorer);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Starts background processes. </summary>
    public static void StartBackgroundProcesses()
    {
      try
      {
        if (_startedBackgroundProcesses)
        {
          return;
        }

        BackgroundProcessesMutex.Wait();

        try
        {
          if (_startedBackgroundProcesses || _startingBackgroundProcesses || _stoppingBackgroundProcesses)
          {
            return;
          }

          _startingBackgroundProcesses = true;

          FileTitles.Load();

          UploadFileWatcher uploadFileWatcher = UploadFileWatcher.Create();

          uploadFileWatcher.Start();

          CleanupFileWatcher cleanupFileWatcher = CleanupFileWatcher.Create();

          cleanupFileWatcher.Start();

          _startedBackgroundProcesses = true;
        }
        finally
        {
          BackgroundProcessesMutex.Release();

          _startingBackgroundProcesses = false;
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Stops background processes. </summary>
    /// <param name="waitWhileProcessing">  true to wait while processing. </param>
    public static void StopBackgroundProcesses(bool waitWhileProcessing)
    {
      try
      {
        _cancelProcessing = true;

        BackgroundProcessesMutex.Wait();

        try
        {
          if (!_startedBackgroundProcesses || _startingBackgroundProcesses || _stoppingBackgroundProcesses)
          {
            return;
          }

          _stoppingBackgroundProcesses = true;

          UploadFileWatcher uploadFileWatcher = UploadFileWatcher.Create();

          uploadFileWatcher.Stop();

          CleanupFileWatcher cleanupFileWatcher = CleanupFileWatcher.Create();

          cleanupFileWatcher.Stop();

          Journal.Stop();

          if (waitWhileProcessing)
          {
            for (int i = 0; i < 300; i++)
            {
              if (!Processing)
              {
                break;
              }

              System.Windows.Forms.Application.DoEvents();
              System.Threading.Thread.Sleep(100);
              System.Windows.Forms.Application.DoEvents();
            }
          }

          _startedBackgroundProcesses = false;
        }
        finally
        {
          BackgroundProcessesMutex.Release();

          _stoppingBackgroundProcesses = false;
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Resets the connection. </summary>
    public static void ResetConnection()
    {
      try
      {
        Connection.Reset();
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Gets the about. </summary>
    /// <returns> The about. </returns>
    public static AboutInfo GetAbout()
    {
      try
      {
        About about = _GetAbout();

        var aboutInfo = new AboutInfo();

        aboutInfo.Name = about.Name;
        aboutInfo.UserDisplayName = about.User.DisplayName;
        aboutInfo.QuotaBytesTotal = DriveService.GetLong(about.QuotaBytesTotal);
        aboutInfo.QuotaBytesUsed = DriveService.GetLong(about.QuotaBytesUsed);
        aboutInfo.QuotaBytesUsedAggregate = DriveService.GetLong(about.QuotaBytesUsedAggregate);
        aboutInfo.QuotaBytesUsedInTrash = DriveService.GetLong(about.QuotaBytesUsedInTrash);
        aboutInfo.RootFolderId = about.RootFolderId;

        return aboutInfo;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    /// <summary> Gets the log. </summary>
    /// <returns> The log. </returns>
    public static LogInfo GetLog()
    {
      try
      {
        try
        {
          Log_Update();
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }

        var logInfo = new LogInfo();

        logInfo.LogLevel = (int)Settings.LogLevel;
        logInfo.FilePath = "";
        logInfo.LocalGoogleDriveData = Settings.LocalGoogleDriveData;

        if (Settings.LogLevel != LogType.None)
        {
          logInfo.FilePath = Settings.ShellLogFilePath;
        }

        try
        {
          if (FileExists(logInfo.FilePath))
          {
            const long MB = 1048576;
            const long maxLength = MB * 10;

            var ioFileInfo = new System.IO.FileInfo(logInfo.FilePath);

            if (ioFileInfo.Length >= maxLength)
            {
              DeleteFile(logInfo.FilePath);
            }
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }

        return logInfo;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private static void Log_Update()
    {
      try
      {
        TimeSpan timeSpan = DateTime.Now.Subtract(_settingsCheckedTime);

        if (timeSpan.TotalMinutes >= 1)
        {
          Settings.Load();

          _settingsLastWriteTime = System.IO.File.GetLastWriteTime(Settings.FilePath);
        }

        _settingsCheckedTime = DateTime.Now;
      }
      catch (Exception exception)
      {
        try
        {
          _loggingError = true;

          Log.Error("Error updating log file!");
          Log.Error(exception, false);
        }
        finally
        {
          _loggingError = false;
        }
      }
    }

    private static void Log_WriteEntry(string dateTime, string message, LogType type)
    {
      try
      {
        if (_loggingError || !_loggingEnabled)
        {
          return;
        }

        _logMutex.Wait();

        try
        {
          Log_Update();

          if (Settings.LogLevel == LogType.None)
          {
            if (_logStream != null)
            {
              _logStream.Close();
              _logStream.Dispose();
              _logStream = null;
            }

            return;
          }

          if (!FileExists(Settings.LogFilePath))
          {
            if (_logStream != null)
            {
              _logStream.Close();
              _logStream.Dispose();
              _logStream = null;
            }
          }

          if (_logStream == null)
          {
            try
            {
              _logStream = new System.IO.FileStream(Settings.LogFilePath,
                                                    System.IO.FileMode.OpenOrCreate,
                                                    System.IO.FileAccess.Write,
                                                    System.IO.FileShare.Write);
            }
            catch (Exception exception)
            {
              _loggingEnabled = false;

              throw new Exception("Could not create log file " + Settings.LogFilePath + " - " + exception.Message);
            }

            long MB = 1048576;
            long maxLength = MB * 10;

            if (_logStream.Length >= maxLength)
            {
              _logStream.Dispose();

              _logStream = new System.IO.FileStream(Settings.LogFilePath,
                                                    System.IO.FileMode.Create,
                                                    System.IO.FileAccess.Write,
                                                    System.IO.FileShare.Write);
            }

            _logStream.Position = _logStream.Length;
          }

          message = dateTime + " - " + type.ToString().ToUpper() + " - " + message + Environment.NewLine;

          byte[] bytes = System.Text.Encoding.ASCII.GetBytes(message);

          _logStream.Write(bytes, 0, bytes.Length);
          _logStream.Flush();

          _logStream.Close();
          _logStream.Dispose();
          _logStream = null;
        }
        finally
        {
          _logMutex.Release();
        }
      }
      catch (Exception exception)
      {
        try
        {
          _loggingError = true;

          Log.Error("Error capturing log file entries!");
          Log.Error(exception, false);
        }
        finally
        {
          _loggingError = false;
        }
      }
    }

    private static void UpdateAboutData(bool forceUpdate = false)
    {
      try
      {
        using (Connection connection = Connection.Create())
        {
          UpdateAboutData(connection.Service, forceUpdate);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    private static void UpdateAboutData(Google.Apis.Drive.v2.DriveService service, bool forceUpdate = false)
    {
      try
      {
        if (String.IsNullOrEmpty(_rootFolderId) || forceUpdate)
        {
          About about = _GetAbout(service, "largestChangeId, rootFolderId");

          _rootFolderId = GetString(about.RootFolderId);
          _largestChangeId = GetLong(about.LargestChangeId);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Copies the file described by copyStream. </summary>
    /// <param name="copyStream"> The copy stream. </param>
    public void CopyFile(DriveService.CopyStream copyStream)
    {
      try
      {
        QueueStream(copyStream);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Copies the files described by copyStreams. </summary>
    /// <param name="copyStreams">  The copy streams. </param>
    public void CopyFiles(IEnumerable<DriveService.CopyStream> copyStreams)
    {
      try
      {
        QueueStreams(copyStreams);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Downloads the file described by downloadStream. </summary>
    /// <param name="downloadStream"> The download stream. </param>
    public void DownloadFile(DriveService.DownloadStream downloadStream)
    {
      try
      {
        if (downloadStream != null && downloadStream.FileInfo != null)
        {
          if (!downloadStream.FileInfo.IsFile)
          {
            DriveService.Stream.Factory.Abort(downloadStream, DriveService.Stream.StatusType.Completed);

            return;
          }
          else if (downloadStream.FileInfo.IsGoogleDoc)
          {
            WriteGoogleDoc(downloadStream.FileInfo);

            DriveService.Stream.Factory.Abort(downloadStream, DriveService.Stream.StatusType.Completed);

            return;
          }
          else if (String.IsNullOrEmpty(downloadStream.FileInfo.DownloadUrl))
          {
            DriveService.Stream.Factory.Abort(downloadStream, DriveService.Stream.StatusType.Completed);

            return;
          }
          else if (downloadStream.FileInfo.FileSize == 0)
          {
            CreateFile(downloadStream.FileInfo);

            DriveService.Stream.Factory.Abort(downloadStream, DriveService.Stream.StatusType.Completed);

            return;
          }
          else
          {
            FileInfoStatus fileInfoStatus = DriveService.GetFileInfoStatus(downloadStream.FileInfo);

            if (fileInfoStatus == FileInfoStatus.ModifiedOnDisk ||
                fileInfoStatus == FileInfoStatus.OnDisk)
            {
              if (downloadStream.CheckIfAlreadyDownloaded)
              {
                DriveService.Stream.Factory.Abort(downloadStream, DriveService.Stream.StatusType.Completed);

                return;
              }
            }
          }
        }

        QueueStream(downloadStream);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Downloads the files described by downloadStreams. </summary>
    /// <param name="downloadStreams">  The download streams. </param>
    public void DownloadFiles(IEnumerable<DriveService.DownloadStream> downloadStreams)
    {
      try
      {
        foreach (DriveService.DownloadStream downloadStream in downloadStreams)
        {
          DownloadFile(downloadStream);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Gets cached file. </summary>
    /// <param name="fileId">           . </param>
    /// <param name="getChildren">      . </param>
    /// <param name="updateCachedData"> true to update cached data. </param>
    /// <returns> The cached file. </returns>
    public FileInfo GetCachedFile(string fileId, bool getChildren = false, bool updateCachedData = false)
    {
      try
      {
        List<File> children = null;

        File file = _GetCachedFile(fileId, getChildren, updateCachedData, ref children);

        var stopWatch = new Log.Stopwatch();

        Log.StartPerformance(ref stopWatch, "Begin - Updating file data from disk");

        FileInfo fileInfo = GetFileInfo(file, children);

        Log.EndPerformance(ref stopWatch, "End - Updating file data from disk");

        return fileInfo;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    /// <summary> Gets a file. </summary>
    /// <param name="fileId">       . </param>
    /// <param name="getChildren">  . </param>
    /// <returns> The file. </returns>
    public FileInfo GetFile(string fileId, bool getChildren = false)
    {
      List<File> children = null;

      return GetFile(fileId, getChildren, ref children);
    }

    /// <summary> Gets a file. </summary>
    /// <param name="fileId">       . </param>
    /// <param name="getChildren">  . </param>
    /// <param name="children">     [in,out]. </param>
    /// <returns> The file. </returns>
    public FileInfo GetFile(string fileId, bool getChildren, ref List<File> children)
    {
      try
      {
        File file = _GetFile(fileId, getChildren, ref children);

        var stopWatch = new Log.Stopwatch();

        Log.StartPerformance(ref stopWatch, "Begin - Updating file data from disk");

        FileInfo fileInfo = GetFileInfo(file, children);

        Log.EndPerformance(ref stopWatch, "End - Updating file data from disk");

        return fileInfo;
      }
      catch (Exception exception)
      {
        Log.Error(exception, false, false);

        return null;
      }
    }

    /// <summary> Gets the files in this collection. </summary>
    /// <exception cref="Exception">  Thrown when an exception error condition occurs. </exception>
    /// <param name="parentId"> . </param>
    /// <returns>
    ///   An enumerator that allows foreach to be used to process the files in this collection.
    /// </returns>
    public IEnumerable<FileInfo> GetFiles(string parentId)
    {
      try
      {
        List<File> files = _ListFiles(parentId, true);

        if (files == null)
        {
          throw new Exception("File '" + parentId + "' no longer exists.");
        }

        var stopWatch = new Log.Stopwatch();

        Log.StartPerformance(ref stopWatch, "Begin - Updating file data from disk");

        List<FileInfo> fileInfo = GetFileInfo(files);

        Log.EndPerformance(ref stopWatch, "End - Updating file data from disk");

        return fileInfo;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    /// <summary> Inserts a file described by insertStream. </summary>
    /// <param name="insertStream"> . </param>
    public void InsertFile(DriveService.InsertStream insertStream)
    {
      try
      {
        QueueStream(insertStream);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Inserts the files described by insertStreams. </summary>
    /// <param name="insertStreams">  . </param>
    public void InsertFiles(IEnumerable<DriveService.InsertStream> insertStreams)
    {
      try
      {
        QueueStreams(insertStreams);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Move file. </summary>
    /// <param name="moveStream"> . </param>
    public void MoveFile(DriveService.MoveStream moveStream)
    {
      try
      {
        QueueStream(moveStream);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Move files. </summary>
    /// <param name="moveStreams">  . </param>
    public void MoveFiles(IEnumerable<DriveService.MoveStream> moveStreams)
    {
      try
      {
        QueueStreams(moveStreams);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Rename file. </summary>
    /// <param name="renameStream"> . </param>
    public void RenameFile(DriveService.RenameStream renameStream)
    {
      try
      {
        QueueStream(renameStream);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Rename files. </summary>
    /// <param name="renameStreams">  . </param>
    public void RenameFiles(IEnumerable<DriveService.RenameStream> renameStreams)
    {
      try
      {
        QueueStreams(renameStreams);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Trash file. </summary>
    /// <param name="trashStream">  . </param>
    public void TrashFile(DriveService.TrashStream trashStream)
    {
      try
      {
        QueueStream(trashStream);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Trash files. </summary>
    /// <param name="trashStreams"> . </param>
    public void TrashFiles(IEnumerable<DriveService.TrashStream> trashStreams)
    {
      try
      {
        QueueStreams(trashStreams);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Untrash file. </summary>
    /// <param name="untrashStream">  . </param>
    public void UntrashFile(DriveService.UntrashStream untrashStream)
    {
      try
      {
        QueueStream(untrashStream);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Untrash files. </summary>
    /// <param name="untrashStreams"> . </param>
    public void UntrashFiles(IEnumerable<DriveService.UntrashStream> untrashStreams)
    {
      try
      {
        QueueStreams(untrashStreams);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Uploads a file. </summary>
    /// <param name="uploadStream"> . </param>
    public void UploadFile(DriveService.UploadStream uploadStream)
    {
      try
      {
        if (uploadStream != null && uploadStream.FileInfo != null)
        {
          if (!uploadStream.FileInfo.IsFile)
          {
            DriveService.Stream.Factory.Abort(uploadStream, DriveService.Stream.StatusType.Completed);

            return;
          }
          else if (uploadStream.FileInfo.IsGoogleDoc)
          {
            DriveService.Stream.Factory.Abort(uploadStream, DriveService.Stream.StatusType.Completed);

            return;
          }
          else
          {
            FileInfoStatus fileInfoStatus = DriveService.GetFileInfoStatus(uploadStream.FileInfo);

            if (fileInfoStatus != FileInfoStatus.ModifiedOnDisk)
            {
              if (uploadStream.CheckIfAlreadyUploaded)
              {
                DriveService.Stream.Factory.Abort(uploadStream, DriveService.Stream.StatusType.Completed);

                return;
              }
            }
          }
        }

        QueueStream(uploadStream);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Uploads the files. </summary>
    /// <param name="uploadStreams">  . </param>
    public void UploadFiles(IEnumerable<DriveService.UploadStream> uploadStreams)
    {
      try
      {
        QueueStreams(uploadStreams);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Finds the files in this collection. </summary>
    /// <exception cref="Exception">  Thrown when an exception error condition occurs. </exception>
    /// <param name="path"> . </param>
    /// <returns>
    ///   An enumerator that allows foreach to be used to process the files in this collection.
    /// </returns>
    public IEnumerable<FileInfo> FindFiles(string path)
    {
      try
      {
        if (String.IsNullOrEmpty(path))
        {
          throw new Exception("Path is blank.");
        }

        path = path.Replace("/", "\\");

        string[] values = path.Split('\\');

        if (values.Length == 0)
        {
          throw new Exception("FilePath has unexpected data.");
        }

        string title = values[values.Length - 1];
        List<FileInfo> foundFiles = null;

        for (int i = 0; i < values.Length - 1; i++)
        {
          string folderName = values[i];

          if (IsRoot(folderName))
          {
            continue;
          }

          foundFiles = FindFiles(foundFiles, folderName);

          if (foundFiles.Count == 0)
          {
            throw new Exception("File was not found.");
          }
        }

        foundFiles = FindFiles(foundFiles, title);

        if (foundFiles.Count == 0)
        {
          throw new Exception("File was not found.");
        }

        return foundFiles;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private List<FileInfo> FindFiles(List<FileInfo> parents, string title)
    {
      try
      {
        if (parents == null)
        {
          return FindFiles("", title);
        }

        var foundFiles = new List<FileInfo>();

        foreach (FileInfo parent in parents)
        {
          string parentId = parent.Id;

          List<FileInfo> foundChildren = FindFiles(parentId, title);

          foreach (FileInfo foundChild in foundChildren)
          {
            foundFiles.Add(foundChild);
          }
        }

        return foundFiles;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private List<FileInfo> FindFiles(string parentId, string title)
    {
      try
      {
        var foundFiles = new List<FileInfo>();
        IEnumerable<FileInfo> files = GetFiles(parentId);

        foreach (FileInfo file in files)
        {
          if (IsFileEqual(file, title))
          {
            foundFiles.Add(file);
          }
        }

        return foundFiles;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private static bool IsFileEqual(FileInfo fileInfo, string title)
    {
      try
      {
        if (String.Equals(fileInfo.OriginalFilename, title, StringComparison.CurrentCultureIgnoreCase))
        {
          return true;
        }

        return false;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    /// <summary> Updates the cached data. </summary>
    public void UpdateCachedData()
    {
      try
      {
        using (Journal journal = Journal.Create())
        {
          journal.UpdateCachedData();
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Cleanup file. </summary>
    /// <param name="fileInfo">                 Information describing the file. </param>
    /// <param name="ignoreFileOnDiskTimeout">  true to ignore file on disk timeout. </param>
    /// <param name="ignoreException">          true to ignore exception. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    public bool CleanupFile(FileInfo fileInfo, bool ignoreFileOnDiskTimeout = false, bool ignoreException = true)
    {
      try
      {
        FileInfoStatus fileInfoStatus = GetFileInfoStatus(fileInfo);

        return CleanupFile(fileInfo, fileInfoStatus, ignoreFileOnDiskTimeout, ignoreException);
      }
      catch (Exception exception)
      {
        Log.Error(exception, !ignoreException);

        return false;
      }
    }

    private bool CleanupFile(FileInfo fileInfo,
                             FileInfoStatus fileInfoStatus,
                             bool ignoreFileOnDiskTimeout = false,
                             bool ignoreException = true)
    {
      try
      {
        CleanupFileMutex.Wait();

        try
        {
          if (fileInfo == null)
          {
            throw new Exception("File is null.");
          }

          Log.Information("Attempting to clean-up file " + fileInfo.Id);

          if (fileInfo.IsRoot)
          {
            throw new Exception("Cannot cleanup root directory.");
          }

          if (fileInfo.IsFolder)
          {
            return true;
          }

          if (fileInfoStatus == FileInfoStatus.ModifiedOnDisk)
          {
            throw new Exception("Cannot delete file '" + fileInfo.Id + "' - File is modified on disk");
          }

          Log.Warning("Attempting to lock file " + fileInfo.Id);

          FileInfoLock fileInfoLock = null;

          try
          {
            fileInfoLock = LockFile(fileInfo, fileInfo.FilePath);
          }
          catch (Exception exception)
          {
            throw new Exception("Cannot lock file '" + fileInfo.Id + "' - " + exception.Message);
          }

          Log.Information("Successfully locked file " + fileInfo.Id);

          try
          {
            Log.Warning("Attempting to check file time on disk " + fileInfo.Id);

            if (!FileExists(fileInfo.FilePath))
            {
              Log.Information("File does not exist " + fileInfo.Id);
            }
            else
            {
              if (ignoreFileOnDiskTimeout)
              {
                Log.Information("Ignoring file on disk timeout " + fileInfo.Id);
              }
              else
              {
                int totalMintuesOnDisk = 0;
                int fileOnDiskTimeout = 0;

                if (!HasFileOnDiskTimedout(fileInfo.FilePath, ref totalMintuesOnDisk, ref fileOnDiskTimeout))
                {
                  throw new Exception("File '" + fileInfo.Id + "' has not timed out on disk (TotalMintuesOnDisk=" +
                                      totalMintuesOnDisk + "; FileOnDiskTimeout=" + fileOnDiskTimeout + ";)");
                }

                Log.Information("File is ready to be cleaned up " + fileInfo.Id);
              }

              try
              {
                Log.Warning("Attempting to delete file " + fileInfo.Id);

                DeleteFile(fileInfo.FilePath);

                if (FileExists(fileInfo.FilePath))
                {
                  throw new Exception("Unknown error");
                }

                Log.Information("Successfully deleted file " + fileInfo.Id);
              }
              catch (Exception exception)
              {
                throw new Exception("Cannot delete '" + fileInfo.Id + "' - " + exception.Message);
              }
            }

            if (!String.IsNullOrEmpty(fileInfo.FilePath))
            {
              Log.Warning("Attempting to get file directory " + fileInfo.Id);

              string folderPath = System.IO.Path.GetDirectoryName(fileInfo.FilePath);

              if (!DirectoryExists(folderPath))
              {
                Log.Information("File directory does not exist " + fileInfo.Id);
              }
              else
              {
                Log.Warning("Checking for file sub-directories " + fileInfo.Id);

                string[] subDirectories = System.IO.Directory.GetDirectories(folderPath);

                if (subDirectories.Length > 0)
                {
                  throw new Exception("Cannot delete '" + fileInfo.Id + "' - Found unexpected subdirectories in folder.");
                }

                Log.Information("File does not contain sub-directories " + fileInfo.Id);
                Log.Warning("Checking for file directory contents " + fileInfo.Id);

                string[] files = System.IO.Directory.GetFiles(folderPath);

                if (files.Length > 1)
                {
                  throw new Exception("Cannot delete '" + fileInfo.Id + "' - Found unexpected files in folder.");
                }

                Log.Information("File directory does not have any content " + fileInfo.Id);

                try
                {
                  Log.Warning("Attempting to delete file directory " + fileInfo.Id);

                  DeleteDirectory(folderPath);

                  if (DirectoryExists(folderPath))
                  {
                    throw new Exception("Unknown error");
                  }

                  Log.Information("Successfully deleted file directory " + fileInfo.Id);
                }
                catch (Exception exception)
                {
                  throw new Exception("Cannot delete file directory '" + fileInfo.Id + "' - " + exception.Message);
                }
              }
            }

            Log.Information("Successfully checked file time on disk " + fileInfo.Id);

            return true;
          }
          finally
          {
            if (fileInfoLock != null)
            {
              fileInfoLock.Dispose();
              fileInfoLock = null;
            }
          }
        }
        finally
        {
          CleanupFileMutex.Release();
        }
      }
      catch (Exception exception)
      {
        if (ignoreException)
        {
          Log.Error(exception, false, false);
        }
        else
        {
          Log.Error(exception, true, true);
        }

        return false;
      }
    }

    private static bool IsFileLocked(string fileId, string filePath)
    {
      try
      {
        using (State state = State.Create())
        {
          return state.IsFileLocked(fileId, filePath);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    private static FileInfoLock LockFile(string fileId, string filePath)
    {
      try
      {
        using (State state = State.Create())
        {
          return state.LockFile(fileId, filePath);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private static FileInfoLock LockFile(string fileId,
                                         string filePath,
                                         System.IO.FileMode fileMode,
                                         System.IO.FileAccess fileAccess,
                                         System.IO.FileShare fileShare)
    {
      try
      {
        using (State state = State.Create())
        {
          return state.LockFile(fileId, filePath, fileMode, fileAccess, fileShare);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private FileInfoLock LockFile(File file, string filePath)
    {
      try
      {
        return LockFile(file.Id, filePath);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private FileInfoLock LockFile(File file,
                                  string filePath,
                                  System.IO.FileMode fileMode,
                                  System.IO.FileAccess fileAccess,
                                  System.IO.FileShare fileShare)
    {
      try
      {
        return LockFile(file.Id, filePath, fileMode, fileAccess, fileShare);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private FileInfoLock LockFile(FileInfo fileInfo, string filePath)
    {
      try
      {
        return LockFile(fileInfo.Id, filePath);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private FileInfoLock LockFile(FileInfo fileInfo,
                                  string filePath,
                                  System.IO.FileMode fileMode,
                                  System.IO.FileAccess fileAccess,
                                  System.IO.FileShare fileShare)
    {
      try
      {
        return LockFile(fileInfo.Id, filePath, fileMode, fileAccess, fileShare);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private static bool IsStreamLocked(string fileId, string filePath, ref DriveService.Stream stream)
    {
      try
      {
        using (State state = State.Create())
        {
          return state.IsStreamLocked(fileId, filePath, ref stream);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    private static FileInfoLock LockStream(DriveService.Stream stream, string filePath)
    {
      try
      {
        using (State state = State.Create())
        {
          return state.LockStream(stream, filePath);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private static FileInfoLock LockStream(DriveService.Stream stream,
                                           string filePath,
                                           System.IO.FileMode fileMode,
                                           System.IO.FileAccess fileAccess,
                                           System.IO.FileShare fileShare)
    {
      try
      {
        using (State state = State.Create())
        {
          return state.LockStream(stream, filePath, fileMode, fileAccess, fileShare);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private static void QueueStream(DriveService.Stream stream)
    {
      try
      {
        using (State state = State.Create())
        {
          state.QueueStream(stream);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    private static void QueueStreams(IEnumerable<DriveService.Stream> streams)
    {
      try
      {
        using (State state = State.Create())
        {
          state.QueueStreams(streams);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    private static About _GetAbout(string fields = null)
    {
      try
      {
        using (Connection connection = Connection.Create())
        {
          return _GetAbout(connection.Service, fields);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private static About _GetAbout(Google.Apis.Drive.v2.DriveService service, string fields = null)
    {
      try
      {
        var stopWatch = new Log.Stopwatch();

        Log.StartPerformance(ref stopWatch, "Begin - Google.Apis.Drive.v2.DriveService.About.Get()");

        try
        {
          AboutResource.GetRequest getRequest = service.About.Get();

          if (!String.IsNullOrEmpty(fields))
          {
            getRequest.Fields = fields;
          }

          About about = getRequest.Execute();

          return about;
        }
        finally
        {
          Log.EndPerformance(ref stopWatch, "End - Google.Apis.Drive.v2.DriveService.About.Get()");
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private static File _GetFile(string fileId)
    {
      try
      {
        using (Connection connection = Connection.Create())
        {
          return _GetFile(connection.Service, fileId);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private static File _GetFile(Google.Apis.Drive.v2.DriveService service, string fileId)
    {
      try
      {
        fileId = GetFileId(fileId);

        File file = null;

        var stopWatch = new Log.Stopwatch();

        Log.StartPerformance(ref stopWatch, "Begin - Google.Apis.Drive.v2.DriveService.Files.Get(fileId=" + fileId + ")");

        try
        {
          FilesResource.GetRequest getRequest = service.Files.Get(fileId);

          getRequest.Fields = RequestFields.FileFields;

          file = getRequest.Execute();

          if (file == null)
          {
            throw new Exception("File '" + fileId + "' no longer exists.");
          }
        }
        finally
        {
          Log.EndPerformance(ref stopWatch, "End - Google.Apis.Drive.v2.DriveService.Files.Get()");
        }

        MakeFileNameWindowsSafe(file);

        return file;
      }
      catch (Exception exception)
      {
        Journal.Refresh();
        Log.Error(exception, breakWithDebugger: false);

        return null;
      }
    }

    private static File _GetFile(string fileId, bool getChildren, ref List<File> children)
    {
      try
      {
        File file = _GetFile(fileId);

        if (file == null)
        {
          throw new Exception("File '" + fileId + "' no longer exists.");
        }

        children = null;

        if (getChildren)
        {
          children = _ListFiles(fileId, true);
        }

        return file;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private static File _GetCachedFile(string fileId, bool getChildren, bool updateCachedData, ref List<File> children)
    {
      try
      {
        children = null;

        // ensures user is logged in and does not try to login multiple times (by hitting subsequent create connection calls)
        using (Connection connection = Connection.Create())
        {
        }

        if (!Settings.UseCaching)
        {
          Log.Performance("Not using caching - fileId=" + fileId);

          return _GetFile(fileId, getChildren, ref children);
        }

        if (!Journal.HasSynced(false))
        {
          Log.Performance("Using caching (no data available) - fileId=" + fileId + ", getChildren=" + getChildren);

          List<File> tempChildren = null;

          File tempFile = _GetFile(fileId, getChildren, ref tempChildren);

          children = new List<File>(tempChildren);

          Journal.Cache(tempFile, tempChildren);

          return tempFile;
        }

        Log.Performance("Using caching (data available) - fileId=" + fileId + ", getChildren=" + getChildren +
                        ", updateCachedData=" + updateCachedData);

        File file = null;

        using (Journal journal = Journal.Create())
        {
          if (updateCachedData)
          {
            journal.UpdateCachedData();
          }

          file = journal.GetCachedFile(fileId, getChildren, out children);
        }

        if (file == null)
        {
          throw new Exception("File '" + fileId + "' no longer exists.");
        }

        if (!getChildren)
        {
          children = null;
        }

        return file;
      }
      catch (Exception exception)
      {
        Log.Error(exception, breakWithDebugger: false);

        return null;
      }
    }

    private static List<File> _ListFiles(string parentId, bool showFeedback)
    {
      try
      {
        using (Connection connection = Connection.Create())
        {
          return _ListFiles(connection.Service, parentId, showFeedback);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private static List<File> _ListFiles(Google.Apis.Drive.v2.DriveService service, string parentId, bool showFeedback)
    {
      try
      {
        _cancelProcessing = false;

        parentId = GetFileId(parentId);

        var files = new List<File>();

        var stopWatch = new Log.Stopwatch();

        Log.StartPerformance(ref stopWatch, "Begin - Google.Apis.Drive.v2.DriveService.Files.List()");

        ListFilesMutex.Wait();

        try
        {
          if (showFeedback)
          {
            FeedbackForm.ShowDialog("Preparing to list files...");
          }

          try
          {
            FilesResource.ListRequest listRequest = service.Files.List();

            listRequest.MaxResults = 100;

            listRequest.Fields = RequestFields.ListFileFields;

            listRequest.Q = "trashed=false and '" + parentId + "' in parents";

            if (DriveService.Settings.FileReturnType == FileReturnType.IgnoreGoogleFiles)
            {
              listRequest.Q += " and mimeType != 'application/vnd.google-apps.document'";
              listRequest.Q += " and mimeType != 'application/vnd.google-apps.drawing'";
              listRequest.Q += " and mimeType != 'application/vnd.google-apps.file'";
              listRequest.Q += " and mimeType != 'application/vnd.google-apps.form'";
              listRequest.Q += " and mimeType != 'application/vnd.google-apps.presentation'";
              listRequest.Q += " and mimeType != 'application/vnd.google-apps.spreadsheet'";
              //listRequest.Q += " and mimeType != 'application/vnd.google-apps.audio'";
              //listRequest.Q += " and mimeType != 'application/vnd.google-apps.video'";
              //listRequest.Q += " and mimeType != 'application/vnd.google-apps.photo'";
              //listRequest.Q += " and mimeType != 'application/vnd.google-apps.fusiontable'";
              //listRequest.Q += " and mimeType != 'application/vnd.google-apps.kix'";
              //listRequest.Q += " and mimeType != 'application/vnd.google-apps.sites'";
              //listRequest.Q += " and mimeType != 'application/vnd.google-apps.script'";
              //listRequest.Q += " and mimeType != 'application/vnd.google-apps.unknown'";
            }

            do
            {
              if (_cancelProcessing)
              {
                return new List<File>();
              }

              if (FeedbackForm.IsCancelling)
              {
                break;
              }

              FileList fileList = listRequest.Execute();

              if (DriveService.Settings.FileReturnType == FileReturnType.FilterGoogleFiles)
              {
                foreach (File file in fileList.Items)
                {
                  if (file.MimeType != null &&
                      file.MimeType != "application/vnd.google-apps.folder" &&
                      file.MimeType.IndexOf("application/vnd.google-apps.") == 0)
                  {
                    continue;
                  }

                  files.Add(file);
                }
              }
              else
              {
                files.AddRange(fileList.Items);
              }

              listRequest.PageToken = fileList.NextPageToken;

              Log.Performance("Loaded " + files.Count + " file(s)");

              FeedbackForm.Message("Loaded " + files.Count + " file(s)...");

              System.Threading.Thread.Sleep(1);
              System.Windows.Forms.Application.DoEvents();
              System.Threading.Thread.Sleep(1);
            } while (!String.IsNullOrEmpty(listRequest.PageToken));
          }
          finally
          {
            Log.EndPerformance(ref stopWatch, "End - Google.Apis.Drive.v2.DriveService.Files.List()");
          }

          foreach (File file in files)
          {
            MakeFileNameWindowsSafe(file);
          }
        }
        finally
        {
          if (showFeedback)
          {
            FeedbackForm.CloseDialog();
          }

          ListFilesMutex.Release();
        }

        return files;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private static List<Change> _ListChanges(long startChangeId, out long largestChangeId)
    {
      largestChangeId = 0;

      try
      {
        using (Connection connection = Connection.Create())
        {
          return _ListChanges(connection.Service, startChangeId, out largestChangeId);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private static List<Change> _ListChanges(Google.Apis.Drive.v2.DriveService service,
                                             long startChangeId,
                                             out long largestChangeId)
    {
      largestChangeId = 0;

      try
      {
        _cancelProcessing = false;

        var changes = new List<Change>();

        if (startChangeId != 0)
        {
          About about = _GetAbout("largestChangeId");

          long aboutLargestChangeId = GetLong(about.LargestChangeId);

          if (aboutLargestChangeId == startChangeId)
          {
            largestChangeId = startChangeId;

            return changes;
          }
        }

        var stopWatch = new Log.Stopwatch();

        Log.StartPerformance(ref stopWatch,
                             "Begin - Google.Apis.Drive.v2.DriveService.Changes.List(startChangeId=" + startChangeId +
                             ")");

        try
        {
          ChangesResource.ListRequest listRequest = service.Changes.List();

          listRequest.MaxResults = 100;

          if (startChangeId != 0)
          {
            listRequest.StartChangeId = startChangeId;
          }

          do
          {
            if (_cancelProcessing)
            {
              return new List<Change>();
            }

            ChangeList changeList = listRequest.Execute();

            changes.AddRange(changeList.Items);

            largestChangeId = GetLong(changeList.LargestChangeId);

            listRequest.PageToken = changeList.NextPageToken;

            Log.Performance("Loaded " + changes.Count + " change(s)");
          } while (!String.IsNullOrEmpty(listRequest.PageToken));
        }
        finally
        {
          Log.EndPerformance(ref stopWatch, "End - Google.Apis.Drive.v2.DriveService.Changes.List()");
        }

        return changes;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private static void MakeFileNameWindowsSafe(File file)
    {
      file.OriginalFilename = file.Title;
      file.Title = Win32.MakeFileNameWindowsSafe(file.Title);
    }

    private static void SortFiles(ref List<File> files, bool watchFileType)
    {
      try
      {
        files.Sort(new FileComparer(watchFileType));
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    private static void SortFiles(ref List<FileInfo> files)
    {
      try
      {
        files.Sort(new FileInfoComparer());
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary> Cleanup files. </summary>
    /// <exception cref="Exception">  Thrown when an exception error condition occurs. </exception>
    /// <param name="ignoreException">  true to ignore exception. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    public static bool CleanupFiles(bool ignoreException = true)
    {
      try
      {
        string[] folders = Settings.GetLocalGoogleDriveDataSubFolders();

        foreach (string folder in folders)
        {
          try
          {
            DeleteDirectory(folder);
          }
          catch (Exception exception)
          {
            if (!ignoreException)
            {
              throw exception;
            }
          }
        }

        return true;
      }
      catch (Exception exception)
      {
        if (ignoreException)
        {
          Log.Error(exception, false, false);
        }
        else
        {
          Log.Error(exception, true, true);
        }

        return false;
      }
    }

    /// <summary> Clears the cache. </summary>
    /// <exception cref="Exception">  Thrown when an exception error condition occurs. </exception>
    /// <param name="ignoreException">  true to ignore exception. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    public static bool ClearCache(bool ignoreException = true)
    {
      try
      {
        try
        {
          DeleteFile(Settings.RootIdFilePath);
        }
        catch (Exception exception)
        {
          if (!ignoreException)
          {
            throw exception;
          }
        }

        try
        {
          DeleteFile(Settings.LastChangeIdPath);
        }
        catch (Exception exception)
        {
          if (!ignoreException)
          {
            throw exception;
          }
        }

        try
        {
          DeleteDirectoryContents(Settings.JournalFolderPath);
        }
        catch (Exception exception)
        {
          if (!ignoreException)
          {
            throw exception;
          }
        }

        Journal.Refresh();

        return true;
      }
      catch (Exception exception)
      {
        if (ignoreException)
        {
          Log.Error(exception, false, false);
        }
        else
        {
          Log.Error(exception, true, true);
        }

        return false;
      }
    }

    /// <summary> Creates desktop initialise. </summary>
    /// <param name="newLocation">  true to new location. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    public static bool CreateDesktopIni(bool newLocation)
    {
      try
      {
        string iniFilePath = "";

        iniFilePath = newLocation ? Settings.DesktopNewIniFilePath : Settings.DesktopOldIniFilePath;

        if (!FileExists(iniFilePath) &&
            Settings.GoogleDriveFolder == string.Format(@"{0}\{1}", Settings.CompanyPath, Settings.Product))
        {
          CreateFile(iniFilePath);

          System.IO.File.WriteAllText(iniFilePath,
                                      "[.ShellClassInfo]" + Environment.NewLine + "CLSID={" + Settings.GUID_GDriveShlExt +
                                      "}");

          System.IO.DirectoryInfo directoryInfo = System.IO.Directory.GetParent(iniFilePath);

          directoryInfo.Attributes |= System.IO.FileAttributes.System;
        }

        return true;
      }
      catch (Exception exception)
      {
        Log.Error(exception, false, true);

        return false;
      }
    }

    /// <summary> Deletes the desktop initialise described by newLocation. </summary>
    /// <param name="newLocation">  true to new location. </param>
    /// <returns> true if it succeeds, false if it fails. </returns>
    public static bool DeleteDesktopIni(bool newLocation)
    {
      try
      {
        string iniFilePath = "";

        iniFilePath = newLocation ? Settings.DesktopNewIniFilePath : Settings.DesktopOldIniFilePath;

        if (FileExists(iniFilePath) &&
            Settings.GoogleDriveFolder == string.Format(@"{0}\{1}", Settings.CompanyPath, Settings.Product))
        {
          DeleteFile(iniFilePath);
        }

        string tempPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        tempPath = System.IO.Path.Combine(tempPath,
                                          string.Format(@"{0}\{1}\Desktop.ini", Settings.CompanyPath, Settings.Product));

        DeleteFile(tempPath);

        return true;
      }
      catch (Exception exception)
      {
        Log.Error(exception, false, true);

        return false;
      }
    }

    private static FileInfo GetFileInfo(File file)
    {
      try
      {
        MakeFileNameWindowsSafe(file);

        var fileInfo = new FileInfo(file);

        UpdateFileInfo(fileInfo);

        return fileInfo;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private static List<FileInfo> GetFileInfo(List<File> files)
    {
      try
      {
        var fileInfoList = new List<FileInfo>();

        foreach (File file in files)
        {
          var fileInfo = new FileInfo(file);

          UpdateFileInfo(fileInfo);

          fileInfoList.Add(fileInfo);
        }

        SortFiles(ref fileInfoList);

        return fileInfoList;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private static FileInfo GetFileInfo(File file, List<File> files)
    {
      try
      {
        FileInfo fileInfo = GetFileInfo(file);

        if (files != null && files.Count > 0)
        {
          fileInfo._files = GetFileInfo(files);
        }

        return fileInfo;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private static void UpdateFileInfo(FileInfo fileInfo)
    {
      try
      {
        File file = fileInfo._file;

        // make sure the order does not change

        fileInfo.Id = GetFileId(GetString(file.Id));
        fileInfo.MimeType = GetMimeType(file);
        fileInfo.Type = GetFileInfoType(file);

        fileInfo.Title = GetTitle(file);
        fileInfo.FileName = GetFileName(file, fileInfo.Type);
        fileInfo.FilePath = GetFilePath(file, fileInfo.Type);
        fileInfo.FileExtension = GetFileExtension(file, fileInfo.Type);
        fileInfo.FileSize = GetLong(file.FileSize);
        fileInfo.Association = GetFileInfoAssociation(file, fileInfo.Type, fileInfo.FileExtension);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    internal static string GetFileExtension(File file)
    {
      try
      {
        FileInfoType type = GetFileInfoType(file);

        return GetFileExtension(file, type);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    internal static string GetFileExtension(File file, FileInfoType type)
    {
      try
      {
        if (type == FileInfoType.Root)
        {
          return "";
        }
        else if (type == FileInfoType.Folder)
        {
          return "";
        }

        string name = GetString(file.Title);

        string extension = System.IO.Path.GetExtension(name);

        if (!String.IsNullOrEmpty(file.FileExtension) && !file.FileExtension.StartsWith("."))
        {
          extension = "." + file.FileExtension;
        }

        return extension;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    internal static string GetFileId(string fileId)
    {
      try
      {
        if (IsRoot(fileId))
        {
          fileId = RootFolderId;
        }

        return fileId;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    internal static System.Drawing.Icon GetFileIcon(FileInfo fileInfo, int width, int height, bool getClosestSize = true)
    {
      try
      {
        if (fileInfo == null ||
            fileInfo.Association == null ||
            String.IsNullOrEmpty(fileInfo.Association.DefaultIcon))
        {
          return null;
        }

        string defaultIcon = fileInfo.Association.DefaultIcon;
        int defaultIconIndex = 0;

        int.TryParse(fileInfo.Association.DefaultIconIndex, out defaultIconIndex);

        System.Drawing.Icon icon = Win32.GetIcon(defaultIcon, defaultIconIndex, width, height, getClosestSize);

        return icon;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    internal static FileInfoAssociation GetFileInfoAssociation(File file)
    {
      try
      {
        FileInfoType type = GetFileInfoType(file);
        string fileExtension = GetFileExtension(file, type);

        return GetFileInfoAssociation(file, type, fileExtension);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    internal static FileInfoAssociation GetFileInfoAssociation(File file, FileInfoType type, string fileExtension)
    {
      try
      {
        FileInfoAssociation association = null;

        if (IsFolder(type))
        {
          association = new FileInfoAssociation();

          association.FileType = "File folder";

          return association;
        }

        if (IsGoogleDoc(type))
        {
          string temp = FileInfoTypeService.GetFileExtension(type);

          if (!String.IsNullOrEmpty(temp))
          {
            fileExtension = temp;
          }
        }

        if (_associations.TryGetValue(fileExtension.ToLower(), out association))
        {
          return association;
        }

        association = new FileInfoAssociation();

        association.FileExtension = fileExtension;

        if (IsFolder(type))
        {
          association.FileType = "File folder";
        }
        else if (IsGoogleDoc(type))
        {
          switch (type)
          {
            case FileInfoType.Document:
              association.FileType = "Google Doc";
              association.Application = Settings.LocationFileName;
              association.DefaultIcon = System.IO.Path.Combine(Settings.ImageFolderPath, "Doc.ico");
              association.DefaultIconIndex = "0";
              break;
            case FileInfoType.Form:
              association.FileType = "Google Form";
              association.Application = Settings.LocationFileName;
              association.DefaultIcon = System.IO.Path.Combine(Settings.ImageFolderPath, "Form.ico");
              association.DefaultIconIndex = "0";
              break;
            case FileInfoType.Drawing:
              association.FileType = "Google Drawing";
              association.Application = Settings.LocationFileName;
              association.DefaultIcon = System.IO.Path.Combine(Settings.ImageFolderPath, "Drawing.ico");
              association.DefaultIconIndex = "0";
              break;
            case FileInfoType.Presentation:
              association.FileType = "Google Presentation";
              association.Application = Settings.LocationFileName;
              association.DefaultIcon = System.IO.Path.Combine(Settings.ImageFolderPath, "Presentation.ico");
              association.DefaultIconIndex = "0";
              break;
            case FileInfoType.Spreadsheet:
              association.FileType = "Google Spreadsheet";
              association.Application = Settings.LocationFileName;
              association.DefaultIcon = System.IO.Path.Combine(Settings.ImageFolderPath, "Spreadsheet.ico");
              association.DefaultIconIndex = "0";
              break;
            case FileInfoType.FusionTable:
              association.FileType = "Google FusionTable";
              association.Application = Settings.LocationFileName;
              association.DefaultIcon = System.IO.Path.Combine(Settings.ImageFolderPath, "FusionTables.ico");
              association.DefaultIconIndex = "0";
              break;
            case FileInfoType.Root:
            case FileInfoType.Folder:
            case FileInfoType.Audio:
            case FileInfoType.File:
            case FileInfoType.Video:
            case FileInfoType.Kix:
            case FileInfoType.Photo:
            case FileInfoType.Script:
            case FileInfoType.Sites:
            case FileInfoType.NotImplemented:
            case FileInfoType.Unknown:
            case FileInfoType.None:
            default:
              association.FileType = "Google File";
              association.Application = Settings.LocationFileName;
              association.DefaultIcon = System.IO.Path.Combine(Settings.ImageFolderPath, "File.ico");
              association.DefaultIconIndex = "0";
              break;
          }
        }
        else if (!String.IsNullOrEmpty(association.FileExtension))
        {
          Win32.GetFileAssociation(association.FileExtension,
                                   ref association.FileType,
                                   ref association.Application,
                                   ref association.DefaultIcon,
                                   ref association.DefaultIconIndex);

          if (association.DefaultIcon == "%1")
          {
            association.DefaultIcon = GetFilePath(file);
            association.DefaultIconIndex = "0";
          }
        }

        _associations.TryAdd(fileExtension.ToLower(), association);

        return association;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    internal static FileInfoStatus GetFileInfoStatus(FileInfo fileInfo)
    {
      try
      {
        var fileInfoStatus = FileInfoStatus.InCloud;

        if (!fileInfo.IsFile)
        {
          return fileInfoStatus;
        }

        File file = fileInfo._file;

        DateTime modifiedDate = GetDateTime(file.ModifiedDate);

        if (FileExists(fileInfo.FilePath))
        {
          if (fileInfo.IsGoogleDoc)
          {
            string currentText = System.IO.File.ReadAllText(fileInfo.FilePath);
            string googleDocText = GetGoogleDocText(fileInfo);

            if (currentText == googleDocText)
            {
              fileInfoStatus = FileInfoStatus.OnDisk;
            }
          }
          else if (String.IsNullOrEmpty(file.DownloadUrl))
          {
          }
          else
          {
            long fileSize = GetLong(file.FileSize);

            var temp = new System.IO.FileInfo(fileInfo.FilePath);

            fileInfo.LastWriteTime = temp.LastWriteTime;
            fileInfo.Length = temp.Length;

            if (IsDateTimeEqual(temp.LastWriteTime, modifiedDate) &&
                temp.Length == fileSize)
            {
              fileInfoStatus = FileInfoStatus.OnDisk;
            }
            else if (IsDateTimeGreaterThan(temp.LastWriteTime, modifiedDate))
            {
              fileInfoStatus = FileInfoStatus.ModifiedOnDisk;
            }
          }
        }

        return fileInfoStatus;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return FileInfoStatus.Unknown;
      }
    }

    internal static FileInfoType GetFileInfoType(File file)
    {
      try
      {
        if (String.IsNullOrEmpty(file.MimeType))
        {
          return FileInfoType.None;
        }

        if (IsRoot(file))
        {
          return FileInfoType.Root;
        }

        if (String.Equals(file.MimeType, "application/vnd.google-apps.audio", StringComparison.CurrentCultureIgnoreCase))
        {
          return FileInfoType.Audio;
        }

        if (String.Equals(file.MimeType,
                          "application/vnd.google-apps.document",
                          StringComparison.CurrentCultureIgnoreCase))
        {
          return FileInfoType.Document;
        }

        if (String.Equals(file.MimeType,
                          "application/vnd.google-apps.drawing",
                          StringComparison.CurrentCultureIgnoreCase))
        {
          return FileInfoType.Drawing;
        }

        if (String.Equals(file.MimeType, "application/vnd.google-apps.file", StringComparison.CurrentCultureIgnoreCase))
        {
          return FileInfoType.File;
        }

        if (String.Equals(file.MimeType, DriveService.MimeType.Folder, StringComparison.CurrentCultureIgnoreCase))
        {
          return FileInfoType.Folder;
        }

        if (String.Equals(file.MimeType, "application/vnd.google-apps.form", StringComparison.CurrentCultureIgnoreCase))
        {
          return FileInfoType.Form;
        }

        if (String.Equals(file.MimeType,
                          "application/vnd.google-apps.fusiontable",
                          StringComparison.CurrentCultureIgnoreCase))
        {
          return FileInfoType.FusionTable;
        }

        if (String.Equals(file.MimeType, "application/vnd.google-apps.kix", StringComparison.CurrentCultureIgnoreCase))
        {
          return FileInfoType.Kix;
        }

        if (String.Equals(file.MimeType, "application/vnd.google-apps.photo", StringComparison.CurrentCultureIgnoreCase))
        {
          return FileInfoType.Photo;
        }

        if (String.Equals(file.MimeType,
                          "application/vnd.google-apps.presentation",
                          StringComparison.CurrentCultureIgnoreCase))
        {
          return FileInfoType.Presentation;
        }

        if (String.Equals(file.MimeType, "application/vnd.google-apps.script", StringComparison.CurrentCultureIgnoreCase))
        {
          return FileInfoType.Script;
        }

        if (String.Equals(file.MimeType, "application/vnd.google-apps.sites", StringComparison.CurrentCultureIgnoreCase))
        {
          return FileInfoType.Sites;
        }

        if (String.Equals(file.MimeType,
                          "application/vnd.google-apps.spreadsheet",
                          StringComparison.CurrentCultureIgnoreCase))
        {
          return FileInfoType.Spreadsheet;
        }

        if (String.Equals(file.MimeType,
                          "application/vnd.google-apps.unknown",
                          StringComparison.CurrentCultureIgnoreCase))
        {
          return FileInfoType.Unknown;
        }

        if (String.Equals(file.MimeType, "application/vnd.google-apps.video", StringComparison.CurrentCultureIgnoreCase))
        {
          return FileInfoType.Video;
        }

        if (file.MimeType.IndexOf("application/vnd.google-apps", StringComparison.CurrentCultureIgnoreCase) == 0)
        {
          return FileInfoType.NotImplemented;
        }

        return FileInfoType.NonGoogleDoc;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return FileInfoType.Unknown;
      }
    }

    internal static string GetFileName(File file)
    {
      try
      {
        FileInfoType type = GetFileInfoType(file);

        return GetFileName(file, type);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    internal static string GetFileName(File file, FileInfoType type)
    {
      try
      {
        if (type == FileInfoType.Root ||
            type == FileInfoType.Folder)
        {
          return "";
        }

        string name = GetString(file.Title);

        name = System.IO.Path.GetFileNameWithoutExtension(name);

        return name;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    internal static string GetFilePath(File file)
    {
      try
      {
        if (String.IsNullOrEmpty(file.Id))
        {
          return "";
        }

        if (String.IsNullOrEmpty(file.Title))
        {
          return "";
        }

        FileInfoType type = GetFileInfoType(file);

        return GetFilePath(file, type);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    internal static string GetFilePath(File file, FileInfoType type)
    {
      try
      {
        if (String.IsNullOrEmpty(file.Id))
        {
          return "";
        }

        if (String.IsNullOrEmpty(file.Title))
        {
          return "";
        }

        if (type == FileInfoType.Root ||
            type == FileInfoType.Folder)
        {
          return Settings.LocalGoogleDriveData;
        }

        string path = System.IO.Path.Combine(FolderPath, file.Id, file.Title);

        return path;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    internal static string GetMimeType(string fileName)
    {
      try
      {
        if (String.IsNullOrEmpty(fileName))
        {
          throw new Exception("FileName is blank.");
        }

        string fileExtension = System.IO.Path.GetExtension(fileName);

        if (!fileExtension.StartsWith("."))
        {
          fileExtension = "." + fileExtension;
        }

        string mimeType = (MimeType.KnownTypes.TryGetValue(fileExtension, out mimeType)
          ? mimeType
          : "application/octet-stream");

        if (string.IsNullOrEmpty(mimeType))
        {
          Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(fileExtension);

          if (regKey != null && regKey.GetValue("Content Type") != null)
          {
            mimeType = regKey.GetValue("Content Type").ToString();
          }
          else
          {
            mimeType = "application/octet-stream";
          }
        }

        return mimeType;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    internal static string GetMimeType(File file)
    {
      try
      {
        if (String.IsNullOrEmpty(file.MimeType))
        {
          return "";
        }

        string mimeType = file.MimeType;

        const string search = "application/vnd.";

        if (mimeType.IndexOf(search) == 0)
        {
          mimeType = mimeType.Substring(search.Length, mimeType.Length - search.Length);
        }

        return mimeType;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    internal static string GetOwnerName(File file)
    {
      try
      {
        if (file.OwnerNames != null)
        {
          if (file.OwnerNames.Count > 0)
          {
            return file.OwnerNames[0];
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

    internal static string GetParentId(File file, bool allowRoot = true)
    {
      try
      {
        if (file != null && file.Parents != null && file.Parents.Count > 0)
        {
          if (!allowRoot && Convert.ToBoolean(file.Parents[0].IsRoot))
          {
            return "";
          }

          return file.Parents[0].Id;
        }

        return "";
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    internal static string GetTitle(string filePath)
    {
      try
      {
        return System.IO.Path.GetFileName(filePath);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    internal static string GetTitle(File file)
    {
      try
      {
        if (IsRoot(file))
        {
          return Settings.Product;
        }

        return GetString(file.Title);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    internal static bool GetBoolean(bool? value, bool defaultValue = false)
    {
      try
      {
        if (value != null && value.HasValue)
        {
          return value.Value;
        }

        return defaultValue;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return defaultValue;
      }
    }

    internal static DateTime GetDateTime(string value)
    {
      try
      {
        return GetDateTime(value, default(DateTime));
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return default(DateTime);
      }
    }

    internal static DateTime GetDateTime(string value, DateTime defaultValue)
    {
      try
      {
        DateTime result;

        if (DateTime.TryParse(value, out result))
        {
          return result.ToLocalTime();
        }

        return defaultValue;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return defaultValue;
      }
    }

    internal static DateTime GetDateTime(DateTime? value)
    {
      try
      {
        return GetDateTime(value, default(DateTime));
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return default(DateTime);
      }
    }

    internal static DateTime GetDateTime(DateTime? value, DateTime defaultValue)
    {
      try
      {
        if (value != null && value.HasValue)
        {
          return value.Value.ToLocalTime();
        }

        return defaultValue;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return defaultValue;
      }
    }

    internal static string GetDateTimeAsString(string value, string defaultValue = "")
    {
      try
      {
        DateTime result = GetDateTime(value);

        if (result == default(DateTime))
        {
          return defaultValue;
        }

        return result.ToString();
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return defaultValue;
      }
    }

    internal static string GetDateTimeAsString(DateTime dateTime)
    {
      try
      {
        //format=2013-10-05T20:28:10.773Z
        dateTime = dateTime.ToUniversalTime();

        string value =
          dateTime.Year +
          "-" +
          dateTime.Month.ToString().PadLeft(2, '0') +
          "-" +
          dateTime.Day.ToString().PadLeft(2, '0') +
          "T" +
          dateTime.Hour.ToString().PadLeft(2, '0') +
          ":" +
          dateTime.Minute.ToString().PadLeft(2, '0') +
          ":" +
          dateTime.Second.ToString().PadLeft(2, '0') +
          "." +
          dateTime.Millisecond.ToString().PadLeft(3, '0') +
          "Z";

        return value;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    internal static string GetFileSize(string value, string defaultValue = "")
    {
      try
      {
        long result = GetLong(value, -1);

        if (result == -1)
        {
          return defaultValue;
        }

        const long TB = 1099511627776;
        const long GB = 1073741824;
        const long MB = 1048576;
        const long KB = 1024;

        if (result >= TB)
        {
          decimal result2 = Math.Round((decimal)result / (decimal)TB, 1);

          return result2.ToString() + " TB";
        }
        else if (result >= GB)
        {
          decimal result2 = Math.Round((decimal)result / (decimal)GB, 1);

          return result2.ToString() + " GB";
        }
        else if (result >= MB)
        {
          decimal result2 = Math.Round((decimal)result / (decimal)MB, 1);

          return result2.ToString() + " MB";
        }
        else if (result >= KB)
        {
          decimal result2 = Math.Round((decimal)result / (decimal)KB, 1);

          return result2.ToString() + " KB";
        }
        else
        {
          return result.ToString() + " bytes";
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return defaultValue;
      }
    }

    internal static int GetInt32(string value, int defaultValue = 0)
    {
      try
      {
        int result = 0;

        if (int.TryParse(value, out result))
        {
          return result;
        }

        return defaultValue;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return defaultValue;
      }
    }

    internal static long GetLong(string value, long defaultValue = 0)
    {
      try
      {
        long result = 0;

        if (long.TryParse(value, out result))
        {
          return result;
        }

        return defaultValue;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return defaultValue;
      }
    }

    internal static long GetLong(long? value, long defaultValue = 0)
    {
      try
      {
        if (value != null && value.HasValue)
        {
          return value.Value;
        }

        return defaultValue;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return defaultValue;
      }
    }

    internal static string GetLongAsString(string value, string defaultValue = "")
    {
      try
      {
        long result = GetLong(value, -1);

        if (result == -1)
        {
          return defaultValue;
        }

        return result.ToString();
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return defaultValue;
      }
    }

    internal static string GetString(string value, string defaultValue = "")
    {
      try
      {
        if (value != null)
        {
          return value;
        }

        return defaultValue;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return defaultValue;
      }
    }

    internal static bool IsFile(FileInfoType type)
    {
      try
      {
        if (!IsFolder(type) && type != FileInfoType.None)
        {
          return true;
        }

        return false;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    internal static bool IsFolder(FileInfoType type)
    {
      try
      {
        if (IsRoot(type) || type == FileInfoType.Folder)
        {
          return true;
        }

        return false;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    internal static bool IsFolder(File file)
    {
      try
      {
        FileInfoType type = GetFileInfoType(file);

        return IsFolder(type);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    internal static bool IsGoogleDoc(File file)
    {
      try
      {
        FileInfoType type = GetFileInfoType(file);

        return IsGoogleDoc(type);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    internal static bool IsGoogleDoc(FileInfoType type)
    {
      try
      {
        if (IsFile(type) && type != FileInfoType.NonGoogleDoc)
        {
          return true;
        }

        return false;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    internal static bool IsRoot(FileInfoType type)
    {
      try
      {
        if (type == FileInfoType.Root)
        {
          return true;
        }

        return false;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    internal static bool IsRoot(File file)
    {
      try
      {
        if (file.Parents == null || file.Parents.Count == 0)
        {
          return true;
        }

        return false;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    internal static bool IsRoot(string fileId)
    {
      try
      {
        if (String.IsNullOrEmpty(fileId) ||
            fileId == "0" ||
            String.Equals(fileId, "root", StringComparison.CurrentCultureIgnoreCase) ||
            String.Equals(fileId, Settings.Product, StringComparison.CurrentCultureIgnoreCase))
        {
          return true;
        }

        return false;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    internal static bool IsTrashed(File file)
    {
      try
      {
        bool trashed = false;

        if (GetBoolean(file.ExplicitlyTrashed))
        {
          trashed = true;
        }
        else if (file.Labels != null && GetBoolean(file.Labels.Trashed))
        {
          trashed = true;
        }

        return trashed;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    internal static bool IsDateTimeEqual(DateTime value1, DateTime value2)
    {
      try
      {
        if (value1.Year == value2.Year &&
            value1.Month == value2.Month &&
            value1.Day == value2.Day &&
            value1.Hour == value2.Hour &&
            value1.Minute == value2.Minute &&
            value1.Second == value2.Second &&
            value1.Millisecond == value2.Millisecond)
        {
          return true;
        }

        return false;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    internal static bool IsDateTimeGreaterThan(DateTime value1, DateTime value2)
    {
      try
      {
        if (value1.Year > value2.Year)
        {
          return true;
        }

        if (value1.Year == value2.Year &&
            value1.Month > value2.Month)
        {
          return true;
        }

        if (value1.Year == value2.Year &&
            value1.Month == value2.Month &&
            value1.Day > value2.Day)
        {
          return true;
        }

        if (value1.Year == value2.Year &&
            value1.Month == value2.Month &&
            value1.Day == value2.Day &&
            value1.Hour > value2.Hour)
        {
          return true;
        }

        if (value1.Year == value2.Year &&
            value1.Month == value2.Month &&
            value1.Day == value2.Day &&
            value1.Hour == value2.Hour &&
            value1.Minute > value2.Minute)
        {
          return true;
        }

        if (value1.Year == value2.Year &&
            value1.Month == value2.Month &&
            value1.Day == value2.Day &&
            value1.Hour == value2.Hour &&
            value1.Minute == value2.Minute &&
            value1.Second > value2.Second)
        {
          return true;
        }

        if (value1.Year == value2.Year &&
            value1.Month == value2.Month &&
            value1.Day == value2.Day &&
            value1.Hour == value2.Hour &&
            value1.Minute == value2.Minute &&
            value1.Second == value2.Second &&
            value1.Millisecond > value2.Millisecond)
        {
          return true;
        }

        return false;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    internal static string GetGoogleDocText(FileInfo fileInfo)
    {
      try
      {
        if (!fileInfo.IsGoogleDoc)
        {
          return "";
        }

        File file = fileInfo._file;
        string url = file.AlternateLink;
        string type = fileInfo.Type.ToString().ToLower();
        string id = fileInfo.Id;

        string text =
          "{\"url\": \"" + url + "\", \"resource_id\": \"" + type + ":" + id + "\"}";

        return text;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    /// <summary> Opens google document. </summary>
    /// <exception cref="Exception">  Thrown when an exception error condition occurs. </exception>
    /// <param name="path"> . </param>
    public static void OpenGoogleDoc(string path)
    {
      try
      {
        if (!FileExists(path))
        {
          throw new Exception("Path '" + path + "' does not exist");
        }

        string text = System.IO.File.ReadAllText(path);

        if (String.IsNullOrEmpty(text))
        {
          throw new Exception("Path '" + path + "' does not contain url for Google Doc");
        }

        string downloadUrl = "";

        string search = "\"url\":";

        int startIndex = text.IndexOf(search);

        if (startIndex > -1)
        {
          startIndex += search.Length;

          startIndex = text.IndexOf("\"", startIndex);

          if (startIndex > -1)
          {
            startIndex += 1;

            int endIndex = text.IndexOf("\"", startIndex);

            if (endIndex > -1)
            {
              downloadUrl = text.Substring(startIndex, endIndex - startIndex);
            }
          }
        }

        if (String.IsNullOrEmpty(downloadUrl))
        {
          throw new Exception("Path '" + path + "' does not contain url in the expected format for Google Doc");
        }

        OpenGoogleDocFromDownloadUrl(downloadUrl);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    internal static void OpenGoogleDocFromDownloadUrl(string downloadUrl)
    {
      try
      {
        if (String.IsNullOrEmpty(downloadUrl))
        {
          throw new Exception("DownloadUrl is blank.");
        }

        string command = "";

        using (var registry = new Registry(RegistryKeyType.ClassesRoot, "ChromeHTML\\shell\\open\\command"))
        {
          if (registry.IsOpen)
          {
            command = Convert.ToString(registry.GetDefaultValue(""));

            if (!String.IsNullOrEmpty(command))
            {
              int startIndex = command.IndexOf("\"");

              if (startIndex > -1)
              {
                startIndex += 1;

                int endIndex = command.IndexOf("\"", startIndex);

                if (endIndex > -1)
                {
                  command = command.Substring(startIndex, endIndex - startIndex);
                }
              }
            }
          }
        }

        if (String.IsNullOrEmpty(command))
        {
          throw new Exception("Could not find Chrome open command (Is Chrome installed?)");
        }

        System.Diagnostics.Process process = System.Diagnostics.Process.Start(command, downloadUrl);

        process = null;
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    internal static void WriteGoogleDoc(FileInfo fileInfo)
    {
      try
      {
        if (!fileInfo.IsGoogleDoc)
        {
          return;
        }

        string filePath = fileInfo.FilePath;

        using (System.IO.FileStream fileStream = OpenFile(filePath, System.IO.FileMode.Create))
        {
          WriteGoogleDoc(fileInfo, fileStream);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    internal static void WriteGoogleDoc(FileInfo fileInfo, System.IO.FileStream fileStream)
    {
      try
      {
        if (!fileInfo.IsGoogleDoc)
        {
          return;
        }

        File file = fileInfo._file;

        string googleDocText = GetGoogleDocText(fileInfo);

        if (String.IsNullOrEmpty(googleDocText))
        {
          throw new Exception("GoogleDoc text is blank.");
        }

        using (var streamWriter = new System.IO.StreamWriter(fileStream))
        {
          streamWriter.Write(googleDocText);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    internal static void CreateDirectory(string directoryName)
    {
      try
      {
        if (String.IsNullOrEmpty(directoryName))
        {
          throw new Exception("DirectoryName is blank");
        }

        if (DirectoryExists(directoryName))
        {
          return;
        }

        Exception lastException = null;

        for (int i = 0; i < 10; i++)
        {
          try
          {
            System.IO.Directory.CreateDirectory(directoryName);

            lastException = null;

            break;
          }
          catch (Exception exception)
          {
            lastException = exception;
          }

          System.Threading.Thread.Sleep(100);
        }

        if (lastException != null)
        {
          throw lastException;
        }

        while (true)
        {
          if (DirectoryExists(directoryName))
          {
            break;
          }

          System.Threading.Thread.Sleep(100);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, true, false);
      }
    }

    internal static void DeleteDirectory(string directoryName, bool recursive = true)
    {
      try
      {
        if (String.IsNullOrEmpty(directoryName))
        {
          return;
        }

        if (!DirectoryExists(directoryName))
        {
          return;
        }

        Exception lastException = null;

        for (int i = 0; i < 10; i++)
        {
          try
          {
            System.IO.Directory.Delete(directoryName, recursive);

            lastException = null;

            break;
          }
          catch (Exception exception)
          {
            lastException = exception;
          }

          System.Threading.Thread.Sleep(100);
        }

        if (lastException != null)
        {
          throw lastException;
        }

        while (true)
        {
          if (!DirectoryExists(directoryName))
          {
            break;
          }

          System.Threading.Thread.Sleep(100);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, true, false);
      }
    }

    internal static void DeleteDirectoryContents(string directoryName, bool recursive = true)
    {
      try
      {
        if (!DirectoryExists(directoryName))
        {
          return;
        }

        string[] files = System.IO.Directory.GetFiles(directoryName);

        foreach (string file in files)
        {
          DeleteFile(file);
        }

        if (!recursive)
        {
          return;
        }

        string[] folders = System.IO.Directory.GetDirectories(directoryName);

        foreach (string folder in folders)
        {
          DeleteDirectory(folder);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, true, false);
      }
    }

    internal static bool DirectoryExists(string directoryName)
    {
      try
      {
        if (String.IsNullOrEmpty(directoryName))
        {
          return false;
        }

        return System.IO.Directory.Exists(directoryName);
      }
      catch (Exception exception)
      {
        Log.Error(exception, true, false);

        return false;
      }
    }

    internal static bool CanOpenFile(string filePath,
                                     System.IO.FileAccess fileAccess,
                                     System.IO.FileShare fileShare,
                                     int checkTimes,
                                     bool checkIfFileOpenedByAnotherProcess = false)
    {
      try
      {
        for (int i = 0; i < checkTimes; i++)
        {
          try
          {
            if (!FileExists(filePath))
            {
              return false;
            }

            using (
              System.IO.FileStream fileStream = OpenFile(filePath,
                                                         System.IO.FileMode.Open,
                                                         fileAccess,
                                                         fileShare,
                                                         checkTimes))
            {
            }

            if (checkIfFileOpenedByAnotherProcess)
            {
              if (IsFileOpenedByAnotherProcess(filePath))
              {
                throw new Exception("File is opened by another process.");
              }
            }

            return true;
          }
          catch (Exception exception)
          {
            if (exception != null)
            {
              //the file is unavailable because it is:
              //still being written to
              //or being processed by another thread
              //or does not exist (has already been processed)
            }
          }

          System.Threading.Thread.Sleep(100);
        }

        return false;
      }
      catch (System.Exception exception)
      {
        Log.Error(exception, true, false);

        return false;
      }
    }

    internal static bool IsFileOpenedByAnotherProcess(string filePath)
    {
      try
      {
        if (!FileExists(filePath))
        {
          return false;
        }

        try
        {
          using (System.IO.File.Open(filePath, System.IO.FileMode.Open))
          {
          }
          return false;
        }
        catch
        {
        }
        return true;
      }
      catch (System.Exception exception)
      {
        Log.Error(exception, true, false);

        return false;
      }
    }

    internal static void CreateFile(FileInfo fileInfo)
    {
      try
      {
        CreateFile(fileInfo.FilePath, fileInfo.ModifiedDate);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    internal static void CreateFile(string filePath)
    {
      try
      {
        CreateFile(filePath, DateTime.Now);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    internal static void CreateFile(string filePath, DateTime modifiedDate)
    {
      try
      {
        if (String.IsNullOrEmpty(filePath))
        {
          throw new Exception("FilePath is blank");
        }

        Exception lastException = null;

        for (int i = 0; i < 10; i++)
        {
          try
          {
            using (System.IO.FileStream fileStream = OpenFile(filePath, System.IO.FileMode.Create))
            {
            }

            lastException = null;

            break;
          }
          catch (Exception exception)
          {
            lastException = exception;
          }

          System.Threading.Thread.Sleep(100);
        }

        if (lastException != null)
        {
          throw lastException;
        }

        while (true)
        {
          if (FileExists(filePath))
          {
            break;
          }

          System.Threading.Thread.Sleep(100);
        }

        System.IO.File.SetCreationTime(filePath, modifiedDate);
        System.IO.File.SetLastAccessTime(filePath, modifiedDate);
        System.IO.File.SetLastWriteTime(filePath, modifiedDate);
      }
      catch (Exception exception)
      {
        Log.Error(exception, true, false);
      }
    }

    internal static void DeleteFile(string filePath)
    {
      try
      {
        if (String.IsNullOrEmpty(filePath))
        {
          return;
        }
        if (!FileExists(filePath))
        {
          return;
        }

        if (IsFileOpenedByAnotherProcess(filePath))
        {
          throw new Exception("The file is opened by another process");
        }

        Exception lastException = null;

        for (int i = 0; i < 10; i++)
        {
          try
          {
            System.IO.File.Delete(filePath);

            lastException = null;

            break;
          }
          catch (Exception exception)
          {
            lastException = exception;
          }

          System.Threading.Thread.Sleep(100);
        }

        if (lastException != null)
        {
          throw lastException;
        }

        while (true)
        {
          if (!FileExists(filePath))
          {
            break;
          }

          System.Threading.Thread.Sleep(100);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, true, false);
      }
    }

    internal static bool FileExists(string filePath)
    {
      try
      {
        if (String.IsNullOrEmpty(filePath))
        {
          return false;
        }

        return System.IO.File.Exists(filePath);
      }
      catch (System.Exception ex)
      {
        Log.Error(ex, true, false);

        return false;
      }
    }

    internal static void MoveFile(string filePath, string filePath2)
    {
      try
      {
        if (String.IsNullOrEmpty(filePath))
        {
          throw new Exception("FilePath is blank");
        }

        if (String.IsNullOrEmpty(filePath2))
        {
          throw new Exception("FilePath2 is blank");
        }

        if (!FileExists(filePath))
        {
          throw new Exception("File " + filePath + " does not exist");
        }

        DeleteFile(filePath2);

        string folderPath = System.IO.Path.GetDirectoryName(filePath2);

        CreateDirectory(folderPath);

        Exception lastException = null;

        for (int i = 0; i < 10; i++)
        {
          try
          {
            System.IO.File.Move(filePath, filePath2);

            lastException = null;

            break;
          }
          catch (Exception exception)
          {
            lastException = exception;
          }

          System.Threading.Thread.Sleep(100);
        }

        if (lastException != null)
        {
          throw lastException;
        }

        while (true)
        {
          if (FileExists(filePath2))
          {
            break;
          }

          System.Threading.Thread.Sleep(100);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, true, false);
      }
    }

    internal static System.IO.FileStream OpenFile(string filePath,
                                                  System.IO.FileMode fileMode = System.IO.FileMode.OpenOrCreate,
                                                  System.IO.FileAccess fileAccess = System.IO.FileAccess.ReadWrite,
                                                  System.IO.FileShare fileShare = System.IO.FileShare.None,
                                                  int checkTimes = 10)
    {
      try
      {
        if (String.IsNullOrEmpty(filePath))
        {
          throw new Exception("FilePath is blank");
        }

        string directoryName = System.IO.Path.GetDirectoryName(filePath);

        CreateDirectory(directoryName);

        System.IO.FileStream fileStream = null;

        Exception lastException = null;

        for (int i = 0; i < checkTimes; i++)
        {
          try
          {
            fileStream = new System.IO.FileStream(filePath, fileMode, fileAccess, fileShare);

            lastException = null;

            break;
          }
          catch (Exception exception)
          {
            lastException = exception;
          }

          System.Threading.Thread.Sleep(100);
        }

        if (lastException != null)
        {
          throw lastException;
        }

        return fileStream;
      }
      catch (Exception exception)
      {
        Log.Error(exception, true, false);

        return null;
      }
    }

    internal static void WriteAllText(string filePath, string text)
    {
      try
      {
        using (System.IO.FileStream fileStream = OpenFile(filePath, System.IO.FileMode.Create))
        {
          using (var streamWriter = new System.IO.StreamWriter(fileStream))
          {
            streamWriter.Write(text);
          }
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    internal static bool HasFileOnDiskTimedout(string filePath)
    {
      try
      {
        int totalMinutesOnDisk = 0;
        int fileOnDiskTimeout = 0;

        return HasFileOnDiskTimedout(filePath, ref totalMinutesOnDisk, ref fileOnDiskTimeout);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    internal static bool HasFileOnDiskTimedout(string filePath, ref int totalMinutesOnDisk, ref int fileOnDiskTimeout)
    {
      try
      {
        DateTime creationTime = System.IO.File.GetCreationTime(filePath);
        DateTime lastAccessTime = System.IO.File.GetLastAccessTime(filePath);
        DateTime lastWriteTime = System.IO.File.GetLastWriteTime(filePath);
        TimeSpan creationTimeSpan = DateTime.Now.Subtract(creationTime);
        TimeSpan lastAccessTimeSpan = DateTime.Now.Subtract(lastAccessTime);
        TimeSpan lastWriteTimeSpan = DateTime.Now.Subtract(lastWriteTime);

        fileOnDiskTimeout = Settings.FileOnDiskTimeout;

        if (creationTimeSpan.TotalMinutes < fileOnDiskTimeout)
        {
          Log.Warning("File creation time " + creationTime + " has not timed out on disk");
          return false;
        }
        else if (lastAccessTimeSpan.TotalMinutes < fileOnDiskTimeout)
        {
          Log.Warning("File last access time " + lastAccessTime + " has not timed out on disk");
          return false;
        }
        else if (lastWriteTimeSpan.TotalMinutes < fileOnDiskTimeout)
        {
          Log.Warning("File last write time " + lastWriteTime + " has not timed out on disk");
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

    internal static DateTime GetFileLastWriteTime(string filePath)
    {
      try
      {
        if (!FileExists(filePath))
        {
          return DateTime.MinValue;
        }

        DateTime lastWriteTime = System.IO.File.GetLastWriteTime(filePath);

        lastWriteTime = new DateTime(lastWriteTime.Year,
                                     lastWriteTime.Month,
                                     lastWriteTime.Day,
                                     lastWriteTime.Hour,
                                     lastWriteTime.Minute,
                                     lastWriteTime.Second,
                                     lastWriteTime.Millisecond,
                                     lastWriteTime.Kind);

        return lastWriteTime;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return DateTime.MinValue;
      }
    }

    internal static DateTime SetFileLastWriteTime(string filePath, DateTime lastWriteTime)
    {
      try
      {
        if (!FileExists(filePath))
        {
          return DateTime.MinValue;
        }

        try
        {
          System.IO.File.SetLastWriteTime(filePath, lastWriteTime);
        }
        catch (Exception exception)
        {
          Log.Error(exception, false, false);
        }

        return GetFileLastWriteTime(filePath);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return DateTime.MinValue;
      }
    }

    internal static long GetFileLength(string filePath)
    {
      try
      {
        if (!FileExists(filePath))
        {
          return 0;
        }

        var fileInfo = new System.IO.FileInfo(filePath);

        long length = fileInfo.Length;

        return length;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    private class Factory
    {
      /// <summary> Gets the instance. </summary>
      /// <returns> The instance. </returns>
      public static DriveService GetInstance()
      {
        try
        {
          return new DriveService();
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      /// <summary>
      ///   Releases the unmanaged resources used by the DriveProxy.API.DriveService.Factory and optionally
      ///   releases the managed resources.
      /// </summary>
      /// <param name="instance"> The instance. </param>
      public static void Dispose(DriveService instance)
      {
      }
    }

    private class FileComparer : IComparer<Google.Apis.Drive.v2.Data.File>
    {
      private bool _watchFileType = true;

      /// <summary> Constructor. </summary>
      /// <param name="watchFileType">  true to watch file type. </param>
      public FileComparer(bool watchFileType)
      {
        _watchFileType = watchFileType;
      }

      /// <summary>
      ///   Compares two Google.Apis.Drive.v2.Data.File objects to determine their relative ordering.
      /// </summary>
      /// <param name="file1">  File to be compared. </param>
      /// <param name="file2">  File to be compared. </param>
      /// <returns>
      ///   Negative if 'file1' is less than 'file2', 0 if they are equal, or positive if it is greater.
      /// </returns>
      public int Compare(Google.Apis.Drive.v2.Data.File file1, Google.Apis.Drive.v2.Data.File file2)
      {
        try
        {
          if (file1 == null && file2 == null)
          {
            return 0;
          }

          if (file1 == null)
          {
            return -1;
          }

          if (file2 == null)
          {
            return 1;
          }

          if (file1.Id == file2.Id)
          {
            return 0;
          }

          var fileInfoType1 = FileInfoType.Unknown;
          var fileInfoType2 = FileInfoType.Unknown;

          if (_watchFileType)
          {
            fileInfoType1 = DriveService.GetFileInfoType(file1);
            fileInfoType2 = DriveService.GetFileInfoType(file2);

            if (fileInfoType1 != fileInfoType2)
            {
              if (fileInfoType1 == FileInfoType.Root || fileInfoType1 == FileInfoType.Folder)
              {
                return -1;
              }

              if (fileInfoType2 == FileInfoType.Root || fileInfoType2 == FileInfoType.Folder)
              {
                return 1;
              }
            }
          }

          string text1 = file1.Title;
          string text2 = file2.Title;

          if (text1 == null && text2 == null)
          {
            return 0;
          }

          if (text1 == null)
          {
            return -1;
          }

          if (text2 == null)
          {
            return 1;
          }

          int result = String.Compare(text1, text2, true);

          if (result != 0)
          {
            return result;
          }

          DateTime? createdDate1 = GetDateTime(file1.CreatedDate);
          DateTime? createdDate2 = GetDateTime(file2.CreatedDate);

          if (createdDate1 == null && createdDate2 == null)
          {
            return 0;
          }

          if (createdDate1 == null)
          {
            return -1;
          }

          if (createdDate2 == null)
          {
            return 1;
          }

          if (createdDate1.HasValue == false && createdDate2.HasValue == false)
          {
            return 0;
          }

          if (createdDate1.HasValue == false)
          {
            return -1;
          }

          if (createdDate2.HasValue == false)
          {
            return 1;
          }

          DateTime dateTime1 = createdDate1.Value;
          DateTime dateTime2 = createdDate2.Value;

          result = DateTime.Compare(dateTime1, dateTime2);

          if (result != 0)
          {
            return result;
          }

          if (fileInfoType1 == FileInfoType.Unknown &&
              fileInfoType2 == FileInfoType.Unknown)
          {
            fileInfoType1 = DriveService.GetFileInfoType(file1);
            fileInfoType2 = DriveService.GetFileInfoType(file2);
          }

          if (fileInfoType1 != fileInfoType2)
          {
            if (fileInfoType1 == FileInfoType.Root || fileInfoType1 == FileInfoType.Folder)
            {
              return -1;
            }

            if (fileInfoType2 == FileInfoType.Root || fileInfoType2 == FileInfoType.Folder)
            {
              return 1;
            }
          }

          return result;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return 0;
        }
      }
    }

    private class FileInfoComparer : IComparer<FileInfo>
    {
      /// <summary> Default constructor. </summary>
      public FileInfoComparer()
      {
      }

      /// <summary> Compares two FileInfo objects to determine their relative ordering. </summary>
      /// <param name="file1">  File information to be compared. </param>
      /// <param name="file2">  File information to be compared. </param>
      /// <returns>
      ///   Negative if 'file1' is less than 'file2', 0 if they are equal, or positive if it is greater.
      /// </returns>
      public int Compare(FileInfo file1, FileInfo file2)
      {
        try
        {
          if (file1 == null && file2 == null)
          {
            return 0;
          }

          if (file1 == null)
          {
            return -1;
          }

          if (file2 == null)
          {
            return 1;
          }

          if (file1.Id == file2.Id)
          {
            return 0;
          }

          FileInfoType fileInfoType1 = file1.Type;
          FileInfoType fileInfoType2 = file2.Type;

          if (fileInfoType1 == FileInfoType.Root || fileInfoType1 == FileInfoType.Folder)
          {
            return -1;
          }

          if (fileInfoType2 == FileInfoType.Root || fileInfoType2 == FileInfoType.Folder)
          {
            return 1;
          }

          string text1 = file1.Title;
          string text2 = file2.Title;

          if (text1 == null && text2 == null)
          {
            return 0;
          }

          if (text1 == null)
          {
            return -1;
          }

          if (text2 == null)
          {
            return 1;
          }

          int result = String.Compare(text1, text2, true);

          if (result != 0)
          {
            return result;
          }

          DateTime? createdDate1 = GetDateTime(file1.CreatedDate);
          DateTime? createdDate2 = GetDateTime(file2.CreatedDate);

          if (createdDate1 == null && createdDate2 == null)
          {
            return 0;
          }

          if (createdDate1 == null)
          {
            return -1;
          }

          if (createdDate2 == null)
          {
            return 1;
          }

          if (createdDate1.HasValue == false && createdDate2.HasValue == false)
          {
            return 0;
          }

          if (createdDate1.HasValue == false)
          {
            return -1;
          }

          if (createdDate2.HasValue == false)
          {
            return 1;
          }

          DateTime dateTime1 = createdDate1.Value;
          DateTime dateTime2 = createdDate2.Value;

          result = DateTime.Compare(dateTime1, dateTime2);

          if (result != 0)
          {
            return result;
          }

          return result;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return 0;
        }
      }
    }
  }
}
