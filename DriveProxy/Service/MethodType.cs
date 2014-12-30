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

namespace DriveProxy.Service
{
  internal enum MethodType
  {
    Unknown = 0,
    GetFiles = 1,
    DownloadFile = 2,
    RenameFile = 4,
    TrashFiles = 5,
    UntrashFiles = 6,
    UploadFile = 8,
    Authenticate = 9,
    InsertFile = 10,
    MoveFiles = 11,
    CopyFiles = 12,
    GetLog = 13,
    GetFilesFromPath = 14,
    GetAbout = 15,
  }
}
