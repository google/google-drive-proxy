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
using System.Windows.Forms;
using DriveProxy.API;
using DriveProxy.Utils;

namespace DriveProxy.Test
{
  public partial class TestForm : Form
  {
    private DriveProxy.Test.Forms.TestControls.AuthenticateControl _authenticateControl;
    private DriveProxy.Test.Forms.TestControls.CleanupFileControl _cleanupFileControl;
    private DriveProxy.Test.Forms.TestControls.CopyFilesControl _copyFilesControl;
    private DriveProxy.Test.Forms.TestControls.DownloadFileControl _downloadFileControl;
    private IDriveService _driveService;
    private DriveProxy.Test.Forms.TestControls.FolderPathControl _folderPathControl;
    private DriveProxy.Test.Forms.TestControls.GetAboutControl _getAboutControl;
    private DriveProxy.Test.Forms.TestControls.GetFileControl _getFileControl;
    private DriveProxy.Test.Forms.TestControls.GetFileStatusControl _getFileStatusControl;
    private DriveProxy.Test.Forms.TestControls.GetFilesFromPathControl _getFilesFromPathControl;
    private DriveProxy.Test.Forms.TestControls.GetLogControl _getLogControl;
    private DriveProxy.Test.Forms.TestControls.InsertFileControl _insertFileControl;
    private bool _isCancelling;
    private DriveProxy.Test.Forms.TestControls.IsSignedInControl _isSignedInControl;
    private DriveProxy.Test.Forms.TestControls.MoveFilesControl _moveFilesControl;
    private DriveProxy.Test.Forms.TestControls.ProcessingControl _processingControl;
    private DriveProxy.Test.Forms.TestControls.RenameFileControl _renameFileControl;
    private DriveProxy.Test.Forms.TestControls.SetLogControl _setLogControl;
    private DriveProxy.Test.Forms.TestControls.SignoutControl _signoutControl;
    private DriveProxy.Test.Forms.TestControls.StartBackgroundProcessesControl _startBackgroundProcessesControl;

    private DriveProxy.Test.Forms.TestControls.StartedBackgroundProcessesControl _startedBackgroundProcessesControl;

    private DriveProxy.Test.Forms.TestControls.StartingBackgroundProcessesControl _startingBackgroundProcessesControl;

    private DriveProxy.Test.Forms.TestControls.StopBackgroundProcessesControl _stopBackgroundProcessesControl;

    private DriveProxy.Test.Forms.TestControls.StoppingBackgroundProcessesControl _stoppingBackgroundProcessesControl;

    private DriveProxy.Test.Forms.TestControls.TrashFileControl _trashFileControl;
    private DriveProxy.Test.Forms.TestControls.UntrashFileControl _untrashFileControl;
    private DriveProxy.Test.Forms.TestControls.UploadFileControl _uploadFileControl;


    public TestForm()
    {
      InitializeComponent();
    }

    private void TestForm_Load(object sender, EventArgs e)
    {
      try
      {
        progressBar.Visible = true;

        _driveService = DriveServiceFactory.CreateService();
      }
      catch (Exception exception)
      {
        MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void TestForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      try
      {
        if (_driveService != null)
        {
          _driveService.Dispose(true, true);
        }
      }
      catch (Exception exception)
      {
        MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void timer_Tick(object sender, EventArgs e)
    {
      timer.Stop();
    }

    private void radioButtonGroup_HideControls(RadioButton checkedChanged)
    {
      foreach (Control control in groupBoxMethods.Controls)
      {
        var radioButton = control as RadioButton;

        if (radioButton != null && radioButton != checkedChanged)
        {
          radioButton.Checked = false;
        }
      }

      foreach (Control control in panelParameters.Controls)
      {
        control.Hide();
      }
    }

    private void radioButtonAuthenticate_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonAuthenticate.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonAuthenticate);

      if (_authenticateControl == null)
      {
        _authenticateControl = new DriveProxy.Test.Forms.TestControls.AuthenticateControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_authenticateControl);
      }

      _authenticateControl.Visible = true;
    }

    private void radioButtonCleanupFile_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonCleanupFile.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonCleanupFile);

      if (_cleanupFileControl == null)
      {
        _cleanupFileControl = new DriveProxy.Test.Forms.TestControls.CleanupFileControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_cleanupFileControl);
      }

      _cleanupFileControl.Visible = true;
    }

    private void radioButtonCopyFiles_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonCopyFiles.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonCopyFiles);

      if (_copyFilesControl == null)
      {
        _copyFilesControl = new DriveProxy.Test.Forms.TestControls.CopyFilesControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_copyFilesControl);
      }

      _copyFilesControl.Visible = true;
    }

    private void radioButtonDownloadFile_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonDownloadFile.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonDownloadFile);

      if (_downloadFileControl == null)
      {
        _downloadFileControl = new DriveProxy.Test.Forms.TestControls.DownloadFileControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_downloadFileControl);
      }

      _downloadFileControl.Visible = true;
    }

    private void radioButtonFolderPath_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonFolderPath.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonFolderPath);

      if (_folderPathControl == null)
      {
        _folderPathControl = new DriveProxy.Test.Forms.TestControls.FolderPathControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_folderPathControl);
      }

      _folderPathControl.Visible = true;
    }

    private void radioButtonGetAbout_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonGetAbout.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonGetAbout);

      if (_getAboutControl == null)
      {
        _getAboutControl = new DriveProxy.Test.Forms.TestControls.GetAboutControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_getAboutControl);
      }

      _getAboutControl.Visible = true;
    }

    private void radioButtonGetFiles_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonGetFile.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonGetFile);

      if (_getFileControl == null)
      {
        _getFileControl = new DriveProxy.Test.Forms.TestControls.GetFileControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_getFileControl);
      }

      _getFileControl.Visible = true;
    }

    private void radioButtonGetFilesFromPathsFromPath_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonGetFilesFromPath.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonGetFilesFromPath);

      if (_getFilesFromPathControl == null)
      {
        _getFilesFromPathControl = new DriveProxy.Test.Forms.TestControls.GetFilesFromPathControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_getFilesFromPathControl);
      }

      _getFilesFromPathControl.Visible = true;
    }

    private void radioButtonGetFileStatus_CheckedChanged_1(object sender, EventArgs e)
    {
      if (!radioButtonGetFileStatus.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonGetFileStatus);

      if (_getFileStatusControl == null)
      {
        _getFileStatusControl = new DriveProxy.Test.Forms.TestControls.GetFileStatusControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_getFileStatusControl);
      }

      _getFileStatusControl.Visible = true;
    }

    private void radioButtonGetLog_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonGetLog.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonGetLog);

      if (_getLogControl == null)
      {
        _getLogControl = new DriveProxy.Test.Forms.TestControls.GetLogControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_getLogControl);
      }

      _getLogControl.Visible = true;
    }

    private void radioButtonInsertFile_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonInsertFile.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonInsertFile);

      if (_insertFileControl == null)
      {
        _insertFileControl = new DriveProxy.Test.Forms.TestControls.InsertFileControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_insertFileControl);
      }

      _insertFileControl.Visible = true;
    }

    private void radioButtonIsSignedIn_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonIsSignedIn.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonIsSignedIn);

      if (_isSignedInControl == null)
      {
        _isSignedInControl = new DriveProxy.Test.Forms.TestControls.IsSignedInControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_isSignedInControl);
      }

      _isSignedInControl.Visible = true;
    }

    private void radioButtonMoveFiles_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonMoveFiles.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonMoveFiles);

      if (_moveFilesControl == null)
      {
        _moveFilesControl = new DriveProxy.Test.Forms.TestControls.MoveFilesControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_moveFilesControl);
      }

      _moveFilesControl.Visible = true;
    }

    private void radioButtonProcessing_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonProcessing.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonProcessing);

      if (_processingControl == null)
      {
        _processingControl = new DriveProxy.Test.Forms.TestControls.ProcessingControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_processingControl);
      }

      _processingControl.Visible = true;
    }

    private void radioButtonRenameFile_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonRenameFile.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonRenameFile);

      if (_renameFileControl == null)
      {
        _renameFileControl = new DriveProxy.Test.Forms.TestControls.RenameFileControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_renameFileControl);
      }

      _renameFileControl.Visible = true;
    }

    private void radioButtonSetLog_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonSetLog.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonSetLog);

      if (_setLogControl == null)
      {
        _setLogControl = new DriveProxy.Test.Forms.TestControls.SetLogControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_setLogControl);
      }

      _setLogControl.Visible = true;

      ILog log = _driveService.GetLog();

      _setLogControl.LogLevel = log.Level;
      _setLogControl.Source = log.Source;
      _setLogControl.FilePath = log.FilePath;
    }

    private void radioButtonSignout_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonSignout.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonSignout);

      if (_signoutControl == null)
      {
        _signoutControl = new DriveProxy.Test.Forms.TestControls.SignoutControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_signoutControl);
      }

      _signoutControl.Visible = true;
    }

    private void radioButtonStartBackgroundProcesses_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonStartBackgroundProcesses.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonStartBackgroundProcesses);

      if (_startBackgroundProcessesControl == null)
      {
        _startBackgroundProcessesControl = new DriveProxy.Test.Forms.TestControls.StartBackgroundProcessesControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_startBackgroundProcessesControl);
      }

      _startBackgroundProcessesControl.Visible = true;
    }

    private void radioButtonStartedBackgroundProcesses_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonStartedBackgroundProcesses.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonStartedBackgroundProcesses);

      if (_startedBackgroundProcessesControl == null)
      {
        _startedBackgroundProcessesControl = new DriveProxy.Test.Forms.TestControls.StartedBackgroundProcessesControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_startedBackgroundProcessesControl);
      }

      _startedBackgroundProcessesControl.Visible = true;
    }

    private void radioButtonStartingBackgroundProcesses_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonStartingBackgroundProcesses.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonStartingBackgroundProcesses);

      if (_startingBackgroundProcessesControl == null)
      {
        _startingBackgroundProcessesControl = new DriveProxy.Test.Forms.TestControls.StartingBackgroundProcessesControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_startingBackgroundProcessesControl);
      }

      _startingBackgroundProcessesControl.Visible = true;
    }

    private void radioButtonStopBackgroundProcesses_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonStopBackgroundProcesses.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonStopBackgroundProcesses);

      if (_stopBackgroundProcessesControl == null)
      {
        _stopBackgroundProcessesControl = new DriveProxy.Test.Forms.TestControls.StopBackgroundProcessesControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_stopBackgroundProcessesControl);
      }

      _stopBackgroundProcessesControl.Visible = true;
    }

    private void radioButtonStoppingBackgroundProcesses_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonStoppingBackgroundProcesses.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonStoppingBackgroundProcesses);

      if (_stoppingBackgroundProcessesControl == null)
      {
        _stoppingBackgroundProcessesControl = new DriveProxy.Test.Forms.TestControls.StoppingBackgroundProcessesControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_stoppingBackgroundProcessesControl);
      }

      _stoppingBackgroundProcessesControl.Visible = true;
    }

    private void radioButtonTrashFile_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonTrashFile.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonTrashFile);

      if (_trashFileControl == null)
      {
        _trashFileControl = new DriveProxy.Test.Forms.TestControls.TrashFileControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_trashFileControl);
      }

      _trashFileControl.Visible = true;
    }

    private void radioButtonUntrashFile_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonUntrashFile.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonUntrashFile);

      if (_untrashFileControl == null)
      {
        _untrashFileControl = new DriveProxy.Test.Forms.TestControls.UntrashFileControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_untrashFileControl);
      }

      _untrashFileControl.Visible = true;
    }

    private void radioButtonUploadFile_CheckedChanged(object sender, EventArgs e)
    {
      if (!radioButtonUploadFile.Checked)
      {
        return;
      }

      radioButtonGroup_HideControls(radioButtonUploadFile);

      if (_uploadFileControl == null)
      {
        _uploadFileControl = new DriveProxy.Test.Forms.TestControls.UploadFileControl
        {
          Width = panelParameters.Width - 25,
          Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panelParameters.Controls.Add(_uploadFileControl);
      }

      _uploadFileControl.Visible = true;
    }

    private void buttonCancel_Click(object sender, EventArgs e)
    {
      _isCancelling = true;
    }

    private void buttonExecute_Click(object sender, EventArgs e)
    {
      try
      {
        UseWaitCursor = true;
        Cursor = Cursors.WaitCursor;

        Application.DoEvents();

        if (radioButtonAuthenticate.Checked)
        {
          button_Authenticate();
        }
        else if (radioButtonCleanupFile.Checked)
        {
          button_CleanupFile();
        }
        else if (radioButtonCopyFiles.Checked)
        {
          button_CopyFiles();
        }
        else if (radioButtonDownloadFile.Checked)
        {
          button_DownloadFile();
        }
        else if (radioButtonFolderPath.Checked)
        {
          button_FolderPath();
        }
        else if (radioButtonGetAbout.Checked)
        {
          button_GetAbout();
        }
        else if (radioButtonGetFile.Checked)
        {
          button_GetFile();
        }
        else if (radioButtonGetFilesFromPath.Checked)
        {
          button_GetFilesFromPath();
        }
        else if (radioButtonGetFileStatus.Checked)
        {
          button_GetFileStatus();
        }
        else if (radioButtonGetLog.Checked)
        {
          button_GetLog();
        }
        else if (radioButtonInsertFile.Checked)
        {
          button_InsertFile();
        }
        else if (radioButtonIsSignedIn.Checked)
        {
          button_IsSignedIn();
        }
        else if (radioButtonMoveFiles.Checked)
        {
          button_MoveFiles();
        }
        else if (radioButtonProcessing.Checked)
        {
          button_Processing();
        }
        else if (radioButtonRenameFile.Checked)
        {
          button_RenameFile();
        }
        else if (radioButtonSetLog.Checked)
        {
          button_SetLog();
        }
        else if (radioButtonSignout.Checked)
        {
          button_Signout();
        }
        else if (radioButtonStartBackgroundProcesses.Checked)
        {
          button_StartBackgroundProcesses();
        }
        else if (radioButtonStartedBackgroundProcesses.Checked)
        {
          button_StartedBackgroundProcesses();
        }
        else if (radioButtonStartingBackgroundProcesses.Checked)
        {
          button_StartingBackgroundProcesses();
        }
        else if (radioButtonStopBackgroundProcesses.Checked)
        {
          button_StopBackgroundProcesses();
        }
        else if (radioButtonStoppingBackgroundProcesses.Checked)
        {
          button_StoppingBackgroundProcesses();
        }
        else if (radioButtonTrashFile.Checked)
        {
          button_TrashFile();
        }
        else if (radioButtonUntrashFile.Checked)
        {
          button_UntrashFile();
        }
        else if (radioButtonUploadFile.Checked)
        {
          button_UploadFile();
        }
      }
      catch (Exception exception)
      {
        MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      finally
      {
        UseWaitCursor = false;
        Cursor = Cursors.Default;

        Application.DoEvents();
      }
    }

    private void button_Authenticate()
    {
      try
      {
        listView_AddItem("Authenticate", "Invoking...");

        bool startBackgroundProcesses = _authenticateControl.StartBackgroundProcesses;
        bool openInExplorer = _authenticateControl.OpenInExplorer;

        _driveService.Authenticate(startBackgroundProcesses, openInExplorer);

        listView_AddItem("Authenticate", "Success");
      }
      catch (Exception exception)
      {
        listView_AddException("Authenticate", exception);
      }
    }

    private void button_CleanupFile()
    {
      try
      {
        listView_AddItem("CleanupFile", "Invoking...");

        string fileId = _cleanupFileControl.FileId;
        bool ignoreFileOnDiskTimeout = _cleanupFileControl.IgnoreFileOnDiskTimeout;

        _driveService.CleanupFile(fileId, ignoreFileOnDiskTimeout);

        listView_AddItem("CleanupFile", "Success");
      }
      catch (Exception exception)
      {
        listView_AddException("CleanupFile", exception);
      }
    }

    private void button_CopyFiles()
    {
      try
      {
        listView_AddItem("CopyFile", "Invoking...");

        string parentId = _copyFilesControl.ParentId;
        string[] fileIds = _copyFilesControl.FileIds.Split(',');

        for (int i = 0; i < fileIds.Length; i++)
        {
          fileIds[i] = fileIds[i].Trim();

          IStream stream = DriveServiceFactory.CreateStream();

          stream.OnProgressStarted += stream_OnProgressStarted;
          stream.OnProgressChanged += stream_OnProgressChanged;
          stream.OnProgressFinished += stream_OnProgressFinished;

          stream.Attributes.Add("MethodName", "CopyFile");

          _driveService.CopyFile(stream, parentId, fileIds[i]);
        }
      }
      catch (Exception exception)
      {
        listView_AddException("CopyFile", exception);
      }
    }

    private void button_DownloadFile()
    {
      try
      {
        listView_AddItem("DownloadFile", "Invoking...");

        string fileId = _downloadFileControl.FileId;
        int chunkSize = _downloadFileControl.ChunkSize;
        bool checkIfAlreadyDownloaded = _downloadFileControl.CheckIfAlreadyDownloaded;

        IStream stream = DriveServiceFactory.CreateStream();

        stream.OnProgressStarted += stream_OnProgressStarted;
        stream.OnProgressChanged += stream_OnProgressChanged;
        stream.OnProgressFinished += stream_OnProgressFinished;

        stream.Attributes.Add("MethodName", "DownloadFile");

        _driveService.DownloadFile(stream, fileId, chunkSize, checkIfAlreadyDownloaded);
      }
      catch (Exception exception)
      {
        listView_AddException("DownloadFile", exception);
      }
    }

    private void button_FolderPath()
    {
      try
      {
        listView_AddItem("FolderPath", "Invoking...");

        string result = _driveService.FolderPath;

        listView_AddItem("FolderPath", result);
      }
      catch (Exception exception)
      {
        listView_AddException("FolderPath", exception);
      }
    }

    private void button_GetAbout()
    {
      try
      {
        listView_AddItem("GetAbout", "Invoking...");

        IAbout result = _driveService.GetAbout();

        listView_AddItem("GetAbout.Name", result.Name);
        listView_AddItem("GetAbout.QuotaBytesTotal", result.QuotaBytesTotal.ToString());
        listView_AddItem("GetAbout.QuotaBytesUsed", result.QuotaBytesUsed.ToString());
        listView_AddItem("GetAbout.QuotaBytesUsedAggregate", result.QuotaBytesUsedAggregate.ToString());
        listView_AddItem("GetAbout.QuotaBytesUsedInTrash", result.QuotaBytesUsedInTrash.ToString());
        listView_AddItem("GetAbout.UserDisplayName", result.UserDisplayName);
      }
      catch (Exception exception)
      {
        listView_AddException("GetAbout", exception);
      }
    }

    private void button_GetFile()
    {
      try
      {
        listView_AddItem("GetFile", "Invoking...");

        string fileId = _getFileControl.FileId;
        bool getChildren = _getFileControl.GetChildren;
        bool useCachedData = _getFileControl.UseCachedData;
        bool updateCachedData = _getFileControl.UpdateCachedData;

        if (useCachedData && updateCachedData)
        {
          _driveService.UpdateCachedData();
        }

        IFile file = null;

        file = useCachedData
          ? _driveService.GetCachedFile(fileId, getChildren)
          : _driveService.GetFile(fileId, getChildren);

        listView_AddFile("GetFile", file);
      }
      catch (Exception exception)
      {
        listView_AddException("GetFile", exception);
      }
    }

    private void button_GetFilesFromPath()
    {
      try
      {
        listView_AddItem("GetFilesFromPath", "Invoking...");

        string path = _getFilesFromPathControl.Path;

        IEnumerable<IFile> files = _driveService.GetFilesFromPath(path);

        listView_AddFiles("GetFilesFromPath", files);
      }
      catch (Exception exception)
      {
        listView_AddException("GetFilesFromPath", exception);
      }
    }

    private void button_GetFileStatus()
    {
      try
      {
        listView_AddItem("GetFileStatus", "Invoking...");

        string fileId = _getFileStatusControl.FileId;

        FileInfoStatus result = _driveService.GetFileStatus(fileId);

        listView_AddItem("GetFileStatus", result.ToString(), fileId);
      }
      catch (Exception exception)
      {
        listView_AddException("GetFileStatus", exception);
      }
    }

    private void button_GetLog()
    {
      try
      {
        listView_AddItem("GetLog", "Invoking...");

        ILog result = _driveService.GetLog();

        listView_AddItem("GetLog.FilePath", result.FilePath);
        listView_AddItem("GetLog.LastError", result.LastError);
        listView_AddItem("GetLog.LastErrorTime", result.LastErrorTime.ToString());
        listView_AddItem("GetLog.Level", result.Level.ToString());
        listView_AddItem("GetLog.Source", result.Source);
      }
      catch (Exception exception)
      {
        listView_AddException("GetLog", exception);
      }
    }

    private void button_InsertFile()
    {
      try
      {
        listView_AddItem("InsertFile", "Invoking...");

        string parentId = _insertFileControl.ParentId;
        string title = _insertFileControl.Title;
        bool isFolder = _insertFileControl.IsFolder;

        IStream stream = DriveServiceFactory.CreateStream();

        stream.OnProgressStarted += stream_OnProgressStarted;
        stream.OnProgressChanged += stream_OnProgressChanged;
        stream.OnProgressFinished += stream_OnProgressFinished;

        stream.Attributes.Add("MethodName", "InsertFile");

        _driveService.InsertFile(stream, parentId, title, isFolder);
      }
      catch (Exception exception)
      {
        listView_AddException("InsertFile", exception);
      }
    }

    private void button_IsSignedIn()
    {
      try
      {
        listView_AddItem("IsSignedIn", "Invoking...");

        bool result = _driveService.IsSignedIn;

        listView_AddItem("IsSignedIn", result.ToString());
      }
      catch (Exception exception)
      {
        listView_AddException("IsSignedIn", exception);
      }
    }

    private void button_MoveFiles()
    {
      try
      {
        listView_AddItem("MoveFile", "Invoking...");

        string parentId = _moveFilesControl.ParentId;
        string[] fileIds = _moveFilesControl.FileIds.Split(',');

        for (int i = 0; i < fileIds.Length; i++)
        {
          fileIds[i] = fileIds[i].Trim();

          IStream stream = DriveServiceFactory.CreateStream();

          stream.OnProgressStarted += stream_OnProgressStarted;
          stream.OnProgressChanged += stream_OnProgressChanged;
          stream.OnProgressFinished += stream_OnProgressFinished;

          stream.Attributes.Add("MethodName", "MoveFile");

          _driveService.MoveFile(stream, parentId, fileIds[i]);
        }
      }
      catch (Exception exception)
      {
        listView_AddException("MoveFile", exception);
      }
    }

    private void button_Processing()
    {
      try
      {
        listView_AddItem("Processing", "Invoking...");

        bool result = _driveService.Processing;

        listView_AddItem("Processing", result.ToString());
      }
      catch (Exception exception)
      {
        listView_AddException("Processing", exception);
      }
    }

    private void button_RenameFile()
    {
      try
      {
        listView_AddItem("RenameFile", "Invoking...");

        string fileId = _renameFileControl.FileId;
        string title = _renameFileControl.Title;

        IStream stream = DriveServiceFactory.CreateStream();

        stream.OnProgressStarted += stream_OnProgressStarted;
        stream.OnProgressChanged += stream_OnProgressChanged;
        stream.OnProgressFinished += stream_OnProgressFinished;

        stream.Attributes.Add("MethodName", "RenameFile");

        _driveService.RenameFile(stream, fileId, title);
      }
      catch (Exception exception)
      {
        listView_AddException("RenameFile", exception);
      }
    }

    private void button_SetLog()
    {
      try
      {
        listView_AddItem("SetLog", "Invoking...");

        LogType logType = _setLogControl.LogLevel;
        string source = _setLogControl.Source;
        string filePath = _setLogControl.FilePath;

        ILog log = DriveServiceFactory.CreateLog();

        log.Level = logType;
        log.Source = source;
        log.FilePath = filePath;

        _driveService.SetLog(log);

        listView_AddItem("SetLog", "Success");
      }
      catch (Exception exception)
      {
        listView_AddException("SetLog", exception);
      }
    }

    private void button_Signout()
    {
      try
      {
        listView_AddItem("Signout", "Invoking...");

        bool authenticate = _signoutControl.Authenticate;
        bool startBackgroundProcesses = _signoutControl.StartBackgroundProcesses;
        bool openInExplorer = _signoutControl.OpenInExplorer;

        if (authenticate)
        {
          _driveService.SignoutAndAuthenticate(startBackgroundProcesses, openInExplorer);
        }
        else
        {
          _driveService.Signout();
        }

        listView_AddItem("Signout", "Success");
      }
      catch (Exception exception)
      {
        listView_AddException("Signout", exception);
      }
    }

    private void button_StartBackgroundProcesses()
    {
      try
      {
        listView_AddItem("StartBackgroundProcesses", "Invoking...");

        _driveService.StartBackgroundProcesses();

        listView_AddItem("StartBackgroundProcesses", "Success");
      }
      catch (Exception exception)
      {
        listView_AddException("StartBackgroundProcesses", exception);
      }
    }

    private void button_StartedBackgroundProcesses()
    {
      try
      {
        listView_AddItem("StartedBackgroundProcesses", "Invoking...");

        bool result = _driveService.StartedBackgroundProcesses;

        listView_AddItem("StartedBackgroundProcesses", result.ToString());
      }
      catch (Exception exception)
      {
        listView_AddException("StartedBackgroundProcesses", exception);
      }
    }

    private void button_StartingBackgroundProcesses()
    {
      try
      {
        listView_AddItem("StartingBackgroundProcesses", "Invoking...");

        bool result = _driveService.StartingBackgroundProcesses;

        listView_AddItem("StartingBackgroundProcesses", result.ToString());
      }
      catch (Exception exception)
      {
        listView_AddException("StartingBackgroundProcesses", exception);
      }
    }

    private void button_StopBackgroundProcesses()
    {
      try
      {
        listView_AddItem("StopBackgroundProcesses", "Invoking...");

        bool waitWhileProcessing = _stopBackgroundProcessesControl.WaitWhileProcessing;

        _driveService.StopBackgroundProcesses(waitWhileProcessing);

        listView_AddItem("StopBackgroundProcesses", "Success");
      }
      catch (Exception exception)
      {
        listView_AddException("StopBackgroundProcesses", exception);
      }
    }

    private void button_StoppingBackgroundProcesses()
    {
      try
      {
        listView_AddItem("StoppingBackgroundProcesses", "Invoking...");

        bool result = _driveService.StoppingBackgroundProcesses;

        listView_AddItem("StoppingBackgroundProcesses", result.ToString());
      }
      catch (Exception exception)
      {
        listView_AddException("StoppingBackgroundProcesses", exception);
      }
    }

    private void button_TrashFile()
    {
      try
      {
        listView_AddItem("TrashFile", "Invoking...");

        string fileId = _trashFileControl.FileId;

        IStream stream = DriveServiceFactory.CreateStream();

        stream.OnProgressStarted += stream_OnProgressStarted;
        stream.OnProgressChanged += stream_OnProgressChanged;
        stream.OnProgressFinished += stream_OnProgressFinished;

        stream.Attributes.Add("MethodName", "TrashFile");

        _driveService.TrashFile(stream, fileId);
      }
      catch (Exception exception)
      {
        listView_AddException("TrashFile", exception);
      }
    }

    private void button_UntrashFile()
    {
      try
      {
        listView_AddItem("UntrashFile", "Invoking...");

        string fileId = _untrashFileControl.FileId;

        IStream stream = DriveServiceFactory.CreateStream();

        stream.OnProgressStarted += stream_OnProgressStarted;
        stream.OnProgressChanged += stream_OnProgressChanged;
        stream.OnProgressFinished += stream_OnProgressFinished;

        stream.Attributes.Add("MethodName", "UntrashFile");

        _driveService.UntrashFile(stream, fileId);
      }
      catch (Exception exception)
      {
        listView_AddException("UntrashFile", exception);
      }
    }

    private void button_UploadFile()
    {
      try
      {
        listView_AddItem("UploadFile", "Invoking...");

        string fileId = _uploadFileControl.FileId;
        int chunkSize = _uploadFileControl.ChunkSize;
        bool checkIfAlreadyUploaded = _uploadFileControl.CheckIfAlreadyUploaded;

        IStream stream = DriveServiceFactory.CreateStream();

        stream.OnProgressStarted += stream_OnProgressStarted;
        stream.OnProgressChanged += stream_OnProgressChanged;
        stream.OnProgressFinished += stream_OnProgressFinished;

        stream.Attributes.Add("MethodName", "UploadFile");

        _driveService.UploadFile(stream, fileId, chunkSize, checkIfAlreadyUploaded);
      }
      catch (Exception exception)
      {
        listView_AddException("UploadFile", exception);
      }
    }

    private void stream_OnProgressStarted(object sender, IStreamEventArgs e)
    {
      if (InvokeRequired)
      {
        BeginInvoke(new MethodInvoker(() => { stream_OnProgressStarted(sender, e); }));
        return;
      }

      try
      {
        progressBar.Value = 0;

        string methodName = e.Stream.Attributes["MethodName"];

        listView_AddItem(methodName, "Started", e.Stream.FileId);
      }
      catch (Exception exception)
      {
        MessageBox.Show(this, "Exception - " + exception.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void stream_OnProgressChanged(object sender, IStreamEventArgs e)
    {
      if (InvokeRequired)
      {
        BeginInvoke(new MethodInvoker(() => { stream_OnProgressChanged(sender, e); }));
        return;
      }

      try
      {
        progressBar.Value = e.Stream.PercentCompleted;

        string methodName = e.Stream.Attributes["MethodName"];

        listView_AddItem(methodName, "Processing " + e.Stream.PercentCompleted + "...", e.Stream.FileId);

        if (_isCancelling)
        {
          e.Stream.Cancel();

          _isCancelling = false;
        }
      }
      catch (Exception exception)
      {
        MessageBox.Show(this, "Exception - " + exception.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void stream_OnProgressFinished(object sender, IStreamEventArgs e)
    {
      if (InvokeRequired)
      {
        BeginInvoke(new MethodInvoker(() => { stream_OnProgressFinished(sender, e); }));
        return;
      }

      try
      {
        progressBar.Value = 0;

        string methodName = e.Stream.Attributes["MethodName"];

        if (e.Stream.Completed)
        {
          listView_AddItem(methodName, "Completed", e.Stream.FileId);
        }
        else if (e.Stream.Cancelled)
        {
          listView_AddItem(methodName, "Cancelled", e.Stream.FileId);
        }
        else if (e.Stream.Failed)
        {
          listView_AddItem(methodName, "Failed - " + e.Stream.ExceptionMessage, e.Stream.FileId);
        }
      }
      catch (Exception exception)
      {
        MessageBox.Show(this, "Exception - " + exception.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void listView_AddItem(string methodName, string info, string fileId = "")
    {
      listView.Sorting = SortOrder.None;
      ListViewItemSorter.SortColumn = -1;

      listView.Items.Insert(0,
                            new ListViewItem(new[]
                            {
                              DateTime.Now.ToString(),
                              methodName,
                              info,
                              fileId
                            }));
    }

    private void listView_AddException(string methodName, Exception exception)
    {
      listView_AddItem(methodName, "Exception - " + exception.Message);
    }

    private void listView_AddFile(string methodName, IFile file, string parentTitle = "")
    {
      try
      {
        string info = file.Title;

        if (!String.IsNullOrEmpty(parentTitle))
        {
          info = parentTitle + "\\" + info;
        }

        listView_AddItem(methodName, info, file.Id);

        if (file.Children != null)
        {
          listView_AddFiles(methodName, file.Children, file.Title);
        }
      }
      catch (Exception exception)
      {
        throw exception;
      }
    }

    private void listView_AddFiles(string methodName, IEnumerable<IFile> files, string parentTitle = "")
    {
      foreach (IFile file in files)
      {
        listView_AddFile(methodName, file, parentTitle);
      }
    }

    private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
    {
      try
      {
        ListViewItemSorter.Sort(listView, e);
      }
      catch (Exception exception)
      {
        MessageBox.Show(this, "Exception - " + exception.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void listView_ItemActivate(object sender, EventArgs e)
    {
      try
      {
        string title = "";
        string fileId = "";

        if (listView.SelectedItems.Count > 0 && listView.SelectedItems[0].SubItems.Count >= 3)
        {
          title = listView.SelectedItems[0].SubItems[2].Text;
        }

        if (!String.IsNullOrEmpty(title))
        {
          if (title.IndexOf("Exception - ") == 0)
          {
            MessageBox.Show(title);
          }
        }

        if (listView.SelectedItems.Count > 0 && listView.SelectedItems[0].SubItems.Count >= 4)
        {
          fileId = listView.SelectedItems[0].SubItems[3].Text;
        }

        if (String.IsNullOrEmpty(fileId))
        {
          return;
        }

        if (radioButtonAuthenticate.Checked)
        {
        }
        else if (radioButtonCleanupFile.Checked)
        {
          _cleanupFileControl.FileId = fileId;
        }
        else if (radioButtonCopyFiles.Checked)
        {
          _copyFilesControl.ParentId = fileId;
        }
        else if (radioButtonDownloadFile.Checked)
        {
          _downloadFileControl.FileId = fileId;
        }
        else if (radioButtonFolderPath.Checked)
        {
        }
        else if (radioButtonGetAbout.Checked)
        {
        }
        else if (radioButtonGetFile.Checked)
        {
          _getFileControl.FileId = fileId;
        }
        else if (radioButtonGetFilesFromPath.Checked)
        {
          _getFilesFromPathControl.Path = title;
        }
        else if (radioButtonGetFileStatus.Checked)
        {
          _getFileStatusControl.FileId = fileId;
        }
        else if (radioButtonGetLog.Checked)
        {
        }
        else if (radioButtonInsertFile.Checked)
        {
          _insertFileControl.ParentId = fileId;
        }
        else if (radioButtonIsSignedIn.Checked)
        {
        }
        else if (radioButtonMoveFiles.Checked)
        {
          _moveFilesControl.ParentId = fileId;
        }
        else if (radioButtonProcessing.Checked)
        {
        }
        else if (radioButtonRenameFile.Checked)
        {
          _renameFileControl.FileId = fileId;
        }
        else if (radioButtonSignout.Checked)
        {
        }
        else if (radioButtonStartBackgroundProcesses.Checked)
        {
        }
        else if (radioButtonStartingBackgroundProcesses.Checked)
        {
        }
        else if (radioButtonStartedBackgroundProcesses.Checked)
        {
        }
        else if (radioButtonStopBackgroundProcesses.Checked)
        {
        }
        else if (radioButtonStoppingBackgroundProcesses.Checked)
        {
        }
        else if (radioButtonTrashFile.Checked)
        {
          _trashFileControl.FileId = fileId;
        }
        else if (radioButtonUntrashFile.Checked)
        {
          _untrashFileControl.FileId = fileId;
        }
        else if (radioButtonUploadFile.Checked)
        {
          _uploadFileControl.FileId = fileId;
        }
      }
      catch (Exception exception)
      {
        MessageBox.Show(this, "Exception - " + exception.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private class ListViewItemSorter : System.Collections.IComparer
    {
      public static int SortColumn = -1;

      public int Compare(object x, object y)
      {
        var itemX = x as ListViewItem;
        var itemY = y as ListViewItem;

        if (itemX == null && itemY == null)
        {
          return 0;
        }
        if (itemX == null)
        {
          return -1;
        }
        if (itemY == null)
        {
          return 1;
        }

        string textX = itemX.SubItems[SortColumn].Text;
        string textY = itemY.SubItems[SortColumn].Text;

        int result = String.Compare(textX, textY);

        return result;
      }

      public static void Sort(ListView listView, ColumnClickEventArgs e)
      {
        if (e.Column != SortColumn || listView.Sorting == SortOrder.Descending)
        {
          listView.Sorting = SortOrder.Ascending;
        }
        else
        {
          listView.Sorting = SortOrder.Descending;
        }

        SortColumn = e.Column;

        listView.ListViewItemSorter = new ListViewItemSorter();
      }
    }
  }
}
