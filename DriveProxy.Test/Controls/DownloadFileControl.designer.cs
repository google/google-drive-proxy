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
    partial class DownloadFileControl
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
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.checkBoxCheckIfAlreadyDownloaded = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "File ID:";
            //
            // textBoxParentID
            //
            this.textBoxParentID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxParentID.Location = new System.Drawing.Point(75, 3);
            this.textBoxParentID.Name = "textBoxParentID";
            this.textBoxParentID.Size = new System.Drawing.Size(230, 20);
            this.textBoxParentID.TabIndex = 1;
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Chunk Size:";
            //
            // numericUpDown1
            //
            this.numericUpDown1.Location = new System.Drawing.Point(75, 29);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown1.TabIndex = 3;
            //
            // checkBoxCheckIfAlreadyDownloaded
            //
            this.checkBoxCheckIfAlreadyDownloaded.AutoSize = true;
            this.checkBoxCheckIfAlreadyDownloaded.Checked = true;
            this.checkBoxCheckIfAlreadyDownloaded.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCheckIfAlreadyDownloaded.Location = new System.Drawing.Point(75, 55);
            this.checkBoxCheckIfAlreadyDownloaded.Name = "checkBoxCheckIfAlreadyDownloaded";
            this.checkBoxCheckIfAlreadyDownloaded.Size = new System.Drawing.Size(167, 17);
            this.checkBoxCheckIfAlreadyDownloaded.TabIndex = 4;
            this.checkBoxCheckIfAlreadyDownloaded.Text = "Check If Already Downloaded";
            this.checkBoxCheckIfAlreadyDownloaded.UseVisualStyleBackColor = true;
            //
            // DownloadFileControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBoxCheckIfAlreadyDownloaded);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxParentID);
            this.Controls.Add(this.label1);
            this.Name = "DownloadFileControl";
            this.Size = new System.Drawing.Size(318, 82);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxParentID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.CheckBox checkBoxCheckIfAlreadyDownloaded;

    }
}
