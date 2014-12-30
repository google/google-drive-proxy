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
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DriveProxy.API;
using Microsoft.Deployment.WindowsInstaller;

namespace DriveProxy.Installer.CustomAction
{
  public class CustomActions
  {
    private static string _folder;

    private static string LocalGoogleDriveData
    {
      get
      {
        {
          return System.IO.Path.Combine(LocalApplicationData, CompanyPath, _folder);
        }
      }
    }

    private static string RoamingGoogleDriveData
    {
      get
      {
        {
          return System.IO.Path.Combine(ApplicationData, CompanyPath, _folder);
        }
      }
    }

    private static string LocalApplicationData
    {
      get
      {
        {
          return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }
      }
    }

    private static string ApplicationData
    {
      get
      {
        {
          return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
      }
    }

    public static string CompanyPath
    {
      get { return GetCustomAttribute<CompanyPathAttr>(); }
    }

    /// <summary>
    ///   Deletes  Roaming Appdata folder and ask if the user wants to delete the Local Appdata folder.
    ///   You should also specify the property DriveProxyFolder in the Session object.
    /// </summary>
    /// <param name="session">
    ///   Session object of the installation process. It contains the DriveProxyFolder property with the
    ///   directory to remove.
    /// </param>
    /// <returns>Returns an ActionResult with the result of the cleanup </returns>
    [CustomAction]
    public static ActionResult CleanUserData(Session session)
    {
      try
      {
        _folder = session["DriveProxyFolder"];

        session.Log("Begin CleanUserData Custom Action");

        session.Log("Begin Remove Roaming Files");
        if (DeleteDirectory(RoamingGoogleDriveData))
        {
          session.Log("Remove Roaming Files Success");
        }

        session.Log("Begin Dialog Result");
        DialogResult result = MessageBox.Show(new Form {TopMost = true},
                                              "Do you want to delete the cached files?",
                                              "Remove Cache",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question,
                                              MessageBoxDefaultButton.Button1);

        if (result == DialogResult.Yes)
        {
          if (DeleteDirectory(LocalGoogleDriveData))
          {
            session.Log("Remove Cached Files Success");
          }
        }

        session.Log("End CleanUserData");
      }
      catch (Exception ex)
      {
        session.Log("ERROR in custom action RemoveCachedFiles {0}", ex.ToString());
      }

      return ActionResult.Success;
    }

    public static string GetCustomAttribute<T>() where T : DPSAttribute
    {
      T attr = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (T), false)
                       .OfType<T>()
                       .FirstOrDefault();
      if (attr != null)
      {
        return attr.attr_;
      }
      return "Unknown";
    }

    internal static bool DeleteDirectory(string directoryName, bool recursive = true)
    {
      bool result;

      try
      {
        if (String.IsNullOrEmpty(directoryName))
        {
          return false;
        }

        if (!System.IO.Directory.Exists(directoryName))
        {
          return false;
        }

        try
        {
          System.IO.Directory.Delete(directoryName, recursive);
          result = true;
        }
        catch (Exception exception)
        {
          result = false;
        }
      }
      catch (Exception exception)
      {
        result = false;
      }
      return result;
    }
  }
}
