using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomGridCell : MenuComponent{
	RectTransform rt;
	Image img;
	Image overlay;
	GameObject monsterIcon;
	Image dungeonFeatureIcon;
	public Coordinates coords;
	public RoomEditorMenu roomEditor;
	public Dungeon dungeon {
		get {return roomEditor.dungeon;}
		set {}
	}
	public DungeonRoom room {
		get {return roomEditor.room;}
		set {}
	}

	Color whiteColor = new Color (0.9f,0.9f,1f,0.75f);
	Color blackColor = new Color (0f,0f,0f,0.75f);
	Color yellowColor = new Color (1f,1f,0.5f,0.5f);
	Color orangeColor = new Color (1f,0.75f,0.5f,0.5f);
	Color greenColor = new Color (0.5f,1f,0.5f,0.5f);
	Color redColor = new Color (1f,0.25f,0.25f,0.5f);
	Color nullColor = new Color (1f,1f,1f,0f);

	// Start is called before the first frame update
	void Awake(){
		rt = GetComponent<RectTransform>();
		img = GetComponent<Image>();
		overlay = transform.Find("Overlay").GetComponent<Image>();
		monsterIcon = transform.Find("MonsterIcon").gameObject;
		dungeonFeatureIcon = transform.Find("DungeonFeatureIcon").GetComponent<Image>();
	}

	public override void UpdateActive(){
		UpdateDisplay();
		if (isHovered){
			roomEditor.hoveredCellCoords = coords;
		}
	}

	public void SetSize(int w){
		rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
		rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, w);
	}

	void UpdateDisplay(){
		//textElement.color = GetColor();
		if(isDisabled){
			overlay.color = nullColor;
		}else if(GetDungeonCell().room != roomEditor.room && GetDungeonCell().room != null){
			overlay.color = blackColor;
		}else{
			overlay.color = nullColor;
		}

		if(GetDungeonCell().monsterID == 0){
			monsterIcon.gameObject.SetActive(false);
		}else{
			monsterIcon.gameObject.SetActive(true);
			monsterIcon.transform.Find("Mask").Find("Image").GetComponent<Image>().sprite = dungeon.GetMonsterByID(GetDungeonCell().monsterID).icon;
		}

		if(GetDungeonCell().dungeonFeatureID == 0){
			dungeonFeatureIcon.gameObject.SetActive(false);
		}else{
			dungeonFeatureIcon.gameObject.SetActive(true);
			dungeonFeatureIcon.sprite = dungeon.GetDungeonFeatureByID(GetDungeonCell().dungeonFeatureID).icon;
		}
	}

	public void SetSprite(DungeonCell dc){
		transform.eulerAngles = new Vector3(0,0,dc.rot);
		if(dc.room == null){
			img.sprite = dungeon.tileset.backgroundSprite;
		}else{
			img.sprite = dc.spr;	
		}
	}


	public DungeonCell GetDungeonCell(){
		return room.floor.GetCellAtCoord(coords + roomEditor.origin + roomEditor.roomOrigin);
	}


	public bool IsInRoom(DungeonRoom dr){
		return GetDungeonCell().room == dr;
	}
}
