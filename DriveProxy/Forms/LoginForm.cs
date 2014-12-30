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

namespace DriveProxy.Forms
{
  partial class LoginForm : Form
  {
    private readonly bool _logout;
    private readonly Uri _startPageUri;
    public string ErrorCode = string.Empty;
    public string SuccessCode = string.Empty;
    private bool _navigatedToStartPageUri;

    public LoginForm(Uri startPageUri, bool logout)
    {
      try
      {
        InitializeComponent();
        Left = -1000;
        Top = -1000;
        Visible = false;
        Opacity = 0;

        _startPageUri = startPageUri;
        _logout = logout;
      }
      catch (Exception exception)
      {
        MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
    {
      try
      {
        if (!_navigatedToStartPageUri)
        {
          _navigatedToStartPageUri = true;

          Close();
          return;

          webBrowser1.Navigate(_startPageUri);
        }
        if (webBrowser1.DocumentTitle.StartsWith("Success code="))
        {
          SuccessCode = webBrowser1.DocumentTitle.Substring(13);

          Close();
        }
        else if (webBrowser1.DocumentTitle.StartsWith("Denied error="))
        {
          ErrorCode = webBrowser1.DocumentTitle.Substring(13);

          var errorCloseTimer = new System.Timers.Timer(500);
          errorCloseTimer.Elapsed += errorCloseTimer_Elapsed;
          errorCloseTimer.AutoReset = false;
          errorCloseTimer.Enabled = true;
        }
      }
      catch (Exception exception)
      {
        MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void errorCloseTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      try
      {
        if (InvokeRequired)
        {
          BeginInvoke(new MethodInvoker(() => { Close(); }));
          return;
        }

        Close();
      }
      catch (Exception exception)
      {
        MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void LoginForm_Load(object sender, EventArgs e)
    {
      try
      {
        if (_startPageUri == null)
        {
          Close();
        }
        Left = -1000;
        Top = -1000;
        Visible = false;
        Opacity = 0;

        if (_logout)
        {
          webBrowser1.Navigate("https://www.google.com/accounts/Logout");
        }
        else
        {
          _navigatedToStartPageUri = true;

          webBrowser1.Navigate(_startPageUri);
        }
      }
      catch (Exception exception)
      {
        MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    public static void ShowDialog(Uri startPageUri, bool logout, ref string successCode, ref string errorCode)
    {
      string formSuccessCode = string.Empty;
      string formErrorCode = string.Empty;

      var t = new System.Threading.Thread(() =>
      {
        using (var form = new LoginForm(startPageUri, logout))
        {
          form.ShowDialog();

          formSuccessCode = form.SuccessCode;
          formErrorCode = form.ErrorCode;
        }
      });

      t.SetApartmentState(System.Threading.ApartmentState.STA);
      t.Start();
      t.Join();

      successCode = formSuccessCode;
      errorCode = formErrorCode;
    }

    public static void Logout()
    {
      string successCode = "";
      string errorCode = "";

      LoginForm.ShowDialog(new Uri("https://www.google.com"), true, ref successCode, ref errorCode);
    }
  }
}
