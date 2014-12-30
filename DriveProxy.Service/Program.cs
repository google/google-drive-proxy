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
using DriveProxy.Forms;
using DriveProxy.Service;
using DriveProxy.Utils;

namespace DriveProxyApp
{
  internal class Program
  {
    public static MainForm MainForm = null;
    public static System.Diagnostics.Process CloseListener = null;

    [STAThread]
    public static void Main(string[] args)
    {
      try
      {
        Application.ThreadException += Application_ThreadException;
        AppDomain.CurrentDomain.UnhandledException +=
          CurrentDomain_UnhandledException;

        if (args.Length > 0)
        {
          const string search = "--file=";

          if (args[0].IndexOf(search) == 0)
          {
            string path = String.Join(" ", args);

            path = path.Substring(search.Length, path.Length - search.Length);

            try
            {
              DriveService.OpenGoogleDoc(path);
            }
            catch (Exception exception)
            {
              Log.Error(exception, false);
            }

            return;
          }
        }

        Log.Information("Starting DriveProxy.Service.exe");

        if (ServicePipe.IsServerRunning(true))
        {
          throw new LogException(ServicePipe.ProcessName + " is already running.", false, false);
        }

        Debugger.Launch();

        DriveService.Init();

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        MainForm = new MainForm();

        Application.Run(MainForm);
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);
      }
      finally
      {
        Log.Warning("Exiting DriveProxy.Service.exe");
      }
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      try
      {
        Log.Level = LogType.Error;

        if (e != null && e.ExceptionObject != null)
        {
          try
          {
            var exception = (Exception)e.ExceptionObject;

            try
            {
              MessageBox.Show("Error: " + exception.Message, DriveService.Settings.Product, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
            }

            try
            {
              Log.Error(exception);
            }
            catch
            {
            }
          }
          catch
          {
          }
        }

        if (e != null)
        {
          if (e.IsTerminating)
          {
            if (MainForm != null)
            {
              MainForm.Close();
            }
          }
        }
      }
      catch
      {
      }
    }

    private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
    {
      try
      {
        Log.Level = LogType.Error;

        if (e != null && e.Exception != null)
        {
          try
          {
            MessageBox.Show("Error: " + e.Exception.Message, DriveService.Settings.Product, MessageBoxButtons.OK, MessageBoxIcon.Error);
          }
          catch
          {
          }

          try
          {
            Log.Error(e.Exception);
          }
          catch
          {
          }
        }
      }
      catch
      {
      }
    }
  }
}
