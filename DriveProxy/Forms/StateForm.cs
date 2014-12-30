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
using Math = DriveProxy.Utils.Math;

namespace DriveProxy.Forms
{
  partial class StateForm : Form
  {
    private readonly IntPtr _parentHwnd = IntPtr.Zero;
    private int _animationHeight = 20;
    private bool _hasError;
    private bool _isClosingPopup;
    private bool _isMaximized;
    private bool _isMaximizingPopup;
    private bool _isMinimized;
    private bool _isMinimizedByUser;
    private bool _isMinimizingPopup;
    private bool _isShowingPopup;
    private int _maxHeight = 350;
    private int _maxWidth = 500;
    private int _parentBottomOffset = 20;
    private System.Drawing.Rectangle _parentRectangle;
    private int _parenRightOffset = 20;
    protected bool _AllowClose = false;
    protected IntPtr _Hwnd = IntPtr.Zero;

    protected System.Drawing.Rectangle _HwndBounds;

    protected bool _IsProcessing = false;
    protected Mutex _Mutex = null;
    protected Mutex _ProgressChangedMutex = new Mutex();
    protected bool _Visibility = false;

    public StateForm()
    {
      try
      {
        InitializeComponent();

        TopMost = true;
        ShowInTaskbar = false;
        Visible = false;
        Opacity = 0;
        StartPosition = System.Windows.Forms.FormStartPosition.Manual;
        Location = new Point(-1000, -1000);

        DriveService.State.OnStartedProcessing += State_StartedProcessing;
        DriveService.State.OnQueuedStream += State_QueuedStream;
        DriveService.State.OnFinishedStream += State_FinishedStream;
        DriveService.State.OnFinishedProcessing += State_FinishedProcessing;
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public IntPtr Hwnd
    {
      get { return _Hwnd; }
      set
      {
        _Hwnd = value;

        Win32.GetWindowRect(_Hwnd, out _HwndBounds);
      }
    }

    public System.Drawing.Rectangle HwndBounds
    {
      get { return _HwndBounds; }
    }

    public bool IsProcessing
    {
      get { return _IsProcessing; }
    }

    public bool AllowClose
    {
      get { return _AllowClose; }
      set { _AllowClose = value; }
    }

    protected override CreateParams CreateParams
    {
      get
      {
        // Turn on WS_EX_TOOLWINDOW style bit
        CreateParams cp = base.CreateParams;
        cp.ExStyle |= 0x80;
        return cp;
      }
    }

    protected Mutex Mutex
    {
      get { return _Mutex ?? (_Mutex = new Mutex()); }
    }

    protected void ShowVisibility()
    {
      _Visibility = true;
      Visible = true;
    }

    protected void HideVisibility()
    {
      _Visibility = false;
      Visible = false;
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
      base.OnVisibleChanged(e);
      Visible = _Visibility;
    }

    private void StateForm_Load(object sender, EventArgs e)
    {
      try
      {
        TopMost = true;
        ShowInTaskbar = false;
        Visible = false;
        Opacity = 0;
        StartPosition = System.Windows.Forms.FormStartPosition.Manual;
        Location = new Point(-1000, -1000);

        timerWaitForClose.Interval = 2000;
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    private void StateForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      try
      {
        if (!AllowClose)
        {
          if (!Win32.IsSystemShuttingDown())
          {
            e.Cancel = true;

            ClosePopup();
          }
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void labelCancelAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      try
      {
        if (InvokeRequired)
        {
          BeginInvoke(new LinkLabelLinkClickedEventHandler(labelCancelAll_LinkClicked), new[] {sender, e});
          return;
        }

        for (int i = 0; i < panelBody.Controls.Count; i++)
        {
          var stateControl = panelBody.Controls[i] as StateControl;

          if (stateControl != null)
          {
            stateControl.Cancel();
          }
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    public void State_StartedProcessing(DriveProxy.API.DriveService.State sender)
    {
      try
      {
        if (InvokeRequired)
        {
          BeginInvoke(new DriveProxy.API.DriveService.State.EventHandler(State_StartedProcessing),
                      new object[] {sender});
          return;
        }

        _IsProcessing = true;

        timerFinishedProcessing.Stop();

        _hasError = false;

        if (!_isMinimizedByUser)
        {
          ShowPopup();
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    public void State_QueuedStream(API.DriveService.State sender, API.DriveService.Stream stream)
    {
      try
      {
        if (InvokeRequired)
        {
          BeginInvoke(new API.DriveService.State.StreamHandler(State_QueuedStream), new object[] {sender, stream});
          return;
        }
        stream.OnProgressChanged += Stream_ProgressChanged;

        var control = new StateControl(stream);

        control.OnRemove += StateControl_Remove;

        AddControl(control);
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    public void State_FinishedStream(DriveProxy.API.DriveService.State sender, API.DriveService.Stream stream)
    {
      try
      {
        if (InvokeRequired)
        {
          BeginInvoke(new DriveProxy.API.DriveService.State.StreamHandler(State_FinishedStream),
                      new object[] {sender, stream});
          return;
        }

        for (int i = 0; i < panelBody.Controls.Count; i++)
        {
          var stateControl = panelBody.Controls[i] as StateControl;

          if (stateControl != null)
          {
            if (stateControl.Stream == stream)
            {
              if (stream.Failed)
              {
                _hasError = true;
              }
              break;
            }
          }
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    public void State_FinishedProcessing(API.DriveService.State sender)
    {
      try
      {
        if (InvokeRequired)
        {
          BeginInvoke(new API.DriveService.State.EventHandler(State_FinishedProcessing), new object[] {sender});
          return;
        }
        _IsProcessing = false;

        Application.DoEvents();

        if (_hasError)
        {
          return;
        }

        timerFinishedProcessing.Start();
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
          if (stream == null)
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

          if (streamType == StreamType.Download)
          {
            pictureBoxState.Image = global::DriveProxy.Properties.Resources.download;

            if (streamProcessing)
            {
              labelTitle.Text = "Downloading file... (" + streamPercentCompleted + "%)";
            }
            else if (streamCompleted)
            {
              labelTitle.Text = "Download file complete";
            }
            else if (streamCancelled)
            {
              labelTitle.Text = "Download file cancelled";
            }
            else if (streamFailed)
            {
              labelTitle.Text = "Download file failed";
            }
          }
          else if (streamType == StreamType.Upload)
          {
            pictureBoxState.Image = global::DriveProxy.Properties.Resources.upload;

            if (streamProcessing)
            {
              labelTitle.Text = "Uploading file... (" + streamPercentCompleted + "%)";
            }
            else if (streamCompleted)
            {
              labelTitle.Text = "Upload file complete";
            }
            else if (streamCancelled)
            {
              labelTitle.Text = "Upload file cancelled";
            }
            else if (streamFailed)
            {
              labelTitle.Text = "Upload file failed";
            }
          }
          else if (streamType == StreamType.Insert)
          {
            pictureBoxState.Image = global::DriveProxy.Properties.Resources.upload;

            if (streamIsFolder)
            {
              if (streamProcessing)
              {
                labelTitle.Text = "Creating folder... (" + streamPercentCompleted + "%)";
              }
              else if (streamCompleted)
              {
                labelTitle.Text = "Create folder completed";
              }
              else if (streamCancelled)
              {
                labelTitle.Text = "Create folder cancelled";
              }
              else if (streamFailed)
              {
                labelTitle.Text = "Create folder failed";
              }
            }
            else
            {
              if (streamProcessing)
              {
                labelTitle.Text = "Creating file... (" + streamPercentCompleted + "%)";
              }
              else if (streamCompleted)
              {
                labelTitle.Text = "Create file completed";
              }
              else if (streamCancelled)
              {
                labelTitle.Text = "Create file cancelled";
              }
              else if (streamFailed)
              {
                labelTitle.Text = "Create file failed";
              }
            }
          }
          else if (streamType == StreamType.Insert)
          {
            pictureBoxState.Image = global::DriveProxy.Properties.Resources.upload;

            if (streamProcessing)
            {
              labelTitle.Text = "Uploading... (" + streamPercentCompleted + "%)";
            }
            else if (streamCompleted)
            {
              labelTitle.Text = "Upload completed";
            }
            else if (streamCancelled)
            {
              labelTitle.Text = "Upload cancelled";
            }
            else if (streamFailed)
            {
              labelTitle.Text = "Upload failed";
            }
          }
          else if (streamType == StreamType.Trash)
          {
            pictureBoxState.Image = global::DriveProxy.Properties.Resources.upload;

            if (streamIsFolder)
            {
              if (streamProcessing)
              {
                labelTitle.Text = "Trashing folder... (" + streamPercentCompleted + "%)";
              }
              else if (streamCompleted)
              {
                labelTitle.Text = "Trash folder completed";
              }
              else if (streamCancelled)
              {
                labelTitle.Text = "Trash folder cancelled";
              }
              else if (streamFailed)
              {
                labelTitle.Text = "Trash folder failed";
              }
            }
            else
            {
              if (streamProcessing)
              {
                labelTitle.Text = "Trashing file... (" + streamPercentCompleted + "%)";
              }
              else if (streamCompleted)
              {
                labelTitle.Text = "Trash file completed";
              }
              else if (streamCancelled)
              {
                labelTitle.Text = "Trash file cancelled";
              }
              else if (streamFailed)
              {
                labelTitle.Text = "Trash file failed";
              }
            }
          }
          else if (streamType == StreamType.Untrash)
          {
            pictureBoxState.Image = global::DriveProxy.Properties.Resources.upload;

            if (streamIsFolder)
            {
              if (streamProcessing)
              {
                labelTitle.Text = "Restoring folder... (" + streamPercentCompleted + "%)";
              }
              else if (streamCompleted)
              {
                labelTitle.Text = "Restore folder completed";
              }
              else if (streamCancelled)
              {
                labelTitle.Text = "Restore folder cancelled";
              }
              else if (streamFailed)
              {
                labelTitle.Text = "Restore folder failed";
              }
            }
            else
            {
              if (streamProcessing)
              {
                labelTitle.Text = "Restoring file... (" + streamPercentCompleted + "%)";
              }
              else if (streamCompleted)
              {
                labelTitle.Text = "Restore file completed";
              }
              else if (streamCancelled)
              {
                labelTitle.Text = "Restore file cancelled";
              }
              else if (streamFailed)
              {
                labelTitle.Text = "Restore file failed";
              }
            }
          }
          else if (streamType == StreamType.Move)
          {
            pictureBoxState.Image = global::DriveProxy.Properties.Resources.upload;

            if (streamIsFolder)
            {
              if (streamProcessing)
              {
                labelTitle.Text = "Moving folder... (" + streamPercentCompleted + "%)";
              }
              else if (streamCompleted)
              {
                labelTitle.Text = "Move folder completed";
              }
              else if (streamCancelled)
              {
                labelTitle.Text = "Move folder cancelled";
              }
              else if (streamFailed)
              {
                labelTitle.Text = "Move folder failed";
              }
            }
            else
            {
              if (streamProcessing)
              {
                labelTitle.Text = "Moving file... (" + streamPercentCompleted + "%)";
              }
              else if (streamCompleted)
              {
                labelTitle.Text = "Move file completed";
              }
              else if (streamCancelled)
              {
                labelTitle.Text = "Move file cancelled";
              }
              else if (streamFailed)
              {
                labelTitle.Text = "Move file failed";
              }
            }
          }
          else if (streamType == StreamType.Copy)
          {
            pictureBoxState.Image = global::DriveProxy.Properties.Resources.upload;

            if (streamIsFolder)
            {
              if (streamProcessing)
              {
                labelTitle.Text = "Copying folder... (" + streamPercentCompleted + "%)";
              }
              else if (streamCompleted)
              {
                labelTitle.Text = "Copy folder completed";
              }
              else if (streamCancelled)
              {
                labelTitle.Text = "Copy folder cancelled";
              }
              else if (streamFailed)
              {
                labelTitle.Text = "Copy folder failed";
              }
            }
            else
            {
              if (streamProcessing)
              {
                labelTitle.Text = "Copying file... (" + streamPercentCompleted + "%)";
              }
              else if (streamCompleted)
              {
                labelTitle.Text = "Copy file completed";
              }
              else if (streamCancelled)
              {
                labelTitle.Text = "Copy file cancelled";
              }
              else if (streamFailed)
              {
                labelTitle.Text = "Copy file failed";
              }
            }
          }

          pictureBoxState.Refresh();
          labelTitle.Refresh();
          panelTitleBar.Refresh();
          panelHeader.Refresh();

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

    private void StateControl_Remove(StateControl sender)
    {
      try
      {
        if (InvokeRequired)
        {
          BeginInvoke(new StateControl.RemoveHandler(StateControl_Remove), new object[] {sender});
          return;
        }

        RemoveControl(sender);
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    protected void AddControl(StateControl stateControl)
    {
      try
      {
        Mutex.Wait();

        try
        {
          int top = 0;

          for (int i = 0; i < panelBody.Controls.Count; i++)
          {
            Control control = panelBody.Controls[i];

            control.Top = top;

            top += control.Height;
          }

          stateControl.Top = top;
          stateControl.Height = 32;

          int width = 0;

          if (panelBody.Controls.Count == 0)
          {
            width = panelBody.Width;
          }
          else
          {
            width = panelBody.Controls[panelBody.Controls.Count - 1].Width;
          }

          stateControl.Width = width;

          stateControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

          panelBody.Controls.Add(stateControl);

          stateControl.Enabled = true;
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

    protected void RemoveControl(StateControl stateControl)
    {
      try
      {
        Mutex.Wait();

        try
        {
          panelBody.Controls.Remove(stateControl);

          UpdateControls();
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

    protected void UpdateControls(bool updateChild = false)
    {
      try
      {
        Mutex.Wait();

        try
        {
          int top = 0;

          for (int i = 0; i < panelBody.Controls.Count; i++)
          {
            Control control = panelBody.Controls[i];

            control.Top = top;

            top += control.Height;

            if (updateChild)
            {
              var stateControl = control as StateControl;

              if (stateControl != null)
              {
                stateControl.UpdateControl();
              }
            }
          }
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

    private void pictureBoxMinimize_Click(object sender, EventArgs e)
    {
      try
      {
        if (pictureBoxMinimize.Tag == null || pictureBoxMinimize.Tag.ToString() == "min")
        {
          _isMinimizedByUser = true;

          MinimizePopup();
        }
        else
        {
          _isMinimizedByUser = false;

          MaximizePopup();
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void pictureBoxClose_Click(object sender, EventArgs e)
    {
      try
      {
        _isMinimizedByUser = false;

        for (int i = 0; i < panelBody.Controls.Count; i++)
        {
          var stateControl = panelBody.Controls[i] as StateControl;

          if (stateControl != null)
          {
            if (stateControl.Processing())
            {
              MinimizePopup();

              return;
            }
          }
        }

        ClosePopup2();
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    public void ShowPopup()
    {
      try
      {
        ShowPopup(HwndBounds);
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    public void ShowPopup(System.Drawing.Rectangle parentRectangle)
    {
      try
      {
        timerClosePopup.Stop();
        timerMinimizePopup.Stop();

        _isClosingPopup = false;
        _isMinimizingPopup = false;

        if (_isShowingPopup || _isMaximizingPopup)
        {
          return;
        }

        if (_isMaximized)
        {
          return;
        }

        try
        {
          _isShowingPopup = true;
          _isMinimized = false;
          _isMinimizedByUser = false;

          _hasError = false;

          if (DriveService.IsSignedIn)
          {
            AboutInfo aboutInfo = DriveService.GetAbout();

            string quotaBytesUsed =
              Math.Divide(aboutInfo.QuotaBytesUsedAggregate, 1073741824, 1).ToString();
            string quotaBytesTotal =
              Math.Divide(aboutInfo.QuotaBytesTotal, 1073741824, 1).ToString();
            string quotaBytesUsedInTrash =
              Math.Divide(aboutInfo.QuotaBytesUsedInTrash, 1073741824, 1).ToString();

            labelStatus.Text = aboutInfo.Name + " Using " + quotaBytesUsed + " GB of " + quotaBytesTotal + " GB - " +
                               quotaBytesUsedInTrash + " GB in Trash";
          }
          _parentRectangle = parentRectangle;
          Width = _maxWidth;
          Height = _maxHeight;

          WindowState = FormWindowState.Normal;

          if (_parentRectangle.IsEmpty && !HwndBounds.IsEmpty)
          {
            _parentRectangle = HwndBounds;
          }

          if (_parentRectangle.IsEmpty)
          {
            _parentRectangle = Screen.PrimaryScreen.WorkingArea;
          }
          else
          {
            bool found = false;

            foreach (Screen screen in Screen.AllScreens)
            {
              if (screen.Bounds.Contains(HwndBounds))
              {
                _parentRectangle = screen.WorkingArea;

                found = true;
              }
            }

            if (!found)
            {
              _parentRectangle = Screen.PrimaryScreen.WorkingArea;
            }
          }

          Height = MinimumSize.Height;

          int left = _parentRectangle.Right - Width - _parenRightOffset;
          int top = _parentRectangle.Bottom - Height - _parentBottomOffset;

          if (left < 0)
          {
            left = 0;
          }

          if (top < 0)
          {
            top = 0;
          }

          Left = left;
          Top = top;
        }
        finally
        {
          timerShowPopup.Start();
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    public void ClosePopup()
    {
      try
      {
        timerWaitForClose.Stop();
        timerWaitForClose.Start();
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    public void ClosePopup2()
    {
      try
      {
        if (_isShowingPopup || _isClosingPopup || _isMaximizingPopup)
        {
          return;
        }

        _isClosingPopup = true;
        _isMinimized = false;
        _isMinimizedByUser = false;

        _hasError = false;

        if (_isMinimizingPopup)
        {
          return;
        }

        MinimizePopup();
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    public void MinimizePopup()
    {
      try
      {
        if (_isShowingPopup || _isMinimizingPopup || _isMaximizingPopup)
        {
          return;
        }

        if (!_isMaximized && _isMinimized)
        {
          return;
        }

        _isMinimizingPopup = true;
        _isMaximized = false;

        timerMinimizePopup.Start();
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    public void MaximizePopup()
    {
      try
      {
        if (_isMaximizingPopup)
        {
          return;
        }

        timerClosePopup.Stop();
        timerMinimizePopup.Stop();

        _isClosingPopup = false;
        _isMinimizingPopup = false;
        _isMinimized = false;
        _isMinimizedByUser = false;

        _isMaximizingPopup = true;

        timerMaximizePopup.Start();
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void timerWatchParent_Tick(object sender, EventArgs e)
    {
      try
      {
        if (Opacity == 0 ||
            _isShowingPopup ||
            _isClosingPopup ||
            _isMinimizingPopup ||
            _isMaximizingPopup)
        {
          return;
        }

        Rectangle tempRectangle = _parentRectangle;

        if (!Win32.GetWindowRect(_parentHwnd, out tempRectangle))
        {
          _parentRectangle = Screen.PrimaryScreen.Bounds;

          return;
        }

        if (tempRectangle == _parentRectangle)
        {
          return;
        }

        _parentRectangle = tempRectangle;

        int left = _parentRectangle.Right - Width - _parenRightOffset;
        int top = _parentRectangle.Bottom - Height - _parentBottomOffset;

        if (left < 0)
        {
          left = 0;
        }

        if (top < 0)
        {
          top = 0;
        }

        Left = left;
        Top = top;
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void timerShowPopup_Tick(object sender, EventArgs e)
    {
      try
      {
        timerShowPopup.Stop();

        Opacity = 1;
        ShowVisibility();

        MaximizePopup();

        _isShowingPopup = false;
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void timerClosePopup_Tick(object sender, EventArgs e)
    {
      try
      {
        timerClosePopup.Stop();

        Opacity = 0;
        HideVisibility();

        labelTitle.Text = "Google Drive";

        _isClosingPopup = false;
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void timerMinimizePopup_Tick(object sender, EventArgs e)
    {
      try
      {
        timerMinimizePopup.Stop();

        if (Height == MinimumSize.Height)
        {
          int left = _parentRectangle.Right - Width - _parenRightOffset;
          int top = _parentRectangle.Bottom - Height - _parentBottomOffset;

          if (left < 0)
          {
            left = 0;
          }

          if (top < 0)
          {
            top = 0;
          }

          Left = left;
          Top = top;

          pictureBoxMinimize.Image = global::DriveProxy.Properties.Resources.max;
          pictureBoxMinimize.Tag = "max";

          _isMinimizingPopup = false;
          _isMinimized = true;

          HideVisibility();

          if (_isClosingPopup)
          {
            timerClosePopup.Start();
          }
        }
        else
        {
          int height = Height - _animationHeight;
          int top = _animationHeight;

          if (height < MinimumSize.Height)
          {
            top = MinimumSize.Height - height;
            height = MinimumSize.Height;
          }

          Height = height;
          Top += top;

          timerMinimizePopup.Start();
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void timerMaximizePopup_Tick(object sender, EventArgs e)
    {
      try
      {
        timerMaximizePopup.Stop();

        if (Height == _maxHeight)
        {
          int left = _parentRectangle.Right - Width - _parenRightOffset;
          int top = _parentRectangle.Bottom - Height - _parentBottomOffset;

          if (left < 0)
          {
            left = 0;
          }

          if (top < 0)
          {
            top = 0;
          }

          Left = left;
          Top = top;

          pictureBoxMinimize.Image = global::DriveProxy.Properties.Resources.min;
          pictureBoxMinimize.Tag = "min";

          _isMaximizingPopup = false;
          _isMaximized = true;

          ShowVisibility();
        }
        else
        {
          int height = Height + _animationHeight;
          int top = _animationHeight;

          if (height > _maxHeight)
          {
            top = height - _maxHeight;
            height = _maxHeight;
          }

          Height = height;
          Top -= top;

          timerMaximizePopup.Start();
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void timerFinishedProcessing_Tick(object sender, EventArgs e)
    {
      try
      {
        if (_isShowingPopup || _isMaximizingPopup)
        {
          return;
        }

        timerFinishedProcessing.Stop();

        UpdateControls(true);

        ClosePopup();
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void timerWaitForClose_Tick(object sender, EventArgs e)
    {
      try
      {
        timerWaitForClose.Stop();

        if (IsProcessing)
        {
          timerWaitForClose.Start();
        }
        else
        {
          ClosePopup2();
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }
  }
}
