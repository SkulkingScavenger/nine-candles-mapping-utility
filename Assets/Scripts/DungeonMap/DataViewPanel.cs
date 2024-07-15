using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DataViewPanel : MenuSection{

	public DungeonMap map;
	public Dungeon dungeon {
		get {return map.dungeon;}
		set {}
	}
	public DungeonFloor currentFloor {
		get {return map.currentFloor;}
		set {}
	}
	public DungeonRoom selectedRoom {
		get {return map.selectedRoom;}
		set {}
	}
	Text numberOfRoomsDisplay;
	Text currentFloorDisplay;
	Text selectedRoomTilesDisplay;
	Text viewOriginDisplay;

	void Awake(){
		numberOfRoomsDisplay = transform.Find("RoomCountValue").GetComponent<Text>();
		currentFloorDisplay = transform.Find("CurrentFloorValue").GetComponent<Text>();
		selectedRoomTilesDisplay = transform.Find("SelectedRoomTilesValue").GetComponent<Text>();
		viewOriginDisplay = transform.Find("ViewOriginValue").GetComponent<Text>();
	}

	public void InitialSetup(DungeonMap dm){
		map = dm;
	}

	void Update(){
		if(menu.isActiveMenu && map != null){
			currentFloorDisplay.text = "" + currentFloor.index;
			numberOfRoomsDisplay.text = "" + currentFloor.rooms.Count;
			if(selectedRoom != null){
				selectedRoomTilesDisplay.text = "" + selectedRoom.cells.Count;	
			}else{
				selectedRoomTilesDisplay.text = "";
			}
			
			viewOriginDisplay.text = "" + map.origin;
		}
	}
}
