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
  public class AboutInfo : XElementObject
  {
    public string Name = "";
    public long QuotaBytesTotal = 0;
    public long QuotaBytesUsed = 0;
    public long QuotaBytesUsedAggregate = 0;
    public long QuotaBytesUsedInTrash = 0;
    public string RootFolderId = "";
    public string UserDisplayName = "";

    public override XElement ToXElement()
    {
      try
      {
        var element = new XElement("About",
                                   new XAttribute("Name", Name),
                                   new XAttribute("UserDisplayName", UserDisplayName),
                                   new XAttribute("QuotaBytesTotal", QuotaBytesTotal),
                                   new XAttribute("QuotaBytesUsed", QuotaBytesUsed),
                                   new XAttribute("QuotaBytesUsedAggregate", QuotaBytesUsedAggregate),
                                   new XAttribute("QuotaBytesUsedInTrash", QuotaBytesUsedInTrash),
                                   new XAttribute("RootFolderId", RootFolderId)
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
