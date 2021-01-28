// Not very useful

using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class HideIcons : MonoBehaviour
{
	IntPtr hWndPm, hWndPm2, hWndDeskop;
	bool isHide = false;

	[DllImport( "user32.dll" )]
	public static extern bool ShowWindow( System.IntPtr hwnd, int nCmdShow );
	[DllImport( "user32.dll" )]
	public static extern IntPtr FindWindow( string strClassName, string nptWindowName );
	[DllImport( "user32.dll" )]
	public static extern IntPtr FindWindowEx( IntPtr parent, int child, string strClassName, string nptWindowName );
	[DllImport( "user32.dll" )]
	public static extern bool IsWindowVisible( IntPtr hend );
	
	void Update()
    {
		if ( Input.GetKeyDown(KeyCode.Space) )
			isHide = !isHide;

		hWndPm = FindWindow( null, "Program Manager" );
		hWndPm2 = FindWindowEx( hWndPm, 0, null, "" );
		hWndDeskop = FindWindowEx( hWndPm2, 0, null, "FolderView" );
		if ( hWndDeskop != IntPtr.Zero )
			ShowWindow( hWndDeskop, isHide ? 0 : 5 );
	}
}
