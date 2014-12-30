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
using System.IO;
using System.Runtime.InteropServices;

namespace DriveProxy.Utils
{
  public abstract class Pipe : IDisposable
  {
    protected IntPtr _handle = IntPtr.Zero;
    protected bool _isConnected = false;
    protected FileAccess _mode;

    protected string _name;
    protected TimeSpan readTimeout = new TimeSpan(0, 0, 30);

    protected Pipe(string name, FileAccess mode, IntPtr handle)
    {
      _name = name;
      _handle = handle;
      _mode = mode;
    }

    public IntPtr Handle
    {
      get { return _handle; }
      set { _handle = value; }
    }

    public string Name
    {
      get { return _name; }
    }

    public FileAccess Mode
    {
      get { return _mode; }
    }

    public TimeSpan ReadTimeout
    {
      get { return readTimeout; }
      set { readTimeout = value; }
    }

    public bool IsCreated
    {
      get
      {
        if (_handle != IntPtr.Zero)
        {
          return true;
        }

        return false;
      }
    }

    public bool IsConnected
    {
      get { return _isConnected; }
    }

    public bool CanRead
    {
      get
      {
        if (Mode == FileAccess.Read ||
            Mode == FileAccess.ReadWrite)
        {
          return true;
        }

        return false;
      }
    }

    public bool CanWrite
    {
      get
      {
        if (Mode == FileAccess.Write ||
            Mode == FileAccess.ReadWrite)
        {
          return true;
        }

        return false;
      }
    }

    public virtual void Dispose()
    {
      try
      {
        Close();
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public int Peek()
    {
      try
      {
        int bytesRead = 0;
        int bytesAvail = 0;
        int bytesLeft = 0;

        if (!Peek(ref bytesRead, ref bytesAvail, ref bytesLeft))
        {
          return 0;
        }

        return bytesAvail;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public bool Peek(ref int bytesRead, ref int bytesAvail, ref int bytesLeft)
    {
      try
      {
        if (_handle == IntPtr.Zero)
        {
          throw new Exception("The pipe has been closed.");
        }

        if (!CanRead)
        {
          throw new Exception("The pipe is not configured for reading.");
        }

        uint bytesRead2 = 0;
        uint bytesAvail2 = 0;
        uint bytesLeft2 = 0;

        if (!Win32.PeekNamedPipe(_handle, null, 0, ref bytesRead2, ref bytesAvail2, ref bytesLeft2))
        {
          int err = Marshal.GetLastWin32Error();

          throw new Exception(string.Format("Pipe.Peek failed, win32 error code {0}, pipe name '{1}' ", err, Name));
        }

        if (bytesRead2 > int.MaxValue)
        {
          bytesRead2 = int.MaxValue;
        }

        if (bytesAvail > int.MaxValue)
        {
          bytesAvail = int.MaxValue;
        }

        if (bytesLeft > int.MaxValue)
        {
          bytesLeft = int.MaxValue;
        }

        bytesRead = (int)bytesRead2;
        bytesAvail = (int)bytesAvail2;
        bytesLeft = (int)bytesLeft2;

        if (bytesAvail > 0)
        {
          return true;
        }

        return false;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return false;
      }
    }

    public byte[] Read()
    {
      try
      {
        return Read(0);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public byte[] Read(int bufferSize)
    {
      try
      {
        int bytesAvail = 0;
        double waitInMilliseconds = ReadTimeout.TotalMilliseconds;
        double waited = 0;

        while (true)
        {
          bytesAvail = Peek();

          if (bytesAvail >= bufferSize)
          {
            break;
          }
          int sleep = 10;

          if (waitInMilliseconds > 50)
          {
            sleep = 50;
          }

          System.Threading.Thread.Sleep(sleep);

          waited += sleep;
        }

        if (bufferSize > 0)
        {
          bytesAvail = bufferSize;
        }

        var bytes = new byte[bytesAvail];

        int bytesRead = Read(bytes);

        if (bytesRead < bytes.Length)
        {
          throw new Exception("Read data is less than requested.");
        }

        return bytes;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }

    public int Read(byte[] buffer)
    {
      try
      {
        return Read(buffer, 0, buffer.Length);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public int Read(byte[] buffer, int offset, int count)
    {
      try
      {
        if (_handle == IntPtr.Zero)
        {
          throw new Exception("The pipe has been closed.");
        }

        if (!CanRead)
        {
          throw new Exception("The pipe is not configured for reading.");
        }

        if (buffer == null)
        {
          throw new Exception("The pipe buffer for reading is null.");
        }

        if (offset < 0)
        {
          throw new Exception("The pipe buffer for reading offset is less than zero.");
        }

        if (count < 0)
        {
          throw new Exception("The pipe buffer for reading count is less than zero.");
        }

        if (buffer.Length < (offset + count))
        {
          throw new Exception("The pipe buffer for reading is not large enough to hold requested data.");
        }

        byte[] buf = buffer;

        if (offset != 0)
        {
          buf = new byte[count];
        }

        uint read = 0;

        if (!Win32.ReadFile(_handle, buf, (uint)count, ref read, IntPtr.Zero))
        {
          int err = Marshal.GetLastWin32Error();

          throw new Exception(string.Format("Pipe.Read failed, win32 error code {0}, pipe name '{1}' ", err, Name));
        }

        if (offset != 0)
        {
          for (int x = 0; x < read; x++)
          {
            buffer[offset + x] = buf[x];
          }
        }

        return (int)read;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public int Write(byte[] buffer)
    {
      try
      {
        return Write(buffer, 0, buffer.Length);
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public int Write(byte[] buffer, int offset, int count)
    {
      try
      {
        if (_handle == IntPtr.Zero)
        {
          throw new Exception("The pipe has been closed.");
        }

        if (!CanWrite)
        {
          throw new Exception("The pipe is not configured for writing.");
        }

        if (buffer == null)
        {
          throw new Exception("The pipe buffer for writing is null.");
        }

        if (offset < 0)
        {
          throw new Exception("The pipe buffer for writing offset is less than zero.");
        }

        if (count < 0)
        {
          throw new Exception("The pipe buffer for writing count is less than zero.");
        }

        if (buffer.Length < (offset + count))
        {
          throw new Exception("The pipe buffer for writing does not contain amount of requested data.");
        }

        if (offset != 0)
        {
          var buf = new byte[count];

          for (int x = 0; x < count; x++)
          {
            buf[x] = buffer[offset + x];
          }

          buffer = buf;
        }

        uint written = 0;

        if (!Win32.WriteFile(_handle, buffer, (uint)count, ref written, IntPtr.Zero))
        {
          int err = Marshal.GetLastWin32Error();

          throw new Exception(string.Format("Pipe.Write failed, win32 error code {0}, pipe name '{1}' ", err, Name));
        }

        if (written < count)
        {
          throw new Exception("Written data is less than requested.");
        }

        return (int)written;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return 0;
      }
    }

    public bool Flush()
    {
      try
      {
        if (_handle == IntPtr.Zero)
        {
          throw new Exception("The pipe has been closed.");
        }

        if (!CanWrite)
        {
          throw new Exception("The pipe is not configured for writing.");
        }

        if (!Win32.FlushFileBuffers(_handle))
        {
          int err = Marshal.GetLastWin32Error();
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

    public void Close()
    {
      try
      {
        if (_handle == IntPtr.Zero)
        {
          return;
        }

        Win32.CloseHandle(_handle);
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
      finally
      {
        _handle = IntPtr.Zero;
      }
    }

    public class Client : Pipe
    {
      public Client(string name, FileAccess mode, IntPtr handle)
        : base(name, mode, handle)
      {
      }

      public static Client Open(string name, FileAccess mode, bool shortRetry = false)
      {
        try
        {
          string pipeName = @"\\.\pipe\" + name;

          IntPtr handle = IntPtr.Zero;

          int retryCount = 300;

          if (shortRetry)
          {
            retryCount = 10;
          }

          for (int i = 0; i < retryCount; i++)
          {
            handle = Win32.CreateFileW(
                                       pipeName,
                                       mode,
                                       FileShare.ReadWrite,
                                       IntPtr.Zero,
                                       FileMode.Open,
                                       0,
                                       IntPtr.Zero);

            if (handle.ToInt32() != Win32.INVALID_HANDLE_VALUE && handle != IntPtr.Zero)
            {
              break;
            }

            System.Threading.Thread.Sleep(10);
          }

          if (handle.ToInt32() == Win32.INVALID_HANDLE_VALUE || handle == IntPtr.Zero)
          {
            int err = Marshal.GetLastWin32Error();

            throw new Exception(string.Format("Pipe.Client.Open failed, win32 error code {0}, pipe name '{1}' ",
                                              err,
                                              pipeName));
          }

          var pipe = new Client(name, mode, handle);

          return pipe;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }
    }

    public class Server : Pipe
    {
      protected Server(string name, FileAccess mode, IntPtr handle)
        : base(name, mode, handle)
      {
      }

      public static Server Create(string name, FileAccess mode, bool shortRetry = false)
      {
        try
        {
          string pipeName = @"\\.\pipe\" + name;

          uint pipeMode = Win32.PIPE_ACCESS_DUPLEX;

          if (mode == FileAccess.Read)
          {
            pipeMode = Win32.PIPE_ACCESS_INBOUND;
          }
          else if (mode == FileAccess.Write)
          {
            pipeMode = Win32.PIPE_ACCESS_OUTBOUND;
          }

          IntPtr handle = IntPtr.Zero;

          int retryCount = 300;

          if (shortRetry)
          {
            retryCount = 100;
          }

          for (int i = 0; i < retryCount; i++)
          {
            handle = Win32.CreateNamedPipeW(
                                            pipeName,
                                            pipeMode,
                                            Win32.PIPE_TYPE_BYTE | Win32.PIPE_WAIT,
                                            Win32.PIPE_UNLIMITED_INSTANCES,
                                            0,
                                            1024,
                                            Win32.NMPWAIT_WAIT_FOREVER,
                                            IntPtr.Zero);

            if (handle.ToInt32() != Win32.INVALID_HANDLE_VALUE && handle != IntPtr.Zero)
            {
              break;
            }

            System.Threading.Thread.Sleep(10);
          }

          if (handle.ToInt32() == Win32.INVALID_HANDLE_VALUE)
          {
            int err = Marshal.GetLastWin32Error();

            throw new Exception(string.Format("Pipe.Server.Create failed, win32 error code {0}, pipe name '{1}' ",
                                              err,
                                              name));
          }

          var pipe = new Server(name, mode, handle);

          return pipe;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return null;
        }
      }

      public bool Connect()
      {
        try
        {
          Disconnect();

          if (_handle == IntPtr.Zero)
          {
            throw new Exception("Pipe has not been created.");
          }

          if (!Win32.ConnectNamedPipe(_handle, IntPtr.Zero))
          {
            var err = (uint)Marshal.GetLastWin32Error();

            if (err != Win32.ERROR_PIPE_CONNECTED)
            {
              return false;
            }
          }

          _isConnected = true;

          return _isConnected;
        }
        catch (Exception exception)
        {
          Log.Error(exception);

          return false;
        }
      }

      public void Disconnect()
      {
        try
        {
          if (_isConnected)
          {
            Win32.DisconnectNamedPipe(_handle);

            _isConnected = false;
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }

      public override void Dispose()
      {
        try
        {
          try
          {
            Close();
          }
          catch (Exception exception)
          {
            Log.Error(exception);
          }
          finally
          {
            base.Dispose();
          }
        }
        catch (Exception exception)
        {
          Log.Error(exception);
        }
      }
    }

    private class Win32
    {
      //Constants for dwDesiredAccess:
      public const UInt32 GENERIC_READ = 0x80000000;
      public const UInt32 GENERIC_WRITE = 0x40000000;

      //Constants for return value:
      public const Int32 INVALID_HANDLE_VALUE = -1;

      //Constants for dwFlagsAndAttributes:
      public const UInt32 FILE_FLAG_OVERLAPPED = 0x40000000;
      public const UInt32 FILE_FLAG_NO_BUFFERING = 0x20000000;

      //Constants for dwCreationDisposition:
      public const UInt32 OPEN_EXISTING = 3;


      /// <summary>
      ///   Outbound pipe access.
      /// </summary>
      public const uint PIPE_ACCESS_OUTBOUND = 0x00000002;

      /// <summary>
      ///   Duplex pipe access.
      /// </summary>
      public const uint PIPE_ACCESS_DUPLEX = 0x00000003;

      /// <summary>
      ///   Inbound pipe access.
      /// </summary>
      public const uint PIPE_ACCESS_INBOUND = 0x00000001;

      /// <summary>
      ///   Pipe blocking mode.
      /// </summary>
      public const uint PIPE_WAIT = 0x00000000;

      /// <summary>
      ///   Pipe non-blocking mode.
      /// </summary>
      public const uint PIPE_NOWAIT = 0x00000001;

      /// <summary>
      ///   Pipe read mode of type Byte.
      /// </summary>
      public const uint PIPE_READMODE_BYTE = 0x00000000;

      /// <summary>
      ///   Pipe read mode of type Message.
      /// </summary>
      public const uint PIPE_READMODE_MESSAGE = 0x00000002;

      /// <summary>
      ///   Byte pipe type.
      /// </summary>
      public const uint PIPE_TYPE_BYTE = 0x00000000;

      /// <summary>
      ///   Message pipe type.
      /// </summary>
      public const uint PIPE_TYPE_MESSAGE = 0x00000004;

      /// <summary>
      ///   Pipe client end.
      /// </summary>
      public const uint PIPE_CLIENT_END = 0x00000000;

      /// <summary>
      ///   Pipe server end.
      /// </summary>
      public const uint PIPE_SERVER_END = 0x00000001;

      /// <summary>
      ///   Unlimited server pipe instances.
      /// </summary>
      public const uint PIPE_UNLIMITED_INSTANCES = 255;

      /// <summary>
      ///   Waits indefinitely when connecting to a pipe.
      /// </summary>
      public const uint NMPWAIT_WAIT_FOREVER = 0xffffffff;

      /// <summary>
      ///   Does not wait for the named pipe.
      /// </summary>
      public const uint NMPWAIT_NOWAIT = 0x00000001;

      /// <summary>
      ///   Uses the default time-out specified in a call to the CreateNamedPipe method.
      /// </summary>
      public const uint NMPWAIT_USE_DEFAULT_WAIT = 0x00000000;

      public const ulong ERROR_PIPE_CONNECTED = 535;

      public const uint FILE_SHARE_READ = 0x00000001;

      public const uint FILE_SHARE_WRITE = 0x00000002;

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
      public static extern IntPtr CreateFileW(
        [MarshalAs(UnmanagedType.LPWStr)] string filename,
        [MarshalAs(UnmanagedType.U4)] FileAccess access,
        [MarshalAs(UnmanagedType.U4)] FileShare share,
        IntPtr securityAttributes,
        [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
        [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
        IntPtr templateFile);

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
        SetLastError = true)]
      public static extern IntPtr CreateNamedPipeW(
        [MarshalAs(UnmanagedType.LPWStr)] String lpName,
        // pipe name
        [MarshalAs(UnmanagedType.U4)] uint dwOpenMode,
        // pipe open mode
        [MarshalAs(UnmanagedType.U4)] uint dwPipeMode,
        // pipe-specific modes
        [MarshalAs(UnmanagedType.U4)] uint nMaxInstances,
        // maximum number of instances
        [MarshalAs(UnmanagedType.U4)] uint nOutBufferSize,
        // output buffer size
        [MarshalAs(UnmanagedType.U4)] uint nInBufferSize,
        // input buffer size
        [MarshalAs(UnmanagedType.U4)] uint nDefaultTimeOut,
        // time-out interval
        IntPtr pipeSecurityDescriptor // SD
        );

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
        SetLastError = true)]
      public static extern bool DisconnectNamedPipe(
        IntPtr hHandle);

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
        SetLastError = true)]
      public static extern bool ConnectNamedPipe(
        IntPtr hHandle,
        // handle to named pipe
        IntPtr lpOverlapped // overlapped structure
        );

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
        SetLastError = true)]
      public static extern bool PeekNamedPipe(IntPtr handle,
                                              byte[] buffer,
                                              uint nBufferSize,
                                              ref uint bytesRead,
                                              ref uint bytesAvail,
                                              ref uint bytesLeftThisMessage);

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
        SetLastError = true)]
      public static extern bool ReadFile(IntPtr handle,
                                         byte[] buffer,
                                         uint toRead,
                                         ref uint read,
                                         IntPtr lpOverLapped);

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
        SetLastError = true)]
      public static extern bool WriteFile(IntPtr handle,
                                          byte[] buffer,
                                          uint count,
                                          ref uint written,
                                          IntPtr lpOverlapped);

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
        SetLastError = true)]
      public static extern bool CloseHandle(IntPtr handle);

      [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall,
        SetLastError = true)]
      public static extern bool FlushFileBuffers(IntPtr handle);
    }
  }
}
