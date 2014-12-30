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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;

namespace DriveProxy.Utils
{
  internal class UserProcess
  {
    public static bool IsRunning(string processName, bool forCurrentUserOnly)
    {
      try
      {
        int processId = Process.GetCurrentProcess().Id;
        string filename = Process.GetCurrentProcess().Modules[0].FileName;
        Process[] processes = GetProcessesByName(processName, forCurrentUserOnly);


        if (processes != null)
        {
          foreach (Process t in processes)
          {
            try
            {
              if (t.Id != processId)
              {
                if (t.Modules[0].FileName == filename)
                {
                  return true;
                }
              }
            }
            catch
            {
              return true;
            }
          }
        }
        return false;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    public static System.Diagnostics.Process[] GetProcessesByName(string processName, bool forCurrentUserOnly)
    {
      try
      {
        System.Diagnostics.Process[] processes = null;

        if (forCurrentUserOnly)
        {
          processes = UserProcess.GetProcessesByName(processName);
        }
        else
        {
          processes = System.Diagnostics.Process.GetProcessesByName(processName);
        }

        return processes;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public static System.Diagnostics.Process[] GetProcessesByName(string processName)
    {
      try
      {
        System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(processName);
        var processList = new List<System.Diagnostics.Process>();

        foreach (System.Diagnostics.Process process in processes)
        {
          var query = new ObjectQuery("Select * from Win32_Process WHERE ProcessID = " + process.Id);

          using (var searcher = new ManagementObjectSearcher(query))
          {
            using (ManagementObjectCollection objs = searcher.Get())
            {
              foreach (ManagementObject obj in objs)
              {
                try
                {
                  var owner = new String[2];

                  try
                  {
                    obj.InvokeMethod("GetOwner", owner);
                  }
                  catch
                  {
                  }

                  if (owner == null || owner.Length == 0)
                  {
                    continue;
                  }

                  string user = owner[0];

                  if (String.IsNullOrEmpty(user))
                  {
                    continue;
                  }

                  if (!String.Equals(user, Environment.UserName, StringComparison.CurrentCultureIgnoreCase))
                  {
                    continue;
                  }

                  string domain = owner[1];

                  if (String.IsNullOrEmpty(domain))
                  {
                    continue;
                  }

                  if (!String.Equals(domain, Environment.UserDomainName, StringComparison.CurrentCultureIgnoreCase))
                  {
                    continue;
                  }

                  object name = obj["Name"];

                  if (name == null)
                  {
                    continue;
                  }

                  string tempName = name.ToString();

                  if (String.IsNullOrEmpty(tempName))
                  {
                    continue;
                  }

                  if (!String.Equals(processName, tempName, StringComparison.CurrentCultureIgnoreCase))
                  {
                    tempName = System.IO.Path.GetFileNameWithoutExtension(tempName);

                    if (!String.Equals(processName, tempName, StringComparison.CurrentCultureIgnoreCase))
                    {
                      continue;
                    }
                  }

                  object processId = obj["ProcessID"];

                  if (processId == null)
                  {
                    continue;
                  }

                  int tempId = 0;

                  if (!int.TryParse(processId.ToString(), out tempId))
                  {
                    continue;
                  }

                  processList.Add(process);
                }
                catch (Exception exception)
                {
                  Log.Error(exception, false);
                }
              }
            }
          }
        }

        return processList.ToArray();
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public static System.Diagnostics.Process Start(string filePath,
                                                   bool checkIfAlreadyRunning,
                                                   string processName,
                                                   bool forCurrentUserOnly)
    {
      try
      {
        if (checkIfAlreadyRunning)
        {
          System.Diagnostics.Process[] processes = GetProcessesByName(processName, forCurrentUserOnly);

          if (processes != null && processes.Length > 0)
          {
            return processes[0];
          }
        }

        if (!System.IO.File.Exists(filePath))
        {
          string tempPath = System.Reflection.Assembly.GetEntryAssembly().Location;

          tempPath = System.IO.Path.GetDirectoryName(tempPath);
          tempPath = System.IO.Path.Combine(tempPath, filePath);

          if (System.IO.File.Exists(tempPath))
          {
            filePath = tempPath;
          }
        }

        return System.Diagnostics.Process.Start(filePath);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    [SuppressUnmanagedCodeSecurity]
    private class NativeMethods
    {
      public enum SECURITY_IMPERSONATION_LEVEL
      {
        SecurityAnonymous,
        SecurityIdentification,
        SecurityImpersonation,
        SecurityDelegation
      }

      public enum TOKEN_TYPE
      {
        TokenPrimary = 1,
        TokenImpersonation
      }

      public const int GENERIC_ALL_ACCESS = 0x10000000;
      public const int CREATE_NO_WINDOW = 0x08000000;

      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
      public static extern bool LogonUser(String lpszUsername,
                                          String lpszDomain,
                                          String lpszPassword,
                                          int dwLogonType,
                                          int dwLogonProvider,
                                          out SafeTokenHandle phToken);

      [
        DllImport("kernel32.dll",
          EntryPoint = "CloseHandle", SetLastError = true,
          CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)
      ]
      public static extern bool CloseHandle(IntPtr handle);

      [
        DllImport("advapi32.dll",
          EntryPoint = "CreateProcessAsUser", SetLastError = true,
          CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)
      ]
      public static extern bool
        CreateProcessAsUser(IntPtr hToken,
                            string lpApplicationName,
                            string lpCommandLine,
                            ref SECURITY_ATTRIBUTES lpProcessAttributes,
                            ref SECURITY_ATTRIBUTES lpThreadAttributes,
                            bool bInheritHandle,
                            Int32 dwCreationFlags,
                            IntPtr lpEnvrionment,
                            string lpCurrentDirectory,
                            ref STARTUPINFO lpStartupInfo,
                            ref PROCESS_INFORMATION lpProcessInformation);

      [
        DllImport("advapi32.dll",
          EntryPoint = "DuplicateTokenEx")
      ]
      public static extern bool
        DuplicateTokenEx(IntPtr hExistingToken,
                         Int32 dwDesiredAccess,
                         ref SECURITY_ATTRIBUTES lpThreadAttributes,
                         Int32 impersonationLevel,
                         Int32 dwTokenType,
                         ref IntPtr phNewToken);

      public static Process CreateProcessAsUser(string filename, string args)
      {
        try
        {
          IntPtr hToken = WindowsIdentity.GetCurrent().Token;
          IntPtr hDupedToken = IntPtr.Zero;

          var pi = new PROCESS_INFORMATION();
          var sa = new SECURITY_ATTRIBUTES();
          sa.Length = Marshal.SizeOf(sa);

          try
          {
            if (!DuplicateTokenEx(
                                  hToken,
                                  GENERIC_ALL_ACCESS,
                                  ref sa,
                                  (int)SECURITY_IMPERSONATION_LEVEL.SecurityIdentification,
                                  (int)TOKEN_TYPE.TokenPrimary,
                                  ref hDupedToken
              ))
            {
              throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            var si = new STARTUPINFO();
            si.cb = Marshal.SizeOf(si);
            si.lpDesktop = "";

            string path = Path.GetFullPath(filename);
            string dir = Path.GetDirectoryName(path);

            // Revert to self to create the entire process; not doing this might
            // require that the currently impersonated user has "Replace a process
            // level token" rights - we only want our service account to need
            // that right.
            using (WindowsImpersonationContext ctx = WindowsIdentity.Impersonate(IntPtr.Zero))
            {
              if (!CreateProcessAsUser(
                                       hDupedToken,
                                       path,
                                       string.Format("\"{0}\" {1}", filename.Replace("\"", "\"\""), args),
                                       ref sa,
                                       ref sa,
                                       false,
                                       0,
                                       IntPtr.Zero,
                                       dir,
                                       ref si,
                                       ref pi
                ))
              {
                throw new Win32Exception(Marshal.GetLastWin32Error());
              }
            }

            return Process.GetProcessById(pi.dwProcessID);
          }
          finally
          {
            if (pi.hProcess != IntPtr.Zero)
            {
              CloseHandle(pi.hProcess);
            }
            if (pi.hThread != IntPtr.Zero)
            {
              CloseHandle(pi.hThread);
            }
            if (hDupedToken != IntPtr.Zero)
            {
              CloseHandle(hDupedToken);
            }
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      public static Process CreateProcessAsUser(string filename, string args, IntPtr hToken)
      {
        try
        {
          IntPtr hDupedToken = IntPtr.Zero;

          var pi = new PROCESS_INFORMATION();
          var sa = new SECURITY_ATTRIBUTES();
          sa.Length = Marshal.SizeOf(sa);

          try
          {
            var si = new STARTUPINFO();
            si.cb = Marshal.SizeOf(si);
            si.lpDesktop = "";

            string path = Path.GetFullPath(filename);
            string dir = Path.GetDirectoryName(path);

            if (!CreateProcessAsUser(hToken,
                                     path,
                                     string.Format("\"{0}\" {1}", filename.Replace("\"", "\"\""), args),
                                     ref sa,
                                     ref sa,
                                     false,
                                     0,
                                     IntPtr.Zero,
                                     dir,
                                     ref si,
                                     ref pi))
            {
              throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return Process.GetProcessById(pi.dwProcessID);
          }
          finally
          {
            if (pi.hProcess != IntPtr.Zero)
            {
              CloseHandle(pi.hProcess);
            }
            if (pi.hThread != IntPtr.Zero)
            {
              CloseHandle(pi.hThread);
            }
            if (hDupedToken != IntPtr.Zero)
            {
              CloseHandle(hDupedToken);
            }
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      [StructLayout(LayoutKind.Sequential)]
      public struct PROCESS_INFORMATION
      {
        public readonly IntPtr hProcess;
        public readonly IntPtr hThread;
        public readonly Int32 dwProcessID;
        public readonly Int32 dwThreadID;
      }

      [StructLayout(LayoutKind.Sequential)]
      public struct SECURITY_ATTRIBUTES
      {
        public Int32 Length;
        public readonly IntPtr lpSecurityDescriptor;
        public readonly bool bInheritHandle;
      }

      [StructLayout(LayoutKind.Sequential)]
      public struct STARTUPINFO
      {
        public Int32 cb;
        public readonly string lpReserved;
        public string lpDesktop;
        public readonly string lpTitle;
        public readonly Int32 dwX;
        public readonly Int32 dwY;
        public readonly Int32 dwXSize;
        public readonly Int32 dwXCountChars;
        public readonly Int32 dwYCountChars;
        public readonly Int32 dwFillAttribute;
        public readonly Int32 dwFlags;
        public readonly Int16 wShowWindow;
        public readonly Int16 cbReserved2;
        public readonly IntPtr lpReserved2;
        public readonly IntPtr hStdInput;
        public readonly IntPtr hStdOutput;
        public readonly IntPtr hStdError;
      }
    }

    public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
      private SafeTokenHandle()
        : base(true)
      {
      }

      [DllImport("kernel32.dll")]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      [SuppressUnmanagedCodeSecurity]
      [return: MarshalAs(UnmanagedType.Bool)]
      private static extern bool CloseHandle(IntPtr handle);

      protected override bool ReleaseHandle()
      {
        try
        {
          return CloseHandle(handle);
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return false;
        }
      }
    }
  }
}
