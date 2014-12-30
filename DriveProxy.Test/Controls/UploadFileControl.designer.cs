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
    partial class UploadFileControl
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
            this.checkBoxCheckIfAlreadyUploaded = new System.Windows.Forms.CheckBox();
            this.numericUpDownChunkSize = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChunkSize)).BeginInit();
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
            // checkBoxCheckIfAlreadyUploaded
            //
            this.checkBoxCheckIfAlreadyUploaded.AutoSize = true;
            this.checkBoxCheckIfAlreadyUploaded.Checked = true;
            this.checkBoxCheckIfAlreadyUploaded.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCheckIfAlreadyUploaded.Location = new System.Drawing.Point(75, 55);
            this.checkBoxCheckIfAlreadyUploaded.Name = "checkBoxCheckIfAlreadyUploaded";
            this.checkBoxCheckIfAlreadyUploaded.Size = new System.Drawing.Size(153, 17);
            this.checkBoxCheckIfAlreadyUploaded.TabIndex = 7;
            this.checkBoxCheckIfAlreadyUploaded.Text = "Check If Already Uploaded";
            this.checkBoxCheckIfAlreadyUploaded.UseVisualStyleBackColor = true;
            //
            // numericUpDownChunkSize
            //
            this.numericUpDownChunkSize.Location = new System.Drawing.Point(75, 29);
            this.numericUpDownChunkSize.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numericUpDownChunkSize.Name = "numericUpDownChunkSize";
            this.numericUpDownChunkSize.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownChunkSize.TabIndex = 6;
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Chunk Size:";
            //
            // UploadFileControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBoxCheckIfAlreadyUploaded);
            this.Controls.Add(this.numericUpDownChunkSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxParentID);
            this.Controls.Add(this.label1);
            this.Name = "UploadFileControl";
            this.Size = new System.Drawing.Size(318, 82);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownChunkSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxParentID;
        private System.Windows.Forms.CheckBox checkBoxCheckIfAlreadyUploaded;
        private System.Windows.Forms.NumericUpDown numericUpDownChunkSize;
        private System.Windows.Forms.Label label2;

    }
}
