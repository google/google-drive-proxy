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
﻿namespace DriveProxy.Test.Forms.TestControls
{
    partial class CopyFilesControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxParentID = new System.Windows.Forms.TextBox();
            this.textBoxFileIds = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Parent ID:";
            //
            // textBoxParentID
            //
            this.textBoxParentID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxParentID.Location = new System.Drawing.Point(66, 3);
            this.textBoxParentID.Name = "textBoxParentID";
            this.textBoxParentID.Size = new System.Drawing.Size(233, 20);
            this.textBoxParentID.TabIndex = 1;
            //
            // textBoxFileIds
            //
            this.textBoxFileIds.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFileIds.Location = new System.Drawing.Point(66, 29);
            this.textBoxFileIds.Name = "textBoxFileIds";
            this.textBoxFileIds.Size = new System.Drawing.Size(233, 20);
            this.textBoxFileIds.TabIndex = 3;
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Files IDs:";
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(63, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(231, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "(add mulitple file ids using a comma to seperate)";
            //
            // MoveFilesControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxFileIds);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxParentID);
            this.Controls.Add(this.label1);
            this.Name = "MoveFilesControl";
            this.Size = new System.Drawing.Size(312, 82);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxParentID;
        private System.Windows.Forms.CheckBox checkBoxIsFolder;
        private System.Windows.Forms.TextBox textBoxFileIds;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;

    }
}
