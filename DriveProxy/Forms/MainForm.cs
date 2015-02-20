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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DriveProxy.API;
using DriveProxy.Service;
using DriveProxy.Utils;

namespace DriveProxy.Forms
{
  public partial class MainForm : Form
  {
    private LogForm _logForm;
    private bool _runTestCases;
    private ServicePipeServer _servicePipeServer;
    private System.Threading.Thread _servicePipeServerThread;
    private bool _servicePipeServer_Cancel = false;
    private StateForm _stateForm;

    public MainForm()
    {
      InitializeComponent();

      ShowInTaskbar = false;
      Visible = false;
      Opacity = 0;
      StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      Location = new Point(-1000, -1000);
      //Hide form from alt-tab
      FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
    }

    private IntPtr Hwnd
    {
      get { return _stateForm.Hwnd; }
      set { _stateForm.Hwnd = value; }
    }

    //Hide form from alt-tab
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

    protected override void OnVisibleChanged(EventArgs e)
    {
      base.OnVisibleChanged(e);
      Visible = false;
    }

    private void FormMain_Load(object sender, EventArgs e)
    {
      try
      {
        ShowInTaskbar = false;
        Visible = false;
        Opacity = 0;
        StartPosition = System.Windows.Forms.FormStartPosition.Manual;
        Location = new Point(-1000, -1000);
        string notifyText = DriveService.Settings.Product + " " + DriveService.Settings.UserName + " on " +
                            Environment.MachineName;
        if (notifyText.Length > 63)
        {
          notifyIcon1.Text = notifyText.Substring(0, 60) + "...";
        }
        else
        {
          notifyIcon1.Text = notifyText;
        }


        foreach (MethodInfo methodInfo in DriveProxy.Service.Service.Methods)
        {
          methodInfo.OnReceivedHwnd += methodInfo_OnReceivedHwnd;
        }

        _stateForm = new StateForm();

        _stateForm.Show();
        _stateForm.Visible = false;
        _stateForm.Hide();

        _logForm = new LogForm();

        _logForm.FormClosed += LogForm_FormClosed;

        _logForm.Visible = false;
        _logForm.Hide();

        StatusForm.Init();
        FeedbackForm.Init();

        if (_runTestCases)
        {
          if (System.Windows.Forms.MessageBox.Show(this,
                                                   "Attempting to run test cases!",
                                                   Text,
                                                   MessageBoxButtons.OKCancel,
                                                   MessageBoxIcon.Warning) != System.Windows.Forms.DialogResult.OK)
          {
            _runTestCases = false;
          }
        }

        _servicePipeServerThread = new System.Threading.Thread(ServicePipeServer_Process);

        _servicePipeServerThread.Start();
        Visible = false;
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void LogForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      try
      {
        _logForm = null;
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void methodInfo_OnReceivedHwnd(IntPtr hwnd)
    {
      try
      {
        Hwnd = hwnd;
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void ServicePipeServer_Process()
    {
      try
      {
        if (_runTestCases)
        {
          Test.Run();
          return;
        }

        _servicePipeServer = new ServicePipeServer();

        _servicePipeServer.Process();
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      try
      {
        if (_stateForm.IsProcessing)
        {
          _stateForm.ShowPopup();

          MessageBox.Show(this,
                          "Please wait until processing has completed.",
                          Text,
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Error);

          return;
        }

        if (
          MessageBox.Show(this,
                          "Are you sure you want to sign in to another account?",
                          Text,
                          MessageBoxButtons.YesNo,
                          MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
        {
          return;
        }

        _stateForm.ClosePopup();

        DriveService.SignoutAndAuthenticate(true);
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void showStatusWindowToolStripMenuItem_Click(object sender, EventArgs e)
    {
      try
      {
        _stateForm.ShowPopup();
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      try
      {
        _logForm.AllowClose = true;

        _logForm.Close();

        _stateForm.AllowClose = true;

        _stateForm.Close();

        StatusForm.Close();
        FeedbackForm.Close();

        notifyIcon1.Visible = false;

        try
        {
          if (_servicePipeServerThread != null)
          {
            ServicePipeClient.CloseServer();

            for (int i = 0; i < 1; i++)
            {
              if (_servicePipeServerThread == null)
              {
                break;
              }

              System.Threading.Thread.Sleep(100);
            }
          }
        }
        catch
        {
        }

        DriveService.StopBackgroundProcesses(true);
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void toolStripMenuItemSignOut_Click(object sender, EventArgs e)
    {
      try
      {
        if (_stateForm.IsProcessing)
        {
          _stateForm.ShowPopup();

          MessageBox.Show(this,
                          "Please wait until processing has completed.",
                          Text,
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Error);

          return;
        }

        if (
          MessageBox.Show(this,
                          "Are you sure you want to sign out?",
                          Text,
                          MessageBoxButtons.YesNo,
                          MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
        {
          return;
        }

        _stateForm.ClosePopup();

        DriveService.Signout();
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
    {
      try
      {
        toolStripMenuItemSignOut.Enabled = DriveService.IsSignedIn;
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void toolStripMenuItemSignIn_Click(object sender, EventArgs e)
    {
      try
      {
        DriveService.Authenticate(true);
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void toolStripMenuItemSettings_Click(object sender, EventArgs e)
    {
      try
      {
        var settingsForm = new SettingsForm();

        settingsForm.ShowDialog(this);
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void toolStripMenuItemClearCache_Click(object sender, EventArgs e)
    {
      try
      {
        DialogResult dialogResult = MessageBox.Show(this,
                                                    "This will delete any of the cached data about files listed in " +
                                                    DriveService.Settings.Product +
                                                    ".\r\n\r\nAre you sure you want to delete cached file data?",
                                                    DriveService.Settings.Product,
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Question);

        if (dialogResult != System.Windows.Forms.DialogResult.Yes)
        {
          return;
        }

        DriveService.ClearCache();
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void toolStripMenuItemCleanupFiles_Click(object sender, EventArgs e)
    {
      try
      {
        DialogResult dialogResult = MessageBox.Show(this,
                                                    "This will remove any of the files on disk that were downloaded from " +
                                                    DriveService.Settings.Product +
                                                    ".\r\n\r\nAre you sure you want to remove any downloaded files?",
                                                    DriveService.Settings.Product,
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Question);

        if (dialogResult != System.Windows.Forms.DialogResult.Yes)
        {
          return;
        }

        DriveService.CleanupFiles();
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void toolStripMenuItemShowLog_Click(object sender, EventArgs e)
    {
      try
      {
        if (_logForm == null)
        {
          _logForm = new LogForm();
        }

        _logForm.Visible = true;
        _logForm.Show();
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void toolStripMenuItemExit_Click(object sender, EventArgs e)
    {
      try
      {
        if (_stateForm.IsProcessing)
        {
          _stateForm.ShowPopup();

          DialogResult dialogResult = MessageBox.Show(this,
                                                      DriveService.Settings.Product +
                                                      " is still processing and recommends waiting for completion.\r\n\r\nAre you sure you want to exit " +
                                                      DriveService.Settings.Product + "...?",
                                                      Text,
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Warning);

          if (dialogResult != System.Windows.Forms.DialogResult.Yes)
          {
            return;
          }
        }
        else
        {
          if (
            MessageBox.Show(this,
                            "Are you sure you want to exit " + DriveService.Settings.Product + "?",
                            Text,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
          {
            return;
          }
        }

        _stateForm.ClosePopup();

        Close();
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }
  }
}
