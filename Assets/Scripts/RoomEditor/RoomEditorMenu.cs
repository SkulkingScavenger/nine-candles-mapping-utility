using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RoomEditorMenu : Menu{

	public DungeonMap map;
	public Dungeon dungeon {
		get {return map.dungeon;}
		set {}
	}
	public DungeonRoom room;
	public DungeonFloor currentFloor {
		get {return room.floor;}
		set {}
	}
	public int gridHeight = 10;
	public int gridWidth = 10;
	private RoomGridCell[,] grid;
	private RoomGridBorder[,] borders;
	private RoomGridEdge[,] horizontalEdges;
	private RoomGridEdge[,] verticalEdges;
	public int cellSize = 48;

	public Coordinates origin = new Coordinates(0,0);
	public Coordinates roomOrigin = new Coordinates(0,0);
	public Coordinates extents = new Coordinates(0,0);
	public Coordinates hoveredCellCoords;
	public RoomGridCell hoveredCell {
		get {
				if(RoomGridCellAt(hoveredCellCoords) != null && RoomGridCellAt(hoveredCellCoords).isHovered){
					return RoomGridCellAt(hoveredCellCoords);
				}else{
					return null;
				}
			}
		set {}
	}

	public bool displayUpdateRequired = false;
	bool gridExists = false;

	public MenuButton closeMenuButton;
	public ContentPanel contentPanel;
	MenuButton[] tabs = new MenuButton[3];
	public int tabIndex = 0;

	DiscreteScrollbar scrollbarVertical;
	DiscreteScrollbar scrollbarHorizontal;
	MenuButton[] scrollButtons = new MenuButton[4];

	GameObject objectPlacementPreview;

	public override void OnMenuOpen(){
		GridSetup();
	}

	public override void OnActivate(){
		UpdateDisplay();
		contentPanel.Reload();
	}

	public override void InitialSetup(){
		Transform tsf;
		tsf = transform;
		objectPlacementPreview = tsf.Find("ObjectPlacementPreview").gameObject;

		tsf = transform.Find("GridSection");
		scrollbarVertical = tsf.Find("ScrollbarVertical").GetComponent<DiscreteScrollbar>();
		scrollbarHorizontal = tsf.Find("ScrollbarHorizontal").GetComponent<DiscreteScrollbar>();
		for(int i=0;i<scrollButtons.Length;i++){
			int index = i;
			scrollButtons[i] = tsf.Find("ScrollButton"+i).GetComponent<MenuButton>();
			scrollButtons[i].onClick.AddListener(delegate { OnScrollButton(index); } );
		}
		scrollbarVertical.menu = this;
		scrollbarVertical.section = tsf.GetComponent<MenuSection>();
		scrollbarVertical.Init();
		scrollbarVertical.onValueChanged.AddListener(delegate {OnScroll();} );
		scrollbarHorizontal.menu = this;
		scrollbarHorizontal.section = tsf.GetComponent<MenuSection>();
		scrollbarHorizontal.Init();
		scrollbarHorizontal.onValueChanged.AddListener(delegate {OnScroll();} );

		tsf = transform.Find("TopButtons");
		closeMenuButton = tsf.Find("CloseMenuButton").GetComponent<MenuButton>();
		closeMenuButton.onClick.AddListener(delegate {CloseMenu();});
		tabs[0] = tsf.Find("RoomPropertiesButton").GetComponent<MenuButton>();
		tabs[0].onClick.AddListener(delegate { SetTabIndex(0);});
		tabs[1] = tsf.Find("DungeonFeaturesButton").GetComponent<MenuButton>();
		tabs[1].onClick.AddListener(delegate { SetTabIndex(1);});
		tabs[2] = tsf.Find("MonstersButton").GetComponent<MenuButton>();
		tabs[2].onClick.AddListener(delegate { SetTabIndex(2);});

		contentPanel = transform.Find("ContentPanel").GetComponent<ContentPanel>();
		contentPanel.roomEditor = this;
		contentPanel.InitialSetup();
	}

	public override void UpdateActive(){
		displayUpdateRequired = false;

		UpdateObjectPlacementPreview();
		SetScrollButtonsEnabled();

		for(int i=0;i<tabs.Length;i++){
			tabs[i].isActive = tabIndex == i;
		}

		if(tabIndex == 1){

		}else if(tabIndex == 2){
			if(IsValidPlacement()){
				DungeonCell dc = hoveredCell.GetDungeonCell();
				if(InputControl.mouseLeftPressed){
					if(InputControl.shiftDown && dc.monsterID != 0){
						dc.monsterID = 0;
						displayUpdateRequired = true;
					}else if(contentPanel.selectedMonster != null){
						dc.monsterID = contentPanel.selectedMonster.systemID;
						displayUpdateRequired = true;
					}
				}else if(InputControl.mouseRightPressed && dc.monsterID != 0){
					dc.monsterID = 0;
					displayUpdateRequired = true;
				}
			}
		}

		Navigate();

		if(displayUpdateRequired){
			UpdateDisplay();
		}
	}

	public void SetRoom(DungeonRoom dr){
		room = dr;
		origin = new Coordinates(0,0);
		extents = room.CalculateExtents();
		roomOrigin = room.origin;
	}

	void UpdateDisplay(){
		int w,h;
		w = grid.GetLength(0);
		h = grid.GetLength(1);
		for (int i=0; i<h; i++){
			for(int j=0; j<w; j++){
				grid[j,i].SetSprite(currentFloor.GetCellAtCoord(origin + roomOrigin + new Coordinates(j,i)));
			}
		}
		w = borders.GetLength(0);
		h = borders.GetLength(1);
		for (int i=0; i<h; i++){
			for(int j=0; j<w; j++){
				borders[j,i].SetSprite(origin + roomOrigin + new Coordinates(j,i));
			}
		}
		w = horizontalEdges.GetLength(0);
		h = horizontalEdges.GetLength(1);
		for (int i=0; i<h; i++){
			for(int j=0; j<w; j++){
				horizontalEdges[j,i].SetSprite();
			}
		}
		w = verticalEdges.GetLength(0);
		h = verticalEdges.GetLength(1);
		for (int i=0; i<h; i++){
			for(int j=0; j<w; j++){
				verticalEdges[j,i].SetSprite();
			}
		}
	}

	void GridSetup(){
		if(gridExists){
			GridClear();
		}
		gridExists = true;
		
		RectTransform gridContainer = transform.Find("GridSection").Find("GridContainer").GetComponent<RectTransform>();
		Rect rect = GetComponent<RectTransform>().rect;
		//Determine how many tiles will fit within the maximum rectangle
		//this number will be equal to (height/width of the screen - margins) / tilewidth rounded down
		//set rect to that size
		int scrollbarWidth = 32;
		float w = (Screen.width - 608 - 32 - scrollbarWidth) / cellSize; //max = 82
		float h = (Screen.height - 192 - 32 - scrollbarWidth) / cellSize;//max = 55
		if(w > extents.x + 1){
			w = extents.x + 1;
		}
		if(h > extents.y + 1){
			h = extents.y + 1;
		}
		gridWidth = (int)w;
		gridHeight = (int)h;

		if(gridWidth + origin.x > extents.x){
			origin.x = extents.x - gridWidth + 1;
		}
		if(gridHeight + origin.y > extents.y){
			origin.y = extents.y - gridHeight + 1;
		}

		gridContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (w + 0.125f)*cellSize);
		gridContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (h + 0.125f)*cellSize);

		GameObject go;
		float x, y;
		grid = new RoomGridCell[gridWidth,gridHeight];
		RoomGridCell rgc;
		Transform tileHolder = gridContainer.Find("TileHolder");
		for (int i=0; i<gridHeight; i++){
			for(int j=0; j<gridWidth; j++){
				go = PoolControl.WithdrawRoomGridCell();
				rgc = go.GetComponent<RoomGridCell>();
				rgc.roomEditor = this;
				rgc.menu = this;
				rgc.section = sections[0];
				rgc.coords = new Coordinates(j,i);
				rgc.SetSize(cellSize);
				go.transform.SetParent(tileHolder);
				x = j*cellSize - gridWidth*cellSize/2 + cellSize/2;
				y = -i*cellSize + gridHeight*cellSize/2 - cellSize/2;
				go.transform.localPosition = new Vector3(x,y,0f);
				grid[j,i] = rgc;
			}
		}

		borders = new RoomGridBorder[gridWidth + 1, gridHeight + 1];
		RoomGridBorder rgb;
		Transform borderHolder = gridContainer.Find("BorderHolder");
		for (int i=0; i<borders.GetLength(1); i++){
			for(int j=0; j<borders.GetLength(0); j++){
				go = PoolControl.WithdrawRoomGridBorder();
				rgb = go.GetComponent<RoomGridBorder>();
				rgb.roomEditor = this;
				rgb.menu = this;
				rgb.section = sections[0];
				rgb.SetSize(cellSize);
				go.transform.SetParent(borderHolder);
				x = j*cellSize - gridWidth*cellSize/2;
				y = -i*cellSize + gridHeight*cellSize/2;
				go.transform.localPosition = new Vector3(x,y,0f);
				borders[j,i] = rgb;
			}
		}

		RoomGridEdge rge;

		horizontalEdges = new RoomGridEdge[gridWidth, gridHeight + 1];
		Transform edgeHolder = gridContainer.Find("EdgeHolder");
		for (int i=0; i<horizontalEdges.GetLength(1); i++){
			for(int j=0; j<horizontalEdges.GetLength(0); j++){
				go = PoolControl.WithdrawRoomGridEdge();
				rge = go.GetComponent<RoomGridEdge>();
				rge.roomEditor = this;
				rge.menu = this;
				rge.section = sections[0];
				go.transform.SetParent(edgeHolder);
				rge.SetSize(cellSize);
				if(i == 0){
					rge.cellA = null;
					rge.cellB = grid[j,i];
				}else if(i == gridHeight){
					rge.cellA = grid[j,i-1];
					rge.cellB = null;
				}else{
					rge.cellA = grid[j,i-1];
					rge.cellB = grid[j,i];
				}
				x = j*cellSize - gridWidth*cellSize/2 + cellSize/2;
				y = -i*cellSize + gridHeight*cellSize/2;
				go.transform.localPosition = new Vector3(x,y,0f);
				go.transform.eulerAngles = new Vector3(0,0,0);
				rge.isHorizontal = true;
				horizontalEdges[j,i] = rge;
			}
		}

		verticalEdges = new RoomGridEdge[gridWidth + 1, gridHeight];
		for (int i=0; i<verticalEdges.GetLength(1); i++){
			for(int j=0; j<verticalEdges.GetLength(0); j++){
				go = PoolControl.WithdrawRoomGridEdge();
				rge = go.GetComponent<RoomGridEdge>();
				rge.roomEditor = this;
				rge.menu = this;
				rge.section = sections[0];
				go.transform.SetParent(edgeHolder);
				rge.SetSize(cellSize);
				if(j == 0){
					rge.cellA = null;
					rge.cellB = grid[j,i];
				}else if(j == gridWidth){
					rge.cellA = grid[j-1,i];
					rge.cellB = null;
				}else{
					rge.cellA = grid[j-1,i];
					rge.cellB = grid[j,i];
				}
				x = j*cellSize - gridWidth*cellSize/2;
				y = -i*cellSize + gridHeight*cellSize/2 - cellSize/2;
				go.transform.localPosition = new Vector3(x,y,0f);
				go.transform.eulerAngles = new Vector3(0,0,90f);
				rge.isHorizontal = false;
				verticalEdges[j,i] = rge;
			}
		}

		SetScrollbars();
		SetObjectPlacementPreview();

		displayUpdateRequired = true;
	}

	void GridClear(){
		for(int i=0;i<grid.GetLength(1);i++){
			for(int j=0;j<grid.GetLength(0);j++){
				if(grid[j,i] != null){
					PoolControl.DepositRoomGridCell(grid[j,i].gameObject);
				}
			}
		}

		for(int i=0;i<borders.GetLength(1);i++){
			for(int j=0;j<borders.GetLength(0);j++){
				if(borders[j,i] != null){
					PoolControl.DepositRoomGridBorder(borders[j,i].gameObject);
				}
			}
		}

		for(int i=0;i<horizontalEdges.GetLength(1);i++){
			for(int j=0;j<horizontalEdges.GetLength(0);j++){
				if(horizontalEdges[j,i] != null){
					PoolControl.DepositRoomGridEdge(horizontalEdges[j,i].gameObject);	
				}
			}
		}

		for(int i=0;i<verticalEdges.GetLength(1);i++){
			for(int j=0;j<verticalEdges.GetLength(0);j++){
				if(verticalEdges[j,i] != null){
					PoolControl.DepositRoomGridEdge(verticalEdges[j,i].gameObject);	
				}
			}
		}
	}

	void Navigate(){
		if(InputControl.arrowPressed[0]){
			if(origin.x + gridWidth < extents.x + 1){
				origin += new Coordinates (1,0);
				displayUpdateRequired = true;
			}
		}
		if(InputControl.arrowPressed[1]){
			if(origin.y > 0){
				origin += new Coordinates (0,-1);
				displayUpdateRequired = true;
			}
		}
		if(InputControl.arrowPressed[2]){
			if(origin.x > 0){
				origin += new Coordinates (-1,0);
				displayUpdateRequired = true;
			}
		}
		if(InputControl.arrowPressed[3]){
			if(origin.y + gridHeight < extents.y + 1){
				origin += new Coordinates (0,1);
				displayUpdateRequired = true;
			}
		}

		SetScrollbars();
	}


	void SetScrollbars(){
		scrollbarHorizontal.SetScrollbar(origin.x, extents.x, grid.GetLength(0));
		scrollbarVertical.SetScrollbar(origin.y, extents.y, grid.GetLength(1));		
	}

	void OnScroll(){
		origin = new Coordinates(scrollbarHorizontal.value, scrollbarVertical.value);
		UpdateDisplay();
	}

	void SetScrollButtonsEnabled(){
		scrollButtons[0].gameObject.SetActive(extents.x > gridWidth);
		scrollButtons[2].gameObject.SetActive(extents.x > gridWidth);
		scrollButtons[1].gameObject.SetActive(extents.y > gridHeight);
		scrollButtons[3].gameObject.SetActive(extents.y > gridHeight);

		scrollButtons[0].isDisabled = origin.x + gridWidth >= extents.x;
		scrollButtons[1].isDisabled = origin.y <= 0;
		scrollButtons[2].isDisabled = origin.x <= 0;
		scrollButtons[3].isDisabled = origin.y + gridHeight >= extents.y;
	}

	void OnScrollButton(int index){
		if(index == 0){
			if(origin.x + gridWidth < extents.x){
				origin += new Coordinates (1,0);
			}
		}
		if(index == 1){
			if(origin.y > 0){
				origin += new Coordinates (0,-1);
			}
		}
		if(index == 2){
			if(origin.x > 0){
				origin += new Coordinates (-1,0);
			}
		}
		if(index == 3){
			if(origin.y + gridHeight < extents.y){
				origin += new Coordinates (0,1);
			}
		}
		SetScrollbars();
		UpdateDisplay();
	}

	void UpdateObjectPlacementPreview(){
		if(!IsValidPlacement() || tabIndex == 0 || (tabIndex == 1 && contentPanel.selectedDungeonFeature == null) || (tabIndex == 2 && contentPanel.selectedMonster == null)){
			objectPlacementPreview.SetActive(false);
		}else{
			objectPlacementPreview.transform.position = hoveredCell.transform.position;
			objectPlacementPreview.SetActive(true);
			if(tabIndex == 1){
				objectPlacementPreview.transform.Find("FeatureIcon").gameObject.SetActive(true);
				objectPlacementPreview.transform.Find("MonsterIcon").gameObject.SetActive(false);
				objectPlacementPreview.transform.Find("FeatureIcon").GetComponent<Image>().sprite = contentPanel.selectedDungeonFeature.icon;
			}else if(tabIndex == 2){
				objectPlacementPreview.transform.Find("FeatureIcon").gameObject.SetActive(false);
				objectPlacementPreview.transform.Find("MonsterIcon").gameObject.SetActive(true);
				objectPlacementPreview.transform.Find("MonsterIcon").Find("Mask").Find("Image").GetComponent<Image>().sprite = contentPanel.selectedMonster.icon;
			}
		}
	}


	void SetObjectPlacementPreview(){
		RectTransform rt = objectPlacementPreview.GetComponent<RectTransform>();
		rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cellSize);
		rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, cellSize);
	}

	void SetTabIndex(int newIndex){
		if(tabIndex == newIndex){return;}
		tabIndex = newIndex;
		contentPanel.Reload();
		UpdateDisplay();
	}

	void CloseMenu(){
		map.Open();
		map.Activate();
		Close();
	}

	public RoomGridCell RoomGridCellAt(Coordinates c){
		return c.InBounds(gridWidth,gridHeight) ? grid[c.x,c.y] : null;
	}

	bool IsValidPlacement(){

		return hoveredCell != null && hoveredCell.GetDungeonCell().room == room;
	}

}
