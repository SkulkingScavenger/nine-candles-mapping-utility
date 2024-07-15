using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapCellBorder : MenuComponent{
	RectTransform rt;
	Image img;
	public DungeonMap map;
	public Dungeon dungeon {
		get {return map.dungeon;}
		set {}
	}

	void Awake(){
		rt = GetComponent<RectTransform>();
		img = GetComponent<Image>();
	}

	public override void UpdateActive(){

	}

	public void SetSize(int w){
		rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
		rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, w);
	}

	public void SetSprite(Coordinates c){
		DungeonFloor f = dungeon.floors[map.floorIndex];
		DungeonCell[] dc = new DungeonCell[5];
		dc[0] = f.GetCellAtCoord(c + new Coordinates(-1,-1));
		dc[1] = f.GetCellAtCoord(c + new Coordinates(-1,0));
		dc[2] = f.GetCellAtCoord(c);
		dc[3] = f.GetCellAtCoord(c + new Coordinates(0,-1));
		dc[4] = dc[0];

		bool[] b = new bool[4];
		DungeonRoom roomA;
		DungeonRoom roomB;
		for(int i=0;i<4;i++){
			roomA = dc[i] == null ? null : dc[i].room;
			roomB = dc[i+1] == null ? null : dc[i+1].room;
			b[i] = roomA != roomB;
		}
		
		int index = 0;
		if(b[0]){
			index += 1;
		}
		if(b[1]){
			index += 2;
		}
		if(b[2]){
			index += 4;
		}
		if(b[3]){
			index += 8;
		}
		img.sprite = dungeon.tileset.borderSprites[index];
	}
}
