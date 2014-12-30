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
    partial class StateControl
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel3 = new DoubleBufferedPanel();
            this.labelRemove = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelCancel = new System.Windows.Forms.LinkLabel();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.panel4 = new DoubleBufferedPanel();
            this.labelDescription = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            //
            // panel3
            //
            this.panel3.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel3.Controls.Add(this.labelRemove);
            this.panel3.Controls.Add(this.pictureBox1);
            this.panel3.Controls.Add(this.labelStatus);
            this.panel3.Controls.Add(this.labelCancel);
            this.panel3.Controls.Add(this.progressBar);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Controls.Add(this.labelDescription);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(316, 32);
            this.panel3.TabIndex = 1;
            //
            // labelRemove
            //
            this.labelRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRemove.AutoSize = true;
            this.labelRemove.Location = new System.Drawing.Point(262, 10);
            this.labelRemove.Name = "labelRemove";
            this.labelRemove.Size = new System.Drawing.Size(47, 13);
            this.labelRemove.TabIndex = 6;
            this.labelRemove.TabStop = true;
            this.labelRemove.Text = "Remove";
            this.labelRemove.Visible = false;
            this.labelRemove.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.labelRemove_LinkClicked);
            //
            // pictureBox1
            //
            this.pictureBox1.Location = new System.Drawing.Point(7, 9);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(14, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            //
            // labelStatus
            //
            this.labelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStatus.AutoEllipsis = true;
            this.labelStatus.Location = new System.Drawing.Point(145, 10);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(112, 18);
            this.labelStatus.TabIndex = 4;
            this.labelStatus.Text = "Waiting...";
            //
            // labelCancel
            //
            this.labelCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCancel.AutoSize = true;
            this.labelCancel.Location = new System.Drawing.Point(266, 10);
            this.labelCancel.Name = "labelCancel";
            this.labelCancel.Size = new System.Drawing.Size(40, 13);
            this.labelCancel.TabIndex = 3;
            this.labelCancel.TabStop = true;
            this.labelCancel.Text = "Cancel";
            this.labelCancel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.labelCancel_LinkClicked);
            //
            // progressBar
            //
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(144, 5);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(115, 23);
            this.progressBar.TabIndex = 2;
            this.progressBar.Visible = false;
            //
            // panel4
            //
            this.panel4.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 31);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(316, 1);
            this.panel4.TabIndex = 1;
            //
            // labelDescription
            //
            this.labelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDescription.AutoEllipsis = true;
            this.labelDescription.Location = new System.Drawing.Point(23, 10);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(111, 18);
            this.labelDescription.TabIndex = 0;
            this.labelDescription.Text = "filename";
            //
            // toolTip
            //
            this.toolTip.Active = false;
            //
            // StateControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel3);
            this.Name = "StateControl";
            this.Size = new System.Drawing.Size(316, 32);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DoubleBufferedPanel panel3;
        private System.Windows.Forms.LinkLabel labelCancel;
        private System.Windows.Forms.ProgressBar progressBar;
        private DoubleBufferedPanel panel4;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel labelRemove;
    }
}
