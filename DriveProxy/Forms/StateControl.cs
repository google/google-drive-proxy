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
using System.Drawing;
using System.Windows.Forms;
using DriveProxy.API;
using DriveProxy.Utils;

namespace DriveProxy.Forms
{
  partial class StateControl : UserControl
  {
    public delegate void RemoveHandler(StateControl sender);

    public API.DriveService.Stream Stream;
    protected Mutex _ProgressChangedMutex = new Mutex();
    protected bool _ScrolledIntoView = false;

    public StateControl(API.DriveService.Stream stream)
    {
      try
      {
        InitializeComponent();

        Enabled = false;
        Stream = stream;

        if (stream.IsFolder)
        {
          pictureBox1.Image = global::DriveProxy.Properties.Resources.smFolder;
        }
        else
        {
          Icon icon = DriveService.GetFileIcon(stream.FileInfo, 16, 16, true);

          if (icon == null)
          {
            pictureBox1.Image = global::DriveProxy.Properties.Resources.smFile;
          }
          else
          {
            pictureBox1.Image = icon.ToBitmap();

            icon.Dispose();
            icon = null;
          }
        }

        labelDescription.Text = stream.Title;

        stream.OnProgressChanged += Stream_ProgressChanged;

        labelStatus.Text = "Queued for " + stream.Type;
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    public StreamType StreamType
    {
      get { return Stream.Type; }
    }

    public event RemoveHandler OnRemove = null;

    public bool Processing()
    {
      try
      {
        return Stream.Processing;
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);

        return false;
      }
    }

    public void Cancel()
    {
      try
      {
        if (!labelCancel.Enabled || !labelCancel.Visible)
        {
          return;
        }

        Stream.Cancel();

        labelCancel.Enabled = false;
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    public void FinishedStream()
    {
      try
      {
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    public void UpdateControl()
    {
      try
      {
        if (Stream == null)
        {
          return;
        }

        Stream_ProgressChanged(Stream);
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void Stream_ProgressChanged(API.DriveService.Stream stream)
    {
      try
      {
        if (InvokeRequired)
        {
          BeginInvoke(new API.DriveService.Stream.ProgressChanged(Stream_ProgressChanged), new object[] {stream});
          return;
        }

        _ProgressChangedMutex.Wait();

        try
        {
          if (!Enabled)
          {
            return;
          }

          var panel = Parent as Panel;

          if (panel == null || stream == null)
          {
            return;
          }

          StreamType streamType = stream.Type;
          bool streamIsFolder = stream.IsFolder;
          bool streamProcessing = stream.Processing;
          bool streamProcessed = stream.Processed;
          bool streamStarting = stream.Starting;
          bool streamCompleted = stream.Completed;
          bool streamCancelled = stream.Cancelled;
          bool streamFailed = stream.Failed;
          string streamExceptionMessage = stream.ExceptionMessage;
          int streamPercentCompleted = stream.PercentCompleted;

          if (!_ScrolledIntoView)
          {
            _ScrolledIntoView = true;

            panel.ScrollControlIntoView(this);
          }

          if (streamProcessed)
          {
            if (progressBar.Value < 100)
            {
              labelStatus.Visible = false;
              progressBar.Visible = true;
              progressBar.Value = 100;
            }
          }

          if (streamType == StreamType.Download)
          {
            if (streamStarting)
            {
              progressBar.Visible = false;
              labelStatus.Visible = true;
              labelStatus.Text = "Downloading File...";
            }
            else if (streamCompleted)
            {
              progressBar.Visible = false;
              labelStatus.Visible = true;
              labelStatus.Text = "Downloaded File";
              labelCancel.Visible = false;
              labelRemove.Visible = true;
            }
            else if (streamCancelled)
            {
              progressBar.Visible = false;
              labelStatus.Visible = true;
              labelStatus.Text = "Download Cancelled";
              labelCancel.Visible = false;
              labelRemove.Visible = true;
            }
            else if (streamFailed)
            {
              progressBar.Visible = false;
              labelStatus.Visible = true;
              labelStatus.Text = "Download Failed";
              toolTip.SetToolTip(labelStatus, streamExceptionMessage);
              toolTip.Active = true;
              labelCancel.Visible = false;
              labelRemove.Visible = true;
            }
            else
            {
              labelStatus.Visible = false;
              progressBar.Visible = true;
              progressBar.Value = streamPercentCompleted;
            }
          }
          else if (streamType == StreamType.Insert)
          {
            if (streamIsFolder)
            {
              if (streamStarting)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Creating Folder...";
              }
              else if (streamCompleted)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Created Folder";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamCancelled)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Create Cancelled";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamFailed)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Create Failed";
                toolTip.SetToolTip(labelStatus, streamExceptionMessage);
                toolTip.Active = true;
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else
              {
                labelStatus.Visible = false;
                progressBar.Visible = true;
                progressBar.Value = streamPercentCompleted;
              }
            }
            else
            {
              if (streamStarting)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Creating File...";
              }
              else if (streamCompleted)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Created File";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamCancelled)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Create Cancelled";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamFailed)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Create Failed";
                toolTip.SetToolTip(labelStatus, streamExceptionMessage);
                toolTip.Active = true;
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else
              {
                labelStatus.Visible = false;
                progressBar.Visible = true;
                progressBar.Value = streamPercentCompleted;
              }
            }
          }
          else if (streamType == StreamType.Upload)
          {
            if (streamStarting)
            {
              progressBar.Visible = false;
              labelStatus.Visible = true;
              labelStatus.Text = "Uploading File...";
            }
            else if (streamCompleted)
            {
              progressBar.Visible = false;
              labelStatus.Visible = true;
              labelStatus.Text = "Uploaded File";
              labelCancel.Visible = false;
              labelRemove.Visible = true;
            }
            else if (streamCancelled)
            {
              progressBar.Visible = false;
              labelStatus.Visible = true;
              labelStatus.Text = "Upload Cancelled";
              labelCancel.Visible = false;
              labelRemove.Visible = true;
            }
            else if (streamFailed)
            {
              progressBar.Visible = false;
              labelStatus.Visible = true;
              labelStatus.Text = "Upload Failed";
              toolTip.SetToolTip(labelStatus, streamExceptionMessage);
              toolTip.Active = true;
              labelCancel.Visible = false;
              labelRemove.Visible = true;
            }
            else
            {
              labelStatus.Visible = false;
              progressBar.Visible = true;
              progressBar.Value = streamPercentCompleted;
            }
          }
          else if (streamType == StreamType.Trash)
          {
            if (streamIsFolder)
            {
              if (streamStarting)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Trashing Folder...";
              }
              else if (streamCompleted)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Trashed Folder";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamCancelled)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Trash Cancelled";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamFailed)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Trash Failed";
                toolTip.SetToolTip(labelStatus, streamExceptionMessage);
                toolTip.Active = true;
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else
              {
                labelStatus.Visible = false;
                progressBar.Visible = true;
                progressBar.Value = streamPercentCompleted;
              }
            }
            else
            {
              if (streamStarting)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Trashing File...";
              }
              else if (streamCompleted)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Trashed File";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamCancelled)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Trash Cancelled";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamFailed)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Trash Failed";
                toolTip.SetToolTip(labelStatus, streamExceptionMessage);
                toolTip.Active = true;
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else
              {
                labelStatus.Visible = false;
                progressBar.Visible = true;
                progressBar.Value = streamPercentCompleted;
              }
            }
          }
          else if (streamType == StreamType.Untrash)
          {
            if (streamIsFolder)
            {
              if (streamStarting)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Restoring Folder...";
              }
              else if (streamCompleted)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Restored Folder";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamCancelled)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Restore Cancelled";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamFailed)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Restore Failed";
                toolTip.SetToolTip(labelStatus, streamExceptionMessage);
                toolTip.Active = true;
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else
              {
                labelStatus.Visible = false;
                progressBar.Visible = true;
                progressBar.Value = streamPercentCompleted;
              }
            }
            else
            {
              if (streamStarting)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Restoring File...";
              }
              else if (streamCompleted)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Restored File";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamCancelled)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Restore Cancelled";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamFailed)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Restore Failed";
                toolTip.SetToolTip(labelStatus, streamExceptionMessage);
                toolTip.Active = true;
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else
              {
                labelStatus.Visible = false;
                progressBar.Visible = true;
                progressBar.Value = streamPercentCompleted;
              }
            }
          }
          else if (streamType == StreamType.Move)
          {
            if (streamIsFolder)
            {
              if (streamStarting)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Moving Folder...";
              }
              else if (streamCompleted)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Moved Folder";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamCancelled)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Move Cancelled";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamFailed)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Move Failed";
                toolTip.SetToolTip(labelStatus, streamExceptionMessage);
                toolTip.Active = true;
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else
              {
                labelStatus.Visible = false;
                progressBar.Visible = true;
                progressBar.Value = streamPercentCompleted;
              }
            }
            else
            {
              if (streamStarting)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Moving File...";
              }
              else if (streamCompleted)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Moved File";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamCancelled)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Move Cancelled";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamFailed)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Move Failed";
                toolTip.SetToolTip(labelStatus, streamExceptionMessage);
                toolTip.Active = true;
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else
              {
                labelStatus.Visible = false;
                progressBar.Visible = true;
                progressBar.Value = streamPercentCompleted;
              }
            }
          }
          else if (streamType == StreamType.Copy)
          {
            if (streamIsFolder)
            {
              if (streamStarting)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Copying Folder...";
              }
              else if (streamCompleted)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Copied Folder";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamCancelled)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Copy Cancelled";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamFailed)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Copy Failed";
                toolTip.SetToolTip(labelStatus, streamExceptionMessage);
                toolTip.Active = true;
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else
              {
                labelStatus.Visible = false;
                progressBar.Visible = true;
                progressBar.Value = streamPercentCompleted;
              }
            }
            else
            {
              if (streamStarting)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Copying File...";
              }
              else if (streamCompleted)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Copied File";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamCancelled)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Copy Cancelled";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamFailed)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Copy Failed";
                toolTip.SetToolTip(labelStatus, streamExceptionMessage);
                toolTip.Active = true;
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else
              {
                labelStatus.Visible = false;
                progressBar.Visible = true;
                progressBar.Value = streamPercentCompleted;
              }
            }
          }
          else if (streamType == StreamType.Rename)
          {
            if (streamIsFolder)
            {
              if (streamStarting)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Renaming Folder...";
              }
              else if (streamCompleted)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Renamed Folder";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamCancelled)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Rename Cancelled";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamFailed)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Rename Failed";
                toolTip.SetToolTip(labelStatus, streamExceptionMessage);
                toolTip.Active = true;
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else
              {
                labelStatus.Visible = false;
                progressBar.Visible = true;
                progressBar.Value = streamPercentCompleted;
              }
            }
            else
            {
              if (streamStarting)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Renaming File...";
              }
              else if (streamCompleted)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Renamed File";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamCancelled)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Rename Cancelled";
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else if (streamFailed)
              {
                progressBar.Visible = false;
                labelStatus.Visible = true;
                labelStatus.Text = "Rename Failed";
                toolTip.SetToolTip(labelStatus, streamExceptionMessage);
                toolTip.Active = true;
                labelCancel.Visible = false;
                labelRemove.Visible = true;
              }
              else
              {
                labelStatus.Visible = false;
                progressBar.Visible = true;
                progressBar.Value = streamPercentCompleted;
              }
            }
          }

          labelDescription.Refresh();
          progressBar.Refresh();
          labelStatus.Refresh();
          labelCancel.Refresh();
          labelRemove.Refresh();
          panel3.Refresh();

          Application.DoEvents();
        }
        finally
        {
          _ProgressChangedMutex.Release();
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, false, false);
      }
    }

    private void labelCancel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      try
      {
        Cancel();
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void labelRemove_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      try
      {
        if (OnRemove != null)
        {
          OnRemove(this);
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }
  }
}
