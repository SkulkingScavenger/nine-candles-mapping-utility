using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FloorNavigationPanel : MenuSection{

	public DungeonMap map;
	public Dungeon dungeon {
		get {return map.dungeon;}
		set {}
	}
	public DungeonFloor currentFloor {
		get {return map.currentFloor;}
		set {}
	}

	DiscreteScrollbar scrollbar;
	List<FloorNavigationSlot> slots;
	int slotHeight = 24;
	int slotCount;
	public int currentIndex = 0;


	MenuButton addFloorButton;
	MenuButton deleteFloorButton;
	MenuButton toHighestButton;
	MenuButton upOneButton;
	MenuButton downOneButton;
	MenuButton toLowestButton;

	void Awake(){
		
	}

	public void InitialSetup(DungeonMap m){
		map = m;

		Transform tsf = transform.Find("ButtonSection");
		addFloorButton = tsf.Find("AddFloorButton").GetComponent<MenuButton>();
		deleteFloorButton = tsf.Find("DeleteFloorButton").GetComponent<MenuButton>();
		toHighestButton = tsf.Find("ToHighestButton").GetComponent<MenuButton>();
		upOneButton = tsf.Find("UpOneButton").GetComponent<MenuButton>();
		downOneButton = tsf.Find("DownOneButton").GetComponent<MenuButton>();
		toLowestButton = tsf.Find("ToLowestButton").GetComponent<MenuButton>();

		addFloorButton.onClick.AddListener(delegate {AddFloor();});;
		deleteFloorButton.onClick.AddListener(delegate {DeleteFloor();});
		toHighestButton.onClick.AddListener(delegate {NavTop();});
		upOneButton.onClick.AddListener(delegate {NavUpOne();});
		downOneButton.onClick.AddListener(delegate {NavDownOne();});
		toLowestButton.onClick.AddListener(delegate {NavBottom();});

		slots = new List<FloorNavigationSlot>();
		GenerateFloorSlots();
		UpdateFloorList();

		scrollbar = transform.Find("DiscreteScrollbar").GetComponent<DiscreteScrollbar>();
		scrollbar.menu = menu;
		scrollbar.section = this;
		scrollbar.Init();
		scrollbar.onValueChanged.AddListener(delegate {OnScrollBarChange();} );
		SetScrollbar();
	}

	void Update(){
		if(menu.isActiveMenu && map != null){
			toHighestButton.isDisabled = (map.floorIndex == 0);
			upOneButton.isDisabled = (map.floorIndex == 0);
			downOneButton.isDisabled = (map.floorIndex == dungeon.floors.Count - 1);
			toLowestButton.isDisabled = (map.floorIndex == dungeon.floors.Count - 1);
			deleteFloorButton.isDisabled = (dungeon.floors.Count == 1);
			SetScrollbar();
		}
	}

	void SetScrollbar(){
		scrollbar.SetScrollbar(currentIndex, dungeon.floors.Count, slotCount);
	}

	void OnScrollBarChange(){
		currentIndex = scrollbar.value;
		UpdateFloorList();
	}

	void Reload(){
		SetScrollbar();
		UpdateFloorList();
	}

	void GenerateFloorSlots(){
		GameObject go;
		FloorNavigationSlot fns;
		int y;
		slotCount = (int)(transform.Find("FloorList").GetComponent<RectTransform>().rect.height / slotHeight);
		for(int i=0;i<slotCount;i++){
			go = PoolControl.WithdrawFloorSlot();
			fns = go.GetComponent<FloorNavigationSlot>();
			fns.panel = this;
			fns.index = i;
			go.transform.SetParent(transform.Find("FloorList"));
			y = -i*slotHeight + slotCount*slotHeight/2; 
			go.transform.localPosition = new Vector3(0,y,0);
			slots.Add(fns);
		}
	}

	void ClearSlots(){
		for(int i=0;i<slots.Count;i++){
			PoolControl.DepositFloorSlot(slots[i].gameObject);
		}
		slots.Clear();
	}

	void UpdateFloorList(){
		for(int i=0;i<slots.Count;i++){
			if(i + currentIndex < dungeon.floors.Count){
				slots[i].label = "Floor " + (i + currentIndex + 1);
				slots[i].isEmpty = false;
			}else{
				slots[i].label = "";
				slots[i].isEmpty = true;
			}
		}
	}

	public void FocusSelectedFloorInList(){
		if(currentIndex < map.floorIndex - slotCount + 1){ //ie current index is 0 and floorIndex is 20
			currentIndex = map.floorIndex - slotCount + 1;
			if(currentIndex < 0){currentIndex = 0;}
		}else if(currentIndex > map.floorIndex ){ //ie current index is 20 and floorIndex is 0
			currentIndex = map.floorIndex; 
		}
		UpdateFloorList();
		scrollbar.SetScrollIndex(currentIndex);
	}

	void NavTop(){
		if(map.floorIndex != 0){
			SetFloorIndex(0);
		}
	}

	void NavUpOne(){
		if(map.floorIndex > 0){
			SetFloorIndex(map.floorIndex - 1);
		}
	}

	void NavDownOne(){
		if(map.floorIndex < dungeon.floors.Count - 1){
			SetFloorIndex(map.floorIndex + 1);
		}
	}

	void NavBottom(){
		if(map.floorIndex != dungeon.floors.Count - 1){
			SetFloorIndex(dungeon.floors.Count - 1);
		}
	}

	void AddFloor(){
		DungeonFloor df = new DungeonFloor(dungeon.floors.Count, dungeon);
		dungeon.floors.Add(df);
		UpdateFloorList();
		SetScrollbar();
	}

	void DeleteFloor(){
		Debug.Log("Delete Floor");
	}

	public void SetFloorIndex(int index){
		map.SetFloorIndex(index);
		FocusSelectedFloorInList();
	}
}
