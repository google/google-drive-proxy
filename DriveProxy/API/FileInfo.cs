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
using System.Xml.Linq;
using DriveProxy.Service;
using DriveProxy.Utils;

namespace DriveProxy.API
{
  public enum FileReturnType
  {
    Unknown = 0,
    FilterGoogleFiles = 1,
    IgnoreGoogleFiles = 2,
    ReturnAllGoogleFiles = 3,
  }

  /// <summary>
  ///   The File Type identifier.
  /// </summary>
  public enum FileInfoType
  {
    // Generic Types
    NonGoogleDoc = 0,

    // Google Doc Types
    NotImplemented = 100,
    None = 101,
    Root = 102,
    Folder = 103,
    Audio = 104,
    Document = 105,
    Drawing = 106,
    File = 107,
    Form = 108,
    FusionTable = 109,
    Photo = 110,
    Presentation = 111,
    Script = 112,
    Sites = 113,
    Spreadsheet = 114,
    Unknown = 115,
    Video = 116,
    Kix = 117,
  }

  /// <summary>
  ///   The location status of the File.
  /// </summary>
  public enum FileInfoStatus
  {
    Unknown = 0,
    InCloud = 1,
    OnDisk = 2,
    ModifiedOnDisk = 3,
  }

  internal class FileInfoTypeService
  {
    public static string GetFileExtension(FileInfoType type)
    {
      switch (type)
      {
        case FileInfoType.Audio:
          return "";

        case FileInfoType.Document:
          return ".gdoc";

        case FileInfoType.Drawing:
          return ".gdraw";

        case FileInfoType.File:
          return "";

        case FileInfoType.Form:
          return ".gform";

        case FileInfoType.FusionTable:
          return ".gtable";

        case FileInfoType.Photo:
          return "";

        case FileInfoType.Presentation:
          return "";

        case FileInfoType.Script:
          return ".gscript";

        case FileInfoType.Sites:
          return "";

        case FileInfoType.Spreadsheet:
          return ".gsheet";

        case FileInfoType.Video:
          return "";

        default:
          return "";
      }
    }
  }

  public class FileInfoAssociation
  {
    public string Application = "";
    public string DefaultIcon = "";
    public string DefaultIconIndex = "";
    public string FileExtension = "";
    public string FileType = "";
  }

  public class FileInfo : XElementObject
  {
    public FileInfoAssociation Association = new FileInfoAssociation();
    public string FileExtension = "";
    public string FileName = "";
    public string FilePath = "";
    public long FileSize = 0;
    public string Id = "";
    public DateTime LastWriteTime = default(DateTime);
    public long Length = 0;
    public string MimeType = "";
    public string Title = "";
    public FileInfoType Type = FileInfoType.Unknown;
    internal Google.Apis.Drive.v2.Data.File _file = null;
    internal List<FileInfo> _files = null;

    public FileInfo(Google.Apis.Drive.v2.Data.File file)
    {
      _file = file;
    }

    public bool Copyable
    {
      get { return DriveService.GetBoolean(_file.Copyable, true); }
    }

    public DateTime CreatedDate
    {
      get { return DriveService.GetDateTime(_file.CreatedDate); }
    }

    public string Description
    {
      get { return DriveService.GetString(_file.Description); }
    }

    public string DownloadUrl
    {
      get { return DriveService.GetString(_file.DownloadUrl); }
    }

    public bool Editable
    {
      get { return DriveService.GetBoolean(_file.Editable, true); }
    }

    public string LastModifyingUserName
    {
      get { return DriveService.GetString(_file.LastModifyingUserName); }
    }

    public DateTime LastViewedByMeDate
    {
      get
      {
        // This date is used for FILETIME structure in C++, the default(DATETIME) value is not compatible.
        // So when the value does not exist, use a different date
        DateTime r = DriveService.GetDateTime(_file.LastViewedByMeDate);

        if (r.Equals(default(DateTime)))
        {
          return DriveService.GetDateTime(_file.CreatedDate);
        }

        return r;
      }
    }

    public DateTime ModifiedByMeDate
    {
      get { return DriveService.GetDateTime(_file.ModifiedByMeDate); }
    }

    public DateTime ModifiedDate
    {
      get { return DriveService.GetDateTime(_file.ModifiedDate); }
    }

    public string OriginalFilename
    {
      get { return DriveService.GetString(_file.OriginalFilename); }
    }

    public string OwnerName
    {
      get
      {
        if (_file.OwnerNames != null && _file.OwnerNames.Count > 0)
        {
          return DriveService.GetString(_file.OwnerNames[0]);
        }

        return "";
      }
    }

    public string ParentId
    {
      get
      {
        if (_file.Parents != null && _file.Parents.Count > 0)
        {
          return DriveService.GetString(_file.Parents[0].Id);
        }

        return "";
      }
    }

    public bool Shared
    {
      get { return DriveService.GetBoolean(_file.Shared); }
    }

    public DateTime SharedWithMeDate
    {
      get { return DriveService.GetDateTime(_file.SharedWithMeDate); }
    }

    public bool Trashed
    {
      get { return DriveService.GetBoolean(_file.Labels.Trashed); }
    }

    public bool IsFolder
    {
      get { return DriveService.IsFolder(Type); }
    }

    public bool IsFile
    {
      get { return DriveService.IsFile(Type); }
    }

    public bool IsGoogleDoc
    {
      get { return DriveService.IsGoogleDoc(Type); }
    }

    public bool IsRoot
    {
      get { return DriveService.IsRoot(Type); }
    }

    public List<FileInfo> Files
    {
      get { return _files ?? (_files = new List<FileInfo>()); }
    }

    private static string GetDateTime(DateTime value)
    {
      try
      {
        string result = value.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss:fff");

        return result;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public override XElement ToXElement()
    {
      try
      {
        var element = new XElement("File",
                                   new XAttribute("Copyable", Copyable),
                                   new XAttribute("CreatedDate", GetDateTime(CreatedDate)),
                                   new XAttribute("Description", Description),
                                   new XAttribute("Editable", Editable),
                                   new XAttribute("FileExtension", FileExtension),
                                   new XAttribute("FileSize", FileSize),
                                   new XAttribute("Id", Id),
                                   new XAttribute("LastModifyingUserName", LastModifyingUserName),
                                   new XAttribute("LastViewedByMeDate", GetDateTime(LastViewedByMeDate)),
                                   new XAttribute("MimeType", MimeType),
                                   new XAttribute("ModifiedByMeDate", GetDateTime(ModifiedByMeDate)),
                                   new XAttribute("ModifiedDate", GetDateTime(ModifiedDate)),
                                   new XAttribute("OriginalFilename", OriginalFilename),
                                   new XAttribute("OwnerName", OwnerName),
                                   new XAttribute("ParentId", ParentId),
                                   new XAttribute("Shared", Shared),
                                   new XAttribute("SharedWithMeDate", GetDateTime(SharedWithMeDate)),
                                   new XAttribute("Title", Title),
                                   new XAttribute("Trashed", Trashed),
                                   new XAttribute("FileName", FileName),
                                   new XAttribute("FilePath", FilePath),
                                   new XAttribute("AssociationFileExtension", Association.FileExtension),
                                   new XAttribute("AssociationFileType", Association.FileType),
                                   new XAttribute("AssociationApplication", Association.Application),
                                   new XAttribute("AssociationDefaultIcon", Association.DefaultIcon),
                                   new XAttribute("AssociationDefaultIconIndex", Association.DefaultIconIndex),
                                   new XAttribute("Type", (int)Type)
          );

        if (Files.Count > 0)
        {
          var childElementList = new XElement("Files");

          foreach (FileInfo file in Files)
          {
            XElement childElement = file.ToXElement();

            childElementList.Add(childElement);
          }

          element.Add(childElementList);
        }

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
