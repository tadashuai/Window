using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class PlatformHelperWindows : PlatformHelper
{
	private static bool SupportsMultiMonitor
	{
		get
		{
			if (!PlatformHelperWindows.hasCheckMultiMonitorSupport)
			{
				PlatformHelperWindows.hasCheckMultiMonitorSupport = true;
				string operatingSystem = SystemInfo.operatingSystem;
				PlatformHelperWindows._supportsMultiMonitor = (operatingSystem.Contains("Windows 10") || operatingSystem.Contains("Windows 8.1"));
			}
			return PlatformHelperWindows._supportsMultiMonitor;
		}
	}

	public override void Setup()
	{
		this.AttemptSetWindowHandle();
		this.RecalculateScalingFactor();
		this.RecalculateWindowSize();
	}

	public override void SetWindowPosition(int x, int y, bool takesTaskbarIntoAccount = false)
	{
		Vector2Int vector2Int = this.ProcessNewWindowPosition(x, y, takesTaskbarIntoAccount);
		Vector2Int bottomLeftDesktopPoint = this.GetBottomLeftDesktopPoint();
		Vector2Int vector2Int2 = new Vector2Int(bottomLeftDesktopPoint.x + vector2Int.x, bottomLeftDesktopPoint.y + vector2Int.y);
		PlatformHelperWindows.SetWindowPos(this.windowHandle, 0, vector2Int2.x, vector2Int2.y, 0, 0, PlatformHelperWindows.SWP.NOSIZE);
	}

	public override Vector2Int GetWindowPosition()
	{
		this.AttemptSetWindowHandle();
		PlatformHelper.Rectangle rectangle;
		PlatformHelperWindows.GetWindowRect(this.windowHandle, out rectangle);
		int y = base.WindowDanceResolution.y;
		Vector2Int bottomLeftDesktopPoint = this.GetBottomLeftDesktopPoint();
		return new Vector2Int(rectangle.Left - bottomLeftDesktopPoint.x, y - (rectangle.Bottom - bottomLeftDesktopPoint.y));
	}

	public override Vector2Int GetWindowCenter()
	{
		Vector2Int windowPosition = this.GetWindowPosition();
		return new Vector2Int(windowPosition.x + this.windowSize.x / 2, windowPosition.y + this.windowSize.y / 2);
	}

	public override Resolution GetCurrentMonitorResolution()
	{
		List<PlatformHelper.Rectangle> monitorRects = new List<PlatformHelper.Rectangle>();
		PlatformHelperWindows.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, delegate(IntPtr hDesktop, IntPtr hdc, ref PlatformHelper.Rectangle pRect, int d)
		{
			monitorRects.Add(pRect);
			return true;
		}, 0);
		PlatformHelper.Rectangle rectangle;
		PlatformHelperWindows.GetWindowRect(this.windowHandle, out rectangle);
		int num = rectangle.Right - rectangle.Left;
		int num2 = rectangle.Bottom - rectangle.Top;
		Vector2Int vector2Int = new Vector2Int(rectangle.Left + Mathf.RoundToInt((float)num / 2f), rectangle.Top + Mathf.RoundToInt((float)num2 / 2f));
		PlatformHelper.Rectangle rectangle2 = monitorRects[0];
		foreach (PlatformHelper.Rectangle rectangle3 in monitorRects)
		{
			if (vector2Int.x >= rectangle3.Left && vector2Int.x <= rectangle3.Right && vector2Int.y >= rectangle3.Top && vector2Int.y <= rectangle3.Bottom)
			{
				rectangle2 = rectangle3;
				break;
			}
		}
		return new Resolution
		{
			width = rectangle2.Right - rectangle2.Left,
			height = rectangle2.Bottom - rectangle2.Top
		};
	}

	public override void SetWindowCenter(int x, int y)
	{
		int x2 = x - this.windowSize.x / 2;
		int y2 = y - this.windowSize.y / 2;
		this.SetWindowPosition(x2, y2, false);
	}

	public override void RecalculateWindowSize()
	{
		PlatformHelper.Rectangle rectangle;
		PlatformHelperWindows.GetWindowRect(this.windowHandle, out rectangle);
		int x = Mathf.RoundToInt((float)(rectangle.Right - rectangle.Left));
		int y = Mathf.RoundToInt((float)(rectangle.Bottom - rectangle.Top));
		this.windowSize = new Vector2Int(x, y);
	}

	public override void RecalculateScalingFactor()
	{
		this.scalingFactor = 1f;
	}

	public override Vector2Int GetWindowDanceResolution()
	{
		this.windowDanceResolutionRect = base.GetMonitorsRect(base.Monitors, false);
		return new Vector2Int(this.windowDanceResolutionRect.Right - this.windowDanceResolutionRect.Left, this.windowDanceResolutionRect.Bottom - this.windowDanceResolutionRect.Top);
	}

	public override Vector2Int GetBottomLeftDesktopPoint()
	{
		Vector2Int result = new Vector2Int(this.windowDanceResolutionRect.Left, this.windowDanceResolutionRect.Top);
		foreach (PlatformHelper.Monitor monitor in base.Monitors)
		{
			if (monitor.bounds.Left > this.windowDanceResolutionRect.Left && monitor.bounds.Left < this.windowDanceResolutionRect.Right && monitor.bounds.Top > this.windowDanceResolutionRect.Top && monitor.bounds.Top < this.windowDanceResolutionRect.Bottom)
			{
				if (monitor.bounds.Left < result.x)
				{
					result.x = monitor.bounds.Left;
				}
				if (monitor.bounds.Top > result.y)
				{
					result.y = monitor.bounds.Top;
				}
			}
		}
		return result;
	}

	public override bool IsCurrentlyFocused()
	{
		return this.windowHandle == PlatformHelperWindows.GetForegroundWindow();
	}

	public override void OnApplicationQuit()
	{
	}

	public override void SetWindowTopLeft()
	{
		Vector2Int corner = this.GetCorner((Vector2Int vertex, Vector2 center) => (float)vertex.x < center.x && (float)vertex.y < center.y);
		PlatformHelperWindows.SetWindowPos(this.windowHandle, 0, corner.x, corner.y, 0, 0, PlatformHelperWindows.SWP.NOSIZE);
	}

	public override void SetWindowTopRight()
	{
		Vector2Int corner = this.GetCorner((Vector2Int vertex, Vector2 center) => (float)vertex.x > center.x && (float)vertex.y < center.y);
		PlatformHelperWindows.SetWindowPos(this.windowHandle, 0, corner.x - this.windowSize[0], corner.y, 0, 0, PlatformHelperWindows.SWP.NOSIZE);
	}

	public override void SetWindowBottomLeft()
	{
		Vector2Int corner = this.GetCorner((Vector2Int vertex, Vector2 center) => (float)vertex.x < center.x && (float)vertex.y > center.y);
		PlatformHelperWindows.SetWindowPos(this.windowHandle, 0, corner.x, corner.y - this.windowSize[1], 0, 0, PlatformHelperWindows.SWP.NOSIZE);
	}

	public override void SetWindowBottomRight()
	{
		Vector2Int corner = this.GetCorner((Vector2Int vertex, Vector2 center) => (float)vertex.x > center.x && (float)vertex.y > center.y);
		PlatformHelperWindows.SetWindowPos(this.windowHandle, 0, corner.x - this.windowSize[0], corner.y - this.windowSize[1], 0, 0, PlatformHelperWindows.SWP.NOSIZE);
	}

	protected override List<PlatformHelper.Monitor> GetMonitors()
	{
		List<PlatformHelper.Rectangle> monitorRects = new List<PlatformHelper.Rectangle>();
		PlatformHelperWindows.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, delegate(IntPtr hMonitor, IntPtr hdc, ref PlatformHelper.Rectangle pRect, int d)
		{
			monitorRects.Add(pRect);
			return PlatformHelperWindows.SupportsMultiMonitor;
		}, 0);
		List<PlatformHelper.Monitor> list = new List<PlatformHelper.Monitor>();
		if (!PlatformHelperWindows.SupportsMultiMonitor)
		{
			list.Add(new PlatformHelper.Monitor(monitorRects[0], 1f));
			return list;
		}
		List<float> list2 = new List<float>();
		List<PlatformHelper.Monitor> list3 = new List<PlatformHelper.Monitor>();
		foreach (PlatformHelper.Rectangle rectangle in monitorRects)
		{
			IntPtr hmonitor = PlatformHelperWindows.MonitorFromPoint(new Point(rectangle.Left + 1, rectangle.Top + 1), 2U);
			uint num = 96U;
			uint num2 = 96U;
			PlatformHelperWindows.GetDpiForMonitor(hmonitor, PlatformHelperWindows.DpiType.Effective, out num, out num2);
			float num3 = 1f * num / 96f;
			PlatformHelper.Monitor item = new PlatformHelper.Monitor(rectangle, num3);
			list3.Add(item);
			if (!list2.Contains(num3))
			{
				list2.Add(num3);
			}
		}
		int num4 = 0;
		foreach (float num5 in list2)
		{
			List<PlatformHelper.Monitor> list4 = new List<PlatformHelper.Monitor>();
			foreach (PlatformHelper.Monitor monitor in list3)
			{
				if (Math.Abs(monitor.scale - num5) < 0.0001f)
				{
					list4.Add(monitor);
				}
			}
			PlatformHelper.Rectangle monitorsRect = base.GetMonitorsRect(list4, false);
			int num6 = (monitorsRect.Bottom - monitorsRect.Top) * (monitorsRect.Right - monitorsRect.Left);
			if (num6 > num4)
			{
				num4 = num6;
				list = list4;
			}
		}
		return list;
	}

	public override bool IsInWindowDanceRect()
	{
		Vector2Int windowCenter = this.GetWindowCenter();
		return windowCenter.x - this.windowSize.x / 2 > this.windowDanceResolutionRect.Left && windowCenter.x + this.windowSize.x / 2 < this.windowDanceResolutionRect.Right && windowCenter.y - this.windowSize.y / 2 > this.windowDanceResolutionRect.Top && windowCenter.y + this.windowSize.y / 2 < this.windowDanceResolutionRect.Bottom;
	}

	public bool IsMinimized(IntPtr handle)
	{
		return PlatformHelperWindows.IsIconic(handle);
	}

	public void SetWindowText(string title)
	{
		PlatformHelperWindows.SetWindowText(this.windowHandle, title);
	}

	public Vector2Int GetCorner(Func<Vector2Int, Vector2, bool> cornerCompare)
	{
		List<Vector2Int> monitorsVertices = base.GetMonitorsVertices(base.Monitors);
		float num = 1E+09f;
		float num2 = -1E+09f;
		float num3 = 1E+09f;
		float num4 = -1E+09f;
		foreach (Vector2Int v in monitorsVertices)
		{
			Vector2 vector = v;
			if (vector.x < num)
			{
				num = vector.x;
			}
			if (vector.x > num2)
			{
				num2 = vector.x;
			}
			if (vector.y < num3)
			{
				num3 = vector.y;
			}
			if (vector.y > num4)
			{
				num4 = vector.y;
			}
		}
		Vector2 vector2 = new Vector2(num + (num2 - num) / 2f, num3 + (num4 - num3) / 2f);
		Vector2Int result = default(Vector2Int);
		float num5 = 0f;
		foreach (Vector2Int vector2Int in monitorsVertices)
		{
			if (cornerCompare(vector2Int, vector2))
			{
				float num6 = Vector2.Distance(vector2Int, vector2);
				if (num6 > num5)
				{
					num5 = num6;
					result = vector2Int;
				}
			}
		}
		return result;
	}

	public string GetControlText(IntPtr hWnd)
	{
		int num = PlatformHelperWindows.SendMessage((int)hWnd, 14, 0, 0).ToInt32();
		if (num == 0)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(num + 1);
		PlatformHelperWindows.SendMessage(hWnd, 13U, stringBuilder.Capacity, stringBuilder);
		return stringBuilder.ToString();
	}

	public string GetWindowTitle(IntPtr handle)
	{
		StringBuilder stringBuilder = new StringBuilder(256);
		if (PlatformHelperWindows.GetWindowText(handle, stringBuilder, 256) > 0)
		{
			return stringBuilder.ToString();
		}
		return null;
	}

	public void SetWindowPosition(IntPtr wHandle, int x, int y, bool takesTaskbarIntoAccount = false)
	{
		Vector2Int vector2Int = this.ProcessNewWindowPosition(x, y, takesTaskbarIntoAccount);
		PlatformHelperWindows.SetWindowPos(wHandle, 0, vector2Int.x, vector2Int.y, 0, 0, PlatformHelperWindows.SWP.NOSIZE | PlatformHelperWindows.SWP.NOREDRAW | PlatformHelperWindows.SWP.NOCOPYBITS | PlatformHelperWindows.SWP.ASYNCWINDOWPOS | PlatformHelperWindows.SWP.NOACTIVATE | PlatformHelperWindows.SWP.NOOWNERZORDER | PlatformHelperWindows.SWP.NOREPOSITION | PlatformHelperWindows.SWP.NOSENDCHANGING | PlatformHelperWindows.SWP.NOZORDER);
	}

	public void FocusWindow(IntPtr customWindowHandle)
	{
		if (customWindowHandle == IntPtr.Zero)
		{
			customWindowHandle = this.windowHandle;
		}
		uint windowThreadProcessId = PlatformHelperWindows.GetWindowThreadProcessId(PlatformHelperWindows.GetForegroundWindow(), IntPtr.Zero);
		uint currentThreadId = PlatformHelperWindows.GetCurrentThreadId();
		if (windowThreadProcessId == 0U)
		{
			return;
		}
		if (windowThreadProcessId != currentThreadId)
		{
			PlatformHelperWindows.AttachThreadInput(windowThreadProcessId, currentThreadId, true);
			PlatformHelperWindows.BringWindowToTop(customWindowHandle);
			PlatformHelperWindows.ShowWindow(customWindowHandle, PlatformHelperWindows.ShowWindowCommands.Show.GetHashCode());
			PlatformHelperWindows.AttachThreadInput(windowThreadProcessId, currentThreadId, false);
			return;
		}
		PlatformHelperWindows.BringWindowToTop(customWindowHandle);
		PlatformHelperWindows.ShowWindow(customWindowHandle, PlatformHelperWindows.ShowWindowCommands.Show.GetHashCode());
	}

	public void AttemptSetWindowHandle()
	{
		if (this.windowHandle != IntPtr.Zero)
		{
			return;
		}
		int currentProcessId = Process.GetCurrentProcess().Id;
		PlatformHelperWindows.EnumWindows(delegate(IntPtr hWnd, IntPtr param)
		{
			uint num;
			PlatformHelperWindows.GetWindowThreadProcessId(hWnd, out num);
			if ((long)currentProcessId == (long)((ulong)num) && PlatformHelperWindows.IsWindowVisible(hWnd))
			{
				IntPtr window = PlatformHelperWindows.GetWindow(hWnd, PlatformHelperWindows.GetWindowType.GW_OWNER);
				this.windowHandle = ((window == IntPtr.Zero) ? hWnd : window);
			}
			return true;
		}, IntPtr.Zero);
	}

	private Vector2Int ProcessNewWindowPosition(int x, int y, bool takesTaskbarIntoAccount)
	{
		this.AttemptSetWindowHandle();
		Vector2Int windowDanceResolution = base.WindowDanceResolution;
		int num = Mathf.RoundToInt((float)windowDanceResolution.x);
		int num2 = Mathf.RoundToInt((float)windowDanceResolution.y);
		int min = 0;
		int min2 = 0;
		int num3 = num;
		int num4 = num2;
		if (this.taskbarHandle == IntPtr.Zero)
		{
			this.taskbarHandle = PlatformHelperWindows.FindWindow("Shell_traywnd", string.Empty);
		}
		PlatformHelper.Rectangle rectangle;
		PlatformHelperWindows.GetWindowRect(this.taskbarHandle, out rectangle);
		int num5 = Mathf.RoundToInt((float)(rectangle.Right - rectangle.Left));
		int num6 = Mathf.RoundToInt((float)(rectangle.Bottom - rectangle.Top));
		if (num != num5)
		{
			if (rectangle.Left == 0)
			{
				min = num5;
			}
			else
			{
				num3 -= num5;
			}
		}
		else if (rectangle.Top == 0)
		{
			num4 -= num6;
		}
		else
		{
			min2 = num6;
		}
		if (takesTaskbarIntoAccount)
		{
			if (x != -100000)
			{
				x = Mathf.Clamp(x, min, num3 - this.windowSize.x);
			}
			if (y != -100000)
			{
				y = Mathf.Clamp(y, min2, num4 - this.windowSize.y);
			}
		}
		if (x == -100000 || y == -100000)
		{
			Vector2Int windowPosition = this.GetWindowPosition();
			if (x == -100000)
			{
				x = windowPosition.x;
			}
			if (y == -100000)
			{
				y = windowPosition.y;
			}
		}
		int y2 = num2 - y - this.windowSize.y;
		return new Vector2Int(x, y2);
	}

	public int[] GetWindowsZOrder(IntPtr[] handles)
	{
		List<IntPtr> list = new List<IntPtr>();
		GCHandle value = GCHandle.Alloc(list);
		try
		{
			PlatformHelperWindows.EnumWindowsProc lpEnumFunc = new PlatformHelperWindows.EnumWindowsProc(PlatformHelperWindows.EnumWindow);
			PlatformHelperWindows.EnumChildWindows(IntPtr.Zero, lpEnumFunc, GCHandle.ToIntPtr(value));
		}
		finally
		{
			if (value.IsAllocated)
			{
				value.Free();
			}
		}
		int[] array = new int[handles.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Array.IndexOf<IntPtr>(list.ToArray(), handles[i]);
		}
		int[] array2 = new int[array.Length];
		Array.Copy(array, array2, array.Length);
		for (int j = 0; j < array.Length; j++)
		{
			int value2 = array2.Min();
			int num = Array.IndexOf<int>(array2, value2);
			array[num] = j;
			array2[num] = 999999999;
		}
		return array;
	}

	private static bool EnumWindow(IntPtr handle, IntPtr pointer)
	{
		List<IntPtr> list = GCHandle.FromIntPtr(pointer).Target as List<IntPtr>;
		if (list == null)
		{
			throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
		}
		list.Add(handle);
		return true;
	}

	[DllImport("user32.dll")]
	private static extern bool IsIconic(IntPtr handle);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool EnumChildWindows(IntPtr hwndParent, PlatformHelperWindows.EnumWindowsProc lpEnumFunc, IntPtr lParam);

	[DllImport("user32.dll")]
	private static extern int SetWindowText(IntPtr hWnd, string text);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool IsWindowVisible(IntPtr hWnd);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

	[DllImport("user32.dll")]
	private static extern IntPtr FindWindow(string className, string windowName);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool GetWindowRect(IntPtr hWnd, out PlatformHelper.Rectangle lpRect);

	[DllImport("gdi32.dll")]
	private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

	[DllImport("user32.dll")]
	private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

	[DllImport("user32.dll")]
	private static extern bool EnumWindows(PlatformHelperWindows.EnumWindowsProc enumProc, IntPtr lParam);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern IntPtr GetWindow(IntPtr hWnd, PlatformHelperWindows.GetWindowType uCmd);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr SendMessage(int hWnd, int Msg, int wparam, int lparam);

	[DllImport("user32.dll")]
	public static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool BringWindowToTop(IntPtr hWnd);

	[DllImport("user32.dll")]
	internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	[DllImport("user32.dll")]
	public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

	[DllImport("kernel32.dll")]
	public static extern uint GetCurrentThreadId();

	[DllImport("user32.dll")]
	public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

	[DllImport("user32")]
	private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lpRect, PlatformHelperWindows.MonitorEnumProc callback, int dwData);

	[DllImport("User32.dll")]
	private static extern IntPtr MonitorFromPoint([In] Point pt, [In] uint dwFlags);

	[DllImport("Shcore.dll")]
	private static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] PlatformHelperWindows.DpiType dpiType, out uint dpiX, out uint dpiY);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern IntPtr GetDC(IntPtr hWnd);

	[DllImport("user32.dll")]
	private static extern bool SetProcessDPIAware();

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	private static extern bool GetMonitorInfo(IntPtr hMonitor, ref PlatformHelperWindows.MonitorInfoEx lpmi);

	[DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "EnumDisplaySettingsW", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool EnumDisplaySettings([MarshalAs(UnmanagedType.LPWStr)] string lpszDeviceName, int iModeNum, ref PlatformHelperWindows.DEVMODE lpDevMode);

	[DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "EnumDisplaySettingsExW", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool EnumDisplaySettingsEx([MarshalAs(UnmanagedType.LPWStr)] string lpszDeviceName, int iModeNum, ref PlatformHelperWindows.DEVMODE lpDevMode, uint dwFlags);

	public PlatformHelperWindows()
	{
	}

	private const int DefaultDpi = 96;

	public IntPtr windowHandle;

	private IntPtr taskbarHandle;

	private PlatformHelper.Rectangle windowDanceResolutionRect;

	private static bool hasCheckMultiMonitorSupport;

	private static bool _supportsMultiMonitor;

	private const int ENUM_CURRENT_SETTINGS = -1;

	private const int ENUM_REGISTRY_SETTINGS = -2;

	private const int CCHDEVICENAME = 32;

	public struct POINT
	{
		public int x;

		public int y;
	}

	public enum DMDO : uint
	{
		DMDO_DEFAULT,
		DMDO_90,
		DMDO_180,
		DMDO_270
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct DEVMODE
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string dmDeviceName;

		public ushort dmSpecVersion;

		public ushort dmDriverVersion;

		public ushort dmSize;

		public ushort dmDriverExtra;

		public uint dmFields;

		public PlatformHelperWindows.POINT dmPosition;

		public PlatformHelperWindows.DMDO dmDisplayOrientation;

		public uint dmDisplayFixedOutput;

		public ushort dmColor;

		public short dmDuplex;

		public short dmYResolution;

		public short dmTTOption;

		public short dmCollate;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string dmFormName;

		public ushort dmLogPixels;

		public uint dmBitsPerPel;

		public uint dmPelsWidth;

		public uint dmPelsHeight;

		public uint dmDisplayFlags;

		public uint dmDisplayFrequency;

		public uint dmICMMethod;

		public uint dmICMIntent;

		public uint dmMediaType;

		public uint dmDitherType;

		public uint dmReserved1;

		public uint dmReserved2;

		public uint dmPanningWidth;

		public uint dmPanningHeight;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	internal struct MonitorInfoEx
	{
		public void Init()
		{
			this.Size = 104;
			this.DeviceName = string.Empty;
		}

		public int Size;

		public PlatformHelper.Rectangle Monitor;

		public PlatformHelper.Rectangle WorkArea;

		public uint Flags;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
		public string DeviceName;
	}

	public enum DpiType
	{
		Effective,
		Angular,
		Raw
	}

	private static class SWP
	{
		// Note: this type is marked as 'beforefieldinit'.
		static SWP()
		{
		}

		public static readonly int NOSIZE = 1;

		public static readonly int NOMOVE = 2;

		public static readonly int NOZORDER = 4;

		public static readonly int NOREDRAW = 8;

		public static readonly int NOACTIVATE = 16;

		public static readonly int DRAWFRAME = 32;

		public static readonly int FRAMECHANGED = 32;

		public static readonly int SHOWWINDOW = 64;

		public static readonly int HIDEWINDOW = 128;

		public static readonly int NOCOPYBITS = 256;

		public static readonly int NOOWNERZORDER = 512;

		public static readonly int NOREPOSITION = 512;

		public static readonly int NOSENDCHANGING = 1024;

		public static readonly int DEFERERASE = 8192;

		public static readonly int ASYNCWINDOWPOS = 16384;
	}

	private enum GetWindowType
	{
		GW_HWNDFIRST,
		GW_HWNDLAST,
		GW_HWNDNEXT,
		GW_HWNDPREV,
		GW_OWNER,
		GW_CHILD,
		GW_ENABLEDPOPUP
	}

	public enum DeviceCap
	{
		DRIVERVERSION,
		TECHNOLOGY = 2,
		HORZSIZE = 4,
		VERTSIZE = 6,
		HORZRES = 8,
		VERTRES = 10,
		BITSPIXEL = 12,
		PLANES = 14,
		NUMBRUSHES = 16,
		NUMPENS = 18,
		NUMMARKERS = 20,
		NUMFONTS = 22,
		NUMCOLORS = 24,
		PDEVICESIZE = 26,
		CURVECAPS = 28,
		LINECAPS = 30,
		POLYGONALCAPS = 32,
		TEXTCAPS = 34,
		CLIPCAPS = 36,
		RASTERCAPS = 38,
		ASPECTX = 40,
		ASPECTY = 42,
		ASPECTXY = 44,
		SHADEBLENDCAPS,
		LOGPIXELSX = 88,
		LOGPIXELSY = 90,
		SIZEPALETTE = 104,
		NUMRESERVED = 106,
		COLORRES = 108,
		PHYSICALWIDTH = 110,
		PHYSICALHEIGHT,
		PHYSICALOFFSETX,
		PHYSICALOFFSETY,
		SCALINGFACTORX,
		SCALINGFACTORY,
		VREFRESH,
		DESKTOPVERTRES,
		DESKTOPHORZRES,
		BLTALIGNMENT
	}

	private enum ShowWindowCommands
	{
		Hide,
		Normal,
		ShowMinimized,
		Maximize,
		ShowMaximized = 3,
		ShowNoActivate,
		Show,
		Minimize,
		ShowMinNoActive,
		ShowNA,
		Restore,
		ShowDefault,
		ForceMinimize
	}

	public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

	private delegate bool MonitorEnumProc(IntPtr hDesktop, IntPtr hdc, ref PlatformHelper.Rectangle pRect, int dwData);
}
