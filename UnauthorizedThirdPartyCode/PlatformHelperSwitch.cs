using System;
using System.Collections.Generic;
using UnityEngine;

public class PlatformHelperSwitch : PlatformHelper
{
	public override void Setup()
	{
		this.RecalculateScalingFactor();
		this.RecalculateWindowSize();
	}

	public override void SetWindowPosition(int x, int y, bool takesTaskbarIntoAccount = false)
	{
	}

	public override Vector2Int GetWindowPosition()
	{
		return new Vector2Int(0, 0);
	}

	public override Vector2Int GetWindowCenter()
	{
		Vector2Int windowPosition = this.GetWindowPosition();
		return new Vector2Int(windowPosition.x + this.windowSize.x / 2, windowPosition.y + this.windowSize.y / 2);
	}

	public override void SetWindowCenter(int x, int y)
	{
		this.SetWindowPosition(x - this.windowSize.x / 2, y - this.windowSize.y / 2, false);
	}

	public override void RecalculateWindowSize()
	{
		this.windowSize = new Vector2Int(0, 0);
	}

	public override void RecalculateScalingFactor()
	{
		this.scalingFactor = 1f;
	}

	public override Resolution GetCurrentMonitorResolution()
	{
		return scrVfxControl.GetHighestResolution();
	}

	public override Vector2Int GetWindowDanceResolution()
	{
		return new Vector2Int(0, 0);
	}

	public override Vector2Int GetBottomLeftDesktopPoint()
	{
		return Vector2Int.zero;
	}

	public override bool IsCurrentlyFocused()
	{
		return false;
	}

	public override void OnApplicationQuit()
	{
	}

	public override void SetWindowTopLeft()
	{
		throw new NotImplementedException();
	}

	public override void SetWindowTopRight()
	{
		throw new NotImplementedException();
	}

	public override void SetWindowBottomLeft()
	{
		throw new NotImplementedException();
	}

	public override void SetWindowBottomRight()
	{
		throw new NotImplementedException();
	}

	protected override List<PlatformHelper.Monitor> GetMonitors()
	{
		throw new NotImplementedException();
	}

	public override bool IsInWindowDanceRect()
	{
		throw new NotImplementedException();
	}

	public PlatformHelperSwitch()
	{
	}
}
