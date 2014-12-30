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
  partial class StateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StateForm));
            this.timerShowPopup = new System.Windows.Forms.Timer(this.components);
            this.panel4 = new DoubleBufferedPanel(this.components);
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.panelHeader = new DoubleBufferedPanel(this.components);
            this.labelCancelAll = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelTitleBar = new DoubleBufferedPanel(this.components);
            this.pictureBoxState = new System.Windows.Forms.PictureBox();
            this.pictureBoxClose = new System.Windows.Forms.PictureBox();
            this.pictureBoxMinimize = new System.Windows.Forms.PictureBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.panelStatusBar = new DoubleBufferedPanel(this.components);
            this.labelStatus = new System.Windows.Forms.Label();
            this.panel1 = new DoubleBufferedPanel(this.components);
            this.panelBody = new DoubleBufferedPanel(this.components);
            this.timerClosePopup = new System.Windows.Forms.Timer(this.components);
            this.timerMaximizePopup = new System.Windows.Forms.Timer(this.components);
            this.timerMinimizePopup = new System.Windows.Forms.Timer(this.components);
            this.timerWatchParent = new System.Windows.Forms.Timer(this.components);
            this.timerFinishedProcessing = new System.Windows.Forms.Timer(this.components);
            this.timerWaitForClose = new System.Windows.Forms.Timer(this.components);
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panelHeader.SuspendLayout();
            this.panelTitleBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxState)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMinimize)).BeginInit();
            this.panelStatusBar.SuspendLayout();
            this.SuspendLayout();
            //
            // timerShowPopup
            //
            this.timerShowPopup.Tick += new System.EventHandler(this.timerShowPopup_Tick);
            //
            // panel4
            //
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.pictureBox2);
            this.panel4.Controls.Add(this.panelHeader);
            this.panel4.Controls.Add(this.panelTitleBar);
            this.panel4.Controls.Add(this.panelStatusBar);
            this.panel4.Controls.Add(this.panel1);
            this.panel4.Controls.Add(this.panelBody);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(500, 350);
            this.panel4.TabIndex = 4;
            //
            // pictureBox2
            //
            this.pictureBox2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.pictureBox2.Location = new System.Drawing.Point(0, 66);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(498, 1);
            this.pictureBox2.TabIndex = 8;
            this.pictureBox2.TabStop = false;
            //
            // panelHeader
            //
            this.panelHeader.BackColor = System.Drawing.SystemColors.Control;
            this.panelHeader.Controls.Add(this.labelCancelAll);
            this.panelHeader.Controls.Add(this.label1);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 30);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(498, 36);
            this.panelHeader.TabIndex = 4;
            //
            // labelCancelAll
            //
            this.labelCancelAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCancelAll.AutoSize = true;
            this.labelCancelAll.Location = new System.Drawing.Point(434, 13);
            this.labelCancelAll.Name = "labelCancelAll";
            this.labelCancelAll.Size = new System.Drawing.Size(54, 13);
            this.labelCancelAll.TabIndex = 4;
            this.labelCancelAll.TabStop = true;
            this.labelCancelAll.Text = "Cancel All";
            this.labelCancelAll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.labelCancelAll_LinkClicked);
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Conversion: off";
            //
            // panelTitleBar
            //
            this.panelTitleBar.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panelTitleBar.BackgroundImage = global::DriveProxy.Properties.Resources.headerBkgd;
            this.panelTitleBar.Controls.Add(this.pictureBoxState);
            this.panelTitleBar.Controls.Add(this.pictureBoxClose);
            this.panelTitleBar.Controls.Add(this.pictureBoxMinimize);
            this.panelTitleBar.Controls.Add(this.labelTitle);
            this.panelTitleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitleBar.Location = new System.Drawing.Point(0, 0);
            this.panelTitleBar.Name = "panelTitleBar";
            this.panelTitleBar.Size = new System.Drawing.Size(498, 30);
            this.panelTitleBar.TabIndex = 6;
            //
            // pictureBoxState
            //
            this.pictureBoxState.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxState.Image = global::DriveProxy.Properties.Resources.upload;
            this.pictureBoxState.Location = new System.Drawing.Point(9, 8);
            this.pictureBoxState.Name = "pictureBoxState";
            this.pictureBoxState.Size = new System.Drawing.Size(10, 13);
            this.pictureBoxState.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxState.TabIndex = 3;
            this.pictureBoxState.TabStop = false;
            //
            // pictureBoxClose
            //
            this.pictureBoxClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxClose.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxClose.Image = global::DriveProxy.Properties.Resources.close;
            this.pictureBoxClose.Location = new System.Drawing.Point(477, 8);
            this.pictureBoxClose.Name = "pictureBoxClose";
            this.pictureBoxClose.Size = new System.Drawing.Size(13, 13);
            this.pictureBoxClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxClose.TabIndex = 2;
            this.pictureBoxClose.TabStop = false;
            this.pictureBoxClose.Click += new System.EventHandler(this.pictureBoxClose_Click);
            //
            // pictureBoxMinimize
            //
            this.pictureBoxMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxMinimize.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxMinimize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxMinimize.Image = global::DriveProxy.Properties.Resources.min;
            this.pictureBoxMinimize.Location = new System.Drawing.Point(456, 8);
            this.pictureBoxMinimize.Name = "pictureBoxMinimize";
            this.pictureBoxMinimize.Size = new System.Drawing.Size(13, 13);
            this.pictureBoxMinimize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxMinimize.TabIndex = 1;
            this.pictureBoxMinimize.TabStop = false;
            this.pictureBoxMinimize.Click += new System.EventHandler(this.pictureBoxMinimize_Click);
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = true;
            this.labelTitle.BackColor = System.Drawing.Color.Transparent;
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(25, 8);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(81, 13);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Google Drive";
            //
            // panelStatusBar
            //
            this.panelStatusBar.Controls.Add(this.labelStatus);
            this.panelStatusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelStatusBar.Location = new System.Drawing.Point(0, 323);
            this.panelStatusBar.Name = "panelStatusBar";
            this.panelStatusBar.Size = new System.Drawing.Size(498, 25);
            this.panelStatusBar.TabIndex = 7;
            //
            // labelStatus
            //
            this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStatus.AutoEllipsis = true;
            this.labelStatus.Location = new System.Drawing.Point(-1, 1);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(499, 22);
            this.labelStatus.TabIndex = 0;
            this.labelStatus.Text = "Using 0 GB of 15 GB - 0 GB in Trash";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // panel1
            //
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.panel1.Location = new System.Drawing.Point(0, 322);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(498, 1);
            this.panel1.TabIndex = 9;
            //
            // panelBody
            //
            this.panelBody.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBody.AutoScroll = true;
            this.panelBody.AutoScrollMargin = new System.Drawing.Size(10, 10);
            this.panelBody.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panelBody.Location = new System.Drawing.Point(0, 66);
            this.panelBody.Name = "panelBody";
            this.panelBody.Size = new System.Drawing.Size(498, 257);
            this.panelBody.TabIndex = 5;
            //
            // timerClosePopup
            //
            this.timerClosePopup.Tick += new System.EventHandler(this.timerClosePopup_Tick);
            //
            // timerMaximizePopup
            //
            this.timerMaximizePopup.Interval = 1;
            this.timerMaximizePopup.Tick += new System.EventHandler(this.timerMaximizePopup_Tick);
            //
            // timerMinimizePopup
            //
            this.timerMinimizePopup.Interval = 1;
            this.timerMinimizePopup.Tick += new System.EventHandler(this.timerMinimizePopup_Tick);
            //
            // timerWatchParent
            //
            this.timerWatchParent.Interval = 10;
            this.timerWatchParent.Tick += new System.EventHandler(this.timerWatchParent_Tick);
            //
            // timerFinishedProcessing
            //
            this.timerFinishedProcessing.Tick += new System.EventHandler(this.timerFinishedProcessing_Tick);
            //
            // timerWaitForClose
            //
            this.timerWaitForClose.Interval = 1000;
            this.timerWaitForClose.Tick += new System.EventHandler(this.timerWaitForClose_Tick);
            //
            // StateForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 350);
            this.Controls.Add(this.panel4);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(216, 32);
            this.Name = "StateForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Google Drive";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StateForm_FormClosing);
            this.Load += new System.EventHandler(this.StateForm_Load);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelTitleBar.ResumeLayout(false);
            this.panelTitleBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxState)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMinimize)).EndInit();
            this.panelStatusBar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerShowPopup;
        private DoubleBufferedPanel panel4;
        private DoubleBufferedPanel panelStatusBar;
        private System.Windows.Forms.Label labelStatus;
        private DoubleBufferedPanel panelBody;
        private DoubleBufferedPanel panelHeader;
        private System.Windows.Forms.LinkLabel labelCancelAll;
        private System.Windows.Forms.Label label1;
        private DoubleBufferedPanel panelTitleBar;
        private System.Windows.Forms.PictureBox pictureBoxMinimize;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.PictureBox pictureBoxClose;
        private System.Windows.Forms.PictureBox pictureBoxState;
        private System.Windows.Forms.Timer timerClosePopup;
        private System.Windows.Forms.Timer timerMaximizePopup;
        private System.Windows.Forms.Timer timerMinimizePopup;
        private System.Windows.Forms.Timer timerWatchParent;
        private DoubleBufferedPanel panel1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Timer timerFinishedProcessing;
        private System.Windows.Forms.Timer timerWaitForClose;

    }
}
