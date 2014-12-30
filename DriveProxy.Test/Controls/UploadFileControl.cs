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

using System.Windows.Forms;

namespace DriveProxy.Test.Forms.TestControls
{
  public partial class UploadFileControl : UserControl
  {
    public UploadFileControl()
    {
      InitializeComponent();
    }

    public string FileId
    {
      get { return textBoxParentID.Text; }
      set { textBoxParentID.Text = value; }
    }

    public int ChunkSize
    {
      get { return (int)numericUpDownChunkSize.Value; }
      set { numericUpDownChunkSize.Value = value; }
    }

    public bool CheckIfAlreadyUploaded
    {
      get { return checkBoxCheckIfAlreadyUploaded.Checked; }
      set { checkBoxCheckIfAlreadyUploaded.Checked = value; }
    }
  }
}
