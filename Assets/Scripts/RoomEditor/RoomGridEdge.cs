using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomGridEdge : MenuComponent{
	RectTransform rt;
	Image img;
	Image overlay;
	public RoomEditorMenu roomEditor;
	public Dungeon dungeon {
		get {return roomEditor.dungeon;}
		set {}
	}
	public DungeonRoom room {
		get {return roomEditor.room;}
		set {}
	}
	public bool isHorizontal = true;
	public RoomGridCell cellA; //north (1) if horizontal, west (2) if vertical
	public RoomGridCell cellB; //south (3) if horizontal, east (0) if vertical

	Color yellowColor = new Color (1f,1f,0.5f,0.5f); //for selection
	Color orangeColor = new Color (1f,0.75f,0.5f,0.5f); //invalid selection
	Color greenColor = new Color (0.5f,1f,0.5f,0.5f); //for adding
	Color redColor = new Color (1f,0.25f,0.25f,0.5f); //for deletion
	Color nullColor = new Color (1f,1f,1f,0f);//when not selected

	void Awake(){
		rt = GetComponent<RectTransform>();
		img = GetComponent<Image>();
		overlay = transform.Find("Overlay").GetComponent<Image>();
	}

	public override void UpdateActive(){
		
	}

	public void SetSize(int cellSize){
		float w = 80 * (cellSize / 96f);
		float h = 16 * (cellSize / 96f);

		rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
		rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
	}

	public void SetSprite(){
		bool hasDoor = false;
		if(cellA != null){
			if(isHorizontal){
				if(cellA.GetDungeonCell().edges[3].hasDoor){
					hasDoor = true;
				}
			}else{
				if(cellA.GetDungeonCell().edges[0].hasDoor){
					hasDoor = true;	
				}
			}
		}else{
			if(isHorizontal){
				if(cellB.GetDungeonCell().edges[1].hasDoor){
					hasDoor = true;
				}
			}else{
				if(cellB.GetDungeonCell().edges[2].hasDoor){
					hasDoor = true;
				}
			}
		}
		if(hasDoor){
			img.sprite = dungeon.tileset.doorSprite;
			img.color = new Color (1f,1f,1f,1f);
		}else{
			img.sprite = dungeon.tileset.doorSprite;
			img.color = nullColor;
		}
	}

	public void SetClickable(bool isClickable){
		img.raycastTarget = isClickable;
		if(!isClickable){
			overlay.color = nullColor;
		}
	}

	public bool IsWall(){
		DungeonRoom dra = null;
		if(cellA != null){
			dra = cellA.GetDungeonCell().room;
		}
		DungeonRoom drb = null;
		if(cellB != null){
			drb = cellB.GetDungeonCell().room;
		}
		return dra != drb;
	}

	public override void OnClick(){

	}

}
