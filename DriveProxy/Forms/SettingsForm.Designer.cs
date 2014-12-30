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

namespace DriveProxy.Forms
{
    partial class SettingsForm
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
      this.buttonOK = new System.Windows.Forms.Button();
      this.buttonCancel = new System.Windows.Forms.Button();
      this.groupBox = new System.Windows.Forms.GroupBox();
      this.checkBoxStartup = new System.Windows.Forms.CheckBox();
      this.comboBoxFileReturnType = new System.Windows.Forms.ComboBox();
      this.label2 = new System.Windows.Forms.Label();
      this.checkBoxUseCaching = new System.Windows.Forms.CheckBox();
      this.comboBoxLogLevel = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.groupBox.SuspendLayout();
      this.SuspendLayout();
      //
      // buttonOK
      //
      this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonOK.Location = new System.Drawing.Point(174, 136);
      this.buttonOK.Name = "buttonOK";
      this.buttonOK.Size = new System.Drawing.Size(75, 23);
      this.buttonOK.TabIndex = 2;
      this.buttonOK.Text = "OK";
      this.buttonOK.UseVisualStyleBackColor = true;
      this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
      //
      // buttonCancel
      //
      this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.buttonCancel.Location = new System.Drawing.Point(255, 136);
      this.buttonCancel.Name = "buttonCancel";
      this.buttonCancel.Size = new System.Drawing.Size(75, 23);
      this.buttonCancel.TabIndex = 0;
      this.buttonCancel.Text = "Cancel";
      this.buttonCancel.UseVisualStyleBackColor = true;
      this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
      //
      // groupBox
      //
      this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox.Controls.Add(this.checkBoxStartup);
      this.groupBox.Controls.Add(this.comboBoxFileReturnType);
      this.groupBox.Controls.Add(this.label2);
      this.groupBox.Controls.Add(this.checkBoxUseCaching);
      this.groupBox.Controls.Add(this.comboBoxLogLevel);
      this.groupBox.Controls.Add(this.label1);
      this.groupBox.Location = new System.Drawing.Point(12, 12);
      this.groupBox.Name = "groupBox";
      this.groupBox.Size = new System.Drawing.Size(318, 118);
      this.groupBox.TabIndex = 1;
      this.groupBox.TabStop = false;
      //
      // checkBoxStartup
      //
      this.checkBoxStartup.AutoSize = true;
      this.checkBoxStartup.Location = new System.Drawing.Point(71, 95);
      this.checkBoxStartup.Name = "checkBoxStartup";
      this.checkBoxStartup.Size = new System.Drawing.Size(152, 17);
      this.checkBoxStartup.TabIndex = 5;
      this.checkBoxStartup.Text = String.Format("Start {0} on startup",DriveService.Settings.Title);
      this.checkBoxStartup.UseVisualStyleBackColor = true;
      //
      // comboBoxFileReturnType
      //
      this.comboBoxFileReturnType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.comboBoxFileReturnType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBoxFileReturnType.FormattingEnabled = true;
      this.comboBoxFileReturnType.Location = new System.Drawing.Point(71, 47);
      this.comboBoxFileReturnType.Name = "comboBoxFileReturnType";
      this.comboBoxFileReturnType.Size = new System.Drawing.Size(228, 21);
      this.comboBoxFileReturnType.TabIndex = 4;
      this.comboBoxFileReturnType.DropDown += new System.EventHandler(this.comboBoxFileReturnType_DropDown);
      //
      // label2
      //
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(14, 50);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(53, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "File Type:";
      //
      // checkBoxUseCaching
      //
      this.checkBoxUseCaching.AutoSize = true;
      this.checkBoxUseCaching.Location = new System.Drawing.Point(71, 76);
      this.checkBoxUseCaching.Name = "checkBoxUseCaching";
      this.checkBoxUseCaching.Size = new System.Drawing.Size(87, 17);
      this.checkBoxUseCaching.TabIndex = 2;
      this.checkBoxUseCaching.Text = "Use Caching";
      this.checkBoxUseCaching.UseVisualStyleBackColor = true;
      //
      // comboBoxLogLevel
      //
      this.comboBoxLogLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.comboBoxLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBoxLogLevel.FormattingEnabled = true;
      this.comboBoxLogLevel.Location = new System.Drawing.Point(71, 20);
      this.comboBoxLogLevel.Name = "comboBoxLogLevel";
      this.comboBoxLogLevel.Size = new System.Drawing.Size(228, 21);
      this.comboBoxLogLevel.TabIndex = 1;
      //
      // label1
      //
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(10, 23);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(57, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Log Level:";
      //
      // SettingsForm
      //
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(342, 165);
      this.Controls.Add(this.groupBox);
      this.Controls.Add(this.buttonCancel);
      this.Controls.Add(this.buttonOK);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "SettingsForm";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = String.Format("{0} Settings", DriveService.Settings.Product);
      this.TopMost = true;
      this.Load += new System.EventHandler(this.SettingsForm_Load);
      this.groupBox.ResumeLayout(false);
      this.groupBox.PerformLayout();
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.ComboBox comboBoxLogLevel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxUseCaching;
        private System.Windows.Forms.ComboBox comboBoxFileReturnType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxStartup;
    }
}
