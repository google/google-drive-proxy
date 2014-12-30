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

namespace DriveProxy.Forms
{
  public partial class LogForm : Form
  {
    protected bool _AllowClose = false;

    public LogForm()
    {
      InitializeComponent();

      Log.OnWriteEntry += Log_OnWriteEntry;
    }

    public bool AllowClose
    {
      get { return _AllowClose; }
      set { _AllowClose = value; }
    }

    private void LogForm_Load(object sender, EventArgs e)
    {
    }

    private void Log_OnWriteEntry(string dateTime, string message, LogType type)
    {
      try
      {
        if (InvokeRequired)
        {
          BeginInvoke(new MethodInvoker(() => { Log_OnWriteEntry(dateTime, message, type); }));
          return;
        }

        try
        {
          AddEntry(dateTime, type, message);
        }
        catch (Exception exception)
        {
          AddEntry(DateTime.Now.ToString(), LogType.Error, "Could not add log message - " + exception.Message);
        }
      }
      catch
      {
        Debugger.Break();
      }
    }

    private void AddEntry(string dateTime, LogType type, string message)
    {
      try
      {
        var listViewItem = new ListViewItem();

        listViewItem.Text = dateTime;
        listViewItem.SubItems.Add(type.ToString());
        listViewItem.SubItems.Add(Environment.UserName + "@" + Environment.UserDomainName);
        listViewItem.SubItems.Add(message);

        listView1.Items.Insert(0, listViewItem);

        if (message.Contains("Begin - Invoking method ID"))
        {
          listViewItem.BackColor = System.Drawing.Color.Green;
        }
        else if (message.Contains("End - Invoking method ID"))
        {
          listViewItem.BackColor = System.Drawing.Color.Red;
        }
        else if (message.Contains("Begin - Google.Apis."))
        {
          listViewItem.BackColor = System.Drawing.Color.Yellow;
        }
        else if (message.Contains("End - Google.Apis."))
        {
          listViewItem.BackColor = System.Drawing.Color.Yellow;
        }
      }
      catch
      {
        Debugger.Break();
      }
    }

    private void LogForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      try
      {
        if (InvokeRequired)
        {
          BeginInvoke(new MethodInvoker(() => { LogForm_FormClosing(null, e); }));
          return;
        }

        if (!AllowClose)
        {
          if (!Win32.IsSystemShuttingDown())
          {
            e.Cancel = true;

            Hide();
          }
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }
  }
}
