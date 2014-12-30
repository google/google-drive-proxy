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
using System.Windows.Forms;
using DriveProxy.Utils;

namespace DriveProxy.Test.Forms.TestControls
{
  public partial class SetLogControl : UserControl
  {
    public SetLogControl()
    {
      InitializeComponent();

      comboBoxLogLevel.Items.Add(LogType.None.ToString());
      comboBoxLogLevel.Items.Add(LogType.All.ToString());
      comboBoxLogLevel.Items.Add(LogType.Warning.ToString());
      comboBoxLogLevel.Items.Add(LogType.Error.ToString());
    }

    public LogType LogLevel
    {
      get
      {
        var value = LogType.None;

        Enum.TryParse(comboBoxLogLevel.SelectedItem.ToString(), out value);

        return value;
      }
      set { comboBoxLogLevel.SelectedItem = value.ToString(); }
    }

    public string Source
    {
      get { return textBoxSource.Text; }
      set { textBoxSource.Text = value; }
    }

    public string FilePath
    {
      get { return textBoxFilePath.Text; }
      set { textBoxFilePath.Text = value; }
    }

    private void buttonFilePath_Click(object sender, EventArgs e)
    {
      saveFileDialog.FileName = textBoxFilePath.Text;

      DialogResult dialogResult = saveFileDialog.ShowDialog(ParentForm);

      if (dialogResult != DialogResult.OK)
      {
        return;
      }

      textBoxFilePath.Text = saveFileDialog.FileName;
    }
  }
}
