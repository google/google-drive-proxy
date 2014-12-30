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
using Math = DriveProxy.Utils.Math;

namespace DriveProxy.API
{
  partial class DriveService
  {
    public abstract class Stream
    {
      public delegate void ProgressChanged(Stream stream);

      public delegate void ProgressFinished(Stream stream);

      public delegate void ProgressStarted(Stream stream);

      public enum StatusType
      {
        NotStarted,
        Queued,
        Starting,
        Processing,
        Cancelling,
        Cancelled,
        Completed,
        Failed,
        Finished,
      }

      protected long _BytesProcessed = 0;
      protected System.Threading.CancellationTokenSource _CancellationTokenSource = null;
      protected string _ExceptionMessage = "";
      protected string _FileId = "";
      protected FileInfo _FileInfo = null;
      private API.DriveService.FileInfoLock _fileInfoLock;

      protected bool _Initialized = false;
      private bool _invokedOnProgressStarted;
      protected int _PercentCompleted = 0;
      protected TimeSpan _RemainingTime = default(TimeSpan);
      protected StatusType _Status = StatusType.NotStarted;
      private StopWatch _stopWatch;
      protected long _TotalBytes = 0;
      private bool _visible = true;

      private API.DriveService _driveService;

      public Stream()
      {
        _Finished = false;
      }

      protected API.DriveService _DriveService
      {
        get { return _driveService ?? (_driveService = API.DriveService.Create()); }
        private set { _driveService = value; }
      }

      public bool Visible
      {
        get { return _visible; }
        set { _visible = value; }
      }

      public FileInfo FileInfo
      {
        get { return _FileInfo; }
      }

      internal API.DriveService.FileInfoLock FileInfoLock
      {
        get { return _fileInfoLock; }
      }

      protected System.IO.FileStream _FileStream
      {
        get
        {
          if (_fileInfoLock != null)
          {
            return _fileInfoLock.FileStream;
          }

          return null;
        }
      }

      protected string _FileStreamPath
      {
        get
        {
          if (_fileInfoLock != null)
          {
            return _fileInfoLock.FilePath;
          }

          return null;
        }
      }

      public abstract StreamType Type { get; }

      public virtual string FileId
      {
        get { return _FileId; }
      }

      public virtual string FilePath
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

      public virtual string FileName
      {
        get
        {
          if (_FileInfo == null)
          {
            return "";
          }

          if (IsFolder)
          {
            return _FileInfo.Title;
          }
          return _FileInfo.FileName;
        }
      }

      public virtual string Title
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

      public virtual string MimeType
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

      public virtual bool IsFolder
      {
        get
        {
          if (_FileInfo == null)
          {
            return false;
          }

          return _FileInfo.IsFolder;
        }
      }

      public StatusType Status
      {
        get { return _Status; }
      }

      public bool NotStarted
      {
        get
        {
          if (Status == StatusType.NotStarted)
          {
            return true;
          }

          return false;
        }
      }

      public bool Queued
      {
        get
        {
          if (Status == StatusType.Queued)
          {
            return true;
          }

          return false;
        }
      }

      public bool Starting
      {
        get
        {
          if (Status == StatusType.Starting)
          {
            return true;
          }

          return false;
        }
      }

      public bool Processing
      {
        get
        {
          if (Status == StatusType.Processing)
          {
            return true;
          }

          return false;
        }
      }

      public bool Cancelling
      {
        get
        {
          if (Status == StatusType.Cancelling)
          {
            return true;
          }

          return false;
        }
      }

      public bool Cancelled
      {
        get
        {
          if (Status == StatusType.Cancelled)
          {
            return true;
          }

          return false;
        }
      }

      public bool Completed
      {
        get
        {
          if (Status == StatusType.Completed)
          {
            return true;
          }

          return false;
        }
      }

      public bool Failed
      {
        get
        {
          if (Status == StatusType.Failed)
          {
            return true;
          }

          return false;
        }
      }

      public bool Started
      {
        get
        {
          if (Starting ||
              Processing ||
              Cancelling)
          {
            return true;
          }

          return false;
        }
      }

      public bool Processed
      {
        get
        {
          if (Cancelled ||
              Completed ||
              Failed)
          {
            return true;
          }

          return false;
        }
      }

      protected bool _Finished { get; set; }

      public bool Finished
      {
        get
        {
          if (!Processed)
          {
            return false;
          }

          return _Finished;
        }
      }

      public long BytesProcessed
      {
        get { return _BytesProcessed; }
      }

      public long TotalBytes
      {
        get { return _TotalBytes; }
      }

      public string ExceptionMessage
      {
        get { return _ExceptionMessage; }
      }

      public int PercentCompleted
      {
        get { return _PercentCompleted; }
      }

      private StopWatch StopWatch
      {
        get { return _stopWatch ?? (_stopWatch = new StopWatch()); }
      }

      public TimeSpan RemainingTime
      {
        get { return _RemainingTime; }
      }

      public System.Threading.CancellationTokenSource CancellationTokenSource
      {
        get { return _CancellationTokenSource; }
      }

      protected bool IsCancellationRequested
      {
        get
        {
          if (_CancellationTokenSource != null && _CancellationTokenSource.IsCancellationRequested)
          {
            return true;
          }

          return false;
        }
      }

      public event ProgressStarted OnProgressStarted = null;
      public event ProgressChanged OnProgressChanged = null;
      public event ProgressFinished OnProgressFinished = null;

      protected virtual void Init()
      {
        try
        {
          _Initialized = true;
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      protected virtual void Queue()
      {
        try
        {
          if (!_Initialized)
          {
            throw new Exception("Stream has not been initialized.");
          }

          if (Status == StatusType.Queued)
          {
            throw new Exception("Stream has already been queued.");
          }

          if (Status != StatusType.NotStarted)
          {
            throw new Exception("Stream has already been started.");
          }

          _Status = StatusType.Queued;
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      protected virtual void Lock()
      {
        try
        {
          try
          {
            if (FileInfo == null)
            {
              throw new Exception("FileInfo is null");
            }

            _fileInfoLock = LockStream(this, FilePath);
          }
          catch (Exception exception)
          {
            throw new Exception("Could not lock file '" + Title + "' - " + exception.Message);
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      protected virtual void Lock(string filePath,
                                  System.IO.FileMode fileMode,
                                  System.IO.FileAccess fileAccess,
                                  System.IO.FileShare fileShare)
      {
        try
        {
          try
          {
            if (FileInfo == null)
            {
              throw new Exception("FileInfo is null");
            }

            _fileInfoLock = LockStream(this, filePath, fileMode, fileAccess, fileShare);
          }
          catch (Exception exception)
          {
            throw new Exception("Could not lock file '" + Title + "' - " + exception.Message);
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, breakWithDebugger:false);
        }
      }

      protected virtual void Start()
      {
        try
        {
          if (Status == StatusType.NotStarted)
          {
            throw new Exception("Stream has not been queued.");
          }
          _Status = StatusType.Starting;

          if (!StopWatch.IsRunning)
          {
            StopWatch.Start();
          }

          InvokeOnProgressStarted();
          InvokeOnProgressChanged();
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      protected virtual void UpdateProgress(StatusType status, long bytesProcessed, long totalBytes, Exception exception)
      {
        try
        {
          _TotalBytes = totalBytes;

          if (status == StatusType.Completed)
          {
            _BytesProcessed = _TotalBytes;
            _PercentCompleted = 100;
            _RemainingTime = new TimeSpan(0);

            if (!Processed)
            {
              InvokeOnProgressChanged();
            }

            _Status = StatusType.Completed;
          }
          else if (status == StatusType.Failed)
          {
            _RemainingTime = new TimeSpan(0);

            if (!Processed)
            {
              InvokeOnProgressChanged();
            }

            if (IsCancellationRequested)
            {
              _Status = StatusType.Cancelled;
            }
            else
            {
              _Status = StatusType.Failed;

              if (exception != null)
              {
                _ExceptionMessage = exception.Message;
              }

              if (String.IsNullOrEmpty(_ExceptionMessage))
              {
                _ExceptionMessage = "Unknown error";
              }
            }
          }
          else
          {
            _Status = IsCancellationRequested ? StatusType.Cancelling : status;

            if (_BytesProcessed < bytesProcessed)
            {
              _BytesProcessed = bytesProcessed;

              try
              {
                _PercentCompleted =
                  Convert.ToInt32(Math.IsWhatPercentOf(_BytesProcessed, _TotalBytes, 0));
              }
              catch
              {
              }

              try
              {
                _RemainingTime = StopWatch.RemainingTimeSpan(_BytesProcessed, _TotalBytes);
              }
              catch
              {
              }
            }
          }

          if (Processed)
          {
            CloseFileStream();
          }
        }
        catch (Exception exception2)
        {
          Log.Error(exception2, false);
        }
      }

      public virtual void Cancel()
      {
        try
        {
          if (Processed)
          {
            return;
          }

          if (IsCancellationRequested || _Status == StatusType.Cancelled)
          {
            return;
          }

          if (Processing)
          {
            CancellationTokenSource.Cancel();
          }
          else
          {
            _Status = StatusType.Cancelled;
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      protected virtual void CloseFileStream()
      {
        try
        {
          if (_fileInfoLock != null)
          {
            _fileInfoLock.Dispose();
            _fileInfoLock = null;
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      protected virtual void Dispose()
      {
        try
        {
          CloseFileStream();

          if (_CancellationTokenSource != null)
          {
            _CancellationTokenSource.Dispose();
            _CancellationTokenSource = null;
          }

          if (_DriveService != null)
          {
            _DriveService.Dispose();
            _DriveService = null;
          }

          if (_stopWatch != null)
          {
            _stopWatch.Stop();
            _stopWatch = null;
          }

          _Finished = true;
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      protected virtual void InvokeOnProgressEvent()
      {
        try
        {
          if (Started && !Processed)
          {
            InvokeOnProgressChanged();
          }
          else if (Processed)
          {
            Dispose();

            InvokeOnProgressFinished();

            Factory.Finish(this);
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private void InvokeOnProgressStarted()
      {
        try
        {
          if (_invokedOnProgressStarted)
          {
            return;
          }

          _invokedOnProgressStarted = true;

          if (OnProgressStarted != null)
          {
            OnProgressStarted(this);
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private void InvokeOnProgressChanged()
      {
        try
        {
          if (!_invokedOnProgressStarted)
          {
            InvokeOnProgressStarted();
          }

          if (OnProgressChanged != null)
          {
            OnProgressChanged(this);
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      private void InvokeOnProgressFinished()
      {
        try
        {
          if (!_invokedOnProgressStarted)
          {
            InvokeOnProgressChanged();
          }

          if (OnProgressFinished != null)
          {
            OnProgressFinished(this);
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception, false);
        }
      }

      internal class Factory
      {
        public static void Queue(Stream stream)
        {
          try
          {
            stream.Queue();
          }
          catch (Exception exception)
          {
            Log.Error(exception);
          }
        }

        public static void Start(Stream stream)
        {
          try
          {
            stream.Start();
          }
          catch (Exception exception)
          {
            Log.Error(exception);
          }
        }

        public static void Abort(Stream stream, StatusType status, Exception exception = null)
        {
          try
          {
            stream.UpdateProgress(status, stream.BytesProcessed, stream.TotalBytes, exception);
            stream.InvokeOnProgressEvent();
            stream.Dispose();
          }
          catch (Exception exception2)
          {
            Log.Error(exception2);
          }
        }

        public static void Finish(Stream stream)
        {
          try
          {
            using (API.DriveService.State state = API.DriveService.State.Create())
            {
              state.FinishStream(stream);
            }
          }
          catch (Exception exception)
          {
            Log.Error(exception);
          }
        }

        public static void Dispose(Stream stream)
        {
          try
          {
            stream.Dispose();
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
