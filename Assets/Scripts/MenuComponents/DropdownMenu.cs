using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DropdownMenu : MenuSection{

	float unhoveredTimer = 0f;
	float closeoutTimer = 2f;
	

	public void InitialSetup(){

	}

	void Update(){
		if(isHovered){
			unhoveredTimer = 0f;
		}else{
			if(InputControl.mouseLeftDown){
				Close();
			}else{
				unhoveredTimer += Time.deltaTime;
				if(unhoveredTimer > closeoutTimer){
					Close();
				}
			}


		}
	}

	public void Open(){
		unhoveredTimer = 0;
		gameObject.SetActive(true);
	}

	public void Close(){
		gameObject.SetActive(false);
	}

}
