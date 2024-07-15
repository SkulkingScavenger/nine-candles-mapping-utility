using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour{


	public GameObject[] UIPrefabs;


	public static PrefabManager Instance;


	public void Init(){
		//ensure uniqueness
		if(Instance != null && Instance != this){
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	public static GameObject GetUIElement(int index){
		if(index < Instance.UIPrefabs.Length){
			return Instance.UIPrefabs[index];
		}else{
			return null;
		}
	}

}

