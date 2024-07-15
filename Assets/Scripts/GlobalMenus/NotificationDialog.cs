using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NotificationDialog : Menu{

	public Text label;

	public delegate void CallbackFunc();
	public CallbackFunc callback;


	void Awake(){
		transform.Find("Button0").GetComponent<MenuButton>().onClick.AddListener(delegate { Accept(); });
		label = transform.Find("Label").GetComponent<Text>();
	}

	void Update(){
		if(isActiveMenu){
			if(MenuControl.cancelPressed || MenuControl.submitPressed || Input.anyKeyDown){
				MenuControl.cancelPressed = false;
				MenuControl.submitPressed = false;
				Accept();
			}
		}
	}

	void Accept(){
		root.isActiveMenu = true;
		gameObject.SetActive(false);
		callback();
	}

}