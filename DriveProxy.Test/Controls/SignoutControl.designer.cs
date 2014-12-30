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
    partial class SignoutControl
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
            this.checkBoxAuthenticate = new System.Windows.Forms.CheckBox();
            this.checkBoxStartBackgroundProcesses = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            //
            // checkBoxOpenInExplorer
            //
            this.checkBoxOpenInExplorer.AutoSize = true;
            this.checkBoxOpenInExplorer.Location = new System.Drawing.Point(7, 50);
            this.checkBoxOpenInExplorer.Name = "checkBoxOpenInExplorer";
            this.checkBoxOpenInExplorer.Size = new System.Drawing.Size(105, 17);
            this.checkBoxOpenInExplorer.TabIndex = 2;
            this.checkBoxOpenInExplorer.Text = "Open In Explorer";
            this.checkBoxOpenInExplorer.UseVisualStyleBackColor = true;
            //
            // checkBoxAuthenticate
            //
            this.checkBoxAuthenticate.AutoSize = true;
            this.checkBoxAuthenticate.Location = new System.Drawing.Point(7, 4);
            this.checkBoxAuthenticate.Name = "checkBoxAuthenticate";
            this.checkBoxAuthenticate.Size = new System.Drawing.Size(86, 17);
            this.checkBoxAuthenticate.TabIndex = 0;
            this.checkBoxAuthenticate.Text = "Authenticate";
            this.checkBoxAuthenticate.UseVisualStyleBackColor = true;
            //
            // checkBoxStartBackgroundProcesses
            //
            this.checkBoxStartBackgroundProcesses.AutoSize = true;
            this.checkBoxStartBackgroundProcesses.Location = new System.Drawing.Point(7, 27);
            this.checkBoxStartBackgroundProcesses.Name = "checkBoxStartBackgroundProcesses";
            this.checkBoxStartBackgroundProcesses.Size = new System.Drawing.Size(161, 17);
            this.checkBoxStartBackgroundProcesses.TabIndex = 1;
            this.checkBoxStartBackgroundProcesses.Text = "Start Background Processes";
            this.checkBoxStartBackgroundProcesses.UseVisualStyleBackColor = true;
            //
            // SignoutControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBoxStartBackgroundProcesses);
            this.Controls.Add(this.checkBoxAuthenticate);
            this.Controls.Add(this.checkBoxOpenInExplorer);
            this.Name = "SignoutControl";
            this.Size = new System.Drawing.Size(172, 75);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxOpenInExplorer;
        private System.Windows.Forms.CheckBox checkBoxAuthenticate;
        private System.Windows.Forms.CheckBox checkBoxStartBackgroundProcesses;




    }
}
