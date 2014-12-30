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
using System.Xml.Linq;
using DriveProxy.Service;
using DriveProxy.Utils;

namespace DriveProxy.API
{
  public class LogInfo : XElementObject
  {
    public string FilePath = "";
    public string LocalGoogleDriveData = "";
    public int LogLevel = 0;

    public override XElement ToXElement()
    {
      try
      {
        var element = new XElement("Log",
                                   new XAttribute("LogLevel", LogLevel),
                                   new XAttribute("FilePath", FilePath),
                                   new XAttribute("LocalGoogleDriveData", LocalGoogleDriveData)
          );

        return element;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }
  }
}
