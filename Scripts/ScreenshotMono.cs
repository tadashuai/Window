using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotMono : MonoBehaviour
{
	void Update()
	{
		if ( Input.GetKeyDown( KeyCode.S ) )
		{
			ScreenCapture.CaptureScreenshot( Application.dataPath + "/Samples/ActionPreviewer/Sc.png" );
		}
		if ( Input.GetKeyDown( KeyCode.C ) )
		{
			StartCoroutine( CaptureByRect() );
		}
	}

	private IEnumerator CaptureByRect()
	{
		yield return new WaitForEndOfFrame();
		Texture2D mTexture = new Texture2D( Screen.width / 2, Screen.height / 2, TextureFormat.RGB24, false );
		mTexture.ReadPixels( new Rect( Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2 ), 0, 0 );
		mTexture.Apply();
		byte[] bytes = mTexture.EncodeToPNG();
		System.IO.File.WriteAllBytes( Application.dataPath + "/Samples/ActionPreviewer/RectSc.png", bytes );

		UnityEditor.AssetDatabase.Refresh();
	}
}
