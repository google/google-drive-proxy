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
    partial class AuthenticateControl
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
            this.checkBoxOpenInExplorer = new System.Windows.Forms.CheckBox();
            this.checkBoxStartBackgroundProcesses = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            //
            // checkBoxOpenInExplorer
            //
            this.checkBoxOpenInExplorer.AutoSize = true;
            this.checkBoxOpenInExplorer.Location = new System.Drawing.Point(7, 29);
            this.checkBoxOpenInExplorer.Name = "checkBoxOpenInExplorer";
            this.checkBoxOpenInExplorer.Size = new System.Drawing.Size(105, 17);
            this.checkBoxOpenInExplorer.TabIndex = 1;
            this.checkBoxOpenInExplorer.Text = "Open In Explorer";
            this.checkBoxOpenInExplorer.UseVisualStyleBackColor = true;
            //
            // checkBoxStartBackgroundProcesses
            //
            this.checkBoxStartBackgroundProcesses.AutoSize = true;
            this.checkBoxStartBackgroundProcesses.Location = new System.Drawing.Point(7, 6);
            this.checkBoxStartBackgroundProcesses.Name = "checkBoxStartBackgroundProcesses";
            this.checkBoxStartBackgroundProcesses.Size = new System.Drawing.Size(161, 17);
            this.checkBoxStartBackgroundProcesses.TabIndex = 0;
            this.checkBoxStartBackgroundProcesses.Text = "Start Background Processes";
            this.checkBoxStartBackgroundProcesses.UseVisualStyleBackColor = true;
            //
            // AuthenticateControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBoxStartBackgroundProcesses);
            this.Controls.Add(this.checkBoxOpenInExplorer);
            this.Name = "AuthenticateControl";
            this.Size = new System.Drawing.Size(175, 49);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxOpenInExplorer;
        private System.Windows.Forms.CheckBox checkBoxStartBackgroundProcesses;


    }
}
