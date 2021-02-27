using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace OpenTK.Platform.Windows
{
	internal static class Functions
	{
		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		internal static extern bool DragAcceptFiles(IntPtr handle, [MarshalAs(UnmanagedType.Bool)] bool fAccept);

		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		internal static extern uint DragQueryFile(IntPtr hDrop, uint iFile, IntPtr lpszFile, uint cch);

		[DllImport("shell32.dll")]
		internal static extern void DragFinish(IntPtr hDrop);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetWindowPos(IntPtr handle, IntPtr insertAfter, int x, int y, int cx, int cy, SetWindowPosFlags flags);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool AdjustWindowRect([In] [Out] ref Win32Rectangle lpRect, WindowStyle dwStyle, bool bMenu);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool AdjustWindowRectEx(ref Win32Rectangle lpRect, WindowStyle dwStyle, [MarshalAs(UnmanagedType.Bool)] bool bMenu, ExtendedWindowStyle dwExStyle);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern IntPtr CreateWindowEx(ExtendedWindowStyle ExStyle, IntPtr ClassAtom, IntPtr WindowName, WindowStyle Style, int X, int Y, int Width, int Height, IntPtr HandleToParentWindow, IntPtr Menu, IntPtr Instance, IntPtr Param);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DestroyWindow(IntPtr windowHandle);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern ushort RegisterClass(ref WindowClass window_class);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern ushort RegisterClassEx(ref ExtendedWindowClass window_class);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool IsWindowUnicode(IntPtr hwnd);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern short UnregisterClass([MarshalAs(UnmanagedType.LPTStr)] string className, IntPtr instance);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern short UnregisterClass(IntPtr className, IntPtr instance);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool GetClassInfoEx(IntPtr hinst, [MarshalAs(UnmanagedType.LPTStr)] string lpszClass, ref ExtendedWindowClass lpwcx);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern bool GetClassInfoEx(IntPtr hinst, UIntPtr lpszClass, ref ExtendedWindowClass lpwcx);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

		internal static IntPtr SetWindowLong(IntPtr handle, GetWindowLongOffsets item, IntPtr newValue)
		{
			IntPtr intPtr = IntPtr.Zero;
			Functions.SetLastError(0);
			if (IntPtr.Size == 4)
			{
				intPtr = new IntPtr(Functions.SetWindowLongInternal(handle, item, newValue.ToInt32()));
			}
			else
			{
				intPtr = Functions.SetWindowLongPtrInternal(handle, item, newValue);
			}
			if (intPtr == IntPtr.Zero)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error != 0)
				{
					throw new PlatformException(string.Format("Failed to modify window border. Error: {0}", lastWin32Error));
				}
			}
			return intPtr;
		}

		internal static IntPtr SetWindowLong(IntPtr handle, WindowProcedure newValue)
		{
			return Functions.SetWindowLong(handle, GetWindowLongOffsets.WNDPROC, Marshal.GetFunctionPointerForDelegate(newValue));
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
		private static extern int SetWindowLongInternal(IntPtr hWnd, GetWindowLongOffsets nIndex, int dwNewLong);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
		private static extern IntPtr SetWindowLongPtrInternal(IntPtr hWnd, GetWindowLongOffsets nIndex, IntPtr dwNewLong);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
		private static extern int SetWindowLongInternal(IntPtr hWnd, GetWindowLongOffsets nIndex, [MarshalAs(UnmanagedType.FunctionPtr)] WindowProcedure dwNewLong);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
		private static extern IntPtr SetWindowLongPtrInternal(IntPtr hWnd, GetWindowLongOffsets nIndex, [MarshalAs(UnmanagedType.FunctionPtr)] WindowProcedure dwNewLong);

		internal static UIntPtr GetWindowLong(IntPtr handle, GetWindowLongOffsets index)
		{
			if (IntPtr.Size == 4)
			{
				return (UIntPtr)Functions.GetWindowLongInternal(handle, index);
			}
			return Functions.GetWindowLongPtrInternal(handle, index);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
		private static extern uint GetWindowLongInternal(IntPtr hWnd, GetWindowLongOffsets nIndex);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
		private static extern UIntPtr GetWindowLongPtrInternal(IntPtr hWnd, GetWindowLongOffsets nIndex);

		[CLSCompliant(false)]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("User32.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool PeekMessage(ref MSG msg, IntPtr hWnd, int messageFilterMin, int messageFilterMax, PeekMessageFlags flags);

		[CLSCompliant(false)]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("User32.dll", CharSet = CharSet.Unicode)]
		internal static extern int GetMessage(ref MSG msg, IntPtr windowHandle, int messageFilterMin, int messageFilterMax);

		[DllImport("User32.dll")]
		internal static extern int GetMessageTime();

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		internal static extern IntPtr SendMessage(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

		[SuppressUnmanagedCodeSecurity]
		[CLSCompliant(false)]
		[DllImport("User32.dll", CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool PostMessage(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		internal static extern void PostQuitMessage(int exitCode);

		[CLSCompliant(false)]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("User32.dll", CharSet = CharSet.Unicode)]
		internal static extern IntPtr DispatchMessage(ref MSG msg);

		[CLSCompliant(false)]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("User32.dll", CharSet = CharSet.Unicode)]
		internal static extern bool TranslateMessage(ref MSG lpMsg);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		internal static extern int GetQueueStatus([MarshalAs(UnmanagedType.U4)] QueueStatusFlags flags);

		[DllImport("User32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr DefWindowProc(IntPtr hWnd, WindowMessage msg, IntPtr wParam, IntPtr lParam);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("winmm.dll")]
		internal static extern IntPtr TimeBeginPeriod(int period);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool QueryPerformanceFrequency(ref long PerformanceFrequency);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool QueryPerformanceCounter(ref long PerformanceCount);

		[DllImport("user32.dll")]
		internal static extern IntPtr GetDC(IntPtr hwnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool ReleaseDC(IntPtr hwnd, IntPtr DC);

		[DllImport("gdi32.dll")]
		internal static extern int ChoosePixelFormat(IntPtr dc, ref PixelFormatDescriptor pfd);

		[DllImport("gdi32.dll")]
		internal static extern int DescribePixelFormat(IntPtr deviceContext, int pixel, int pfdSize, ref PixelFormatDescriptor pixelFormat);

		[DllImport("gdi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetPixelFormat(IntPtr dc, int format, ref PixelFormatDescriptor pfd);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("gdi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SwapBuffers(IntPtr dc);

		[DllImport("kernel32.dll")]
		internal static extern IntPtr GetProcAddress(IntPtr handle, string funcname);

		[DllImport("kernel32.dll")]
		internal static extern IntPtr GetProcAddress(IntPtr handle, IntPtr funcname);

		[DllImport("kernel32.dll")]
		internal static extern void SetLastError(int dwErrCode);

		[DllImport("kernel32.dll")]
		internal static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPTStr)] string module_name);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr LoadLibrary(string dllName);

		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool FreeLibrary(IntPtr handle);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern short GetAsyncKeyState(VirtualKeys vKey);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern short GetKeyState(VirtualKeys vKey);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern uint MapVirtualKey(uint uCode, MapVirtualKeyType uMapType);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern uint MapVirtualKey(VirtualKeys vkey, MapVirtualKeyType uMapType);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommand nCmdShow);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern bool SetWindowText(IntPtr hWnd, [MarshalAs(UnmanagedType.LPTStr)] string lpString);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern int GetWindowText(IntPtr hWnd, [MarshalAs(UnmanagedType.LPTStr)] [In] [Out] StringBuilder lpString, int nMaxCount);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool ScreenToClient(IntPtr hWnd, ref Point point);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool ClientToScreen(IntPtr hWnd, ref Point point);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool GetClientRect(IntPtr windowHandle, out Win32Rectangle clientRectangle);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool GetWindowRect(IntPtr windowHandle, out Win32Rectangle windowRectangle);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll")]
		internal static extern bool GetWindowInfo(IntPtr hwnd, ref WindowInfo wi);

		[DllImport("dwmapi.dll")]
		public unsafe static extern IntPtr DwmGetWindowAttribute(IntPtr hwnd, DwmWindowAttribute dwAttribute, void* pvAttribute, int cbAttribute);

		[DllImport("user32.dll")]
		public static extern IntPtr GetFocus();

		[DllImport("user32.dll")]
		public static extern bool IsWindowVisible(IntPtr intPtr);

		[DllImport("user32.dll")]
		public static extern IntPtr LoadIcon(IntPtr hInstance, string lpIconName);

		[DllImport("user32.dll")]
		public static extern IntPtr LoadCursor(IntPtr hInstance, string lpCursorName);

		[DllImport("user32.dll")]
		public static extern IntPtr LoadCursor(IntPtr hInstance, IntPtr lpCursorName);

		public static IntPtr LoadCursor(CursorName lpCursorName)
		{
			return Functions.LoadCursor(IntPtr.Zero, new IntPtr((int)lpCursorName));
		}

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr CreateIconIndirect(ref IconInfo iconInfo);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool GetIconInfo(IntPtr hIcon, out IconInfo pIconInfo);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool DestroyIcon(IntPtr hIcon);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool BringWindowToTop(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SetParent(IntPtr child, IntPtr newParent);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, DeviceNotification Flags);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool UnregisterDeviceNotification(IntPtr Handle);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int ChangeDisplaySettings(DeviceMode device_mode, ChangeDisplaySettingsEnum flags);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int ChangeDisplaySettingsEx([MarshalAs(UnmanagedType.LPTStr)] string lpszDeviceName, DeviceMode lpDevMode, IntPtr hwnd, ChangeDisplaySettingsEnum dwflags, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EnumDisplayDevices([MarshalAs(UnmanagedType.LPTStr)] string lpDevice, int iDevNum, [In] [Out] WindowsDisplayDevice lpDisplayDevice, int dwFlags);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool EnumDisplaySettings([MarshalAs(UnmanagedType.LPTStr)] string device_name, int graphics_mode, [In] [Out] DeviceMode device_mode);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool EnumDisplaySettings([MarshalAs(UnmanagedType.LPTStr)] string device_name, DisplayModeSettingsEnum graphics_mode, [In] [Out] DeviceMode device_mode);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool EnumDisplaySettingsEx([MarshalAs(UnmanagedType.LPTStr)] string lpszDeviceName, DisplayModeSettingsEnum iModeNum, [In] [Out] DeviceMode lpDevMode, int dwFlags);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool EnumDisplaySettingsEx([MarshalAs(UnmanagedType.LPTStr)] string lpszDeviceName, int iModeNum, [In] [Out] DeviceMode lpDevMode, int dwFlags);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfo lpmi);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr MonitorFromPoint(POINT pt, MonitorFrom dwFlags);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr MonitorFromWindow(IntPtr hwnd, MonitorFrom dwFlags);

		[DllImport("user32.dll")]
		internal static extern bool SetProcessDPIAware();

		[DllImport("gdi32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern int GetDeviceCaps(IntPtr hDC, DeviceCaps nIndex);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool TrackMouseEvent(ref TrackMouseEventStructure lpEventTrack);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern bool ReleaseCapture();

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern IntPtr SetCapture(IntPtr hwnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern IntPtr GetCapture();

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern IntPtr SetFocus(IntPtr hwnd);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern int ShowCursor(bool show);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern bool ClipCursor(ref Win32Rectangle rcClip);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern bool ClipCursor(IntPtr rcClip);

		[DllImport("user32.dll")]
		public static extern bool SetCursorPos(int X, int Y);

		[DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		internal unsafe static extern int GetMouseMovePointsEx(uint cbSize, MouseMovePoint* pointsIn, MouseMovePoint* pointsBufferOut, int nBufPoints, uint resolution);

		[DllImport("user32.dll")]
		public static extern IntPtr SetCursor(IntPtr hCursor);

		[DllImport("user32.dll")]
		public static extern IntPtr GetCursor();

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool GetCursorPos(ref POINT point);

		[SuppressUnmanagedCodeSecurity]
		[CLSCompliant(false)]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern IntPtr DefRawInputProc(RawInput[] RawInput, int Input, uint SizeHeader);

		[CLSCompliant(false)]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern IntPtr DefRawInputProc(ref RawInput RawInput, int Input, uint SizeHeader);

		[CLSCompliant(false)]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern IntPtr DefRawInputProc(IntPtr RawInput, int Input, uint SizeHeader);

		[CLSCompliant(false)]
		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool RegisterRawInputDevices(RawInputDevice[] RawInputDevices, uint NumDevices, uint Size);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool RegisterRawInputDevices(RawInputDevice[] RawInputDevices, int NumDevices, int Size);

		[CLSCompliant(false)]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern uint GetRawInputBuffer([Out] RawInput[] Data, [In] [Out] ref uint Size, [In] uint SizeHeader);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetRawInputBuffer([Out] RawInput[] Data, [In] [Out] ref int Size, [In] int SizeHeader);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetRawInputBuffer([Out] IntPtr Data, [In] [Out] ref int Size, [In] int SizeHeader);

		[CLSCompliant(false)]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern uint GetRegisteredRawInputDevices([Out] RawInput[] RawInputDevices, [In] [Out] ref uint NumDevices, uint cbSize);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetRegisteredRawInputDevices([Out] RawInput[] RawInputDevices, [In] [Out] ref int NumDevices, int cbSize);

		[CLSCompliant(false)]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern uint GetRawInputDeviceList([In] [Out] RawInputDeviceList[] RawInputDeviceList, [In] [Out] ref uint NumDevices, uint Size);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetRawInputDeviceList([In] [Out] RawInputDeviceList[] RawInputDeviceList, [In] [Out] ref int NumDevices, int Size);

		[CLSCompliant(false)]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern uint GetRawInputDeviceList([In] [Out] IntPtr RawInputDeviceList, [In] [Out] ref uint NumDevices, uint Size);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetRawInputDeviceList([In] [Out] IntPtr RawInputDeviceList, [In] [Out] ref int NumDevices, int Size);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetRawInputDeviceInfo(IntPtr Device, [MarshalAs(UnmanagedType.U4)] RawInputDeviceInfoEnum Command, [In] [Out] IntPtr Data, [In] [Out] ref int Size);

		[CLSCompliant(false)]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetRawInputDeviceInfo(IntPtr Device, [MarshalAs(UnmanagedType.U4)] RawInputDeviceInfoEnum Command, [In] [Out] byte[] Data, [In] [Out] ref int Size);

		[CLSCompliant(false)]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern uint GetRawInputDeviceInfo(IntPtr Device, [MarshalAs(UnmanagedType.U4)] RawInputDeviceInfoEnum Command, [In] [Out] RawInputDeviceInfo Data, [In] [Out] ref uint Size);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetRawInputDeviceInfo(IntPtr Device, [MarshalAs(UnmanagedType.U4)] RawInputDeviceInfoEnum Command, [In] [Out] RawInputDeviceInfo Data, [In] [Out] ref int Size);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetRawInputData(IntPtr RawInput, GetRawInputDataEnum Command, [Out] IntPtr Data, [In] [Out] ref int Size, int SizeHeader);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetRawInputData(IntPtr RawInput, GetRawInputDataEnum Command, out RawInput Data, [In] [Out] ref int Size, int SizeHeader);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", SetLastError = true)]
		internal unsafe static extern int GetRawInputData(IntPtr RawInput, GetRawInputDataEnum Command, RawInput* Data, [In] [Out] ref int Size, int SizeHeader);

		internal unsafe static int GetRawInputData(IntPtr raw, out RawInputHeader header)
		{
			int sizeInBytes = RawInputHeader.SizeInBytes;
			fixed (RawInputHeader* ptr = &header)
			{
				RawInputHeader* value = ptr;
				Functions.GetRawInputData(raw, GetRawInputDataEnum.HEADER, (IntPtr)((void*)value), ref sizeInBytes, API.RawInputHeaderSize);
				int sizeInBytes2 = RawInputHeader.SizeInBytes;
			}
			return sizeInBytes;
		}

		internal unsafe static int GetRawInputData(IntPtr raw, out RawInput data)
		{
			int sizeInBytes = RawInput.SizeInBytes;
			fixed (RawInput* ptr = &data)
			{
				RawInput* value = ptr;
				Functions.GetRawInputData(raw, GetRawInputDataEnum.INPUT, (IntPtr)((void*)value), ref sizeInBytes, API.RawInputHeaderSize);
			}
			return sizeInBytes;
		}

		internal unsafe static int GetRawInputData(IntPtr raw, byte[] data)
		{
			int result = data.Length;
			fixed (byte[] array = data)
			{
				byte* value;
				if (data == null || array.Length == 0)
				{
					value = null;
				}
				else
				{
					value = &array[0];
				}
				Functions.GetRawInputData(raw, GetRawInputDataEnum.INPUT, (IntPtr)((void*)value), ref result, API.RawInputHeaderSize);
			}
			return result;
		}

		internal unsafe static IntPtr NextRawInputStructure(IntPtr data)
		{
			return Functions.RawInputAlign((IntPtr)((void*)((byte*)((void*)data) + API.RawInputHeaderSize)));
		}

		private unsafe static IntPtr RawInputAlign(IntPtr data)
		{
			return (IntPtr)((void*)((byte*)((void*)data) + (IntPtr.Size - 1 & ~(IntPtr.Size - 1))));
		}

		[DllImport("gdi32.dll", SetLastError = true)]
		internal static extern IntPtr GetStockObject(StockObjects fnObject);

		[DllImport("gdi32.dll", SetLastError = true)]
		internal static extern bool DeleteObject([In] IntPtr hObject);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern UIntPtr SetTimer(IntPtr hWnd, UIntPtr nIDEvent, uint uElapse, Functions.TimerProc lpTimerFunc);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool KillTimer(IntPtr hWnd, UIntPtr uIDEvent);

		[DllImport("shell32.dll")]
		public static extern IntPtr SHGetFileInfo(string pszPath, int dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, ShGetFileIconFlags uFlags);

		[DllImport("Advapi32.dll")]
		internal static extern int RegOpenKeyEx(IntPtr hKey, [MarshalAs(UnmanagedType.LPTStr)] string lpSubKey, int ulOptions, uint samDesired, out IntPtr phkResult);

		[DllImport("Advapi32.dll")]
		internal static extern int RegGetValue(IntPtr hkey, [MarshalAs(UnmanagedType.LPTStr)] string lpSubKey, [MarshalAs(UnmanagedType.LPTStr)] string lpValue, int dwFlags, out int pdwType, StringBuilder pvData, ref int pcbData);

		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		public delegate void TimerProc(IntPtr hwnd, WindowMessage uMsg, UIntPtr idEvent, int dwTime);
	}
}
