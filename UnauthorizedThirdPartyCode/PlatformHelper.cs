using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PlatformHelper
{
	public Vector2Int WindowDanceResolution
	{
		get
		{
			if (this._windowDanceResolution == Vector2Int.zero || this.updateWindowDanceResolution)
			{
				this.updateWindowDanceResolution = false;
				this._windowDanceResolution = this.GetWindowDanceResolution();
			}
			return this._windowDanceResolution;
		}
	}

	public List<PlatformHelper.Monitor> Monitors
	{
		get
		{
			if (this._monitors == null || this.updateWindowDanceResolution)
			{
				this._monitors = this.GetMonitors();
			}
			return this._monitors;
		}
	}

	public abstract void Setup();

	public abstract Vector2Int GetBottomLeftDesktopPoint();

	public abstract void RecalculateWindowSize();

	public abstract void RecalculateScalingFactor();

	public abstract Vector2Int GetWindowDanceResolution();

	public abstract void OnApplicationQuit();

	public abstract bool IsCurrentlyFocused();

	public abstract void SetWindowPosition(int x, int y, bool takesTaskbarIntoAccount = false);

	public abstract void SetWindowTopLeft();

	public abstract void SetWindowTopRight();

	public abstract void SetWindowBottomLeft();

	public abstract void SetWindowBottomRight();

	public abstract void SetWindowCenter(int x, int y);

	public abstract Vector2Int GetWindowPosition();

	public abstract Vector2Int GetWindowCenter();

	public abstract Resolution GetCurrentMonitorResolution();

	public abstract bool IsInWindowDanceRect();

	protected abstract List<PlatformHelper.Monitor> GetMonitors();

	public List<Vector2Int> GetMonitorsVertices(List<PlatformHelper.Monitor> monitors)
	{
		List<Vector2Int> list = new List<Vector2Int>();
		foreach (PlatformHelper.Monitor monitor in monitors)
		{
			list.Add(new Vector2Int(monitor.bounds.Left, monitor.bounds.Top));
			list.Add(new Vector2Int(monitor.bounds.Right, monitor.bounds.Top));
			list.Add(new Vector2Int(monitor.bounds.Right, monitor.bounds.Bottom));
			list.Add(new Vector2Int(monitor.bounds.Left, monitor.bounds.Bottom));
		}
		return list;
	}

	public PlatformHelper.Rectangle GetMonitorsRect(List<PlatformHelper.Monitor> monitors, bool onlyHorizontal)
	{
		PlatformHelper.<>c__DisplayClass31_0 CS$<>8__locals1 = new PlatformHelper.<>c__DisplayClass31_0();
		CS$<>8__locals1.onlyHorizontal = onlyHorizontal;
		if (monitors.Count == 1)
		{
			return monitors[0].bounds;
		}
		CS$<>8__locals1.monitorsBounds = new List<PlatformHelper.Rectangle>();
		foreach (PlatformHelper.Monitor monitor in monitors)
		{
			CS$<>8__locals1.monitorsBounds.Add(monitor.bounds);
		}
		CS$<>8__locals1.currentVertexBounds = CS$<>8__locals1.monitorsBounds[0];
		List<Vector2Int> monitorsVertices = this.GetMonitorsVertices(monitors);
		CS$<>8__locals1.top = 0;
		CS$<>8__locals1.bottom = 0;
		CS$<>8__locals1.maxTop = 0;
		CS$<>8__locals1.maxBottom = 0;
		List<PlatformHelper.Rectangle> list = new List<PlatformHelper.Rectangle>();
		int num = 0;
		foreach (Vector2Int vector2Int in monitorsVertices)
		{
			CS$<>8__locals1.currentVertexBounds = CS$<>8__locals1.monitorsBounds[Mathf.FloorToInt((float)num / 4f)];
			List<int> list2 = CS$<>8__locals1.<GetMonitorsRect>g__GetSide|0(vector2Int.y, vector2Int.x, true);
			List<int> list3 = CS$<>8__locals1.<GetMonitorsRect>g__GetSide|0(vector2Int.y, vector2Int.x, false);
			using (List<int>.Enumerator enumerator3 = list2.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					int left = enumerator3.Current;
					using (List<int>.Enumerator enumerator4 = list3.GetEnumerator())
					{
						while (enumerator4.MoveNext())
						{
							int right = enumerator4.Current;
							if (right > left)
							{
								CS$<>8__locals1.bottom = (CS$<>8__locals1.top = (CS$<>8__locals1.maxTop = (CS$<>8__locals1.maxBottom = vector2Int.y)));
								CS$<>8__locals1.<GetMonitorsRect>g__GetHeight|1(left, right);
								if (CS$<>8__locals1.bottom > CS$<>8__locals1.top && right > left && list.FindIndex((PlatformHelper.Rectangle r) => CS$<>8__locals1.top == r.Top && left == r.Left && right == r.Right && CS$<>8__locals1.bottom == r.Bottom) == -1)
								{
									PlatformHelper.Rectangle item = new PlatformHelper.Rectangle
									{
										Top = CS$<>8__locals1.top,
										Left = left,
										Right = right,
										Bottom = CS$<>8__locals1.bottom
									};
									list.Add(item);
								}
							}
						}
					}
				}
			}
			num++;
		}
		list.AddRange(CS$<>8__locals1.monitorsBounds);
		int num2 = 0;
		PlatformHelper.Rectangle result = default(PlatformHelper.Rectangle);
		list = list.Distinct<PlatformHelper.Rectangle>().ToList<PlatformHelper.Rectangle>();
		foreach (PlatformHelper.Rectangle rectangle in list)
		{
			int num3 = Mathf.Abs((rectangle.Bottom - rectangle.Top) * (rectangle.Right - rectangle.Left));
			if (num3 > num2)
			{
				num2 = num3;
				result = rectangle;
			}
		}
		return result;
	}

	protected PlatformHelper()
	{
	}

	public const int DontModify = -100000;

	public Vector2Int windowSize;

	public float scalingFactor = 1f;

	public bool updateWindowDanceResolution;

	private Vector2Int _windowDanceResolution = Vector2Int.zero;

	private List<PlatformHelper.Monitor> _monitors;

	public struct Rectangle
	{
		public override string ToString()
		{
			return string.Format("Left: {0}, Top: {1}, Right: {2}, Bottom: {3}", new object[]
			{
				this.Left,
				this.Top,
				this.Right,
				this.Bottom
			});
		}

		public int Left;

		public int Top;

		public int Right;

		public int Bottom;
	}

	public class Monitor
	{
		public Monitor(PlatformHelper.Rectangle bounds, float scale)
		{
			this.bounds = bounds;
			this.scale = scale;
		}

		public override string ToString()
		{
			return string.Format("bounds: {0}, scale: {1}", this.bounds.ToString(), this.scale);
		}

		public PlatformHelper.Rectangle bounds;

		public float scale;
	}
}
