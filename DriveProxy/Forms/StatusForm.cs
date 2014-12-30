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
using DriveProxy.API;
using DriveProxy.Utils;

namespace DriveProxy.Forms
{
  partial class StatusForm : Form
  {
    protected static StatusForm _Instance = null;
    protected bool _AllowClose = false;
    protected bool _Visibility = false;

    public StatusForm()
    {
      InitializeComponent();
    }

    //Hide form from alt-tab
    protected override CreateParams CreateParams
    {
      get
      {
        // Turn on WS_EX_TOOLWINDOW style bit
        CreateParams cp = base.CreateParams;
        cp.ExStyle |= 0x80;
        return cp;
      }
    }

    public bool AllowClose
    {
      get { return _AllowClose; }
      set { _AllowClose = value; }
    }

    protected void ShowVisibility()
    {
      _Visibility = true;
      Visible = true;
    }

    protected void HideVisibility()
    {
      _Visibility = false;
      Visible = false;
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
      base.OnVisibleChanged(e);
      Visible = _Visibility;
    }

    public void SetStatusMessage(string message)
    {
      try
      {
        if (InvokeRequired)
        {
          BeginInvoke(new MethodInvoker(() => { SetStatusMessage(message); }));
          return;
        }

        label1.Text = message;
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void StatusForm_Load(object sender, EventArgs e)
    {
      try
      {
        if (InvokeRequired)
        {
          BeginInvoke(new MethodInvoker(() => { StatusForm_Load(null, null); }));
          return;
        }

        Left = -1000;
        Top = -1000;
        Opacity = 0;

        HideVisibility();

        timer1.Stop();
        timer2.Stop();
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void StatusForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      try
      {
        if (InvokeRequired)
        {
          BeginInvoke(new MethodInvoker(() => { StatusForm_FormClosing(null, e); }));
          return;
        }

        if (!AllowClose)
        {
          if (!Win32.IsSystemShuttingDown())
          {
            e.Cancel = true;

            CloseDialog();
          }
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      try
      {
        if (InvokeRequired)
        {
          BeginInvoke(new MethodInvoker(() => { timer1_Tick(null, null); }));
          return;
        }

        timer1.Stop();

        double left = (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / (double)2) -
                      (_Instance.Width / (double)2);
        double top = (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / (double)2) -
                     (_Instance.Height / (double)2);

        Left = Convert.ToInt32(left);
        Top = Convert.ToInt32(top);
        Opacity = 100;
        ShowVisibility();
        Focus();
        Refresh();

        Application.DoEvents();
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    private void timer2_Tick(object sender, EventArgs e)
    {
      try
      {
        if (InvokeRequired)
        {
          BeginInvoke(new MethodInvoker(() => { timer2_Tick(null, null); }));
          return;
        }

        timer2.Stop();

        Left = -1000;
        Top = -1000;
        Opacity = 0;

        HideVisibility();
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    public static void Init()
    {
      try
      {
        if (_Instance == null)
        {
          _Instance = new StatusForm();

          _Instance.Show();
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    public new static void ShowDialog()
    {
      try
      {
        if (_Instance == null)
        {
          return;
        }

        if (_Instance.InvokeRequired)
        {
          _Instance.BeginInvoke(new MethodInvoker(() => { ShowDialog(); }));
          return;
        }

        _Instance.timer2.Enabled = false;
        _Instance.timer1.Enabled = true;
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    public static void Exception(Exception exception)
    {
      try
      {
        MessageBox.Show(null, "Error: " + exception.Message, DriveService.Settings.Product, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      catch (Exception exception2)
      {
        Log.Error(exception2, false);
      }
    }

    public static void CloseDialog()
    {
      try
      {
        if (_Instance == null)
        {
          return;
        }

        if (_Instance.InvokeRequired)
        {
          try
          {
            IAsyncResult asyncResult = _Instance.BeginInvoke(new MethodInvoker(() => { CloseDialog(); }));
            System.Threading.Tasks.Task<object> task = System.Threading.Tasks.Task<object>.Factory.FromAsync(
                                                                                                             asyncResult,
                                                                                                             _Instance
                                                                                                               .EndInvoke);
            task.Wait(); // need to wait until this is finished, or the window could flicker on/off screen annoyingly
            return;
          }
          catch (Exception exception)
          {
            Log.Warning(exception.Message);
            return;
          }
        }

        _Instance.HideVisibility();

        _Instance.timer1.Enabled = false;
        _Instance.timer2.Enabled = true;
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }

    public new static void Close()
    {
      try
      {
        if (_Instance == null)
        {
          return;
        }

        if (_Instance != null)
        {
          _Instance.AllowClose = true;
          _Instance.timer1.Stop();
          _Instance.timer2.Stop();
          Form form = _Instance;
          form.Close();
        }
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
    }
  }
}
