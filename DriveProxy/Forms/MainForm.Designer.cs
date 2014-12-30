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
﻿namespace DriveProxy.Forms
{
    public partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cancelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemClearCache = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCleanupFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemSignOut = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemShowLog = new System.Windows.Forms.ToolStripMenuItem();
            this.showStatusWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            //
            // notifyIcon1
            //
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Google Drive";
            this.notifyIcon1.Visible = true;
            //
            // contextMenuStrip1
            //
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cancelToolStripMenuItem,
            this.toolStripMenuItem1,
            this.toolStripMenuItemSettings,
            this.toolStripSeparator3,
            this.toolStripMenuItemClearCache,
            this.toolStripMenuItemCleanupFiles,
            this.toolStripSeparator2,
            this.toolStripMenuItemSignOut,
            this.toolStripMenuItem3,
            this.toolStripMenuItemShowLog,
            this.showStatusWindowToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolStripMenuItemExit});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(233, 232);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            //
            // cancelToolStripMenuItem
            //
            this.cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
            this.cancelToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.cancelToolStripMenuItem.Text = "Cancel";
            //
            // toolStripMenuItem1
            //
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(229, 6);
            //
            // toolStripMenuItemSettings
            //
            this.toolStripMenuItemSettings.Name = "toolStripMenuItemSettings";
            this.toolStripMenuItemSettings.Size = new System.Drawing.Size(232, 22);
            this.toolStripMenuItemSettings.Text = "Settings";
            this.toolStripMenuItemSettings.Click += new System.EventHandler(this.toolStripMenuItemSettings_Click);
            //
            // toolStripSeparator3
            //
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(229, 6);
            //
            // toolStripMenuItemClearCache
            //
            this.toolStripMenuItemClearCache.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolStripMenuItemClearCache.Name = "toolStripMenuItemClearCache";
            this.toolStripMenuItemClearCache.Size = new System.Drawing.Size(232, 22);
            this.toolStripMenuItemClearCache.Text = "Delete cached file data";
            this.toolStripMenuItemClearCache.Click += new System.EventHandler(this.toolStripMenuItemClearCache_Click);
            //
            // toolStripMenuItemCleanupFiles
            //
            this.toolStripMenuItemCleanupFiles.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolStripMenuItemCleanupFiles.Name = "toolStripMenuItemCleanupFiles";
            this.toolStripMenuItemCleanupFiles.Size = new System.Drawing.Size(232, 22);
            this.toolStripMenuItemCleanupFiles.Text = "Remove any downloaded files";
            this.toolStripMenuItemCleanupFiles.Click += new System.EventHandler(this.toolStripMenuItemCleanupFiles_Click);
            //
            // toolStripSeparator2
            //
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(229, 6);
            //
            // toolStripMenuItemSignOut
            //
            this.toolStripMenuItemSignOut.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolStripMenuItemSignOut.Name = "toolStripMenuItemSignOut";
            this.toolStripMenuItemSignOut.Size = new System.Drawing.Size(232, 22);
            this.toolStripMenuItemSignOut.Text = "Sign out";
            this.toolStripMenuItemSignOut.Click += new System.EventHandler(this.toolStripMenuItemSignOut_Click);
            //
            // toolStripMenuItem3
            //
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(229, 6);
            //
            // toolStripMenuItemShowLog
            //
            this.toolStripMenuItemShowLog.Name = "toolStripMenuItemShowLog";
            this.toolStripMenuItemShowLog.Size = new System.Drawing.Size(232, 22);
            this.toolStripMenuItemShowLog.Text = "Show log";
            this.toolStripMenuItemShowLog.Click += new System.EventHandler(this.toolStripMenuItemShowLog_Click);
            //
            // showStatusWindowToolStripMenuItem
            //
            this.showStatusWindowToolStripMenuItem.Name = "showStatusWindowToolStripMenuItem";
            this.showStatusWindowToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.showStatusWindowToolStripMenuItem.Text = "Show status window";
            this.showStatusWindowToolStripMenuItem.Click += new System.EventHandler(this.showStatusWindowToolStripMenuItem_Click);
            //
            // toolStripSeparator1
            //
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(229, 6);
            //
            // toolStripMenuItemExit
            //
            this.toolStripMenuItemExit.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.toolStripMenuItemExit.Name = "toolStripMenuItemExit";
            this.toolStripMenuItemExit.Size = new System.Drawing.Size(232, 22);
            this.toolStripMenuItemExit.Text = "Exit";
            this.toolStripMenuItemExit.Click += new System.EventHandler(this.toolStripMenuItemExit_Click);
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.ShowInTaskbar = false;
            this.Text = "Google Drive";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cancelToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem showStatusWindowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSignOut;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemClearCache;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCleanupFiles;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemShowLog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExit;
    }
}

