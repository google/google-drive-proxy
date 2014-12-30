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
using DriveProxy.API;

namespace DriveProxy.Test
{
    partial class TestForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestForm));
            this.groupBoxMethods = new System.Windows.Forms.GroupBox();
            this.panelMethods = new System.Windows.Forms.Panel();
            this.radioButtonUntrashFile = new System.Windows.Forms.RadioButton();
            this.radioButtonTrashFile = new System.Windows.Forms.RadioButton();
            this.radioButtonSetLog = new System.Windows.Forms.RadioButton();
            this.radioButtonCleanupFile = new System.Windows.Forms.RadioButton();
            this.radioButtonGetFileStatus = new System.Windows.Forms.RadioButton();
            this.radioButtonGetLog = new System.Windows.Forms.RadioButton();
            this.radioButtonStartBackgroundProcesses = new System.Windows.Forms.RadioButton();
            this.radioButtonGetAbout = new System.Windows.Forms.RadioButton();
            this.radioButtonAuthenticate = new System.Windows.Forms.RadioButton();
            this.radioButtonStoppingBackgroundProcesses = new System.Windows.Forms.RadioButton();
            this.radioButtonInsertFile = new System.Windows.Forms.RadioButton();
            this.radioButtonGetFile = new System.Windows.Forms.RadioButton();
            this.radioButtonStartingBackgroundProcesses = new System.Windows.Forms.RadioButton();
            this.radioButtonDownloadFile = new System.Windows.Forms.RadioButton();
            this.radioButtonStartedBackgroundProcesses = new System.Windows.Forms.RadioButton();
            this.radioButtonGetFilesFromPath = new System.Windows.Forms.RadioButton();
            this.radioButtonStopBackgroundProcesses = new System.Windows.Forms.RadioButton();
            this.radioButtonUploadFile = new System.Windows.Forms.RadioButton();
            this.radioButtonProcessing = new System.Windows.Forms.RadioButton();
            this.radioButtonRenameFile = new System.Windows.Forms.RadioButton();
            this.radioButtonFolderPath = new System.Windows.Forms.RadioButton();
            this.radioButtonMoveFiles = new System.Windows.Forms.RadioButton();
            this.radioButtonIsSignedIn = new System.Windows.Forms.RadioButton();
            this.radioButtonSignout = new System.Windows.Forms.RadioButton();
            this.panelParameters = new System.Windows.Forms.Panel();
            this.groupBoxParameters = new System.Windows.Forms.GroupBox();
            this.groupBoxResult = new System.Windows.Forms.GroupBox();
            this.listView = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonExecute = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.buttonCancel = new System.Windows.Forms.Button();
            this.radioButtonCopyFiles = new System.Windows.Forms.RadioButton();
            this.groupBoxMethods.SuspendLayout();
            this.panelMethods.SuspendLayout();
            this.groupBoxParameters.SuspendLayout();
            this.groupBoxResult.SuspendLayout();
            this.SuspendLayout();
            //
            // groupBoxMethods
            //
            this.groupBoxMethods.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxMethods.Controls.Add(this.panelMethods);
            this.groupBoxMethods.Location = new System.Drawing.Point(12, 12);
            this.groupBoxMethods.Name = "groupBoxMethods";
            this.groupBoxMethods.Size = new System.Drawing.Size(671, 222);
            this.groupBoxMethods.TabIndex = 0;
            this.groupBoxMethods.TabStop = false;
            this.groupBoxMethods.Text = "Method";
            //
            // panelMethods
            //
            this.panelMethods.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMethods.AutoScroll = true;
            this.panelMethods.Controls.Add(this.radioButtonCopyFiles);
            this.panelMethods.Controls.Add(this.radioButtonUntrashFile);
            this.panelMethods.Controls.Add(this.radioButtonTrashFile);
            this.panelMethods.Controls.Add(this.radioButtonSetLog);
            this.panelMethods.Controls.Add(this.radioButtonCleanupFile);
            this.panelMethods.Controls.Add(this.radioButtonGetFileStatus);
            this.panelMethods.Controls.Add(this.radioButtonGetLog);
            this.panelMethods.Controls.Add(this.radioButtonStartBackgroundProcesses);
            this.panelMethods.Controls.Add(this.radioButtonGetAbout);
            this.panelMethods.Controls.Add(this.radioButtonAuthenticate);
            this.panelMethods.Controls.Add(this.radioButtonStoppingBackgroundProcesses);
            this.panelMethods.Controls.Add(this.radioButtonInsertFile);
            this.panelMethods.Controls.Add(this.radioButtonGetFile);
            this.panelMethods.Controls.Add(this.radioButtonStartingBackgroundProcesses);
            this.panelMethods.Controls.Add(this.radioButtonDownloadFile);
            this.panelMethods.Controls.Add(this.radioButtonStartedBackgroundProcesses);
            this.panelMethods.Controls.Add(this.radioButtonGetFilesFromPath);
            this.panelMethods.Controls.Add(this.radioButtonStopBackgroundProcesses);
            this.panelMethods.Controls.Add(this.radioButtonUploadFile);
            this.panelMethods.Controls.Add(this.radioButtonProcessing);
            this.panelMethods.Controls.Add(this.radioButtonRenameFile);
            this.panelMethods.Controls.Add(this.radioButtonFolderPath);
            this.panelMethods.Controls.Add(this.radioButtonMoveFiles);
            this.panelMethods.Controls.Add(this.radioButtonIsSignedIn);
            this.panelMethods.Controls.Add(this.radioButtonSignout);
            this.panelMethods.Location = new System.Drawing.Point(6, 19);
            this.panelMethods.Name = "panelMethods";
            this.panelMethods.Size = new System.Drawing.Size(659, 197);
            this.panelMethods.TabIndex = 0;
            //
            // radioButtonUntrashFile
            //
            this.radioButtonUntrashFile.AutoSize = true;
            this.radioButtonUntrashFile.Location = new System.Drawing.Point(525, 106);
            this.radioButtonUntrashFile.Name = "radioButtonUntrashFile";
            this.radioButtonUntrashFile.Size = new System.Drawing.Size(86, 17);
            this.radioButtonUntrashFile.TabIndex = 20;
            this.radioButtonUntrashFile.TabStop = true;
            this.radioButtonUntrashFile.Text = "Untrash Files";
            this.radioButtonUntrashFile.UseVisualStyleBackColor = true;
            this.radioButtonUntrashFile.CheckedChanged += new System.EventHandler(this.radioButtonUntrashFile_CheckedChanged);
            //
            // radioButtonTrashFile
            //
            this.radioButtonTrashFile.AutoSize = true;
            this.radioButtonTrashFile.Location = new System.Drawing.Point(525, 83);
            this.radioButtonTrashFile.Name = "radioButtonTrashFile";
            this.radioButtonTrashFile.Size = new System.Drawing.Size(76, 17);
            this.radioButtonTrashFile.TabIndex = 19;
            this.radioButtonTrashFile.TabStop = true;
            this.radioButtonTrashFile.Text = "Trash Files";
            this.radioButtonTrashFile.UseVisualStyleBackColor = true;
            this.radioButtonTrashFile.CheckedChanged += new System.EventHandler(this.radioButtonTrashFile_CheckedChanged);
            //
            // radioButtonSetLog
            //
            this.radioButtonSetLog.AutoSize = true;
            this.radioButtonSetLog.Location = new System.Drawing.Point(11, 175);
            this.radioButtonSetLog.Name = "radioButtonSetLog";
            this.radioButtonSetLog.Size = new System.Drawing.Size(62, 17);
            this.radioButtonSetLog.TabIndex = 7;
            this.radioButtonSetLog.TabStop = true;
            this.radioButtonSetLog.Text = "Set Log";
            this.radioButtonSetLog.UseVisualStyleBackColor = true;
            this.radioButtonSetLog.CheckedChanged += new System.EventHandler(this.radioButtonSetLog_CheckedChanged);
            //
            // radioButtonCleanupFile
            //
            this.radioButtonCleanupFile.AutoSize = true;
            this.radioButtonCleanupFile.Location = new System.Drawing.Point(317, 152);
            this.radioButtonCleanupFile.Name = "radioButtonCleanupFile";
            this.radioButtonCleanupFile.Size = new System.Drawing.Size(83, 17);
            this.radioButtonCleanupFile.TabIndex = 18;
            this.radioButtonCleanupFile.TabStop = true;
            this.radioButtonCleanupFile.Text = "Cleanup File";
            this.radioButtonCleanupFile.UseVisualStyleBackColor = true;
            this.radioButtonCleanupFile.CheckedChanged += new System.EventHandler(this.radioButtonCleanupFile_CheckedChanged);
            //
            // radioButtonGetFileStatus
            //
            this.radioButtonGetFileStatus.AutoSize = true;
            this.radioButtonGetFileStatus.Location = new System.Drawing.Point(317, 60);
            this.radioButtonGetFileStatus.Name = "radioButtonGetFileStatus";
            this.radioButtonGetFileStatus.Size = new System.Drawing.Size(94, 17);
            this.radioButtonGetFileStatus.TabIndex = 14;
            this.radioButtonGetFileStatus.TabStop = true;
            this.radioButtonGetFileStatus.Text = "Get File Status";
            this.radioButtonGetFileStatus.UseVisualStyleBackColor = true;
            this.radioButtonGetFileStatus.CheckedChanged += new System.EventHandler(this.radioButtonGetFileStatus_CheckedChanged_1);
            //
            // radioButtonGetLog
            //
            this.radioButtonGetLog.AutoSize = true;
            this.radioButtonGetLog.Location = new System.Drawing.Point(11, 152);
            this.radioButtonGetLog.Name = "radioButtonGetLog";
            this.radioButtonGetLog.Size = new System.Drawing.Size(63, 17);
            this.radioButtonGetLog.TabIndex = 6;
            this.radioButtonGetLog.TabStop = true;
            this.radioButtonGetLog.Text = "Get Log";
            this.radioButtonGetLog.UseVisualStyleBackColor = true;
            this.radioButtonGetLog.CheckedChanged += new System.EventHandler(this.radioButtonGetLog_CheckedChanged);
            //
            // radioButtonStartBackgroundProcesses
            //
            this.radioButtonStartBackgroundProcesses.AutoSize = true;
            this.radioButtonStartBackgroundProcesses.Location = new System.Drawing.Point(116, 14);
            this.radioButtonStartBackgroundProcesses.Name = "radioButtonStartBackgroundProcesses";
            this.radioButtonStartBackgroundProcesses.Size = new System.Drawing.Size(160, 17);
            this.radioButtonStartBackgroundProcesses.TabIndex = 7;
            this.radioButtonStartBackgroundProcesses.TabStop = true;
            this.radioButtonStartBackgroundProcesses.Text = "Start Background Processes";
            this.radioButtonStartBackgroundProcesses.UseVisualStyleBackColor = true;
            this.radioButtonStartBackgroundProcesses.CheckedChanged += new System.EventHandler(this.radioButtonStartBackgroundProcesses_CheckedChanged);
            //
            // radioButtonGetAbout
            //
            this.radioButtonGetAbout.AutoSize = true;
            this.radioButtonGetAbout.Location = new System.Drawing.Point(11, 129);
            this.radioButtonGetAbout.Name = "radioButtonGetAbout";
            this.radioButtonGetAbout.Size = new System.Drawing.Size(73, 17);
            this.radioButtonGetAbout.TabIndex = 5;
            this.radioButtonGetAbout.TabStop = true;
            this.radioButtonGetAbout.Text = "Get About";
            this.radioButtonGetAbout.UseVisualStyleBackColor = true;
            this.radioButtonGetAbout.CheckedChanged += new System.EventHandler(this.radioButtonGetAbout_CheckedChanged);
            //
            // radioButtonAuthenticate
            //
            this.radioButtonAuthenticate.AutoSize = true;
            this.radioButtonAuthenticate.Location = new System.Drawing.Point(11, 14);
            this.radioButtonAuthenticate.Name = "radioButtonAuthenticate";
            this.radioButtonAuthenticate.Size = new System.Drawing.Size(85, 17);
            this.radioButtonAuthenticate.TabIndex = 0;
            this.radioButtonAuthenticate.TabStop = true;
            this.radioButtonAuthenticate.Text = "Authenticate";
            this.radioButtonAuthenticate.UseVisualStyleBackColor = true;
            this.radioButtonAuthenticate.CheckedChanged += new System.EventHandler(this.radioButtonAuthenticate_CheckedChanged);
            //
            // radioButtonStoppingBackgroundProcesses
            //
            this.radioButtonStoppingBackgroundProcesses.AutoSize = true;
            this.radioButtonStoppingBackgroundProcesses.Location = new System.Drawing.Point(116, 106);
            this.radioButtonStoppingBackgroundProcesses.Name = "radioButtonStoppingBackgroundProcesses";
            this.radioButtonStoppingBackgroundProcesses.Size = new System.Drawing.Size(180, 17);
            this.radioButtonStoppingBackgroundProcesses.TabIndex = 11;
            this.radioButtonStoppingBackgroundProcesses.TabStop = true;
            this.radioButtonStoppingBackgroundProcesses.Text = "Stopping Background Processes";
            this.radioButtonStoppingBackgroundProcesses.UseVisualStyleBackColor = true;
            this.radioButtonStoppingBackgroundProcesses.CheckedChanged += new System.EventHandler(this.radioButtonStoppingBackgroundProcesses_CheckedChanged);
            //
            // radioButtonInsertFile
            //
            this.radioButtonInsertFile.AutoSize = true;
            this.radioButtonInsertFile.Location = new System.Drawing.Point(317, 106);
            this.radioButtonInsertFile.Name = "radioButtonInsertFile";
            this.radioButtonInsertFile.Size = new System.Drawing.Size(70, 17);
            this.radioButtonInsertFile.TabIndex = 16;
            this.radioButtonInsertFile.TabStop = true;
            this.radioButtonInsertFile.Text = "Insert File";
            this.radioButtonInsertFile.UseVisualStyleBackColor = true;
            this.radioButtonInsertFile.CheckedChanged += new System.EventHandler(this.radioButtonInsertFile_CheckedChanged);
            //
            // radioButtonGetFile
            //
            this.radioButtonGetFile.AutoSize = true;
            this.radioButtonGetFile.Location = new System.Drawing.Point(317, 14);
            this.radioButtonGetFile.Name = "radioButtonGetFile";
            this.radioButtonGetFile.Size = new System.Drawing.Size(61, 17);
            this.radioButtonGetFile.TabIndex = 12;
            this.radioButtonGetFile.TabStop = true;
            this.radioButtonGetFile.Text = "Get File";
            this.radioButtonGetFile.UseVisualStyleBackColor = true;
            this.radioButtonGetFile.CheckedChanged += new System.EventHandler(this.radioButtonGetFiles_CheckedChanged);
            //
            // radioButtonStartingBackgroundProcesses
            //
            this.radioButtonStartingBackgroundProcesses.AutoSize = true;
            this.radioButtonStartingBackgroundProcesses.Location = new System.Drawing.Point(116, 83);
            this.radioButtonStartingBackgroundProcesses.Name = "radioButtonStartingBackgroundProcesses";
            this.radioButtonStartingBackgroundProcesses.Size = new System.Drawing.Size(174, 17);
            this.radioButtonStartingBackgroundProcesses.TabIndex = 10;
            this.radioButtonStartingBackgroundProcesses.TabStop = true;
            this.radioButtonStartingBackgroundProcesses.Text = "Starting Background Processes";
            this.radioButtonStartingBackgroundProcesses.UseVisualStyleBackColor = true;
            this.radioButtonStartingBackgroundProcesses.CheckedChanged += new System.EventHandler(this.radioButtonStartingBackgroundProcesses_CheckedChanged);
            //
            // radioButtonDownloadFile
            //
            this.radioButtonDownloadFile.AutoSize = true;
            this.radioButtonDownloadFile.Location = new System.Drawing.Point(317, 83);
            this.radioButtonDownloadFile.Name = "radioButtonDownloadFile";
            this.radioButtonDownloadFile.Size = new System.Drawing.Size(92, 17);
            this.radioButtonDownloadFile.TabIndex = 15;
            this.radioButtonDownloadFile.TabStop = true;
            this.radioButtonDownloadFile.Text = "Download File";
            this.radioButtonDownloadFile.UseVisualStyleBackColor = true;
            this.radioButtonDownloadFile.CheckedChanged += new System.EventHandler(this.radioButtonDownloadFile_CheckedChanged);
            //
            // radioButtonStartedBackgroundProcesses
            //
            this.radioButtonStartedBackgroundProcesses.AutoSize = true;
            this.radioButtonStartedBackgroundProcesses.Location = new System.Drawing.Point(116, 60);
            this.radioButtonStartedBackgroundProcesses.Name = "radioButtonStartedBackgroundProcesses";
            this.radioButtonStartedBackgroundProcesses.Size = new System.Drawing.Size(172, 17);
            this.radioButtonStartedBackgroundProcesses.TabIndex = 9;
            this.radioButtonStartedBackgroundProcesses.TabStop = true;
            this.radioButtonStartedBackgroundProcesses.Text = "Started Background Processes";
            this.radioButtonStartedBackgroundProcesses.UseVisualStyleBackColor = true;
            this.radioButtonStartedBackgroundProcesses.CheckedChanged += new System.EventHandler(this.radioButtonStartedBackgroundProcesses_CheckedChanged);
            //
            // radioButtonGetFilesFromPath
            //
            this.radioButtonGetFilesFromPath.AutoSize = true;
            this.radioButtonGetFilesFromPath.Location = new System.Drawing.Point(317, 37);
            this.radioButtonGetFilesFromPath.Name = "radioButtonGetFilesFromPath";
            this.radioButtonGetFilesFromPath.Size = new System.Drawing.Size(117, 17);
            this.radioButtonGetFilesFromPath.TabIndex = 13;
            this.radioButtonGetFilesFromPath.TabStop = true;
            this.radioButtonGetFilesFromPath.Text = "Get Files From Path";
            this.radioButtonGetFilesFromPath.UseVisualStyleBackColor = true;
            this.radioButtonGetFilesFromPath.CheckedChanged += new System.EventHandler(this.radioButtonGetFilesFromPathsFromPath_CheckedChanged);
            //
            // radioButtonStopBackgroundProcesses
            //
            this.radioButtonStopBackgroundProcesses.AutoSize = true;
            this.radioButtonStopBackgroundProcesses.Location = new System.Drawing.Point(116, 37);
            this.radioButtonStopBackgroundProcesses.Name = "radioButtonStopBackgroundProcesses";
            this.radioButtonStopBackgroundProcesses.Size = new System.Drawing.Size(160, 17);
            this.radioButtonStopBackgroundProcesses.TabIndex = 8;
            this.radioButtonStopBackgroundProcesses.TabStop = true;
            this.radioButtonStopBackgroundProcesses.Text = "Stop Background Processes";
            this.radioButtonStopBackgroundProcesses.UseVisualStyleBackColor = true;
            this.radioButtonStopBackgroundProcesses.CheckedChanged += new System.EventHandler(this.radioButtonStopBackgroundProcesses_CheckedChanged);
            //
            // radioButtonUploadFile
            //
            this.radioButtonUploadFile.AutoSize = true;
            this.radioButtonUploadFile.Location = new System.Drawing.Point(317, 129);
            this.radioButtonUploadFile.Name = "radioButtonUploadFile";
            this.radioButtonUploadFile.Size = new System.Drawing.Size(78, 17);
            this.radioButtonUploadFile.TabIndex = 17;
            this.radioButtonUploadFile.TabStop = true;
            this.radioButtonUploadFile.Text = "Upload File";
            this.radioButtonUploadFile.UseVisualStyleBackColor = true;
            this.radioButtonUploadFile.CheckedChanged += new System.EventHandler(this.radioButtonUploadFile_CheckedChanged);
            //
            // radioButtonProcessing
            //
            this.radioButtonProcessing.AutoSize = true;
            this.radioButtonProcessing.Location = new System.Drawing.Point(11, 106);
            this.radioButtonProcessing.Name = "radioButtonProcessing";
            this.radioButtonProcessing.Size = new System.Drawing.Size(77, 17);
            this.radioButtonProcessing.TabIndex = 4;
            this.radioButtonProcessing.TabStop = true;
            this.radioButtonProcessing.Text = "Processing";
            this.radioButtonProcessing.UseVisualStyleBackColor = true;
            this.radioButtonProcessing.CheckedChanged += new System.EventHandler(this.radioButtonProcessing_CheckedChanged);
            //
            // radioButtonRenameFile
            //
            this.radioButtonRenameFile.AutoSize = true;
            this.radioButtonRenameFile.Location = new System.Drawing.Point(525, 14);
            this.radioButtonRenameFile.Name = "radioButtonRenameFile";
            this.radioButtonRenameFile.Size = new System.Drawing.Size(84, 17);
            this.radioButtonRenameFile.TabIndex = 8;
            this.radioButtonRenameFile.TabStop = true;
            this.radioButtonRenameFile.Text = "Rename File";
            this.radioButtonRenameFile.UseVisualStyleBackColor = true;
            this.radioButtonRenameFile.CheckedChanged += new System.EventHandler(this.radioButtonRenameFile_CheckedChanged);
            //
            // radioButtonFolderPath
            //
            this.radioButtonFolderPath.AutoSize = true;
            this.radioButtonFolderPath.Location = new System.Drawing.Point(11, 83);
            this.radioButtonFolderPath.Name = "radioButtonFolderPath";
            this.radioButtonFolderPath.Size = new System.Drawing.Size(76, 17);
            this.radioButtonFolderPath.TabIndex = 3;
            this.radioButtonFolderPath.TabStop = true;
            this.radioButtonFolderPath.Text = "FolderPath";
            this.radioButtonFolderPath.UseVisualStyleBackColor = true;
            this.radioButtonFolderPath.CheckedChanged += new System.EventHandler(this.radioButtonFolderPath_CheckedChanged);
            //
            // radioButtonMoveFiles
            //
            this.radioButtonMoveFiles.AutoSize = true;
            this.radioButtonMoveFiles.Location = new System.Drawing.Point(525, 37);
            this.radioButtonMoveFiles.Name = "radioButtonMoveFiles";
            this.radioButtonMoveFiles.Size = new System.Drawing.Size(76, 17);
            this.radioButtonMoveFiles.TabIndex = 9;
            this.radioButtonMoveFiles.TabStop = true;
            this.radioButtonMoveFiles.Text = "Move Files";
            this.radioButtonMoveFiles.UseVisualStyleBackColor = true;
            this.radioButtonMoveFiles.CheckedChanged += new System.EventHandler(this.radioButtonMoveFiles_CheckedChanged);
            //
            // radioButtonIsSignedIn
            //
            this.radioButtonIsSignedIn.AutoSize = true;
            this.radioButtonIsSignedIn.Location = new System.Drawing.Point(11, 60);
            this.radioButtonIsSignedIn.Name = "radioButtonIsSignedIn";
            this.radioButtonIsSignedIn.Size = new System.Drawing.Size(81, 17);
            this.radioButtonIsSignedIn.TabIndex = 2;
            this.radioButtonIsSignedIn.TabStop = true;
            this.radioButtonIsSignedIn.Text = "Is Signed In";
            this.radioButtonIsSignedIn.UseVisualStyleBackColor = true;
            this.radioButtonIsSignedIn.CheckedChanged += new System.EventHandler(this.radioButtonIsSignedIn_CheckedChanged);
            //
            // radioButtonSignout
            //
            this.radioButtonSignout.AutoSize = true;
            this.radioButtonSignout.Location = new System.Drawing.Point(11, 37);
            this.radioButtonSignout.Name = "radioButtonSignout";
            this.radioButtonSignout.Size = new System.Drawing.Size(61, 17);
            this.radioButtonSignout.TabIndex = 1;
            this.radioButtonSignout.TabStop = true;
            this.radioButtonSignout.Text = "Signout";
            this.radioButtonSignout.UseVisualStyleBackColor = true;
            this.radioButtonSignout.CheckedChanged += new System.EventHandler(this.radioButtonSignout_CheckedChanged);
            //
            // panelParameters
            //
            this.panelParameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelParameters.AutoScroll = true;
            this.panelParameters.Location = new System.Drawing.Point(6, 19);
            this.panelParameters.Name = "panelParameters";
            this.panelParameters.Size = new System.Drawing.Size(659, 115);
            this.panelParameters.TabIndex = 1;
            //
            // groupBoxParameters
            //
            this.groupBoxParameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxParameters.Controls.Add(this.panelParameters);
            this.groupBoxParameters.Location = new System.Drawing.Point(12, 240);
            this.groupBoxParameters.Name = "groupBoxParameters";
            this.groupBoxParameters.Size = new System.Drawing.Size(671, 140);
            this.groupBoxParameters.TabIndex = 2;
            this.groupBoxParameters.TabStop = false;
            this.groupBoxParameters.Text = "Parameters";
            //
            // groupBoxResult
            //
            this.groupBoxResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxResult.Controls.Add(this.listView);
            this.groupBoxResult.Location = new System.Drawing.Point(12, 386);
            this.groupBoxResult.Name = "groupBoxResult";
            this.groupBoxResult.Size = new System.Drawing.Size(671, 213);
            this.groupBoxResult.TabIndex = 3;
            this.groupBoxResult.TabStop = false;
            this.groupBoxResult.Text = "Result";
            //
            // listView
            //
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader2,
            this.columnHeader5,
            this.columnHeader1});
            this.listView.FullRowSelect = true;
            this.listView.Location = new System.Drawing.Point(6, 19);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(659, 188);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
            this.listView.ItemActivate += new System.EventHandler(this.listView_ItemActivate);
            //
            // columnHeader4
            //
            this.columnHeader4.Text = "Time";
            this.columnHeader4.Width = 135;
            //
            // columnHeader2
            //
            this.columnHeader2.Text = "Method Name";
            this.columnHeader2.Width = 120;
            //
            // columnHeader5
            //
            this.columnHeader5.Text = "Info";
            this.columnHeader5.Width = 250;
            //
            // columnHeader1
            //
            this.columnHeader1.Text = "File ID";
            this.columnHeader1.Width = 139;
            //
            // buttonExecute
            //
            this.buttonExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExecute.Location = new System.Drawing.Point(602, 605);
            this.buttonExecute.Name = "buttonExecute";
            this.buttonExecute.Size = new System.Drawing.Size(75, 23);
            this.buttonExecute.TabIndex = 4;
            this.buttonExecute.Text = "Execute";
            this.buttonExecute.UseVisualStyleBackColor = true;
            this.buttonExecute.Click += new System.EventHandler(this.buttonExecute_Click);
            //
            // progressBar
            //
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.progressBar.Location = new System.Drawing.Point(18, 605);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(170, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 5;
            this.progressBar.Visible = false;
            //
            // timer
            //
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            //
            // buttonCancel
            //
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(521, 605);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            //
            // radioButtonCopyFiles
            //
            this.radioButtonCopyFiles.AutoSize = true;
            this.radioButtonCopyFiles.Location = new System.Drawing.Point(525, 60);
            this.radioButtonCopyFiles.Name = "radioButtonCopyFiles";
            this.radioButtonCopyFiles.Size = new System.Drawing.Size(73, 17);
            this.radioButtonCopyFiles.TabIndex = 21;
            this.radioButtonCopyFiles.TabStop = true;
            this.radioButtonCopyFiles.Text = "Copy Files";
            this.radioButtonCopyFiles.UseVisualStyleBackColor = true;
            this.radioButtonCopyFiles.CheckedChanged += new System.EventHandler(this.radioButtonCopyFiles_CheckedChanged);
            //
            // TestForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 643);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonExecute);
            this.Controls.Add(this.groupBoxResult);
            this.Controls.Add(this.groupBoxMethods);
            this.Controls.Add(this.groupBoxParameters);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(331, 535);
            this.Name = "TestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = String.Format("{0} Tester",DriveService.Settings.Product);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TestForm_FormClosing);
            this.Load += new System.EventHandler(this.TestForm_Load);
            this.groupBoxMethods.ResumeLayout(false);
            this.panelMethods.ResumeLayout(false);
            this.panelMethods.PerformLayout();
            this.groupBoxParameters.ResumeLayout(false);
            this.groupBoxResult.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxMethods;
        private System.Windows.Forms.RadioButton radioButtonAuthenticate;
        private System.Windows.Forms.RadioButton radioButtonGetFile;
        private System.Windows.Forms.Panel panelParameters;
        private System.Windows.Forms.GroupBox groupBoxParameters;
        private System.Windows.Forms.GroupBox groupBoxResult;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.Button buttonExecute;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.RadioButton radioButtonDownloadFile;
        private System.Windows.Forms.RadioButton radioButtonGetFilesFromPath;
        private System.Windows.Forms.RadioButton radioButtonInsertFile;
        private System.Windows.Forms.RadioButton radioButtonUploadFile;
        private System.Windows.Forms.RadioButton radioButtonRenameFile;
        private System.Windows.Forms.RadioButton radioButtonMoveFiles;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.RadioButton radioButtonSignout;
        private System.Windows.Forms.RadioButton radioButtonFolderPath;
        private System.Windows.Forms.RadioButton radioButtonIsSignedIn;
        private System.Windows.Forms.RadioButton radioButtonStopBackgroundProcesses;
        private System.Windows.Forms.RadioButton radioButtonStartBackgroundProcesses;
        private System.Windows.Forms.RadioButton radioButtonProcessing;
        private System.Windows.Forms.RadioButton radioButtonStartedBackgroundProcesses;
        private System.Windows.Forms.RadioButton radioButtonStartingBackgroundProcesses;
        private System.Windows.Forms.RadioButton radioButtonStoppingBackgroundProcesses;
        private System.Windows.Forms.RadioButton radioButtonGetLog;
        private System.Windows.Forms.RadioButton radioButtonGetAbout;
        private System.Windows.Forms.Panel panelMethods;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.RadioButton radioButtonGetFileStatus;
        private System.Windows.Forms.RadioButton radioButtonCleanupFile;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.RadioButton radioButtonSetLog;
        private System.Windows.Forms.RadioButton radioButtonUntrashFile;
        private System.Windows.Forms.RadioButton radioButtonTrashFile;
        private System.Windows.Forms.RadioButton radioButtonCopyFiles;

    }
}
