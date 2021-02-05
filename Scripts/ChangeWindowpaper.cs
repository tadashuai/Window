using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class ChangeWindowpaper : MonoBehaviour
{
	public Texture2D tex;

	[DllImport( "user32.dll", EntryPoint = "SystemParametersInfo" )]
	public static extern int SystemParametersInfo( int uAction, int uParam, string lpvParam, int fuWinIni );

	private void Awake()
	{
		SavePNG();

	}

	public void SavePNG()
	{
		byte[] bytes = DeCompress( tex ).EncodeToPNG();
		string bmpPath = Environment.GetFolderPath( Environment.SpecialFolder.MyPictures ).Replace( "\\", "/" ) + "/大帅比";
		if ( !Directory.Exists( bmpPath ) )
		{
			Directory.CreateDirectory( bmpPath );
		}
		bmpPath += "/honglao.png";
		File.WriteAllBytes( bmpPath, bytes );
		int r = SystemParametersInfo( 0x0014, 1, bmpPath.Replace( "/", "\\" ), 0x1|0x2 );
		RegistryKey hk = Registry.CurrentUser;
		RegistryKey run = hk.CreateSubKey( @"Control Panel\Desktop\" );
		run.SetValue( "Wallpaper", bmpPath );
	}

	public Texture2D DeCompress( Texture2D source )
	{
		RenderTexture renderTex = RenderTexture.GetTemporary(
					source.width,
					source.height,
					0,
					RenderTextureFormat.Default,
					RenderTextureReadWrite.Linear );

		Graphics.Blit( source, renderTex );
		RenderTexture previous = RenderTexture.active;
		RenderTexture.active = renderTex;
		Texture2D readableText = new Texture2D( source.width, source.height );
		readableText.ReadPixels( new Rect( 0, 0, renderTex.width, renderTex.height ), 0, 0 );
		readableText.Apply();
		RenderTexture.active = previous;
		RenderTexture.ReleaseTemporary( renderTex );
		return readableText;
	}
}
