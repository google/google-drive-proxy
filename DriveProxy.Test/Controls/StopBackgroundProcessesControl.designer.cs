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
    partial class StopBackgroundProcessesControl
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
            this.checkBoxWaitWhileProcessing = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            //
            // checkBoxWaitWhileProcessing
            //
            this.checkBoxWaitWhileProcessing.AutoSize = true;
            this.checkBoxWaitWhileProcessing.Checked = true;
            this.checkBoxWaitWhileProcessing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxWaitWhileProcessing.Location = new System.Drawing.Point(7, 4);
            this.checkBoxWaitWhileProcessing.Name = "checkBoxWaitWhileProcessing";
            this.checkBoxWaitWhileProcessing.Size = new System.Drawing.Size(133, 17);
            this.checkBoxWaitWhileProcessing.TabIndex = 1;
            this.checkBoxWaitWhileProcessing.Text = "Wait While Processing";
            this.checkBoxWaitWhileProcessing.UseVisualStyleBackColor = true;
            //
            // StopBackgroundProcessesControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBoxWaitWhileProcessing);
            this.Name = "StopBackgroundProcessesControl";
            this.Size = new System.Drawing.Size(147, 24);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxWaitWhileProcessing;



    }
}
