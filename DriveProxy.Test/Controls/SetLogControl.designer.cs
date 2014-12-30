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
    partial class SetLogControl
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
            this.comboBoxLogLevel = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxFilePath = new System.Windows.Forms.TextBox();
            this.buttonFilePath = new System.Windows.Forms.Button();
            this.textBoxSource = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Log Level";
            //
            // comboBoxLogLevel
            //
            this.comboBoxLogLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLogLevel.FormattingEnabled = true;
            this.comboBoxLogLevel.Location = new System.Drawing.Point(71, 7);
            this.comboBoxLogLevel.Name = "comboBoxLogLevel";
            this.comboBoxLogLevel.Size = new System.Drawing.Size(169, 21);
            this.comboBoxLogLevel.TabIndex = 1;
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "File Path";
            //
            // textBoxFilePath
            //
            this.textBoxFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFilePath.Location = new System.Drawing.Point(71, 60);
            this.textBoxFilePath.Name = "textBoxFilePath";
            this.textBoxFilePath.Size = new System.Drawing.Size(140, 20);
            this.textBoxFilePath.TabIndex = 5;
            //
            // buttonFilePath
            //
            this.buttonFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFilePath.Location = new System.Drawing.Point(217, 58);
            this.buttonFilePath.Name = "buttonFilePath";
            this.buttonFilePath.Size = new System.Drawing.Size(24, 24);
            this.buttonFilePath.TabIndex = 6;
            this.buttonFilePath.Text = "...";
            this.buttonFilePath.UseVisualStyleBackColor = true;
            this.buttonFilePath.Click += new System.EventHandler(this.buttonFilePath_Click);
            //
            // textBoxSource
            //
            this.textBoxSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSource.Location = new System.Drawing.Point(71, 34);
            this.textBoxSource.Name = "textBoxSource";
            this.textBoxSource.Size = new System.Drawing.Size(169, 20);
            this.textBoxSource.TabIndex = 3;
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Source";
            //
            // SetLogControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxSource);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonFilePath);
            this.Controls.Add(this.textBoxFilePath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxLogLevel);
            this.Controls.Add(this.label1);
            this.Name = "SetLogControl";
            this.Size = new System.Drawing.Size(256, 96);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxLogLevel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxFilePath;
        private System.Windows.Forms.Button buttonFilePath;
        private System.Windows.Forms.TextBox textBoxSource;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;




    }
}
