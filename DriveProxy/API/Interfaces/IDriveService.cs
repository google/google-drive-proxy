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
using DriveProxy.Utils;

namespace DriveProxy.API
{
  /// <summary>
  ///   Google Drive Service interface for accessing a Windows user's Google Drive account
  /// </summary>
  public interface IDriveService : IDisposable
  {
    /// <summary>
    ///   The login status of the current user.
    /// </summary>
    bool IsSignedIn { get; }

    /// <summary>
    ///   The path to the local file containing the Google Drive documents.
    /// </summary>
    string FolderPath { get; }

    /// <summary>
    ///   A variable used to determine whether the DriveService is currently processing files.
    /// </summary>
    bool Processing { get; }

    /// <summary>
    ///   The status of the background processes.
    /// </summary>
    bool StartedBackgroundProcesses { get; }

    /// <summary>
    ///   Determines whether StartBackgroundProcesses() is currently being called.
    /// </summary>
    bool StartingBackgroundProcesses { get; }

    /// <summary>
    ///   Determines whether StopBackgroundProcesses() is currently being called.
    /// </summary>
    bool StoppingBackgroundProcesses { get; }

    /// <summary>
    ///   An event that fires when the state service begins processing a series of queued streams.
    /// </summary>
    event IEventHandler OnQueueStartedProcessing;

    /// <summary>
    ///   An event that fires when the state service begins processing a stream.
    /// </summary>
    event IStreamEventHandler OnQueueStartedStream;

    /// <summary>
    ///   An event that fires when the state service finishes processing a stream.
    /// </summary>
    event IStreamEventHandler OnQueueFinishedStream;

    /// <summary>
    ///   An event that fires when the state service finishes processing a series of queued streams.
    /// </summary>
    event IEventHandler OnQueueFinishedProcessing;

    /// <summary>
    ///   Authenticates the current user against Google Drive. Opens the Google Drive user dialog on failure.
    /// </summary>
    /// <param name="startBackgroundProcesses">Starts up the background processes.</param>
    /// <param name="openInExplorer">Opens and instance of Windows Explorer to the local Google Drive location.</param>
    void Authenticate(bool startBackgroundProcesses, bool openInExplorer = false);

    /// <summary>
    ///   Logs the current user out of Google Drive.
    /// </summary>
    void Signout();

    /// <summary>
    ///   Logs the current user out of Google Drive and opens the Google Drive user dialog.
    /// </summary>
    /// <param name="startBackgroundProcesses">Starts up the background processes.</param>
    /// <param name="openInExplorer">Opens and instance of Windows Explorer to the local Google Drive location.</param>
    void SignoutAndAuthenticate(bool startBackgroundProcesses, bool openInExplorer = false);

    /// <summary>
    ///   Starts the background processes that monitor and update the Google Drive files.
    /// </summary>
    void StartBackgroundProcesses();

    /// <summary>
    ///   Stops the background processes that monitor and update the Google Drive files.
    /// </summary>
    /// <param name="waitWhileProcessing">
    ///   Allow any files currently processing complete before stopping the background
    ///   processes.
    /// </param>
    void StopBackgroundProcesses(bool waitWhileProcessing);

    /// <summary>
    ///   Gets user and quota data about the current user.
    /// </summary>
    /// <returns>IAbout interface (see IAbout for more information).</returns>
    IAbout GetAbout();

    /// <summary>
    ///   Gets the DriveService log
    /// </summary>
    /// <returns>ILog interface (see ILog for more information)</returns>
    ILog GetLog();

    /// <summary>
    ///   Sets the DriveService log
    /// </summary>
    /// <param name="log">The the new log object.</param>
    void SetLog(ILog log);

    /// <summary>
    ///   Dispose of the DriveService
    /// </summary>
    /// <param name="stopBackgroundProcesses">Call StopBackgroundProcess() before Disposal.</param>
    /// <param name="waitWhileProcessing">
    ///   Allow any files currently processing complete before stopping the background
    ///   processes.
    /// </param>
    void Dispose(bool stopBackgroundProcesses, bool waitWhileProcessing);

    /// <summary>
    ///   Get a Google Drive file.
    /// </summary>
    /// <param name="fileId">The requested file's unique file id.</param>
    /// <param name="getChildren">Searches for children of the requested file (Used for folders).</param>
    /// <returns>IFile interface (see IFile for more information)</returns>
    IFile GetFile(string fileId = null, bool getChildren = true);

    /// <summary>
    ///   Gets multiple Google Drive files.
    /// </summary>
    /// <param name="fileIds">The unique file ids for each of the requested files.</param>
    /// <param name="getChildren">Searches for children of the requested files (Used for folders).</param>
    /// <returns>An IEnumerable of IFile interfaces (see IFile for more information)</returns>
    IEnumerable<IFile> GetFiles(IEnumerable<string> fileIds, bool getChildren = true);

    /// <summary>
    ///   Gets all files in a requested path.
    /// </summary>
    /// <param name="path">The path to look in for Google Drive files.</param>
    /// <returns>An IEnumberable of IFile interfaces (see IFile for more information)</returns>
    IEnumerable<IFile> GetFilesFromPath(string path);

    /// <summary>
    ///   Checks whether the requested file is located locally, or in the cloud.
    /// </summary>
    /// <param name="fileId">The requested file's unique file id.</param>
    /// <returns>FileInfoStatus interface (see FileInfoStatus for more information)</returns>
    FileInfoStatus GetFileStatus(string fileId);

    /// <summary>
    ///   Checks whether the requested file is located locally, or in the cloud.
    /// </summary>
    /// <param name="file">The requested IFile object</param>
    /// <returns>FileInfoStatus interface (see FileInfoStatus for more information)</returns>
    FileInfoStatus GetFileStatus(IFile file);

    /// <summary>
    ///   Gets a file from the cached files
    /// </summary>
    /// <param name="fileId">The requested file's unique file id.</param>
    /// <param name="getChildren">Searches for children of the requested file (Used for folders).</param>
    /// <returns>IFile interface (see IFile for more information)</returns>
    IFile GetCachedFile(string fileId = null, bool getChildren = true);

    /// <summary>
    ///   Get multiple files from the cached files.
    /// </summary>
    /// <param name="fileIds">The unique file ids for each of the requested files.</param>
    /// <param name="getChildren">Searches for children of the requested files (Used for folders).</param>
    /// <returns>An IEnumerable of IFile interfaces (see IFile for more information)</returns>
    IEnumerable<IFile> GetCachedFiles(IEnumerable<string> fileIds, bool getChildren = true);

    /// <summary>
    ///   Update the cached files list.
    /// </summary>
    void UpdateCachedData();

    /// <summary>
    ///   Copy a Google Drive file.
    /// </summary>
    /// <param name="stream">The CopyStream for the requested file.</param>
    /// <param name="parentId">The requested file's parent folder's unique file id.</param>
    /// <param name="fileId">The requested file's unique file id.</param>
    void CopyFile(IStream stream, string parentId, string fileId);

    /// <summary>
    ///   Copy a Google Drive file.
    /// </summary>
    /// <param name="stream">The CopyStream for the requested file.</param>
    /// <param name="parentId">The requested file's parent folder's unique file id.</param>
    /// <param name="file">The requested IFile object</param>
    void CopyFile(IStream stream, string parentId, IFile file);

    /// <summary>
    ///   Download a Google Drive file.
    /// </summary>
    /// <param name="stream">The DownloadStream for the requested file.</param>
    /// <param name="fileId">The requested file's unique file id.</param>
    /// <param name="chunkSize">The chunk size for downloading the file. 0 defaults to Maximum chunk size.</param>
    /// <param name="checkIfAlreadyDownloaded">Skips download if the file already exists on the local machine.</param>
    void DownloadFile(IStream stream, string fileId, int chunkSize = 0, bool checkIfAlreadyDownloaded = true);

    /// <summary>
    ///   Download a Google Drive file.
    /// </summary>
    /// <param name="stream">The DownloadStream for the requested file.</param>
    /// <param name="file">The requested IFile object</param>
    /// <param name="chunkSize">The chunk size for downloading the file. 0 defaults to Maximum chunk size.</param>
    /// <param name="checkIfAlreadyDownloaded">Skips download if the file already exists on the local machine.</param>
    void DownloadFile(IStream stream, IFile file, int chunkSize = 0, bool checkIfAlreadyDownloaded = true);

    /// <summary>
    ///   Insert a new file into the Google Drive location.
    /// </summary>
    /// <param name="stream">The InsertStream for the new file.</param>
    /// <param name="parentId">The new file's parent folder's unique file id.</param>
    /// <param name="title">The title (name) for the new file.</param>
    /// <param name="isFolder">Creates the new file as a folder.</param>
    void InsertFile(IStream stream, string parentId, string title, bool isFolder);

    /// <summary>
    ///   Insert a new file into the Google Drive location.
    /// </summary>
    /// <param name="stream">The InsertStream for the new file.</param>
    /// <param name="parent">The new file's parent folder IFile object.</param>
    /// <param name="title">The title (name) for the new file.</param>
    /// <param name="isFolder">Creates the new file as a folder.</param>
    void InsertFile(IStream stream, IFile parent, string title, bool isFolder);

    /// <summary>
    ///   Move a Google Drive file.
    /// </summary>
    /// <param name="stream">The MoveStream for the requested file.</param>
    /// <param name="parentId">The requested file's parent folder's unique file id.</param>
    /// <param name="fileId">The requested file's unique file id.</param>
    void MoveFile(IStream stream, string parentId, string fileId);

    /// <summary>
    ///   Move a Google Drive file.
    /// </summary>
    /// <param name="stream">The MoveStream for the requested file.</param>
    /// <param name="parentId">The requested file's parent folder's unique file id.</param>
    /// <param name="file">The requested IFile object</param>
    void MoveFile(IStream stream, string parentId, IFile file);

    /// <summary>
    ///   Rename a Google Drive file.
    /// </summary>
    /// <param name="stream">The RenameStream for the requested file.</param>
    /// <param name="fileId">The requested file's unique file id.</param>
    /// <param name="title">The new title (name) for the requested file.</param>
    void RenameFile(IStream stream, string fileId, string title);

    /// <summary>
    ///   Rename a Google Drive file.
    /// </summary>
    /// <param name="stream">The RenameStream for the requested file.</param>
    /// <param name="file">The requested IFile object.</param>
    /// <param name="title">The new title (name) for the requested file.</param>
    void RenameFile(IStream stream, IFile file, string title);

    /// <summary>
    ///   Upload a file from the local machine to the Google Drive location.
    /// </summary>
    /// <param name="stream">The UploadStream for the requested file.</param>
    /// <param name="fileId">The requested file's unique file id.</param>
    /// <param name="chunkSize">The chunk size for uploading the file. 0 defaults to Maximum chunk size.</param>
    /// <param name="checkIfAlreadyUploaded">
    ///   Skips upload if the file in the Google Drive location is identical to the
    ///   requested file.
    /// </param>
    void UploadFile(IStream stream, string fileId, int chunkSize = 0, bool checkIfAlreadyUploaded = true);

    /// <summary>
    ///   Upload a file from the local machine to the Google Drive location.
    /// </summary>
    /// <param name="stream">The UploadStream for the requested file.</param>
    /// <param name="file">The requested IFile object</param>
    /// <param name="chunkSize">The chunk size for uploading the file. 0 defaults to Maximum chunk size.</param>
    /// <param name="checkIfAlreadyUploaded">
    ///   Skips upload if the file in the Google Drive location is identical to the
    ///   requested file.
    /// </param>
    void UploadFile(IStream stream, IFile file, int chunkSize = 0, bool checkIfAlreadyUploaded = true);

    /// <summary>
    ///   Send a Google Drive file to the Google Drive trash.
    /// </summary>
    /// <param name="stream">The TrashStream for the requested file.</param>
    /// <param name="fileId">The requested file's unique file id.</param>
    void TrashFile(IStream stream, string fileId);

    /// <summary>
    ///   Send a Google Drive file to the Google Drive trash.
    /// </summary>
    /// <param name="stream">The TrashStream for the requested file.</param>
    /// <param name="file">The requested IFile object</param>
    void TrashFile(IStream stream, IFile file);

    /// <summary>
    ///   Restore a Google Drive file from the Google Drive trash.
    /// </summary>
    /// <param name="stream">The UntrashStream for the requested file.</param>
    /// <param name="fileId">The requested file's unique file id.</param>
    void UntrashFile(IStream stream, string fileId);

    /// <summary>
    ///   Restore a Google Drive file from the Google Drive trash.
    /// </summary>
    /// <param name="stream">The UntrashStream for the requested file.</param>
    /// <param name="file">The requested IFile object</param>
    void UntrashFile(IStream stream, IFile file);

    /// <summary>
    ///   Remove a Google Drive file from the local location. (Does not delete the file from Google Drive)
    /// </summary>
    /// <param name="fileId">The requested file's unique file id.</param>
    /// <param name="ignoreFileOnDiskTimeout">Stops trying to remove the local file if the disk times out.</param>
    void CleanupFile(string fileId, bool ignoreFileOnDiskTimeout = true);

    /// <summary>
    ///   Remove a Google Drive file from the local location. (Does not delete the file from Google Drive)
    /// </summary>
    /// <param name="file">The requested IFile object</param>
    /// <param name="ignoreFileOnDiskTimeout">Stops trying to remove the local file if the disk times out.</param>
    void CleanupFile(IFile file, bool ignoreFileOnDiskTimeout = true);

    /// <summary>
    ///   Open a Google Drive document.
    /// </summary>
    /// <param name="fileId">The requested file's unique file id.</param>
    void OpenGoogleDoc(string fileId);

    /// <summary>
    ///   Open a Google Drive document.
    /// </summary>
    /// <param name="file">The requested IFile object.</param>
    void OpenGoogleDoc(IFile file);
  }

  /// <summary>
  ///   Contains general information about the logged in Google Drive account
  /// </summary>
  public interface IAbout
  {
    /// <summary>
    ///   The name of the currently logged in user.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///   The display name of the currently logged in user.
    /// </summary>
    string UserDisplayName { get; }

    /// <summary>
    ///   The total amount of space, in bytes, the logged in user has available.
    /// </summary>
    long QuotaBytesTotal { get; }

    /// <summary>
    ///   The total amount of space, in bytes, the logged in user has used.
    /// </summary>
    long QuotaBytesUsed { get; }

    /// <summary>
    ///   Aggregate of the total space, in bytes, used by the logged in user.
    /// </summary>
    long QuotaBytesUsedAggregate { get; }

    /// <summary>
    ///   The total amount of space, in bytes, the logged in user's Trashed documents use.
    /// </summary>
    long QuotaBytesUsedInTrash { get; }
  }

  /// <summary>
  ///   The Log file for DriveServices.
  /// </summary>
  public interface ILog
  {
    /// <summary>
    ///   The level of detail that the log will record.
    /// </summary>
    LogType Level { get; set; }

    /// <summary>
    ///   The Source field in the Event Viewer log.
    /// </summary>
    string Source { get; set; }

    /// <summary>
    ///   The path to the log file.
    /// </summary>
    string FilePath { get; set; }

    /// <summary>
    ///   The last error recorded by the log.
    /// </summary>
    string LastError { get; }

    /// <summary>
    ///   The time that the last error was recorded by the log.
    /// </summary>
    DateTime LastErrorTime { get; }
  }

  /// <summary>
  ///   Google Drive File interface providing file properties of objects in a Google Drive account
  /// </summary>
  public interface IFile
  {
    /// <summary>
    ///   File path to application associated with file extension
    /// </summary>
    string AssociationApplication { get; }

    /// <summary>
    ///   File path to icon associated with file extension
    /// </summary>
    string AssociationDefaultIcon { get; }

    /// <summary>
    ///   Index of icon associated with file extension
    /// </summary>
    int AssociationDefaultIconIndex { get; }

    /// <summary>
    ///   Friendly name of file type associated with file extension
    /// </summary>
    string AssociationFileType { get; }

    /// <summary>
    ///   Indicates if file is copyable
    /// </summary>
    bool Copyable { get; }

    /// <summary>
    ///   File created date
    /// </summary>
    DateTime CreatedDate { get; }

    /// <summary>
    ///   File description
    /// </summary>
    string Description { get; }

    /// <summary>
    ///   Indicates if file is editable
    /// </summary>
    bool Editable { get; }

    /// <summary>
    ///   File extension (if available)
    /// </summary>
    string FileExtension { get; }

    /// <summary>
    ///   File name without extension
    /// </summary>
    string FileName { get; }

    /// <summary>
    ///   File path on disk (if available)
    ///   (ex. \User\AppData\Local\Google\Google Drive\Id\Title)
    /// </summary>
    string FilePath { get; }

    /// <summary>
    ///   File size
    /// </summary>
    long FileSize { get; }

    /// <summary>
    ///   File id of object in cloud
    /// </summary>
    string Id { get; }

    /// <summary>
    ///   Name of last modifying user
    /// </summary>
    string LastModifyingUserName { get; }

    /// <summary>
    ///   Date file was last viewed by the logged in user
    /// </summary>
    DateTime LastViewedByMeDate { get; }

    /// <summary>
    ///   Mime type of file object
    /// </summary>
    string MimeType { get; }

    /// <summary>
    ///   Date file was modified by the logged in user
    /// </summary>
    DateTime ModifiedByMeDate { get; }

    /// <summary>
    ///   Date file was last modified
    /// </summary>
    DateTime ModifiedDate { get; }

    /// <summary>
    ///   Original file name before being altered (if found another file with the same title)
    /// </summary>
    string OriginalFilename { get; }

    /// <summary>
    ///   Name of user who owns the file object
    /// </summary>
    string OwnerName { get; }

    /// <summary>
    ///   Id of the parent folder in the cloud
    /// </summary>
    string ParentId { get; }

    /// <summary>
    ///   Indicates if the file is shared
    /// </summary>
    bool Shared { get; }

    /// <summary>
    ///   Date the logged in user shared the file object
    /// </summary>
    DateTime SharedWithMeDate { get; }

    /// <summary>
    ///   Title of file (includes both file name and file extension)
    /// </summary>
    string Title { get; }

    /// <summary>
    ///   Children of the file object
    /// </summary>
    IEnumerable<IFile> Children { get; }
  }

  /// <summary>
  ///   A delegate handler for Events.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  public delegate void IEventHandler(object sender, IEventArgs e);

  /// <summary>
  ///   Arguments supplied for Events.
  /// </summary>
  public interface IEventArgs
  {
  }

  /// <summary>
  ///   A delegate handler for Stream Events.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  public delegate void IStreamEventHandler(object sender, IStreamEventArgs e);

  /// <summary>
  ///   Arguments supplied for Stream Events.
  /// </summary>
  public interface IStreamEventArgs
  {
    IStream Stream { get; }
  }

  /// <summary>
  ///   A stream used to manipulate Google Drive files.
  /// </summary>
  public interface IStream
  {
    /// <summary>
    ///   The Google Drive file that will be handled by the stream.
    /// </summary>
    IFile File { get; }

    /// <summary>
    ///   The stream's type. Determines how the file is manipulated.
    /// </summary>
    StreamType Type { get; }

    /// <summary>
    ///   The unique id of the file that is being handled by the stream.
    /// </summary>
    string FileId { get; }

    /// <summary>
    ///   The path to the file that is being handled by the stream.
    /// </summary>
    string FilePath { get; }

    /// <summary>
    ///   The name of the file that is being handled by the stream.
    /// </summary>
    string FileName { get; }

    /// <summary>
    ///   The title of the file that is being handled by the stream. Includes the file extension.
    /// </summary>
    string Title { get; }

    /// <summary>
    ///   The MIME type of the file that is being handled by the stream.
    /// </summary>
    string MimeType { get; }

    /// <summary>
    ///   Determines whether the file being handled by the stream is a folder.
    /// </summary>
    bool IsFolder { get; }

    /// <summary>
    ///   Determines whether progress on the stream has been started.
    /// </summary>
    bool NotStarted { get; }

    /// <summary>
    ///   Determines if the stream is in queue.
    /// </summary>
    bool Queued { get; }

    /// <summary>
    ///   Determines whether the stream is currently starting.
    /// </summary>
    bool Starting { get; }

    /// <summary>
    ///   Determines whether the stream is currently being processed.
    /// </summary>
    bool Processing { get; }

    /// <summary>
    ///   Determines whether the stream is currently canceling.
    /// </summary>
    bool Cancelling { get; }

    /// <summary>
    ///   Determines whether the stream has been canceled.
    /// </summary>
    bool Cancelled { get; }

    /// <summary>
    ///   Determines whether the stream has completed its process.
    /// </summary>
    bool Completed { get; }

    /// <summary>
    ///   Determines whether the stream failed to complete its process.
    /// </summary>
    bool Failed { get; }

    /// <summary>
    ///   Determines whether the stream has been started.
    /// </summary>
    bool Started { get; }

    /// <summary>
    ///   Determines whether the stream has been processed.
    /// </summary>
    bool Processed { get; }

    /// <summary>
    ///   Determines if all of the stream's processes have been completed.
    /// </summary>
    bool Finished { get; }

    /// <summary>
    ///   The total number of bytes that the stream has processed.
    /// </summary>
    long BytesProcessed { get; }

    /// <summary>
    ///   The total number of bytes the file to be processed by the stream has.
    /// </summary>
    long TotalBytes { get; }

    /// <summary>
    ///   The exception messaged encountered by the stream.
    /// </summary>
    string ExceptionMessage { get; }

    /// <summary>
    ///   The current percentage of completion of the process the stream is currently processing.
    /// </summary>
    int PercentCompleted { get; }

    /// <summary>
    ///   The estimated amount of time left to complete the current stream process.
    /// </summary>
    TimeSpan RemainingTime { get; }

    /// <summary>
    ///   Attributes added to the stream to keep track of variables pertinent to this stream.
    /// </summary>
    IAttributes Attributes { get; }

    /// <summary>
    ///   An event that fires when stream progress is started.
    /// </summary>
    event IStreamEventHandler OnProgressStarted;

    /// <summary>
    ///   An event that fires when stream progress changes.
    /// </summary>
    event IStreamEventHandler OnProgressChanged;

    /// <summary>
    ///   An event that fires when stream progress completes.
    /// </summary>
    event IStreamEventHandler OnProgressFinished;

    /// <summary>
    ///   Cancels the current stream process.
    /// </summary>
    void Cancel();
  }

  /// <summary>
  ///   A key value pair that can be used to store unrequired data.
  /// </summary>
  public interface IAttribute
  {
    /// <summary>
    ///   The name used to reference the attribute.
    /// </summary>
    string Key { get; set; }

    /// <summary>
    ///   The value of the attribute.
    /// </summary>
    string Value { get; set; }
  }

  /// <summary>
  ///   A collection of IAttributes
  /// </summary>
  public interface IAttributes : IEnumerable<IAttribute>
  {
    /// <summary>
    ///   The attribute found based on the specified index.
    /// </summary>
    /// <param name="index">The index of the requested attribute.</param>
    /// <returns>IAttribute interface (see IAttribute for more information)</returns>
    IAttribute this[int index] { get; }

    /// <summary>
    ///   The attribute found based on the specified key.
    /// </summary>
    /// <param name="key">The key name of the requested attribute.</param>
    /// <returns>IAttribute interface (see IAttribute for more information)</returns>
    string this[string key] { get; }

    /// <summary>
    ///   Removes all attributes from Attributes.
    /// </summary>
    void Clear();

    /// <summary>
    ///   Adds a new attribute.
    /// </summary>
    /// <param name="key">The attribute's key name.</param>
    /// <param name="value">The attribute's value.</param>
    void Add(string key, string value);

    /// <summary>
    ///   Get an attribute from the Attributes.
    /// </summary>
    /// <param name="index">The index of the requested attribute.</param>
    /// <returns>IAttribute interface (see IAttribute for more information)</returns>
    IAttribute GetItem(int index);

    /// <summary>
    ///   Get the value of an attribute from the Attributes.
    /// </summary>
    /// <param name="index">The index of the requested attribute.</param>
    /// <returns>The value of the requested attribute.</returns>
    string GetValue(int index);

    /// <summary>
    ///   Get the value of an attribute from the Attributes.
    /// </summary>
    /// <param name="key">The key of the requested attribute.</param>
    /// <returns>The value of the requested attribute.</returns>
    string GetValue(string key);

    /// <summary>
    ///   Get the index of an attribute.
    /// </summary>
    /// <param name="key">The key of the requested attribute.</param>
    /// <returns>The index of the requested attribute.</returns>
    int IndexOf(string key);

    /// <summary>
    ///   Remove an attribute from the Attributes.
    /// </summary>
    /// <param name="index">The index of the requested attribute.</param>
    void Remove(int index);

    /// <summary>
    ///   Remove an attribute from the Attributes.
    /// </summary>
    /// <param name="key">The key of the requested attribute.</param>
    void Remove(string key);

    /// <summary>
    ///   Remove an attribute from the Attributes.
    /// </summary>
    /// <param name="attribute">The requested attribute.</param>
    void Remove(IAttribute attribute);

    /// <summary>
    ///   Change the value of an attribute from the Attributes.
    /// </summary>
    /// <param name="key">The key of the requested attribute.</param>
    /// <param name="value">The new value of the requested attribute.</param>
    void Update(string key, string value);
  }

  /// <summary>
  ///   Factory class used to create the IDriveService object
  /// </summary>
  public class DriveServiceFactory
  {
    protected static DriveService _DriveService = null;

    protected DriveServiceFactory()
    {
    }

    /// <summary>
    ///   Creates the IDriveService object
    /// </summary>
    /// <returns>Instance of object implementing the IDriveService interface</returns>
    public static IDriveService CreateService()
    {
      try
      {
        Debugger.Launch();

        if (!DriveProxy.API.DriveService.IsInitialized)
        {
          DriveProxy.API.DriveService.Init(false, API.DriveService.Settings.Product, false);

          DriveProxy.API.DriveService.Settings.FileReturnType = FileReturnType.ReturnAllGoogleFiles;

          DriveProxy.API.DriveService.State.OnStartedProcessing +=
            State_StartedProcessing;
          DriveProxy.API.DriveService.State.OnQueuedStream +=
            State_QueuedStream;
          DriveProxy.API.DriveService.State.OnFinishedStream +=
            State_FinishedStream;
          DriveProxy.API.DriveService.State.OnFinishedProcessing +=
            State_FinishedProcessing;
        }

        return _DriveService ?? (_DriveService = new DriveService());
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    private static void State_StartedProcessing(DriveProxy.API.DriveService.State sender)
    {
      try
      {
        if (_DriveService != null)
        {
          _DriveService.InvokeQueueStartedProcessing(sender, new EventArgs());
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    private static void State_QueuedStream(DriveProxy.API.DriveService.State sender,
                                           DriveProxy.API.DriveService.Stream stream)
    {
      try
      {
        if (_DriveService != null)
        {
          _DriveService.InvokeQueueStartedStream(sender, new StreamEventArgs(new Stream(stream)));
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    private static void State_FinishedStream(DriveProxy.API.DriveService.State sender,
                                             DriveProxy.API.DriveService.Stream stream)
    {
      try
      {
        if (_DriveService != null)
        {
          _DriveService.InvokeQueueFinishedStream(sender, new StreamEventArgs(new Stream(stream)));
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    private static void State_FinishedProcessing(DriveProxy.API.DriveService.State sender)
    {
      try
      {
        if (_DriveService != null)
        {
          _DriveService.InvokeQueueFinishedProcessing(sender, new EventArgs());
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    /// <summary>
    ///   Creates a new IStream object.
    /// </summary>
    /// <returns>IStream object (see IStream for more information)</returns>
    public static IStream CreateStream()
    {
      try
      {
        return new Stream();
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    /// <summary>
    ///   Creates a new ILog object.
    /// </summary>
    /// <returns>ILog object (see ILog for more information)</returns>
    public static ILog CreateLog()
    {
      try
      {
        using (IDriveService driveService = CreateService())
        {
          ILog log = driveService.GetLog();

          return log;
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    protected class About : IAbout
    {
      internal About()
      {
        _AboutInfo = null;
      }

      internal About(AboutInfo aboutInfo)
      {
        _AboutInfo = aboutInfo;
      }

      internal AboutInfo _AboutInfo { get; set; }

      public string Name
      {
        get
        {
          if (_AboutInfo == null)
          {
            return "";
          }

          return _AboutInfo.Name;
        }
      }

      public string UserDisplayName
      {
        get
        {
          if (_AboutInfo == null)
          {
            return "";
          }

          return _AboutInfo.UserDisplayName;
        }
      }

      public long QuotaBytesTotal
      {
        get
        {
          if (_AboutInfo == null)
          {
            return 0;
          }

          return _AboutInfo.QuotaBytesTotal;
        }
      }

      public long QuotaBytesUsed
      {
        get
        {
          if (_AboutInfo == null)
          {
            return 0;
          }

          return _AboutInfo.QuotaBytesUsed;
        }
      }

      public long QuotaBytesUsedAggregate
      {
        get
        {
          if (_AboutInfo == null)
          {
            return 0;
          }

          return _AboutInfo.QuotaBytesUsedAggregate;
        }
      }

      public long QuotaBytesUsedInTrash
      {
        get
        {
          if (_AboutInfo == null)
          {
            return 0;
          }

          return _AboutInfo.QuotaBytesUsedInTrash;
        }
      }
    }

    protected class Attribute : IAttribute
    {
      protected string _Key = null;

      protected string _Value = null;

      public Attribute(string key, string value)
      {
        if (String.IsNullOrEmpty(key))
        {
          throw new Exception("Key is null or blank.");
        }

        _Key = key;
        _Value = value;
      }

      public string Key
      {
        get { return _Key ?? (_Key = ""); }
        set { _Key = value; }
      }

      public string Value
      {
        get { return _Value; }
        set { _Value = value; }
      }
    }

    protected class Attributes : List<IAttribute>, IAttributes
    {
      public new IAttribute this[int index]
      {
        get { return GetItem(index); }
      }

      public string this[string key]
      {
        get { return GetValue(key); }
      }

      public new void Clear()
      {
        base.Clear();
      }

      public void Add(string key, string value)
      {
        Update(key, value);
      }

      public IAttribute GetItem(int index)
      {
        if (index < 0 || index >= Count)
        {
          throw new Exception("Index is outside of range.");
        }

        IAttribute attribute = base[index];

        return attribute;
      }

      public string GetValue(int index)
      {
        IAttribute attribute = GetItem(index);

        return attribute.Value;
      }

      public string GetValue(string key)
      {
        if (String.IsNullOrEmpty(key))
        {
          throw new Exception("Key is null or blank.");
        }

        int index = IndexOf(key);

        if (index > -1)
        {
          return GetValue(index);
        }

        return null;
      }

      public int IndexOf(string key)
      {
        if (String.IsNullOrEmpty(key))
        {
          throw new Exception("Key is null or blank.");
        }

        int index = 0;

        foreach (IAttribute attribute in this)
        {
          if (String.Equals(key, attribute.Key, StringComparison.CurrentCultureIgnoreCase))
          {
            return index;
          }

          index++;
        }

        return -1;
      }

      public void Remove(int index)
      {
        RemoveAt(index);
      }

      public void Remove(string key)
      {
        int index = IndexOf(key);

        if (index > -1)
        {
          Remove(index);
        }
      }

      public new void Remove(IAttribute attribute)
      {
        base.Remove(attribute);
      }

      public void Update(string key, string value)
      {
        if (String.IsNullOrEmpty(key))
        {
          throw new Exception("Key is null or blank.");
        }

        IAttribute attribute = null;

        int index = IndexOf(key);

        if (index > -1)
        {
          attribute = base[index];

          attribute.Value = value;
        }
        else
        {
          attribute = new Attribute(key, value);

          Add(attribute);
        }
      }
    }

    protected class DriveService : IDriveService
    {
      private readonly DriveProxy.API.DriveService _service;

      public DriveService()
      {
        try
        {
          _service = DriveProxy.API.DriveService.Create();
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public event IEventHandler OnQueueStartedProcessing = null;
      public event IStreamEventHandler OnQueueStartedStream = null;
      public event IStreamEventHandler OnQueueFinishedStream = null;
      public event IEventHandler OnQueueFinishedProcessing = null;

      public bool IsSignedIn
      {
        get
        {
          try
          {
            return DriveProxy.API.DriveService.IsSignedIn;
          }
          catch (Exception exception)
          {
            Log.Error(exception);

            return false;
          }
        }
      }

      public string FolderPath
      {
        get
        {
          try
          {
            return DriveProxy.API.DriveService.FolderPath;
          }
          catch (Exception exception)
          {
            Log.Error(exception);
            return null;
          }
        }
      }

      public bool Processing
      {
        get
        {
          try
          {
            return DriveProxy.API.DriveService.Processing;
          }
          catch (Exception exception)
          {
            Log.Error(exception);

            return false;
          }
        }
      }

      public bool StartedBackgroundProcesses
      {
        get
        {
          try
          {
            return DriveProxy.API.DriveService.StartedBackgroundProcesses;
          }
          catch (Exception exception)
          {
            Log.Error(exception);

            return false;
          }
        }
      }

      public bool StartingBackgroundProcesses
      {
        get
        {
          try
          {
            return DriveProxy.API.DriveService.StartingBackgroundProcesses;
          }
          catch (Exception exception)
          {
            Log.Error(exception);

            return false;
          }
        }
      }

      public bool StoppingBackgroundProcesses
      {
        get
        {
          try
          {
            return DriveProxy.API.DriveService.StoppingBackgroundProcesses;
          }
          catch (Exception exception)
          {
            Log.Error(exception);

            return false;
          }
        }
      }

      public void Dispose()
      {
        try
        {
          if (_service != null)
          {
            _service.Dispose();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void Dispose(bool stopBackgroundProcesses, bool waitWhileProcessing)
      {
        try
        {
          if (stopBackgroundProcesses)
          {
            StopBackgroundProcesses(waitWhileProcessing);
          }

          if (waitWhileProcessing)
          {
            for (int i = 0; i < 30; i++)
            {
              if (!Processing)
              {
                break;
              }

              System.Windows.Forms.Application.DoEvents();

              System.Threading.Thread.Sleep(100);
            }
          }

          if (_service != null)
          {
            _service.Dispose();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void Authenticate(bool startBackgroundProcesses, bool openInExplorer = false)
      {
        try
        {
          DriveProxy.API.DriveService.Authenticate(startBackgroundProcesses, openInExplorer);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void Signout()
      {
        try
        {
          DriveProxy.API.DriveService.Signout();
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void SignoutAndAuthenticate(bool startBackgroundProcesses, bool openInExplorer = false)
      {
        try
        {
          DriveProxy.API.DriveService.SignoutAndAuthenticate(startBackgroundProcesses, openInExplorer);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void StartBackgroundProcesses()
      {
        try
        {
          DriveProxy.API.DriveService.StartBackgroundProcesses();
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void StopBackgroundProcesses(bool waitWhileProcessing)
      {
        try
        {
          DriveProxy.API.DriveService.StopBackgroundProcesses(waitWhileProcessing);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public IAbout GetAbout()
      {
        try
        {
          AboutInfo aboutInfo = DriveProxy.API.DriveService.GetAbout();

          var about = new About(aboutInfo);

          return about;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      public ILog GetLog()
      {
        try
        {
          var log = new Log();

          log._Level = DriveProxy.API.DriveService.Settings.LogLevel;
          log._FilePath = DriveProxy.API.DriveService.Settings.LogFilePath;
          log._Source = Utils.Log.Source;
          log._LastError = Utils.Log.LastError;
          log._LastErrorTime = Utils.Log.LastErrorTime;

          return log;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      public void SetLog(ILog log)
      {
        try
        {
          DriveProxy.API.DriveService.Settings.LogLevel = log.Level;
          DriveProxy.API.DriveService.Settings.LogFilePath = log.FilePath;
          Utils.Log.Source = log.Source;
          DriveProxy.API.DriveService.Settings.Save();
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public IFile GetFile(string fileId = null, bool getChildren = true)
      {
        try
        {
          FileInfo fileInfo = _service.GetFile(fileId, getChildren);

          if (fileInfo == null)
          {
            throw new Exception("File " + fileId + " does not exist.");
          }

          var file = new File(fileInfo);

          if (getChildren)
          {
            var fileChildren = new List<File>();

            foreach (FileInfo fileInfoChild in fileInfo.Files)
            {
              var fileChild = new File(fileInfoChild);

              fileChildren.Add(fileChild);
            }

            file._Children = fileChildren;
          }

          return file;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      public IEnumerable<IFile> GetFiles(IEnumerable<string> fileIds, bool getChildren = true)
      {
        try
        {
          var files = new List<IFile>();

          foreach (string fileId in fileIds)
          {
            IFile file = GetFile(fileId, getChildren);

            if (file == null)
            {
              throw new Exception("File " + fileId + " does not exist.");
            }

            files.Add(file);
          }

          return files;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      public IEnumerable<IFile> GetFilesFromPath(string path)
      {
        try
        {
          IEnumerable<FileInfo> fileInfoList = _service.FindFiles(path);

          var files = new List<File>();

          foreach (FileInfo fileInfo in fileInfoList)
          {
            var file = new File(fileInfo);

            files.Add(file);
          }

          return files;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      public FileInfoStatus GetFileStatus(string fileId)
      {
        try
        {
          IFile file = GetFile(fileId, false);

          if (file == null)
          {
            throw new Exception("File " + fileId + " does not exist.");
          }

          return GetFileStatus(file);
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return FileInfoStatus.Unknown;
        }
      }

      public FileInfoStatus GetFileStatus(IFile file)
      {
        try
        {
          var tempFile = file as File;

          if (tempFile == null)
          {
            throw new Exception("IFile is null or an unexpected type.");
          }

          FileInfoStatus fileInfoStatus = DriveProxy.API.DriveService.GetFileInfoStatus(tempFile._FileInfo);

          return fileInfoStatus;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return FileInfoStatus.Unknown;
        }
      }

      public IFile GetCachedFile(string fileId = null, bool getChildren = true)
      {
        try
        {
          FileInfo fileInfo = _service.GetCachedFile(fileId, getChildren, false);

          var file = new File(fileInfo);

          if (getChildren)
          {
            var fileChildren = new List<File>();

            foreach (FileInfo fileInfoChild in fileInfo.Files)
            {
              var fileChild = new File(fileInfoChild);

              fileChildren.Add(fileChild);
            }

            file._Children = fileChildren;
          }

          return file;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      public IEnumerable<IFile> GetCachedFiles(IEnumerable<string> fileIds, bool getChildren = true)
      {
        try
        {
          var files = new List<IFile>();

          foreach (string fileId in fileIds)
          {
            IFile file = GetCachedFile(fileId, getChildren);

            files.Add(file);
          }

          return files;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      public void UpdateCachedData()
      {
        try
        {
          _service.UpdateCachedData();
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void CopyFile(IStream stream, string parentId, string fileId)
      {
        try
        {
          if (String.IsNullOrEmpty(fileId))
          {
            throw new Exception("FileId is null or blank.");
          }

          IFile file = GetFile(fileId, false);

          if (file == null)
          {
            throw new Exception("File " + fileId + " does not exist.");
          }

          CopyFile(stream, parentId, file);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void CopyFile(IStream stream, string parentId, IFile file)
      {
        try
        {
          var tempStream = stream as Stream;

          if (tempStream == null)
          {
            throw new Exception("IStream is null or an unexpected type.");
          }

          var tempFile = file as File;

          if (tempFile == null)
          {
            throw new Exception("IFile is null or an unexpected type.");
          }


          var copyStream = new API.DriveService.CopyStream();

          copyStream.Init(parentId, tempFile._FileInfo);

          tempStream._Service = this;
          tempStream._Stream = copyStream;

          _service.CopyFile(copyStream);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void DownloadFile(IStream stream, string fileId, int chunkSize = 0, bool checkIfAlreadyDownloaded = true)
      {
        try
        {
          if (String.IsNullOrEmpty(fileId))
          {
            throw new Exception("FileId is null or blank.");
          }

          IFile file = GetFile(fileId, false);

          if (file == null)
          {
            throw new Exception("File " + fileId + " does not exist.");
          }

          DownloadFile(stream, file, chunkSize, checkIfAlreadyDownloaded);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void DownloadFile(IStream stream, IFile file, int chunkSize = 0, bool checkIfAlreadyDownloaded = true)
      {
        try
        {
          var tempStream = stream as Stream;

          if (tempStream == null)
          {
            throw new Exception("IStream is null or an unexpected type.");
          }

          var tempFile = file as File;

          if (tempFile == null)
          {
            throw new Exception("IFile is null or an unexpected type.");
          }

          var downloadStream = new API.DriveService.DownloadStream();
          var parameters = new API.DriveService.DownloadStream.Parameters();

          parameters.ChunkSize = chunkSize;
          parameters.CheckIfAlreadyDownloaded = checkIfAlreadyDownloaded;

          downloadStream.Init(tempFile._FileInfo, parameters);

          tempStream._Service = this;
          tempStream._Stream = downloadStream;

          _service.DownloadFile(downloadStream);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void InsertFile(IStream stream, string parentId, string title, bool isFolder)
      {
        try
        {
          var tempStream = stream as Stream;

          if (tempStream == null)
          {
            throw new Exception("IStream is null or an unexpected type.");
          }

          var insertStream = new API.DriveService.InsertStream();

          string mimeType = null;

          if (isFolder)
          {
            mimeType = DriveProxy.API.DriveService.MimeType.Folder;
          }

          insertStream.Init(parentId, title, mimeType);

          tempStream._Service = this;
          tempStream._Stream = insertStream;

          _service.InsertFile(insertStream);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void InsertFile(IStream stream, IFile parent, string title, bool isFolder)
      {
        try
        {
          if (parent == null)
          {
            throw new Exception("IFile is null or an unexpected type.");
          }

          InsertFile(stream, parent.Id, title, isFolder);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void MoveFile(IStream stream, string parentId, string fileId)
      {
        try
        {
          if (String.IsNullOrEmpty(fileId))
          {
            throw new Exception("FileId is null or blank.");
          }

          IFile file = GetFile(fileId, false);

          if (file == null)
          {
            throw new Exception("File " + fileId + " does not exist.");
          }

          MoveFile(stream, parentId, file);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void MoveFile(IStream stream, string parentId, IFile file)
      {
        try
        {
          var tempStream = stream as Stream;

          if (tempStream == null)
          {
            throw new Exception("IStream is null or an unexpected type.");
          }

          var tempFile = file as File;

          if (tempFile == null)
          {
            throw new Exception("IFile is null or an unexpected type.");
          }


          var moveStream = new API.DriveService.MoveStream();

          moveStream.Init(parentId, tempFile._FileInfo);

          tempStream._Service = this;
          tempStream._Stream = moveStream;

          _service.MoveFile(moveStream);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void RenameFile(IStream stream, string fileId, string title)
      {
        try
        {
          if (String.IsNullOrEmpty(fileId))
          {
            throw new Exception("FileId is null or blank.");
          }

          IFile file = GetFile(fileId, false);

          if (file == null)
          {
            throw new Exception("File " + fileId + " does not exist.");
          }

          RenameFile(stream, file, title);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void RenameFile(IStream stream, IFile file, string title)
      {
        try
        {
          var tempStream = stream as Stream;

          if (tempStream == null)
          {
            throw new Exception("IStream is null or an unexpected type.");
          }

          var tempFile = file as File;

          if (tempFile == null)
          {
            throw new Exception("IFile is null or an unexpected type.");
          }


          var renameStream = new API.DriveService.RenameStream();


          renameStream.Init(tempFile._FileInfo, title, tempFile.MimeType);

          tempStream._Service = this;
          tempStream._Stream = renameStream;

          _service.RenameFile(renameStream);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void UploadFile(IStream stream, string fileId, int chunkSize = 0, bool checkIfAlreadyUploaded = true)
      {
        try
        {
          if (String.IsNullOrEmpty(fileId))
          {
            throw new Exception("FileId is null or blank.");
          }

          IFile file = GetFile(fileId, false);

          if (file == null)
          {
            throw new Exception("File " + fileId + " does not exist.");
          }

          UploadFile(stream, file, chunkSize, checkIfAlreadyUploaded);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void UploadFile(IStream stream, IFile file, int chunkSize = 0, bool checkIfAlreadyUploaded = true)
      {
        try
        {
          var tempStream = stream as Stream;

          if (tempStream == null)
          {
            throw new Exception("IStream is null or an unexpected type.");
          }

          var tempFile = file as File;

          if (tempFile == null)
          {
            throw new Exception("IFile is null or an unexpected type.");
          }

          var uploadStream = new API.DriveService.UploadStream();
          var parameters = new API.DriveService.UploadStream.Parameters();

          parameters.ChunkSize = chunkSize;
          parameters.CheckIfAlreadyUploaded = checkIfAlreadyUploaded;

          uploadStream.Init(tempFile._FileInfo, parameters);

          tempStream._Service = this;
          tempStream._Stream = uploadStream;

          _service.UploadFile(uploadStream);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void TrashFile(IStream stream, string fileId)
      {
        try
        {
          if (String.IsNullOrEmpty(fileId))
          {
            throw new Exception("FileId is null or blank.");
          }

          IFile file = GetFile(fileId, false);

          if (file == null)
          {
            throw new Exception("File " + fileId + " does not exist.");
          }

          TrashFile(stream, file);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void TrashFile(IStream stream, IFile file)
      {
        try
        {
          var tempStream = stream as Stream;

          if (tempStream == null)
          {
            throw new Exception("IStream is null or an unexpected type.");
          }

          var tempFile = file as File;

          if (tempFile == null)
          {
            throw new Exception("IFile is null or an unexpected type.");
          }

          var trashStream = new API.DriveService.TrashStream();

          trashStream.Init(tempFile._FileInfo);

          tempStream._Service = this;
          tempStream._Stream = trashStream;

          _service.TrashFile(trashStream);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void UntrashFile(IStream stream, string fileId)
      {
        try
        {
          if (String.IsNullOrEmpty(fileId))
          {
            throw new Exception("FileId is null or blank.");
          }

          IFile file = GetFile(fileId, false);

          if (file == null)
          {
            throw new Exception("File " + fileId + " does not exist.");
          }

          UntrashFile(stream, file);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void UntrashFile(IStream stream, IFile file)
      {
        try
        {
          var tempStream = stream as Stream;

          if (tempStream == null)
          {
            throw new Exception("IStream is null or an unexpected type.");
          }

          var tempFile = file as File;

          if (tempFile == null)
          {
            throw new Exception("IFile is null or an unexpected type.");
          }

          var untrashStream = new API.DriveService.UntrashStream();

          untrashStream.Init(tempFile._FileInfo);

          tempStream._Service = this;
          tempStream._Stream = untrashStream;

          _service.UntrashFile(untrashStream);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void CleanupFile(string fileId, bool ignoreFileOnDiskTimeout = true)
      {
        try
        {
          if (String.IsNullOrEmpty(fileId))
          {
            throw new Exception("FileId is null or blank.");
          }

          IFile file = GetFile(fileId, false);

          if (file == null)
          {
            throw new Exception("File " + fileId + " does not exist.");
          }

          CleanupFile(file, ignoreFileOnDiskTimeout);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void CleanupFile(IFile file, bool ignoreFileOnDiskTimeout = true)
      {
        try
        {
          var tempFile = file as File;

          if (tempFile == null)
          {
            throw new Exception("IFile is null or an unexpected type.");
          }

          _service.CleanupFile(tempFile._FileInfo, ignoreFileOnDiskTimeout, false);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void OpenGoogleDoc(string fileId)
      {
        try
        {
          if (String.IsNullOrEmpty(fileId))
          {
            throw new Exception("FileId is null or blank.");
          }

          IFile file = GetFile(fileId, false);

          if (file == null)
          {
            throw new Exception("File " + fileId + " does not exist.");
          }

          OpenGoogleDoc(file);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void OpenGoogleDoc(IFile file)
      {
        try
        {
          var tempFile = file as File;

          if (tempFile == null)
          {
            throw new Exception("IFile is null or an unexpected type.");
          }

          if (!DriveProxy.API.DriveService.IsGoogleDoc(tempFile.Type))
          {
            throw new Exception("IFile.Type is not a valid Google Doc.");
          }

          DriveProxy.API.DriveService.OpenGoogleDocFromDownloadUrl(tempFile._FileInfo.DownloadUrl);
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public void InvokeQueueStartedProcessing(object sender, IEventArgs e)
      {
        try
        {
          if (OnQueueStartedProcessing != null)
          {
            OnQueueStartedProcessing(sender, e);
          }
        }
        catch
        {
        }
      }

      public void InvokeQueueStartedStream(object sender, IStreamEventArgs e)
      {
        try
        {
          if (OnQueueStartedStream != null)
          {
            OnQueueStartedStream(sender, e);
          }
        }
        catch
        {
        }
      }

      public void InvokeQueueFinishedStream(object sender, IStreamEventArgs e)
      {
        try
        {
          if (OnQueueFinishedStream != null)
          {
            OnQueueFinishedStream(sender, e);
          }
        }
        catch
        {
        }
      }

      public void InvokeQueueFinishedProcessing(object sender, IEventArgs e)
      {
        try
        {
          if (OnQueueFinishedProcessing != null)
          {
            OnQueueFinishedProcessing(sender, e);
          }
        }
        catch
        {
        }
      }
    }

    protected class EventArgs : IEventArgs
    {
    }

    protected class File : IFile
    {
      internal IEnumerable<IFile> _Children = null;

      internal File()
      {
        _FileInfo = null;
      }

      internal File(FileInfo fileInfo)
      {
        _FileInfo = fileInfo;
      }

      internal FileInfo _FileInfo { get; set; }

      public bool Trashed
      {
        get
        {
          if (_FileInfo == null)
          {
            return false;
          }

          return _FileInfo.Trashed;
        }
      }

      public FileInfoType Type
      {
        get
        {
          if (_FileInfo == null)
          {
            return FileInfoType.Unknown;
          }

          return _FileInfo.Type;
        }
      }

      public string AssociationApplication
      {
        get
        {
          if (_FileInfo == null || _FileInfo.Association == null)
          {
            return "";
          }

          return _FileInfo.Association.Application;
        }
      }

      public string AssociationDefaultIcon
      {
        get
        {
          if (_FileInfo == null || _FileInfo.Association == null)
          {
            return "";
          }

          return _FileInfo.Association.DefaultIcon;
        }
      }

      public int AssociationDefaultIconIndex
      {
        get
        {
          if (_FileInfo == null || _FileInfo.Association == null)
          {
            return 0;
          }

          return DriveProxy.API.DriveService.GetInt32(_FileInfo.Association.DefaultIconIndex);
        }
      }

      public string AssociationFileType
      {
        get
        {
          if (_FileInfo == null || _FileInfo.Association == null)
          {
            return "";
          }

          return _FileInfo.Association.FileType;
        }
      }

      public bool Copyable
      {
        get
        {
          if (_FileInfo == null)
          {
            return false;
          }

          return _FileInfo.Copyable;
        }
      }

      public DateTime CreatedDate
      {
        get
        {
          if (_FileInfo == null)
          {
            return default(DateTime);
          }

          return _FileInfo.CreatedDate;
        }
      }

      public string Description
      {
        get
        {
          if (_FileInfo == null)
          {
            return "";
          }

          return _FileInfo.Description;
        }
      }

      public bool Editable
      {
        get
        {
          if (_FileInfo == null)
          {
            return false;
          }

          return _FileInfo.Editable;
        }
      }

      public string FileExtension
      {
        get
        {
          if (_FileInfo == null)
          {
            return "";
          }

          return _FileInfo.FileExtension;
        }
      }

      public string FileName
      {
        get
        {
          if (_FileInfo == null)
          {
            return "";
          }

          return _FileInfo.FileName;
        }
      }

      public long FileSize
      {
        get
        {
          if (_FileInfo == null)
          {
            return 0;
          }

          return _FileInfo.FileSize;
        }
      }

      public string FilePath
      {
        get
        {
          if (_FileInfo == null)
          {
            return "";
          }

          return _FileInfo.FilePath;
        }
      }

      public string Id
      {
        get
        {
          if (_FileInfo == null)
          {
            return "";
          }

          return _FileInfo.Id;
        }
      }

      public string LastModifyingUserName
      {
        get
        {
          if (_FileInfo == null)
          {
            return "";
          }

          return _FileInfo.LastModifyingUserName;
        }
      }

      public DateTime LastViewedByMeDate
      {
        get
        {
          if (_FileInfo == null)
          {
            return default(DateTime);
          }

          return _FileInfo.LastViewedByMeDate;
        }
      }

      public string MimeType
      {
        get
        {
          if (_FileInfo == null)
          {
            return "";
          }

          return _FileInfo.MimeType;
        }
      }

      public DateTime ModifiedByMeDate
      {
        get
        {
          if (_FileInfo == null)
          {
            return default(DateTime);
          }

          return _FileInfo.ModifiedByMeDate;
        }
      }

      public DateTime ModifiedDate
      {
        get
        {
          if (_FileInfo == null)
          {
            return default(DateTime);
          }

          return _FileInfo.ModifiedDate;
        }
      }

      public string OriginalFilename
      {
        get
        {
          if (_FileInfo == null)
          {
            return "";
          }

          return _FileInfo.OriginalFilename;
        }
      }

      public string OwnerName
      {
        get
        {
          if (_FileInfo == null)
          {
            return "";
          }

          return _FileInfo.OwnerName;
        }
      }

      public string ParentId
      {
        get
        {
          if (_FileInfo == null)
          {
            return "";
          }

          return _FileInfo.ParentId;
        }
      }

      public bool Shared
      {
        get
        {
          if (_FileInfo == null)
          {
            return false;
          }

          return _FileInfo.Shared;
        }
      }

      public DateTime SharedWithMeDate
      {
        get
        {
          if (_FileInfo == null)
          {
            return default(DateTime);
          }

          return _FileInfo.SharedWithMeDate;
        }
      }

      public string Title
      {
        get
        {
          if (_FileInfo == null)
          {
            return "";
          }

          return _FileInfo.Title;
        }
      }

      public IEnumerable<IFile> Children
      {
        get { return _Children; }
      }
    }

    protected class Log : ILog
    {
      internal string _FilePath = "";
      internal string _LastError = "";
      internal DateTime _LastErrorTime = default(DateTime);
      internal LogType _Level = LogType.None;

      internal string _Source = "";

      public LogType Level
      {
        get { return _Level; }
        set { _Level = value; }
      }

      public string Source
      {
        get { return _Source; }
        set { _Source = value; }
      }

      public string FilePath
      {
        get { return _FilePath; }
        set { _FilePath = value; }
      }

      public string LastError
      {
        get { return _LastError; }
      }

      public DateTime LastErrorTime
      {
        get { return _LastErrorTime; }
      }

      internal static void Error(Exception exception)
      {
        Utils.Log.Error(exception);
      }
    }

    protected class Stream : IStream
    {
      protected IAttributes _Attributes = null;
      protected File _File = null;
      private DriveProxy.API.DriveService.Stream _stream;

      internal Stream()
      {
        _Service = null;
      }

      internal Stream(DriveProxy.API.DriveService.Stream stream)
      {
        _Service = null;
        _Stream = stream;
      }

      internal DriveProxy.API.DriveService.Stream _Stream
      {
        get { return _stream; }
        set
        {
          _stream = value;

          if (_stream != null)
          {
            _stream.OnProgressChanged += stream_OnProgressChanged;
            _stream.OnProgressFinished += stream_OnProgressFinished;
            _stream.OnProgressStarted += stream_OnProgressStarted;
          }
        }
      }

      internal DriveService _Service { get; set; }
      public event IStreamEventHandler OnProgressStarted = null;
      public event IStreamEventHandler OnProgressChanged = null;
      public event IStreamEventHandler OnProgressFinished = null;

      public IFile File
      {
        get
        {
          if (_Stream == null)
          {
            return null;
          }

          if (_File == null)
          {
            _File = new File();
          }

          _File._FileInfo = _Stream.FileInfo;

          return _File;
        }
      }

      public StreamType Type
      {
        get
        {
          if (_Stream == null)
          {
            return StreamType.Unknown;
          }

          return _Stream.Type;
        }
      }

      public string FileId
      {
        get
        {
          if (_Stream == null)
          {
            return "";
          }

          return _Stream.FileId;
        }
      }

      public string FilePath
      {
        get
        {
          if (_Stream == null)
          {
            return "";
          }

          return _Stream.FilePath;
        }
      }

      public string FileName
      {
        get
        {
          if (_Stream == null)
          {
            return "";
          }

          return _Stream.FileName;
        }
      }

      public string Title
      {
        get
        {
          if (_Stream == null)
          {
            return "";
          }

          return _Stream.Title;
        }
      }

      public string MimeType
      {
        get
        {
          if (_Stream == null)
          {
            return "";
          }

          return _Stream.MimeType;
        }
      }

      public bool IsFolder
      {
        get
        {
          if (_Stream == null)
          {
            return false;
          }

          return _Stream.IsFolder;
        }
      }

      public bool NotStarted
      {
        get
        {
          if (_Stream == null)
          {
            return true;
          }

          return _Stream.NotStarted;
        }
      }

      public bool Queued
      {
        get
        {
          if (_Stream == null)
          {
            return false;
          }

          return _Stream.Queued;
        }
      }

      public bool Starting
      {
        get
        {
          if (_Stream == null)
          {
            return false;
          }

          return _Stream.Starting;
        }
      }

      public bool Processing
      {
        get
        {
          if (_Stream == null)
          {
            return false;
          }

          return _Stream.Processing;
        }
      }

      public bool Cancelling
      {
        get
        {
          if (_Stream == null)
          {
            return false;
          }

          return _Stream.Cancelling;
        }
      }

      public bool Cancelled
      {
        get
        {
          if (_Stream == null)
          {
            return false;
          }

          return _Stream.Cancelled;
        }
      }

      public bool Completed
      {
        get
        {
          if (_Stream == null)
          {
            return false;
          }

          return _Stream.Completed;
        }
      }

      public bool Failed
      {
        get
        {
          if (_Stream == null)
          {
            return false;
          }

          return _Stream.Failed;
        }
      }

      public bool Started
      {
        get
        {
          if (_Stream == null)
          {
            return false;
          }

          return _Stream.Started;
        }
      }

      public bool Processed
      {
        get
        {
          if (_Stream == null)
          {
            return false;
          }

          return _Stream.Processed;
        }
      }

      public bool Finished
      {
        get
        {
          if (_Stream == null)
          {
            return false;
          }

          return _Stream.Finished;
        }
      }

      public long BytesProcessed
      {
        get
        {
          if (_Stream == null)
          {
            return 0;
          }

          return _Stream.BytesProcessed;
        }
      }

      public long TotalBytes
      {
        get
        {
          if (_Stream == null)
          {
            return 0;
          }

          return _Stream.TotalBytes;
        }
      }

      public string ExceptionMessage
      {
        get
        {
          if (_Stream == null)
          {
            return "";
          }

          return _Stream.ExceptionMessage;
        }
      }

      public int PercentCompleted
      {
        get
        {
          if (_Stream == null)
          {
            return 0;
          }

          return _Stream.PercentCompleted;
        }
      }

      public TimeSpan RemainingTime
      {
        get
        {
          if (_Stream == null)
          {
            return default(TimeSpan);
          }

          return _Stream.RemainingTime;
        }
      }

      public IAttributes Attributes
      {
        get { return _Attributes ?? (_Attributes = new Attributes()); }
      }

      public void Cancel()
      {
        if (_Stream == null)
        {
          return;
        }

        _Stream.Cancel();
      }

      private void stream_OnProgressStarted(DriveProxy.API.DriveService.Stream stream)
      {
        try
        {
          if (OnProgressStarted != null)
          {
            OnProgressStarted(this, new StreamEventArgs(this));
          }
        }
        catch
        {
        }
      }

      private void stream_OnProgressChanged(DriveProxy.API.DriveService.Stream stream)
      {
        try
        {
          if (OnProgressChanged != null)
          {
            OnProgressChanged(this, new StreamEventArgs(this));
          }
        }
        catch
        {
        }
      }

      private void stream_OnProgressFinished(DriveProxy.API.DriveService.Stream stream)
      {
        try
        {
          if (OnProgressFinished != null)
          {
            OnProgressFinished(this, new StreamEventArgs(this));
          }
        }
        catch
        {
        }
      }
    }

    protected class StreamEventArgs : IStreamEventArgs
    {
      protected IStream _Stream = null;

      public StreamEventArgs(IStream stream)
      {
        _Stream = stream;
      }

      public IStream Stream
      {
        get { return _Stream; }
      }
    }
  }
}
