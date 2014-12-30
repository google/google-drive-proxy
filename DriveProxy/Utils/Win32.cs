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
using System.Runtime.InteropServices;
using System.Text;

namespace DriveProxy.Utils
{
  internal class Win32
  {
    private static List<char> _invalidFileNameChars;

    public static List<char> InvalidFileNameChars
    {
      get {
        return _invalidFileNameChars ??
               (_invalidFileNameChars = new List<char>(System.IO.Path.GetInvalidFileNameChars()));
      }
    }

    public static bool CenterOnParent(IntPtr parent, System.Windows.Forms.Form form)
    {
      try
      {
        if (parent == IntPtr.Zero)
        {
          return false;
        }

        System.Drawing.Rectangle rectangle;

        if (!GetWindowRect(parent, out rectangle))
        {
          form.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

          return false;
        }

        double parentWidth = rectangle.Width / (double)2;

        if (parentWidth < 1)
        {
          parentWidth = 1;
        }

        double formWidth = form.Width / (double)2;

        if (parentWidth < 1)
        {
          parentWidth = 1;
        }

        int left = Convert.ToInt32(parentWidth - formWidth);

        left = rectangle.Left + left;

        if (left < 0)
        {
          left = 0;
        }

        double parentHeight = rectangle.Height / (double)2;

        if (parentHeight < 1)
        {
          parentHeight = 1;
        }

        double formHeight = form.Height / (double)2;

        if (parentHeight < 1)
        {
          parentHeight = 1;
        }

        int top = Convert.ToInt32(parentHeight - formHeight);

        top = rectangle.Top + top;

        if (top < 0)
        {
          top = 0;
        }

        form.Left = left;
        form.Top = top;

        form.TopMost = true;

        return true;
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);

        return false;
      }
    }

    public static bool GetWindowRect(IntPtr hWnd, out System.Drawing.Rectangle rectangle)
    {
      rectangle = new System.Drawing.Rectangle();

      try
      {
        var rect = new Shell.RECT();

        if (!Shell.GetWindowRect(hWnd, out rect))
        {
          return false;
        }

        rectangle.X = rect.Left;
        rectangle.Y = rect.Top;
        rectangle.Width = rect.Right - rect.Left;
        rectangle.Height = rect.Bottom - rect.Top;

        return true;
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);

        return false;
      }
    }

    public static System.Drawing.Icon GetIcon(string filePath,
                                              int iconIndex,
                                              int width,
                                              int height,
                                              bool getClosestSize = true)
    {
      try
      {
        bool getLargeIcon = false;

        if (width >= 32 && height >= 32)
        {
          getLargeIcon = true;
        }

        return GetIcon(filePath, iconIndex, getLargeIcon);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public static System.Drawing.Icon GetIcon(string filePath, int iconIndex, bool getLargeIcon = true)
    {
      try
      {
        uint iconCount = 1;
        var largeIcons = new IntPtr[iconCount];
        var smallIcons = new IntPtr[iconCount];

        iconCount = Shell.ExtractIconEx(filePath, iconIndex, largeIcons, smallIcons, iconCount);

        if (iconCount == 0)
        {
          return null;
        }

        IntPtr hIcon = (getLargeIcon ? largeIcons[0] : smallIcons[0]);

        if (hIcon == null || hIcon == IntPtr.Zero)
        {
          return null;
        }

        // Copy (clone) the returned icon to a new object, thus allowing us to clean-up properly
        var icon = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(hIcon).Clone();
        Shell.DestroyIcon(hIcon); // Cleanup

        return icon;
      }
      catch (Exception exception)
      {
        Log.Error(exception, false);

        return null;
      }
    }

    public static string MakeFileNameWindowsSafe(string filename)
    {
      try
      {
        if (String.IsNullOrEmpty(filename))
        {
          return "";
        }

        foreach (char c in InvalidFileNameChars)
        {
          filename = filename.Replace(c, '_');
        }

        return filename;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public static bool GetFileAssociation(string fileExtension,
                                          ref string fileType,
                                          ref string application,
                                          ref string defaultIcon,
                                          ref string defaultIconIndex)
    {
      try
      {
        if (String.IsNullOrEmpty(fileExtension))
        {
          return false;
        }
        fileType = Shell.GetAssociation(Shell.AssocF.Verify, Shell.AssocStr.FriendlyDocName, fileExtension);

        if (String.IsNullOrEmpty(fileType))
        {
          fileType = fileExtension.ToUpper() + " File";
        }

        application = Shell.GetAssociation(Shell.AssocF.Verify, Shell.AssocStr.DDEApplication, fileExtension);
        defaultIcon = Shell.GetAssociation(Shell.AssocF.Verify, Shell.AssocStr.DEFAULTICON, fileExtension);

        defaultIcon = defaultIcon.Replace("\"", "");

        string iconPath = defaultIcon;
        int index = iconPath.IndexOf(",");

        if (index > 0)
        {
          defaultIcon = iconPath.Substring(0, index);
          defaultIconIndex = iconPath.Substring(index + 1);
        }
        else
        {
          defaultIconIndex = "";
        }

        return true;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    public static int SetForegroundWindow(IntPtr hwnd)
    {
      try
      {
        return Shell.SetForegroundWindow(hwnd);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public static bool IsSystemShuttingDown()
    {
      try
      {
        int result = Win32.Shell.GetSystemMetrics(Win32.Shell.SystemMetric.SM_SHUTTINGDOWN);

        if (result == 0)
        {
          return false;
        }

        return true;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    /// <summary>
    ///   It closes all specified windows folders.
    /// </summary>
    /// <param name="folderName">The name of the folder to be closed</param>
    public static void CloseFolders(String folderName)
    {
      string explorerPath = String.Empty;
      IntPtr hWnd = Win32.Shell.FindWindowExW(IntPtr.Zero, IntPtr.Zero, "CabinetWClass", null);
      while (!hWnd.Equals(IntPtr.Zero))
      {
        var windowName = new StringBuilder(Win32.Shell.MAX_FOLDER_NAME);
        Win32.Shell.GetWindowTextW(hWnd, windowName, Win32.Shell.MAX_FOLDER_NAME);

        Type typeFromProgId = Type.GetTypeFromProgID("Shell.Application");
        dynamic shellAplicationInstance = Activator.CreateInstance(typeFromProgId);
        try
        {
          dynamic explorerWindows = shellAplicationInstance.Windows();
          foreach (dynamic explorerWindow in explorerWindows)
          {
            if (explorerWindow == null || explorerWindow.hwnd != (long)hWnd)
            {
              continue;
            }
            string path = System.IO.Path.GetFileName((string)explorerWindow.FullName);
            if (path != null && path.ToLower() == "explorer.exe")
            {
              explorerPath = explorerWindow.document.focuseditem != null ? explorerWindow.document.focuseditem.path : UrlDecode(explorerWindow.LocationURL);
            }
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
        finally
        {
          Marshal.FinalReleaseComObject(shellAplicationInstance);
        }

        if (explorerPath.Contains(folderName))
        {
          Win32.Shell.PostMessageW(hWnd, Win32.Shell.WM_CLOSE, 0, 0);
        }
        hWnd = Win32.Shell.FindWindowExW(IntPtr.Zero, hWnd, "CabinetWClass", null);
      }
    }

    /// <summary>
    ///   Converts a string that has been encoded for transmission in a URL into a decoded string.
    /// </summary>
    /// <param name="srtUrl">String to be decoded</param>
    /// <returns></returns>
    private static string UrlDecode(string srtUrl)
    {
      return srtUrl.Replace("%20", " ");
    }

    private class FileAssoc
    {
      public string AssocF = "";
      public string AssocStr = "";
      public string Value = "";

      public override string ToString()
      {
        try
        {
          return AssocF + ":" + AssocStr + ":" + Value;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }
    }

    private class Shell
    {
      [Flags]
      public enum AssocF
      {
        Init_NoRemapCLSID = 0x1,
        Init_ByExeName = 0x2,
        Open_ByExeName = 0x2,
        Init_DefaultToStar = 0x4,
        Init_DefaultToFolder = 0x8,
        NoUserSettings = 0x10,
        NoTruncate = 0x20,
        Verify = 0x40,
        RemapRunDll = 0x80,
        NoFixUps = 0x100,
        IgnoreBaseClass = 0x200
      }

      public enum AssocStr
      {
        Command = 1,
        Executable,
        FriendlyDocName,
        FriendlyAppName,
        NoOpen,
        ShellNewValue,
        DDECommand,
        DDEIfExec,
        DDEApplication,
        DDETopic,
        INFOTIP,
        QUICKTIP,
        TILEINFO,
        CONTENTTYPE,
        DEFAULTICON,
        SHELLEXTENSION,
        DROPTARGET,
        DELEGATEEXECUTE,
        SUPPORTED_URI_PROTOCOLS,
        MAX
      }

      [Flags]
      public enum SHGFI
      {
        /// <summary>get icon</summary>
        Icon = 0x000000100,

        /// <summary>get display name</summary>
        DisplayName = 0x000000200,

        /// <summary>get type name</summary>
        TypeName = 0x000000400,

        /// <summary>get attributes</summary>
        Attributes = 0x000000800,

        /// <summary>get icon location</summary>
        IconLocation = 0x000001000,

        /// <summary>return exe type</summary>
        ExeType = 0x000002000,

        /// <summary>get system icon index</summary>
        SysIconIndex = 0x000004000,

        /// <summary>put a link overlay on icon</summary>
        LinkOverlay = 0x000008000,

        /// <summary>show icon in selected state</summary>
        Selected = 0x000010000,

        /// <summary>get only specified attributes</summary>
        Attr_Specified = 0x000020000,

        /// <summary>get large icon</summary>
        LargeIcon = 0x000000000,

        /// <summary>get small icon</summary>
        SmallIcon = 0x000000001,

        /// <summary>get open icon</summary>
        OpenIcon = 0x000000002,

        /// <summary>get shell size icon</summary>
        ShellIconSize = 0x000000004,

        /// <summary>pszPath is a pidl</summary>
        PIDL = 0x000000008,

        /// <summary>use passed dwFileAttribute</summary>
        UseFileAttributes = 0x000000010,

        /// <summary>apply the appropriate overlays</summary>
        AddOverlays = 0x000000020,

        /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
        OverlayIndex = 0x000000040,
      }

      /// <summary>
      ///   Flags used with the Windows API (User32.dll):GetSystemMetrics(SystemMetric smIndex)
      ///   This Enum and declaration signature was written by Gabriel T. Sharp
      ///   ai_productions@verizon.net or osirisgothra@hotmail.com
      ///   Obtained on pinvoke.net, please contribute your code to support the wiki!
      /// </summary>
      public enum SystemMetric
      {
        /// <summary>
        ///   The flags that specify how the system arranged minimized windows. For more information, see the Remarks section in
        ///   this topic.
        /// </summary>
        SM_ARRANGE = 56,

        /// <summary>
        ///   The value that specifies how the system is started:
        ///   0 Normal boot
        ///   1 Fail-safe boot
        ///   2 Fail-safe with network boot
        ///   A fail-safe boot (also called SafeBoot, Safe Mode, or Clean Boot) bypasses the user startup files.
        /// </summary>
        SM_CLEANBOOT = 67,

        /// <summary>
        ///   The number of display monitors on a desktop. For more information, see the Remarks section in this topic.
        /// </summary>
        SM_CMONITORS = 80,

        /// <summary>
        ///   The number of buttons on a mouse, or zero if no mouse is installed.
        /// </summary>
        SM_CMOUSEBUTTONS = 43,

        /// <summary>
        ///   The width of a window border, in pixels. This is equivalent to the SM_CXEDGE value for windows with the 3-D look.
        /// </summary>
        SM_CXBORDER = 5,

        /// <summary>
        ///   The width of a cursor, in pixels. The system cannot create cursors of other sizes.
        /// </summary>
        SM_CXCURSOR = 13,

        /// <summary>
        ///   This value is the same as SM_CXFIXEDFRAME.
        /// </summary>
        SM_CXDLGFRAME = 7,

        /// <summary>
        ///   The width of the rectangle around the location of a first click in a double-click sequence, in pixels. ,
        ///   The second click must occur within the rectangle that is defined by SM_CXDOUBLECLK and SM_CYDOUBLECLK for the system
        ///   to consider the two clicks a double-click. The two clicks must also occur within a specified time.
        ///   To set the width of the double-click rectangle, call SystemParametersInfo with SPI_SETDOUBLECLKWIDTH.
        /// </summary>
        SM_CXDOUBLECLK = 36,

        /// <summary>
        ///   The number of pixels on either side of a mouse-down point that the mouse pointer can move before a drag operation
        ///   begins.
        ///   This allows the user to click and release the mouse button easily without unintentionally starting a drag operation.
        ///   If this value is negative, it is subtracted from the left of the mouse-down point and added to the right of it.
        /// </summary>
        SM_CXDRAG = 68,

        /// <summary>
        ///   The width of a 3-D border, in pixels. This metric is the 3-D counterpart of SM_CXBORDER.
        /// </summary>
        SM_CXEDGE = 45,

        /// <summary>
        ///   The thickness of the frame around the perimeter of a window that has a caption but is not sizable, in pixels.
        ///   SM_CXFIXEDFRAME is the height of the horizontal border, and SM_CYFIXEDFRAME is the width of the vertical border.
        ///   This value is the same as SM_CXDLGFRAME.
        /// </summary>
        SM_CXFIXEDFRAME = 7,

        /// <summary>
        ///   The width of the left and right edges of the focus rectangle that the DrawFocusRectdraws.
        ///   This value is in pixels.
        ///   Windows 2000:  This value is not supported.
        /// </summary>
        SM_CXFOCUSBORDER = 83,

        /// <summary>
        ///   This value is the same as SM_CXSIZEFRAME.
        /// </summary>
        SM_CXFRAME = 32,

        /// <summary>
        ///   The width of the client area for a full-screen window on the primary display monitor, in pixels.
        ///   To get the coordinates of the portion of the screen that is not obscured by the system taskbar or by application
        ///   desktop toolbars,
        ///   call the SystemParametersInfofunction with the SPI_GETWORKAREA value.
        /// </summary>
        SM_CXFULLSCREEN = 16,

        /// <summary>
        ///   The width of the arrow bitmap on a horizontal scroll bar, in pixels.
        /// </summary>
        SM_CXHSCROLL = 21,

        /// <summary>
        ///   The width of the thumb box in a horizontal scroll bar, in pixels.
        /// </summary>
        SM_CXHTHUMB = 10,

        /// <summary>
        ///   The default width of an icon, in pixels. The LoadIcon function can load only icons with the dimensions
        ///   that SM_CXICON and SM_CYICON specifies.
        /// </summary>
        SM_CXICON = 11,

        /// <summary>
        ///   The width of a grid cell for items in large icon view, in pixels. Each item fits into a rectangle of size
        ///   SM_CXICONSPACING by SM_CYICONSPACING when arranged. This value is always greater than or equal to SM_CXICON.
        /// </summary>
        SM_CXICONSPACING = 38,

        /// <summary>
        ///   The default width, in pixels, of a maximized top-level window on the primary display monitor.
        /// </summary>
        SM_CXMAXIMIZED = 61,

        /// <summary>
        ///   The default maximum width of a window that has a caption and sizing borders, in pixels.
        ///   This metric refers to the entire desktop. The user cannot drag the window frame to a size larger than these
        ///   dimensions.
        ///   A window can override this value by processing the WM_GETMINMAXINFO message.
        /// </summary>
        SM_CXMAXTRACK = 59,

        /// <summary>
        ///   The width of the default menu check-mark bitmap, in pixels.
        /// </summary>
        SM_CXMENUCHECK = 71,

        /// <summary>
        ///   The width of menu bar buttons, such as the child window close button that is used in the multiple document interface,
        ///   in pixels.
        /// </summary>
        SM_CXMENUSIZE = 54,

        /// <summary>
        ///   The minimum width of a window, in pixels.
        /// </summary>
        SM_CXMIN = 28,

        /// <summary>
        ///   The width of a minimized window, in pixels.
        /// </summary>
        SM_CXMINIMIZED = 57,

        /// <summary>
        ///   The width of a grid cell for a minimized window, in pixels. Each minimized window fits into a rectangle this size
        ///   when arranged.
        ///   This value is always greater than or equal to SM_CXMINIMIZED.
        /// </summary>
        SM_CXMINSPACING = 47,

        /// <summary>
        ///   The minimum tracking width of a window, in pixels. The user cannot drag the window frame to a size smaller than these
        ///   dimensions.
        ///   A window can override this value by processing the WM_GETMINMAXINFO message.
        /// </summary>
        SM_CXMINTRACK = 34,

        /// <summary>
        ///   The amount of border padding for captioned windows, in pixels. Windows XP/2000:  This value is not supported.
        /// </summary>
        SM_CXPADDEDBORDER = 92,

        /// <summary>
        ///   The width of the screen of the primary display monitor, in pixels. This is the same value obtained by calling
        ///   GetDeviceCaps as follows: GetDeviceCaps( hdcPrimaryMonitor, HORZRES).
        /// </summary>
        SM_CXSCREEN = 0,

        /// <summary>
        ///   The width of a button in a window caption or title bar, in pixels.
        /// </summary>
        SM_CXSIZE = 30,

        /// <summary>
        ///   The thickness of the sizing border around the perimeter of a window that can be resized, in pixels.
        ///   SM_CXSIZEFRAME is the width of the horizontal border, and SM_CYSIZEFRAME is the height of the vertical border.
        ///   This value is the same as SM_CXFRAME.
        /// </summary>
        SM_CXSIZEFRAME = 32,

        /// <summary>
        ///   The recommended width of a small icon, in pixels. Small icons typically appear in window captions and in small icon
        ///   view.
        /// </summary>
        SM_CXSMICON = 49,

        /// <summary>
        ///   The width of small caption buttons, in pixels.
        /// </summary>
        SM_CXSMSIZE = 52,

        /// <summary>
        ///   The width of the virtual screen, in pixels. The virtual screen is the bounding rectangle of all display monitors.
        ///   The SM_XVIRTUALSCREEN metric is the coordinates for the left side of the virtual screen.
        /// </summary>
        SM_CXVIRTUALSCREEN = 78,

        /// <summary>
        ///   The width of a vertical scroll bar, in pixels.
        /// </summary>
        SM_CXVSCROLL = 2,

        /// <summary>
        ///   The height of a window border, in pixels. This is equivalent to the SM_CYEDGE value for windows with the 3-D look.
        /// </summary>
        SM_CYBORDER = 6,

        /// <summary>
        ///   The height of a caption area, in pixels.
        /// </summary>
        SM_CYCAPTION = 4,

        /// <summary>
        ///   The height of a cursor, in pixels. The system cannot create cursors of other sizes.
        /// </summary>
        SM_CYCURSOR = 14,

        /// <summary>
        ///   This value is the same as SM_CYFIXEDFRAME.
        /// </summary>
        SM_CYDLGFRAME = 8,

        /// <summary>
        ///   The height of the rectangle around the location of a first click in a double-click sequence, in pixels.
        ///   The second click must occur within the rectangle defined by SM_CXDOUBLECLK and SM_CYDOUBLECLK for the system to
        ///   consider
        ///   the two clicks a double-click. The two clicks must also occur within a specified time. To set the height of the
        ///   double-click
        ///   rectangle, call SystemParametersInfo with SPI_SETDOUBLECLKHEIGHT.
        /// </summary>
        SM_CYDOUBLECLK = 37,

        /// <summary>
        ///   The number of pixels above and below a mouse-down point that the mouse pointer can move before a drag operation
        ///   begins.
        ///   This allows the user to click and release the mouse button easily without unintentionally starting a drag operation.
        ///   If this value is negative, it is subtracted from above the mouse-down point and added below it.
        /// </summary>
        SM_CYDRAG = 69,

        /// <summary>
        ///   The height of a 3-D border, in pixels. This is the 3-D counterpart of SM_CYBORDER.
        /// </summary>
        SM_CYEDGE = 46,

        /// <summary>
        ///   The thickness of the frame around the perimeter of a window that has a caption but is not sizable, in pixels.
        ///   SM_CXFIXEDFRAME is the height of the horizontal border, and SM_CYFIXEDFRAME is the width of the vertical border.
        ///   This value is the same as SM_CYDLGFRAME.
        /// </summary>
        SM_CYFIXEDFRAME = 8,

        /// <summary>
        ///   The height of the top and bottom edges of the focus rectangle drawn byDrawFocusRect.
        ///   This value is in pixels.
        ///   Windows 2000:  This value is not supported.
        /// </summary>
        SM_CYFOCUSBORDER = 84,

        /// <summary>
        ///   This value is the same as SM_CYSIZEFRAME.
        /// </summary>
        SM_CYFRAME = 33,

        /// <summary>
        ///   The height of the client area for a full-screen window on the primary display monitor, in pixels.
        ///   To get the coordinates of the portion of the screen not obscured by the system taskbar or by application desktop
        ///   toolbars,
        ///   call the SystemParametersInfo function with the SPI_GETWORKAREA value.
        /// </summary>
        SM_CYFULLSCREEN = 17,

        /// <summary>
        ///   The height of a horizontal scroll bar, in pixels.
        /// </summary>
        SM_CYHSCROLL = 3,

        /// <summary>
        ///   The default height of an icon, in pixels. The LoadIcon function can load only icons with the dimensions SM_CXICON and
        ///   SM_CYICON.
        /// </summary>
        SM_CYICON = 12,

        /// <summary>
        ///   The height of a grid cell for items in large icon view, in pixels. Each item fits into a rectangle of size
        ///   SM_CXICONSPACING by SM_CYICONSPACING when arranged. This value is always greater than or equal to SM_CYICON.
        /// </summary>
        SM_CYICONSPACING = 39,

        /// <summary>
        ///   For double byte character set versions of the system, this is the height of the Kanji window at the bottom of the
        ///   screen, in pixels.
        /// </summary>
        SM_CYKANJIWINDOW = 18,

        /// <summary>
        ///   The default height, in pixels, of a maximized top-level window on the primary display monitor.
        /// </summary>
        SM_CYMAXIMIZED = 62,

        /// <summary>
        ///   The default maximum height of a window that has a caption and sizing borders, in pixels. This metric refers to the
        ///   entire desktop.
        ///   The user cannot drag the window frame to a size larger than these dimensions. A window can override this value by
        ///   processing
        ///   the WM_GETMINMAXINFO message.
        /// </summary>
        SM_CYMAXTRACK = 60,

        /// <summary>
        ///   The height of a single-line menu bar, in pixels.
        /// </summary>
        SM_CYMENU = 15,

        /// <summary>
        ///   The height of the default menu check-mark bitmap, in pixels.
        /// </summary>
        SM_CYMENUCHECK = 72,

        /// <summary>
        ///   The height of menu bar buttons, such as the child window close button that is used in the multiple document
        ///   interface, in pixels.
        /// </summary>
        SM_CYMENUSIZE = 55,

        /// <summary>
        ///   The minimum height of a window, in pixels.
        /// </summary>
        SM_CYMIN = 29,

        /// <summary>
        ///   The height of a minimized window, in pixels.
        /// </summary>
        SM_CYMINIMIZED = 58,

        /// <summary>
        ///   The height of a grid cell for a minimized window, in pixels. Each minimized window fits into a rectangle this size
        ///   when arranged.
        ///   This value is always greater than or equal to SM_CYMINIMIZED.
        /// </summary>
        SM_CYMINSPACING = 48,

        /// <summary>
        ///   The minimum tracking height of a window, in pixels. The user cannot drag the window frame to a size smaller than
        ///   these dimensions.
        ///   A window can override this value by processing the WM_GETMINMAXINFO message.
        /// </summary>
        SM_CYMINTRACK = 35,

        /// <summary>
        ///   The height of the screen of the primary display monitor, in pixels. This is the same value obtained by calling
        ///   GetDeviceCaps as follows: GetDeviceCaps( hdcPrimaryMonitor, VERTRES).
        /// </summary>
        SM_CYSCREEN = 1,

        /// <summary>
        ///   The height of a button in a window caption or title bar, in pixels.
        /// </summary>
        SM_CYSIZE = 31,

        /// <summary>
        ///   The thickness of the sizing border around the perimeter of a window that can be resized, in pixels.
        ///   SM_CXSIZEFRAME is the width of the horizontal border, and SM_CYSIZEFRAME is the height of the vertical border.
        ///   This value is the same as SM_CYFRAME.
        /// </summary>
        SM_CYSIZEFRAME = 33,

        /// <summary>
        ///   The height of a small caption, in pixels.
        /// </summary>
        SM_CYSMCAPTION = 51,

        /// <summary>
        ///   The recommended height of a small icon, in pixels. Small icons typically appear in window captions and in small icon
        ///   view.
        /// </summary>
        SM_CYSMICON = 50,

        /// <summary>
        ///   The height of small caption buttons, in pixels.
        /// </summary>
        SM_CYSMSIZE = 53,

        /// <summary>
        ///   The height of the virtual screen, in pixels. The virtual screen is the bounding rectangle of all display monitors.
        ///   The SM_YVIRTUALSCREEN metric is the coordinates for the top of the virtual screen.
        /// </summary>
        SM_CYVIRTUALSCREEN = 79,

        /// <summary>
        ///   The height of the arrow bitmap on a vertical scroll bar, in pixels.
        /// </summary>
        SM_CYVSCROLL = 20,

        /// <summary>
        ///   The height of the thumb box in a vertical scroll bar, in pixels.
        /// </summary>
        SM_CYVTHUMB = 9,

        /// <summary>
        ///   Nonzero if User32.dll supports DBCS; otherwise, 0.
        /// </summary>
        SM_DBCSENABLED = 42,

        /// <summary>
        ///   Nonzero if the debug version of User.exe is installed; otherwise, 0.
        /// </summary>
        SM_DEBUG = 22,

        /// <summary>
        ///   Nonzero if the current operating system is Windows 7 or Windows Server 2008 R2 and the Tablet PC Input
        ///   service is started; otherwise, 0. The return value is a bitmask that specifies the type of digitizer input supported
        ///   by the device.
        ///   For more information, see Remarks.
        ///   Windows Server 2008, Windows Vista, and Windows XP/2000:  This value is not supported.
        /// </summary>
        SM_DIGITIZER = 94,

        /// <summary>
        ///   Nonzero if Input Method Manager/Input Method Editor features are enabled; otherwise, 0.
        ///   SM_IMMENABLED indicates whether the system is ready to use a Unicode-based IME on a Unicode application.
        ///   To ensure that a language-dependent IME works, check SM_DBCSENABLED and the system ANSI code page.
        ///   Otherwise the ANSI-to-Unicode conversion may not be performed correctly, or some components like fonts
        ///   or registry settings may not be present.
        /// </summary>
        SM_IMMENABLED = 82,

        /// <summary>
        ///   Nonzero if there are digitizers in the system; otherwise, 0. SM_MAXIMUMTOUCHES returns the aggregate maximum of the
        ///   maximum number of contacts supported by every digitizer in the system. If the system has only single-touch
        ///   digitizers,
        ///   the return value is 1. If the system has multi-touch digitizers, the return value is the number of simultaneous
        ///   contacts
        ///   the hardware can provide. Windows Server 2008, Windows Vista, and Windows XP/2000:  This value is not supported.
        /// </summary>
        SM_MAXIMUMTOUCHES = 95,

        /// <summary>
        ///   Nonzero if the current operating system is the Windows XP, Media Center Edition, 0 if not.
        /// </summary>
        SM_MEDIACENTER = 87,

        /// <summary>
        ///   Nonzero if drop-down menus are right-aligned with the corresponding menu-bar item; 0 if the menus are left-aligned.
        /// </summary>
        SM_MENUDROPALIGNMENT = 40,

        /// <summary>
        ///   Nonzero if the system is enabled for Hebrew and Arabic languages, 0 if not.
        /// </summary>
        SM_MIDEASTENABLED = 74,

        /// <summary>
        ///   Nonzero if a mouse is installed; otherwise, 0. This value is rarely zero, because of support for virtual mice and
        ///   because
        ///   some systems detect the presence of the port instead of the presence of a mouse.
        /// </summary>
        SM_MOUSEPRESENT = 19,

        /// <summary>
        ///   Nonzero if a mouse with a horizontal scroll wheel is installed; otherwise 0.
        /// </summary>
        SM_MOUSEHORIZONTALWHEELPRESENT = 91,

        /// <summary>
        ///   Nonzero if a mouse with a vertical scroll wheel is installed; otherwise 0.
        /// </summary>
        SM_MOUSEWHEELPRESENT = 75,

        /// <summary>
        ///   The least significant bit is set if a network is present; otherwise, it is cleared. The other bits are reserved for
        ///   future use.
        /// </summary>
        SM_NETWORK = 63,

        /// <summary>
        ///   Nonzero if the Microsoft Windows for Pen computing extensions are installed; zero otherwise.
        /// </summary>
        SM_PENWINDOWS = 41,

        /// <summary>
        ///   This system metric is used in a Terminal Services environment to determine if the current Terminal Server session is
        ///   being remotely controlled. Its value is nonzero if the current session is remotely controlled; otherwise, 0.
        ///   You can use terminal services management tools such as Terminal Services Manager (tsadmin.msc) and shadow.exe to
        ///   control a remote session. When a session is being remotely controlled, another user can view the contents of that
        ///   session
        ///   and potentially interact with it.
        /// </summary>
        SM_REMOTECONTROL = 0x2001,

        /// <summary>
        ///   This system metric is used in a Terminal Services environment. If the calling process is associated with a Terminal
        ///   Services
        ///   client session, the return value is nonzero. If the calling process is associated with the Terminal Services console
        ///   session,
        ///   the return value is 0.
        ///   Windows Server 2003 and Windows XP:  The console session is not necessarily the physical console.
        ///   For more information, seeWTSGetActiveConsoleSessionId.
        /// </summary>
        SM_REMOTESESSION = 0x1000,

        /// <summary>
        ///   Nonzero if all the display monitors have the same color format, otherwise, 0. Two displays can have the same bit
        ///   depth,
        ///   but different color formats. For example, the red, green, and blue pixels can be encoded with different numbers of
        ///   bits,
        ///   or those bits can be located in different places in a pixel color value.
        /// </summary>
        SM_SAMEDISPLAYFORMAT = 81,

        /// <summary>
        ///   This system metric should be ignored; it always returns 0.
        /// </summary>
        SM_SECURE = 44,

        /// <summary>
        ///   The build number if the system is Windows Server 2003 R2; otherwise, 0.
        /// </summary>
        SM_SERVERR2 = 89,

        /// <summary>
        ///   Nonzero if the user requires an application to present information visually in situations where it would otherwise
        ///   present
        ///   the information only in audible form; otherwise, 0.
        /// </summary>
        SM_SHOWSOUNDS = 70,

        /// <summary>
        ///   Nonzero if the current session is shutting down; otherwise, 0. Windows 2000:  This value is not supported.
        /// </summary>
        SM_SHUTTINGDOWN = 0x2000,

        /// <summary>
        ///   Nonzero if the computer has a low-end (slow) processor; otherwise, 0.
        /// </summary>
        SM_SLOWMACHINE = 73,

        /// <summary>
        ///   Nonzero if the current operating system is Windows 7 Starter Edition, Windows Vista Starter, or Windows XP Starter
        ///   Edition; otherwise, 0.
        /// </summary>
        SM_STARTER = 88,

        /// <summary>
        ///   Nonzero if the meanings of the left and right mouse buttons are swapped; otherwise, 0.
        /// </summary>
        SM_SWAPBUTTON = 23,

        /// <summary>
        ///   Nonzero if the current operating system is the Windows XP Tablet PC edition or if the current operating system is
        ///   Windows Vista
        ///   or Windows 7 and the Tablet PC Input service is started; otherwise, 0. The SM_DIGITIZER setting indicates the type of
        ///   digitizer
        ///   input supported by a device running Windows 7 or Windows Server 2008 R2. For more information, see Remarks.
        /// </summary>
        SM_TABLETPC = 86,

        /// <summary>
        ///   The coordinates for the left side of the virtual screen. The virtual screen is the bounding rectangle of all display
        ///   monitors.
        ///   The SM_CXVIRTUALSCREEN metric is the width of the virtual screen.
        /// </summary>
        SM_XVIRTUALSCREEN = 76,

        /// <summary>
        ///   The coordinates for the top of the virtual screen. The virtual screen is the bounding rectangle of all display
        ///   monitors.
        ///   The SM_CYVIRTUALSCREEN metric is the height of the virtual screen.
        /// </summary>
        SM_YVIRTUALSCREEN = 77,
      }

      public static int GWL_STYLE = -16;
      public static long WS_CHILD = 0x40000000;
      public static long WS_POPUP = 0x80000000;
      //Send as a signal that a window or an application should terminate.
      public static uint WM_CLOSE = 0x10;
      //The maximum length of a folder name.
      public static int MAX_FOLDER_NAME = 255;

      [DllImport("user32.dll")]
      public static extern int GetSystemMetrics(SystemMetric smIndex);

      [DllImport("shell32.dll", EntryPoint = "ExtractIconA", CharSet = CharSet.Ansi, SetLastError = true,
        ExactSpelling = true)]
      public static extern IntPtr ExtractIcon(int hInst, string lpszExeFileName, int nIconIndex);

      [DllImport("shell32.dll", CharSet = CharSet.Auto)]
      public static extern uint ExtractIconEx(string szFileName,
                                              int nIconIndex,
                                              IntPtr[] phiconLarge,
                                              IntPtr[] phiconSmall,
                                              uint nIcons);

      [DllImport("user32")]
      public static extern int SetForegroundWindow(IntPtr hwnd);

      [DllImport("user32.dll")]
      public static extern long SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);

      [DllImport("user32.dll")]
      public static extern long GetWindowLong(IntPtr hWnd, int nIndex);

      [DllImport("user32.dll")]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

      [DllImport("user32.dll", SetLastError = true)]
      public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

      [DllImport("user32.dll", SetLastError = true)]
      public static extern IntPtr GetWindow(IntPtr hWndChild, uint uCmd);

      //Retrieves a handle to a window whose class name and window name match the specified strings.
      [DllImport("user32.dll", EntryPoint = "FindWindowExW")]
      public static extern IntPtr FindWindowExW(IntPtr hwndParent,
                                                IntPtr hwndChildAfter,
                                                [In, MarshalAs(UnmanagedType.LPTStr)] String lpszClass,
                                                [In, MarshalAs(UnmanagedType.LPTStr)] String lpszWindow);

      //Posts a message in the message queue associated with the thread that created the specified window and returns
      //without waiting for the thread to process the message.
      [DllImport("user32.dll", EntryPoint = "PostMessageW")]
      public static extern IntPtr PostMessageW([In] IntPtr hWnd, uint msg, int wParam, int lParam);

      //Copies the text of the specified window's title bar (if it has one) into a buffer.
      //If the specified window is a control, the text of the control is copied.
      [DllImport("user32.dll", EntryPoint = "GetWindowTextW")]
      public static extern int GetWindowTextW([In] IntPtr hWnd,
                                              [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString,
                                              int nMaxCount);

      public static string GetAssociation(AssocF assocF, AssocStr assocStr, string doctype)
      {
        try
        {
          uint pcchOut = 0; // size of output buffer

          // First call is to get the required size of output buffer
          AssocQueryString(assocF, assocStr, doctype, null, null, ref pcchOut);

          if (pcchOut == 0)
          {
            return String.Empty;
          }

          // Allocate the output buffer
          var pszOut = new StringBuilder((int)pcchOut);

          // Get the full pathname to the program in pszOut
          AssocQueryString(assocF, assocStr, doctype, null, pszOut, ref pcchOut);

          string doc = pszOut.ToString();
          return doc;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      public static bool GetLargeIcon(string strPath, ref string szDisplayName, ref string szTypeName, ref int iIcon)
      {
        try
        {
          return GetIcon(strPath, false, ref szDisplayName, ref szTypeName, ref iIcon);
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return false;
        }
      }

      public static bool GetSmallIcon(string strPath,
                                      bool bSmall,
                                      ref string szDisplayName,
                                      ref string szTypeName,
                                      ref int iIcon)
      {
        try
        {
          return GetIcon(strPath, true, ref szDisplayName, ref szTypeName, ref iIcon);
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return false;
        }
      }

      public static bool GetIcon(string strPath,
                                 bool bSmall,
                                 ref string szDisplayName,
                                 ref string szTypeName,
                                 ref int iIcon)
      {
        try
        {
          if (String.IsNullOrEmpty(strPath))
          {
            return false;
          }

          if (!System.IO.File.Exists(strPath))
          {
            return false;
          }

          uint dwFileAttributes = 0;
          var info = new SHFILEINFO();
          var cbFileInfo = (uint)Marshal.SizeOf(info);
          SHGFI flags;

          if (bSmall)
          {
            flags = SHGFI.Icon | SHGFI.SmallIcon | SHGFI.UseFileAttributes;
          }
          else
          {
            flags = SHGFI.Icon | SHGFI.LargeIcon | SHGFI.UseFileAttributes;
          }

          IntPtr intPtr = SHGetFileInfo(strPath, dwFileAttributes, ref info, cbFileInfo, (uint)flags);

          szDisplayName = info.szDisplayName;
          szTypeName = info.szTypeName;
          iIcon = info.iIcon;

          if (intPtr != IntPtr.Zero)
          {
            DestroyIcon(info.hIcon);
          }

          return true;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return false;
        }
      }

      [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
      private static extern uint AssocQueryString(AssocF flags,
                                                  AssocStr str,
                                                  string pszAssoc,
                                                  string pszExtra,
                                                  [Out] StringBuilder pszOut,
                                                  [In] [Out] ref uint pcchOut);

      [DllImport("shell32.dll", CharSet = CharSet.Auto)]
      private static extern IntPtr SHGetFileInfo(string pszPath,
                                                 uint dwFileAttributes,
                                                 ref SHFILEINFO psfi,
                                                 uint cbFileInfo,
                                                 uint uFlags);

      [DllImport("User32.dll", CharSet = CharSet.Auto)]
      public static extern int DestroyIcon(IntPtr hIcon);

      [DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true,
        CallingConvention = CallingConvention.StdCall)]
      private static extern int ExtractIconEx(string sFile,
                                              int iIndex,
                                              out IntPtr piLargeVersion,
                                              out IntPtr piSmallVersion,
                                              int amountIcons);

      [StructLayout(LayoutKind.Sequential)]
      public struct RECT
      {
        public readonly int Left; // x position of upper-left corner
        public readonly int Top; // y position of upper-left corner
        public readonly int Right; // x position of lower-right corner
        public readonly int Bottom; // y position of lower-right corner
      }

      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
      public struct SHFILEINFO
      {
        public readonly IntPtr hIcon;
        public readonly int iIcon;
        public readonly uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public readonly string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public readonly string szTypeName;
      };
    }
  }
}
