using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesetManager : MonoBehaviour{

	public static TilesetManager Instance;

	public List<Tileset> tilesets;

	public void Init(){
		if(Instance != null && Instance != this){
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	public static Tileset GetAtIndex(int index){
		return Instance.tilesets[index];
	}
}
