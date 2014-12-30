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
using System.Text;

namespace DriveProxy.API
{
  partial class DriveService
  {
    private class RequestFields
    {
      private static string _aboutFields = "";

      private static string _fileFields = "";

      private static string _listFileFields = "";

      public static string AboutFields
      {
        get
        {
          if (String.IsNullOrEmpty(_aboutFields))
          {
            var sb = new StringBuilder();

            sb.Append("name,");
            sb.Append("userDisplayName,");
            sb.Append("quotaBytesTotal,");
            sb.Append("quotaBytesUsed,");
            sb.Append("quotaBytesUsedAggregate,");
            sb.Append("quotaBytesUsedInTrash,");
            sb.Append("rootFolderId,");
            sb.Append("largestChangeId"); // end of list, no comma

            _aboutFields = sb.ToString();
          }

          return _aboutFields;
        }
      }

      public static string FileFields
      {
        get
        {
          if (String.IsNullOrEmpty(_fileFields))
          {
            var sb = new StringBuilder();

            sb.Append("alternateLink,");
            sb.Append("copyable,");
            sb.Append("createdDate,");
            sb.Append("description,");
            sb.Append("downloadUrl,");
            sb.Append("editable,");
            sb.Append("fileExtension,");
            sb.Append("fileSize,");
            sb.Append("id,");
            sb.Append("labels,");
            sb.Append("lastModifyingUserName,");
            sb.Append("lastViewedByMeDate,");
            sb.Append("mimeType,");
            sb.Append("modifiedByMeDate,");
            sb.Append("modifiedDate,");
            sb.Append("ownerNames,");
            sb.Append("parents,");
            sb.Append("shared,");
            sb.Append("sharedWithMeDate,");
            sb.Append("title"); // end of list, no comma

            _fileFields = sb.ToString();
          }

          return _fileFields;
        }
      }

      public static string ListFileFields
      {
        get
        {
          if (string.IsNullOrEmpty(_listFileFields))
          {
            var sb = new StringBuilder();

            sb.Append("etag,");
            sb.Append("items("); //open items list
            sb.Append(FileFields); //access via property, so private is populated
            sb.Append("),"); // The ) is important, its the end of the items list
            sb.Append("kind,");
            sb.Append("nextLink,");
            sb.Append("nextPageToken,");
            sb.Append("selfLink"); //end of list, no comma

            _listFileFields = sb.ToString();
          }

          return _listFileFields;
        }
      }
    }
  }
}
