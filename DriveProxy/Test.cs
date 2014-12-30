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
using DriveProxy.API;
using DriveProxy.Methods;

namespace DriveProxy
{
  internal static class Test
  {
    public static void Run()
    {
      try
      {
        if (System.Diagnostics.Debugger.IsAttached)
        {
          System.Diagnostics.Debugger.Break();
        }


        var stopwatch = new System.Diagnostics.Stopwatch();

        stopwatch.Start();


        // authenticate service
        var authenticateMethodInfo = new AuthenticateMethodInfo();

        authenticateMethodInfo.Invoke(false);


        // get all files in root
        var getFilesMethodInfo = new GetFilesMethodInfo();

        FileInfo getFilesMethodInfoResult = getFilesMethodInfo.Invoke("", true, true, true);


        // create a new folder

        var insertFileMethodInfo = new InsertFileMethodInfo();

        FileInfo insertFileMethodInfoResult = insertFileMethodInfo.Invoke("", "testinsertFolder", true);

        string testFolderId = insertFileMethodInfoResult.Id;


        // rename the new folder
        var renameFileMethodInfo = new RenameFileMethodInfo();

        FileInfo renameFileMethodInfoResult = renameFileMethodInfo.Invoke(testFolderId,
                                                                          "testrenameFolder",
                                                                          null);


        // create a new file
        insertFileMethodInfo = new InsertFileMethodInfo();

        insertFileMethodInfoResult = insertFileMethodInfo.Invoke(testFolderId, "testinsertfile.txt");

        string testFileId = insertFileMethodInfoResult.Id;


        // write text to the new file and upload
        DriveService.WriteAllText(insertFileMethodInfoResult.FilePath, "testuploadfile-" + DateTime.Now);

        var uploadFileMethodInfo = new UploadFileMethodInfo();

        FileInfo uploadFileMethodInfoResult = uploadFileMethodInfo.Invoke(testFileId);


        DriveService.DeleteFile(insertFileMethodInfoResult.FilePath);

        // download the new file
        var downloadFileMethodInfo = new DownloadFileMethodInfo();

        FileInfo downloadFileMethodInfoResult = downloadFileMethodInfo.Invoke(testFileId);


        // rename the new file
        renameFileMethodInfo = new RenameFileMethodInfo();

        renameFileMethodInfoResult = renameFileMethodInfo.Invoke(testFileId, "testrenamefile.txt", null);


        // trash the folder and file
        var trashFileMethodInfo = new TrashFileMethodInfo();

        var trashFileIds = new List<string>();

        trashFileIds.Add(testFileId);
        trashFileIds.Add(testFolderId);

        List<string> trashedFileIds = trashFileMethodInfo.Invoke(trashFileIds);


        // untrash the folder and file
        var untrashFileMethodInfo = new UntrashFileMethodInfo();

        var untrashFileIds = new List<string>();

        untrashFileIds.Add(testFolderId);
        untrashFileIds.Add(testFileId);

        List<string> untrashedFileIds = untrashFileMethodInfo.Invoke(untrashFileIds);


        // copy the file into its own folder
        var copyFileMethodInfo = new CopyFileMethodInfo();

        var copyFileIds = new List<string>();

        copyFileIds.Add(testFileId);

        List<string> copiedFileIds = copyFileMethodInfo.Invoke(testFolderId, copyFileIds);

        string testFileId2 = copiedFileIds[0];


        // rename the copied file
        renameFileMethodInfo = new RenameFileMethodInfo();

        renameFileMethodInfoResult = renameFileMethodInfo.Invoke(testFileId2, "testcopiedfile.txt", null);


        // create a new folder
        insertFileMethodInfo = new InsertFileMethodInfo();

        insertFileMethodInfoResult = insertFileMethodInfo.Invoke("", "testinsertFolder2", true);

        string testFolderId2 = insertFileMethodInfoResult.Id;


        // copy testFile2 into the new folder
        copyFileMethodInfo = new CopyFileMethodInfo();

        copyFileIds = new List<string>();

        copyFileIds.Add(testFileId2);

        copiedFileIds = copyFileMethodInfo.Invoke(testFolderId2, copyFileIds);


        // move testFile into the new folder
        var moveFileMethodInfo = new MoveFileMethodInfo();

        var moveFileIds = new List<string>();

        moveFileIds.Add(testFileId);

        List<string> movedFileIds = moveFileMethodInfo.Invoke(testFolderId2, moveFileIds);


        // rename the copied file
        renameFileMethodInfo = new RenameFileMethodInfo();

        renameFileMethodInfoResult = renameFileMethodInfo.Invoke(testFileId, "testcopiedfile2.txt", null);


        // copy testFolder into new folder
        copyFileMethodInfo = new CopyFileMethodInfo();

        copyFileIds = new List<string>();

        copyFileIds.Add(testFolderId);

        copiedFileIds = copyFileMethodInfo.Invoke(testFolderId2, copyFileIds);

        string testSubFolderId = copiedFileIds[0];


        // move testFolder2 into testFolder
        moveFileMethodInfo = new MoveFileMethodInfo();

        moveFileIds = new List<string>();

        moveFileIds.Add(testFolderId2);

        movedFileIds = moveFileMethodInfo.Invoke(testFolderId, moveFileIds);


        // trash test folders

        trashFileIds = new List<string>();

        trashFileIds.Add(testFolderId);
        trashFileIds.Add(testFolderId2);

        trashedFileIds = trashFileMethodInfo.Invoke(trashFileIds);


        stopwatch.Stop();

        System.Windows.Forms.MessageBox.Show("Successfully ran test cases in " + stopwatch.Elapsed.TotalSeconds +
                                             " seconds");
      }
      catch (Exception exception)
      {
        System.Windows.Forms.MessageBox.Show("Error running test cases - " + exception.Message);
      }
    }
  }
}
