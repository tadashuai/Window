using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotMono : MonoBehaviour
{
	public Canvas canvas;
	public GameObject imageGo;

	void Update()
	{
		if ( Input.GetKeyDown( KeyCode.S ) )
		{
			ScreenCapture.CaptureScreenshot( Application.dataPath + "/Samples/ActionPreviewer/Sc.png" );
		}
		if ( Input.GetKeyDown( KeyCode.C ) )
		{
			for ( int i = 0; i < 12; i++ )
			{
				StartCoroutine( CaptureByRect( i ) );
			}
		}
	}

	private IEnumerator CaptureByRect( int i )
	{
		yield return new WaitForEndOfFrame();
		Texture2D mTexture = new Texture2D( Screen.width / 4, Screen.height / 3, TextureFormat.RGB24, false );
		mTexture.ReadPixels( new Rect( ( i % 4 ) * ( Screen.width / 4 ), ( i / 4 ) * ( Screen.height / 3 ), Screen.width / 4, Screen.height / 3 ), 0, 0 );
		mTexture.Apply();
		GameObject go = Instantiate( imageGo, canvas.transform );
		go.name = "picture" + ( i % 4 ).ToString() + ( i / 4 ).ToString();
		Image image = go.GetComponent<Image>();
		RectTransform rt = image.GetComponent<RectTransform>();
		rt.sizeDelta = new Vector2( Screen.width / 4, Screen.height / 3 );
		rt.anchoredPosition = new Vector2( ( i % 4 ) * ( Screen.width / 4 ), ( i / 4 - 2 ) * ( Screen.height / 3 ) );
		Sprite sprite = Sprite.Create( mTexture, new Rect( 0, 0, mTexture.width, mTexture.height ), Vector2.zero );
		image.sprite = sprite;

		if ( i == 11 )
			Destroy( imageGo );

		//byte[] bytes = mTexture.EncodeToPNG();
		//System.IO.File.WriteAllBytes( Application.dataPath + "/Samples/ActionPreviewer/RectSc.png", bytes );

		//UnityEditor.AssetDatabase.Refresh();


	}
}
