using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScreenControl : MonoBehaviour{
	public static ScreenControl Instance { get; private set; }

	private int tileWidth = 32;

	void Awake(){
		if(Instance != null && Instance != this){
			Destroy(gameObject);
		}
		Instance = this;
	}


	void Update(){

	}

	public void SetScreenSize(){
		// Camera cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		// int tileCountH = Screen.width / tileWidth;
		// int tileCountV = Screen.height / tileWidth;
		// Debug.Log(Screen.width);
		// Debug.Log(tileCountH);
		// cam.orthographicSize = tileCountH/2;
		// cam.pixelRect = new Rect(0, 0, tileCountH * tileWidth, tileCountV * tileWidth);
	}

	public void CalculateMargin(int screenWidth){
		int tileCount = screenWidth / tileWidth;
		int marginWidth = (screenWidth % tileWidth) / 2;
	}

}


public class ScreenResolution {
	public int width;
	public int height;
	/*
	base 
	5:3 (1024x768)
	16:9 (1920x1080)
	*/
}