using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGridCell : MenuComponent{
	RectTransform rt;
	Image img;
	Image overlay;
	public Coordinates coords;
	public DungeonMap map;
	public Dungeon dungeon {
		get {return map.dungeon;}
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
	}

	public override void UpdateActive(){
		UpdateDisplay();
		if (isHovered){
			map.hoveredCellCoords = coords;
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
		}else{
			if (map.mode == 0){
				if(map.selectedRoom != null && IsInRoom(map.selectedRoom)){
					overlay.color = whiteColor;
				}else{
					DungeonRoom dr = map.hoveredRoom;
					if(dr != null && IsInRoom(dr)){
						overlay.color = yellowColor;
					}else{
						overlay.color = nullColor;
					}
				}
			}else if(map.mode == 1){
				if(isHovered){
					if(GetDungeonCell().room == null || InputControl.ctrlDown){
						overlay.color = greenColor;
					}else{
						overlay.color = orangeColor;
					}
				}else{
					overlay.color = nullColor;
				}
				
			}else if(map.mode == 2){
				if(isHovered){
					if(InputControl.shiftDown){
						overlay.color = redColor;
					}else{
						if(CanAnnex() && (GetDungeonCell().room == null || InputControl.ctrlDown)){
							overlay.color = greenColor;
						}else{
							overlay.color = orangeColor;
						}
					}
				}else{
					overlay.color = nullColor;
					if(InputControl.shiftDown){
						if(!IsInRoom(map.selectedRoom)){
							overlay.color = blackColor;
						}
					}else{
						if(CanAnnex() && (GetDungeonCell().room == null || InputControl.ctrlDown)){
							overlay.color = yellowColor;
						}else if(IsInRoom(map.selectedRoom)){
							overlay.color = whiteColor;
						}
					}
				}	
			}else if(map.mode == 3){
				if(IsInFloatingRoom()){
					if(GetDungeonCell().room == null){
						overlay.color = greenColor;
					}else{
						overlay.color = orangeColor;
					}
				}else{
					overlay.color = nullColor;
				}
			}else if(map.mode == 4){
				overlay.color = nullColor;
			}
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

	public bool CanAnnex(){
		bool output = false;
		DungeonCell dc = GetDungeonCell();
		if(dc.room != map.selectedRoom){
			Coordinates[] neighbors = dc.coords.OrthogonalNeighbors();
			for(int i=0;i<4;i++){
				if(neighbors[i].InBounds(dungeon.width,dungeon.height) && map.currentFloor.GetCellAtCoord(neighbors[i]).room == map.selectedRoom){
					return true;
				}
			}
		}
		return output;
	}

	public DungeonCell GetDungeonCell(){
		return map.currentFloor.GetCellAtCoord(coords + map.origin);
	}


	public bool IsInRoom(DungeonRoom dr){
		return GetDungeonCell().room == dr;
	}

	public bool IsInFloatingRoom(){
		bool output = false;
		if(map.hoveredCell != null && coords.InBounds(map.hoveredCellCoords.x + map.floatingRoomExtents.x + 1, map.hoveredCellCoords.y + map.floatingRoomExtents.y + 1)){
			for(int i=0;i<map.floatingRoom.cells.Count;i++){
				if (coords == map.hoveredCellCoords + (map.floatingRoom.cells[i] - map.floatingRoomOrigin)){
					output = true;
					break;
				}
			}
		}
		return output;
	}

}
