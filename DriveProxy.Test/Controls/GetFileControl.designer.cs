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
    partial class GetFileControl
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
            this.checkBoxUseCachedData = new System.Windows.Forms.CheckBox();
            this.checkBoxGetChildren = new System.Windows.Forms.CheckBox();
            this.checkBoxUpdateCachedData = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "File ID:";
            //
            // textBoxParentID
            //
            this.textBoxParentID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxParentID.Location = new System.Drawing.Point(50, 3);
            this.textBoxParentID.Name = "textBoxParentID";
            this.textBoxParentID.Size = new System.Drawing.Size(255, 20);
            this.textBoxParentID.TabIndex = 1;
            //
            // checkBoxUseCachedData
            //
            this.checkBoxUseCachedData.AutoSize = true;
            this.checkBoxUseCachedData.Checked = true;
            this.checkBoxUseCachedData.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxUseCachedData.Location = new System.Drawing.Point(50, 52);
            this.checkBoxUseCachedData.Name = "checkBoxUseCachedData";
            this.checkBoxUseCachedData.Size = new System.Drawing.Size(111, 17);
            this.checkBoxUseCachedData.TabIndex = 3;
            this.checkBoxUseCachedData.Text = "Use Cached Data";
            this.checkBoxUseCachedData.UseVisualStyleBackColor = true;
            //
            // checkBoxGetChildren
            //
            this.checkBoxGetChildren.AutoSize = true;
            this.checkBoxGetChildren.Checked = true;
            this.checkBoxGetChildren.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxGetChildren.Location = new System.Drawing.Point(50, 29);
            this.checkBoxGetChildren.Name = "checkBoxGetChildren";
            this.checkBoxGetChildren.Size = new System.Drawing.Size(84, 17);
            this.checkBoxGetChildren.TabIndex = 4;
            this.checkBoxGetChildren.Text = "Get Children";
            this.checkBoxGetChildren.UseVisualStyleBackColor = true;
            //
            // checkBoxUpdateCachedData
            //
            this.checkBoxUpdateCachedData.AutoSize = true;
            this.checkBoxUpdateCachedData.Checked = true;
            this.checkBoxUpdateCachedData.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxUpdateCachedData.Location = new System.Drawing.Point(50, 75);
            this.checkBoxUpdateCachedData.Name = "checkBoxUpdateCachedData";
            this.checkBoxUpdateCachedData.Size = new System.Drawing.Size(127, 17);
            this.checkBoxUpdateCachedData.TabIndex = 5;
            this.checkBoxUpdateCachedData.Text = "Update Cached Data";
            this.checkBoxUpdateCachedData.UseVisualStyleBackColor = true;
            //
            // GetFileControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBoxUpdateCachedData);
            this.Controls.Add(this.checkBoxGetChildren);
            this.Controls.Add(this.checkBoxUseCachedData);
            this.Controls.Add(this.textBoxParentID);
            this.Controls.Add(this.label1);
            this.Name = "GetFileControl";
            this.Size = new System.Drawing.Size(318, 104);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxParentID;
        private System.Windows.Forms.CheckBox checkBoxUseCachedData;
        private System.Windows.Forms.CheckBox checkBoxGetChildren;
        private System.Windows.Forms.CheckBox checkBoxUpdateCachedData;

    }
}
