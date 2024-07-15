using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;



public class ConfirmationDialog : Menu{

	public string label = "";

	public delegate void CallbackFunc(bool isConfirmed);
	public CallbackFunc callback;
	Text labelText;


	void Awake(){
		Transform tsf = transform.Find("Panel").Find("ButtonSection");
		tsf.Find("CancelButton").GetComponent<MenuButton>().onClick.AddListener(delegate { Cancel(); });
		tsf.Find("ConfirmButton").GetComponent<MenuButton>().onClick.AddListener(delegate { Confirm(); });

		tsf = transform.Find("Panel");
		labelText = tsf.Find("Label").GetComponent<Text>();
	}

	void Update(){
		if(isActiveMenu){
			if(MenuControl.cancelPressed){
				MenuControl.cancelPressed = false;
				Cancel();
			}else if(MenuControl.submitPressed){
				MenuControl.submitPressed = false;
				Confirm();
			}

		}
	}

	void OnEnable(){
		labelText.text = label;
	}

	void Cancel(){
		root.isActiveMenu = true;
		gameObject.SetActive(false);
		callback(false);
	}

	void Confirm(){
		root.isActiveMenu = true;
		gameObject.SetActive(false);
		callback(true);
	}

}
