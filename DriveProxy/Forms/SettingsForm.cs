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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DriveProxy.API;
using DriveProxy.Utils;

namespace DriveProxy.Forms
{
  partial class SettingsForm : Form
  {
    public SettingsForm()
    {
      InitializeComponent();
    }

    private void SettingsForm_Load(object sender, EventArgs e)
    {
      try
      {
        comboBoxLogLevel.Items.Add(LogType.None.ToString());
        comboBoxLogLevel.Items.Add(LogType.All.ToString());
        comboBoxLogLevel.Items.Add(LogType.Warning.ToString());
        comboBoxLogLevel.Items.Add(LogType.Error.ToString());
        comboBoxLogLevel.Items.Add(LogType.Performance.ToString());

        ComboBoxItem.AddItem(comboBoxFileReturnType,
                             "Filter Google Files (Recommended)",
                             FileReturnType.FilterGoogleFiles);
        ComboBoxItem.AddItem(comboBoxFileReturnType,
                             "Ignore Google Files (Possible slow performance)",
                             FileReturnType.IgnoreGoogleFiles);
        ComboBoxItem.AddItem(comboBoxFileReturnType,
                             "Return All Google Files (Not recommended)",
                             FileReturnType.ReturnAllGoogleFiles);

        comboBoxLogLevel.SelectedItem = DriveService.Settings.LogLevel.ToString();
        ComboBoxItem.SetSelectedItem(comboBoxFileReturnType, DriveService.Settings.FileReturnType);
        checkBoxUseCaching.Checked = DriveService.Settings.UseCaching;
        checkBoxStartup.Checked = DriveService.Settings.IsStartingStartOnStartup;
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);

        MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void buttonOK_Click(object sender, EventArgs e)
    {
      try
      {
        DriveService.Settings.LogLevel =
          (LogType)Enum.Parse(typeof(LogType), comboBoxLogLevel.SelectedItem.ToString());
        DriveService.Settings.FileReturnType = (FileReturnType)ComboBoxItem.GetSelectedItem(comboBoxFileReturnType);
        DriveService.Settings.UseCaching = checkBoxUseCaching.Checked;
        DriveService.Settings.IsStartingStartOnStartup = checkBoxStartup.Checked;

        DriveService.Settings.Save();

        Close();
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);

        MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void buttonCancel_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void comboBoxFileReturnType_DropDownStyleChanged(object sender, EventArgs e)
    {
    }

    private void comboBoxFileReturnType_DropDown(object sender, EventArgs e)
    {
      try
      {
        var comboBox = (ComboBox)sender;

        ComboBoxItem.DropDown(comboBox);
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);

        MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private class ComboBoxItem
    {
      public readonly string Text = "";
      public readonly object Value;

      public ComboBoxItem(string text, object value)
      {
        Text = text;
        Value = value;
      }

      public override string ToString()
      {
        return Text;
      }

      public static void AddItem(ComboBox comboBox, string text, object value)
      {
        comboBox.Items.Add(new ComboBoxItem(text, value));
      }

      public static object GetSelectedItem(ComboBox comboBox)
      {
        if (comboBox.SelectedItem == null)
        {
          return null;
        }

        var item = (ComboBoxItem)comboBox.SelectedItem;

        return item.Value;
      }

      public static void SetSelectedItem(ComboBox comboBox, object value)
      {
        int selectedIndex = -1;

        for (int i = 0; i < comboBox.Items.Count; i++)
        {
          var item = comboBox.Items[i] as ComboBoxItem;

          if (item != null)
          {
            if (item.Value.ToString() == value.ToString())
            {
              selectedIndex = i;

              break;
            }
          }
        }

        comboBox.SelectedIndex = selectedIndex;
      }

      public static void DropDown(ComboBox comboBox)
      {
        if (comboBox.Tag != null)
        {
          return;
        }

        int width = comboBox.DropDownWidth;
        Graphics g = comboBox.CreateGraphics();
        Font font = comboBox.Font;

        int vertScrollBarWidth = (comboBox.Items.Count > comboBox.MaxDropDownItems)
          ? SystemInformation.VerticalScrollBarWidth
          : 0;

        IEnumerable<string> itemsList = comboBox.Items.Cast<object>().Select(item => item.ToString());

        foreach (string s in itemsList)
        {
          int newWidth = (int)g.MeasureString(s, font).Width + vertScrollBarWidth;

          if (width < newWidth)
          {
            width = newWidth;
          }
        }

        comboBox.DropDownWidth = width;

        comboBox.Tag = true;
      }
    }
  }
}
