using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonMap : Menu{
	public int gridHeight = 10;
	public int gridWidth = 10;
	private MapGridCell[,] grid;
	private MapCellBorder[,] borders;
	private MapCellEdge[,] horizontalEdges;
	private MapCellEdge[,] verticalEdges;
	public int cellSize = 16;
	public Dungeon dungeon {
		get {return MasterControl.dungeon;}
		set {}
	}
	public int floorIndex = 0;
	public DungeonFloor currentFloor {
		get {return dungeon.floors[floorIndex];}
		set {}
	}
	public Coordinates origin = new Coordinates(0,0);
	public Coordinates hoveredCellCoords;
	public MapGridCell hoveredCell {
		get {
				if(MapGridCellAt(hoveredCellCoords) != null && MapGridCellAt(hoveredCellCoords).isHovered){
					return MapGridCellAt(hoveredCellCoords);
				}else{
					return null;
				}
			}
		set {}
	}
	public DungeonRoom hoveredRoom {
		get {
				if(hoveredCell != null){
					return hoveredCell.GetDungeonCell().room;
				}else{
					return null;
				}
			}
		set {}
	}
	public DungeonRoom selectedRoom;
	public List<DungeonRoom> secondarySelections;
	private int _mode = 0;
	public int mode {
		get {return _mode;}
		set {SetMode(value);}
	}

	public DungeonFloor clipboard;
	public DungeonRoom floatingRoom;
	public Coordinates floatingRoomExtents;
	public DungeonFloor floatingRoomOriginalFloor;
	public Coordinates floatingRoomOrigin = new Coordinates(-1,-1);
	bool isMovingRoom = false;

	public bool displayUpdateRequired = false;
	bool gridExists = false;

	DataViewPanel dataPanel;
	FloorNavigationPanel floorNavigationPanel;

	MenuButton selectRoomButton;
	MenuButton newRoomButton;
	MenuButton editTilesButton;
	MenuButton editDoorsButton;
	MenuButton editContentButton;

	RoomEditorMenu roomEditorMenu;

	DiscreteScrollbar scrollbarVertical;
	DiscreteScrollbar scrollbarHorizontal;
	MenuButton[] scrollButtons = new MenuButton[4];

	public override void InitialSetup(){
		dungeon = MasterControl.dungeon;

		Transform tsf;
		tsf = transform.Find("MapSection");
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

		tsf = transform.Find("ButtonHolder");
		selectRoomButton = tsf.Find("SelectRoomButton").GetComponent<MenuButton>();
		newRoomButton = tsf.Find("NewRoomButton").GetComponent<MenuButton>();
		editTilesButton = tsf.Find("EditTilesButton").GetComponent<MenuButton>();
		editDoorsButton = tsf.Find("EditDoorsButton").GetComponent<MenuButton>();
		editContentButton = tsf.Find("EditContentButton").GetComponent<MenuButton>();

		selectRoomButton.onClick.AddListener(delegate {mode = 0;});
		newRoomButton.onClick.AddListener(delegate {mode = 1;});
		editTilesButton.onClick.AddListener(delegate {mode = 2;});
		editDoorsButton.onClick.AddListener(delegate {mode = 4;});
		editContentButton.onClick.AddListener(delegate {OpenRoomEditorMenu();});


		tsf = transform.Find("SettingsPanel").Find("TileSizePanel");
		tsf.Find("TileSizeVeryLargeButton").GetComponent<MenuButton>().onClick.AddListener(delegate {SetCellSize(96);});
		tsf.Find("TileSizeLargeButton").GetComponent<MenuButton>().onClick.AddListener(delegate {SetCellSize(48);});
		tsf.Find("TileSizeMediumButton").GetComponent<MenuButton>().onClick.AddListener(delegate {SetCellSize(32);});
		tsf.Find("TileSizeSmallButton").GetComponent<MenuButton>().onClick.AddListener(delegate {SetCellSize(24);});
		tsf.Find("TileSizeVerySmallButton").GetComponent<MenuButton>().onClick.AddListener(delegate {SetCellSize(16);});

		dataPanel = transform.Find("DataViewPanel").GetComponent<DataViewPanel>();
		dataPanel.InitialSetup(this);

		floorNavigationPanel = transform.Find("FloorNavigationPanel").GetComponent<FloorNavigationPanel>();
		floorNavigationPanel.InitialSetup(this);

		secondarySelections = new List<DungeonRoom>();
		clipboard = new DungeonFloor(-1,dungeon);

		roomEditorMenu = MenuControl.canvas.transform.Find("RoomEditorMenu").GetComponent<RoomEditorMenu>();
	}

	public override void OnActivate(){
		mode = 0;
		GridSetup();
		UpdateDisplay();
	}

	public void OnDungeonReload(){
		mode = 0;
		GridSetup();
		UpdateDisplay();
	}

	public override void UpdateActive(){
		displayUpdateRequired = false;

		selectRoomButton.isActive = mode == 0;
		newRoomButton.isActive =  mode == 1;
		editTilesButton.isActive =  mode == 2;
		editDoorsButton.isActive =  mode == 4;
		editContentButton.isActive = false;

		selectRoomButton.isDisabled = false;
		newRoomButton.isDisabled = false;
		editTilesButton.isDisabled = selectedRoom == null;
		editDoorsButton.isDisabled = currentFloor.rooms.Count == 0;
		editContentButton.isDisabled = selectedRoom == null;

		SetScrollButtonsEnabled();

		if(mode == 0){
			if(InputControl.mouseRightPressed || InputControl.cancelPressed){
				SelectRoom(null);
			}else if(selectedRoom != null && InputControl.tabPressed){
				mode = 2;
			}else if(InputControl.mouseLeftPressed){
				if(hoveredRoom != null && hoveredRoom != selectedRoom){
					SelectRoom(hoveredRoom);	
				}
			}else if(InputControl.pastePressed){
				isMovingRoom = false;
				BeginPasteRoom();
			}else if(selectedRoom != null && InputControl.movePressed){
				isMovingRoom = true;
				CutRoom();
				BeginPasteRoom();
			}else if(selectedRoom != null && InputControl.delPressed){
				DeleteRoom();
			}else if(selectedRoom != null && InputControl.copyPressed){
				CopyRoom();
			}else if(selectedRoom != null && InputControl.cutPressed){
				CutRoom();
			}
		}else if(mode == 1){ //Create Room Mode
			if(InputControl.mouseRightPressed || InputControl.cancelPressed){
				mode = 0;
			}else if(InputControl.mouseLeftPressed && hoveredCell != null){
				if(CellPlacementHasCollision(hoveredCellCoords + origin)){
					if(InputControl.ctrlDown){
						if(WillCellRemovalDestroyRoom(hoveredCellCoords + origin, false)){
							Coordinates contestedCoords = hoveredCellCoords + origin;
							OpenConfirmationDialog("You are attempting to remove the last cell of a different room. Continue and destroy that room?", (bool confirm) => {
								if(confirm){
									AddRoom(contestedCoords);
								}
							});
						}else if(WillCellRemovalBreakRoom(hoveredCellCoords + origin, false)){
							Coordinates contestedCoords = hoveredCellCoords + origin;
							OpenConfirmationDialog("Break room continuity and create additional rooms?", (bool confirm) => {
								if(confirm){
									BreakRoomContinuity(contestedCoords);
									AddRoom(contestedCoords);
								}
							});
						}else{
							AddRoom(hoveredCellCoords + origin);
						}
					}
				}else{
					AddRoom(hoveredCellCoords + origin);
				}
			}
		}else if(mode == 2){ //Edit Mode
			if(InputControl.mouseRightPressed || InputControl.cancelPressed){
				mode = 0;
				SelectRoom(null);
			}else if(InputControl.tabPressed){
				mode = 0;
			}else if(InputControl.mouseLeftDown && hoveredCell != null && hoveredCell.isHovered){
				if(InputControl.shiftDown){
					if(WillCellRemovalDestroyRoom(hoveredCellCoords + origin, true)){
						Coordinates contestedCoords = hoveredCellCoords + origin;
						OpenConfirmationDialog("You are attempting to remove the last cell of this room. Continue and destroy the room?", (bool confirm) => {
							if(confirm){
								selectedRoom.RemoveCellAtCoord(contestedCoords);
								SelectRoom(null);
								mode = 0;
							}
						});
					}else if(WillCellRemovalBreakRoom(hoveredCellCoords + origin, true)){
						Coordinates contestedCoords = hoveredCellCoords + origin;
						OpenConfirmationDialog("Break room continuity and create additional rooms?", (bool confirm) => {
							if(confirm){
								BreakRoomContinuity(contestedCoords);
								selectedRoom.RemoveCellAtCoord(contestedCoords);
								displayUpdateRequired = true;
							}
						});
					}else{
						selectedRoom.RemoveCellAtCoord(hoveredCellCoords + origin);
						displayUpdateRequired = true;
					}
					
				}else if(hoveredCell.CanAnnex()){
					if(CellPlacementHasCollision(hoveredCellCoords + origin)){
						if(InputControl.ctrlDown){
							if(WillCellRemovalDestroyRoom(hoveredCellCoords + origin, false)){
								Coordinates contestedCoords = hoveredCellCoords + origin;
								OpenConfirmationDialog("You are attempting to remove the last cell of a different room. Continue and destroy that room?", (bool confirm) => {
									if(confirm){
										selectedRoom.AddCellAtCoord(contestedCoords);
										displayUpdateRequired = true;
									}
								});
							}else if(WillCellRemovalBreakRoom(hoveredCellCoords + origin, false)){
								Coordinates contestedCoords = hoveredCellCoords + origin;
								OpenConfirmationDialog("Break room continuity and create additional rooms?", (bool confirm) => {
									if(confirm){
										BreakRoomContinuity(contestedCoords);
										selectedRoom.AddCellAtCoord(contestedCoords);
										displayUpdateRequired = true;
									}
								});
							}else{
								selectedRoom.AddCellAtCoord(hoveredCellCoords + origin);
								displayUpdateRequired = true;
							}
						}
					}else{
						selectedRoom.AddCellAtCoord(hoveredCellCoords + origin);
						displayUpdateRequired = true;
					}
				}	
			}
		}else if(mode == 3){ //Bulk Placement
			if(InputControl.mouseRightPressed || InputControl.cancelPressed){
				CancelPaste();
				mode = 0;
				SelectRoom(null);
			}else if(InputControl.mouseLeftPressed && hoveredCell != null){
				if(IsValidPlacement(hoveredCellCoords + origin)){
					PlacePastedRoom(hoveredCellCoords + origin);
				}
			}
		}else if(mode == 4){
			if(InputControl.mouseRightPressed || InputControl.cancelPressed){
				mode = 0;
				SelectRoom(null);
			}
		}
		if(InputControl.newPressed){
			mode = 1;
		}else if(InputControl.edgePressed){
			mode = 4;
		}
		Navigate();

		if(displayUpdateRequired){
			UpdateDisplay();
		}
	}

	void SetMode(int newMode){
		if(_mode == newMode){return;}

		if(_mode == 3){
			CancelPaste();
		}

		if(_mode == 4){
			SetEdgeClickability(false);
		}else if(newMode == 4){
			SetEdgeClickability(true);
		}
		_mode = newMode;
	}

	void SetEdgeClickability(bool areEdgesClickable){
		for(int i=0;i<horizontalEdges.GetLength(1);i++){
			for(int j=0;j<horizontalEdges.GetLength(0);j++){
				horizontalEdges[j,i].SetClickable(areEdgesClickable);
			}
		}
		for(int i=0;i<verticalEdges.GetLength(1);i++){
			for(int j=0;j<verticalEdges.GetLength(0);j++){
				verticalEdges[j,i].SetClickable(areEdgesClickable);
			}
		}
	}

	void GridSetup(){
		if(gridExists){
			GridClear();
		}
		gridExists = true;
		
		RectTransform mapContainer = transform.Find("MapSection").Find("MapContainer").GetComponent<RectTransform>();
		//Rect rect = GetComponent<RectTransform>().rect;
		//Determine how many tiles will fit within the maximum rectangle
		//this number will be equal to (height/width of the screen - margins) / tilewidth rounded down
		//set rect to that size
		int scrollbarWidth = 32;
		float w = (Screen.width - 608 - 32 - scrollbarWidth) / cellSize; //max = 82
		float h = (Screen.height - 192 - 32 - scrollbarWidth) / cellSize;//max = 55
		if(w > dungeon.width){
			w = dungeon.width;
		}
		if(h > dungeon.height){
			h = dungeon.height;
		}
		gridWidth = (int)w;
		gridHeight = (int)h;

		if(gridWidth + origin.x > dungeon.height){
			origin.x = dungeon.width - gridWidth;
		}
		if(gridHeight + origin.y > dungeon.height){
			origin.y = dungeon.height - gridHeight;
		}

		mapContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (w + 0.125f)*cellSize);
		mapContainer.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (h + 0.125f)*cellSize);

		GameObject go;
		float x, y;
		grid = new MapGridCell[gridWidth,gridHeight];
		MapGridCell gc;
		Transform tileHolder = mapContainer.Find("TileHolder");
		for (int i=0; i<gridHeight; i++){
			for(int j=0; j<gridWidth; j++){
				go = PoolControl.WithdrawMapGridCell();
				gc = go.GetComponent<MapGridCell>();
				gc.map = this;
				gc.menu = this;
				gc.section = sections[0];
				gc.coords = new Coordinates(j,i);
				gc.SetSize(cellSize);
				go.transform.SetParent(tileHolder);
				x = j*cellSize - gridWidth*cellSize/2 + cellSize/2;
				y = -i*cellSize + gridHeight*cellSize/2 - cellSize/2;
				go.transform.localPosition = new Vector3(x,y,0f);
				grid[j,i] = gc;
			}
		}

		borders = new MapCellBorder[gridWidth + 1, gridHeight + 1];
		MapCellBorder mcb;
		Transform borderHolder = mapContainer.Find("BorderHolder");
		for (int i=0; i<borders.GetLength(1); i++){
			for(int j=0; j<borders.GetLength(0); j++){
				go = PoolControl.WithdrawMapGridBorder();
				mcb = go.GetComponent<MapCellBorder>();
				mcb.map = this;
				mcb.menu = this;
				mcb.section = sections[0];
				mcb.SetSize(cellSize);
				go.transform.SetParent(borderHolder);
				x = j*cellSize - gridWidth*cellSize/2;
				y = -i*cellSize + gridHeight*cellSize/2;
				go.transform.localPosition = new Vector3(x,y,0f);
				borders[j,i] = mcb;
			}
		}

		MapCellEdge mce;

		horizontalEdges = new MapCellEdge[gridWidth, gridHeight + 1];
		Transform edgeHolder = mapContainer.Find("EdgeHolder");
		for (int i=0; i<horizontalEdges.GetLength(1); i++){
			for(int j=0; j<horizontalEdges.GetLength(0); j++){
				go = PoolControl.WithdrawMapGridEdge();
				mce = go.GetComponent<MapCellEdge>();
				mce.map = this;
				mce.menu = this;
				mce.section = sections[0];
				go.transform.SetParent(edgeHolder);
				mce.SetSize(cellSize);
				if(i == 0){
					mce.cellA = null;
					mce.cellB = grid[j,i];
				}else if(i == gridHeight){
					mce.cellA = grid[j,i-1];
					mce.cellB = null;
				}else{
					mce.cellA = grid[j,i-1];
					mce.cellB = grid[j,i];
				}
				x = j*cellSize - gridWidth*cellSize/2 + cellSize/2;
				y = -i*cellSize + gridHeight*cellSize/2;
				go.transform.localPosition = new Vector3(x,y,0f);
				go.transform.eulerAngles = new Vector3(0,0,0);
				mce.isHorizontal = true;
				horizontalEdges[j,i] = mce;
			}
		}

		verticalEdges = new MapCellEdge[gridWidth + 1, gridHeight];
		for (int i=0; i<verticalEdges.GetLength(1); i++){
			for(int j=0; j<verticalEdges.GetLength(0); j++){
				go = PoolControl.WithdrawMapGridEdge();
				mce = go.GetComponent<MapCellEdge>();
				mce.map = this;
				mce.menu = this;
				mce.section = sections[0];
				go.transform.SetParent(edgeHolder);
				mce.SetSize(cellSize);
				if(j == 0){
					mce.cellA = null;
					mce.cellB = grid[j,i];
				}else if(j == gridWidth){
					mce.cellA = grid[j-1,i];
					mce.cellB = null;
				}else{
					mce.cellA = grid[j-1,i];
					mce.cellB = grid[j,i];
				}
				x = j*cellSize - gridWidth*cellSize/2;
				y = -i*cellSize + gridHeight*cellSize/2 - cellSize/2;
				go.transform.localPosition = new Vector3(x,y,0f);
				go.transform.eulerAngles = new Vector3(0,0,90f);
				mce.isHorizontal = false;
				verticalEdges[j,i] = mce;
			}
		}

		SetScrollbars();

		displayUpdateRequired = true;
	}

	void GridClear(){
		for(int i=0;i<grid.GetLength(1);i++){
			for(int j=0;j<grid.GetLength(0);j++){
				if(grid[j,i] != null){
					PoolControl.DepositMapGridCell(grid[j,i].gameObject);
				}
			}
		}

		for(int i=0;i<borders.GetLength(1);i++){
			for(int j=0;j<borders.GetLength(0);j++){
				if(borders[j,i] != null){
					PoolControl.DepositMapGridBorder(borders[j,i].gameObject);
				}
			}
		}

		for(int i=0;i<horizontalEdges.GetLength(1);i++){
			for(int j=0;j<horizontalEdges.GetLength(0);j++){
				if(horizontalEdges[j,i] != null){
					PoolControl.DepositMapGridEdge(horizontalEdges[j,i].gameObject);	
				}
			}
		}

		for(int i=0;i<verticalEdges.GetLength(1);i++){
			for(int j=0;j<verticalEdges.GetLength(0);j++){
				if(verticalEdges[j,i] != null){
					PoolControl.DepositMapGridEdge(verticalEdges[j,i].gameObject);	
				}
			}
		}
	}

	void Navigate(){
		if(InputControl.arrowPressed[0]){
			if(origin.x + gridWidth < dungeon.width){
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
			if(origin.y + gridHeight < dungeon.height){
				origin += new Coordinates (0,1);
				displayUpdateRequired = true;
			}
		}
		if(InputControl.floorAscendPressed && floorIndex > 0){
			SetFloorIndex(floorIndex - 1);
			displayUpdateRequired = true;
			floorNavigationPanel.FocusSelectedFloorInList();
		}
		if(InputControl.floorDescendPressed && floorIndex < dungeon.floors.Count - 1){
			SetFloorIndex(floorIndex + 1);
			displayUpdateRequired = true;
			floorNavigationPanel.FocusSelectedFloorInList();
		}
		SetScrollbars();
	}

	void SetScrollbars(){
		scrollbarHorizontal.SetScrollbar(origin.x, dungeon.width, grid.GetLength(0));
		scrollbarVertical.SetScrollbar(origin.y, dungeon.height, grid.GetLength(1));		
	}

	void OnScroll(){
		origin = new Coordinates(scrollbarHorizontal.value, scrollbarVertical.value);
		UpdateDisplay();
	}

	void SetScrollButtonsEnabled(){
		scrollButtons[0].gameObject.SetActive(dungeon.width > gridWidth);
		scrollButtons[2].gameObject.SetActive(dungeon.width > gridWidth);
		scrollButtons[1].gameObject.SetActive(dungeon.height > gridHeight);
		scrollButtons[3].gameObject.SetActive(dungeon.height > gridHeight);

		scrollButtons[0].isDisabled = origin.x + gridWidth >= dungeon.width;
		scrollButtons[1].isDisabled = origin.y <= 0;
		scrollButtons[2].isDisabled = origin.x <= 0;
		scrollButtons[3].isDisabled = origin.y + gridHeight >= dungeon.height;
	}

	void OnScrollButton(int index){
		if(index == 0){
			if(origin.x + gridWidth < dungeon.width){
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
			if(origin.y + gridHeight < dungeon.height){
				origin += new Coordinates (0,1);
			}
		}
		SetScrollbars();
		UpdateDisplay();
	}

	public void SetFloorIndex(int index){
		mode = 0;
		SelectRoom(null);
		floorIndex = index;
		displayUpdateRequired = true;
	}

	public void UpdateDisplay(){
		int w,h;
		w = grid.GetLength(0);
		h = grid.GetLength(1);
		for (int i=0; i<h; i++){
			for(int j=0; j<w; j++){
				grid[j,i].SetSprite(currentFloor.GetCellAtCoord(origin + new Coordinates(j,i)));
			}
		}
		w = borders.GetLength(0);
		h = borders.GetLength(1);
		for (int i=0; i<h; i++){
			for(int j=0; j<w; j++){
				borders[j,i].SetSprite(origin + new Coordinates(j,i));
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

	public void OpenRoomEditorMenu(){
		if(selectedRoom == null){return;}
		roomEditorMenu.map = this;
		roomEditorMenu.SetRoom(selectedRoom);
		roomEditorMenu.Open();
		roomEditorMenu.Activate();
	}

	void AddRoom(Coordinates worldCoords){
		DungeonRoom dr = new DungeonRoom(currentFloor);
		dr.AddCellAtCoord(worldCoords);
		mode = 2;
		SelectRoom(dr);
	}

	void SelectRoom(DungeonRoom dr){
		selectedRoom = dr;
		displayUpdateRequired = true;
	}

	public MapGridCell MapGridCellAt(Coordinates c){
		return c.InBounds(gridWidth,gridHeight) ? grid[c.x,c.y] : null;
	}

	public bool CellPlacementHasCollision(Coordinates worldCoordinates){
		DungeonCell dc = currentFloor.GetCellAtCoord(worldCoordinates);
		return dc.room != null;
	}

	public bool WillCellRemovalDestroyRoom(Coordinates worldCoordinates, bool deletionMode){
		DungeonCell dc = currentFloor.GetCellAtCoord(worldCoordinates);
		if(dc.room == null){
			return false;
		}
		DungeonRoom dr;
		if(deletionMode){
			dr = selectedRoom;
		}else{
			dr = dc.room;
		}

		return dr.cells.Count == 1;
	}

	public bool WillCellRemovalBreakRoom(Coordinates c, bool deletionMode){
		DungeonCell dc = currentFloor.GetCellAtCoord(c);
		if(dc.room == null){
			return false;
		}
		DungeonRoom dr;
		if(deletionMode){
			dr = selectedRoom;
		}else{
			dr = dc.room;
		}

		bool[,] tempGrid = new bool[dungeon.width,dungeon.height];
		tempGrid[c.x,c.y] = true;

		for(int i=0;i<dr.cells.Count;i++){
			if(dr.cells[i]!=c){
				c = dr.cells[i];
				break;
			}
		}
		tempGrid[c.x,c.y] = true;

		CheckNeighboringCells(tempGrid, c, dr);

		for(int i=0; i<dr.cells.Count; i++){
			if(tempGrid[dr.cells[i].x,dr.cells[i].y] == false){
				return true;
			}
		}

		return false;
	}

	public void BreakRoomContinuity(Coordinates c){
		DungeonCell dc = currentFloor.GetCellAtCoord(c);
		DungeonRoom dr = dc.room;

		bool[,] tempGrid = new bool[dungeon.width,dungeon.height];
		tempGrid[c.x,c.y] = true; //mark starting cell. its invalid
		
		int killswitch = 0;
		List<Coordinates> orphans = new List<Coordinates>();
		Coordinates start = c;
		while(true){

			//for each cell in the threatened room, cycle through until you find one that isn't the threatened cell
			//this is just a way to make sure the search doesn't start on the threatened cell (since obviously that cell connects the halves)
			for(int i=0;i<dr.cells.Count;i++){
				if(dr.cells[i]!=c){
					start = dr.cells[i];
					break;
				}
			}

			tempGrid[start.x,start.y] = true;

			CheckNeighboringCells(tempGrid, start, dr); //perform recursive check on the temp grid
			
			//for each cell in the original room, add all cells that weren't marked "true" to the list of orphans
			//if any orphans were found, then run the loop again to check if there are orphans of the orphan
			orphans.Clear();
			for(int i=0; i<dr.cells.Count; i++){
				if(tempGrid[dr.cells[i].x,dr.cells[i].y] == false){
					orphans.Add(dr.cells[i]);
				}
			}

			//if any orphans were found
			//create a new room and add the orphans to its list of cells
			//if no orpahans were found, break the loop
			if(orphans.Count > 0){
				dr = new DungeonRoom(currentFloor);
				for(int i=0; i<orphans.Count; i++){
					dr.AddCellAtCoord(orphans[i]);
				}
			}else{
				break;
			}

			killswitch += 1;
			if(killswitch > 4){
				Debug.Log("Killswitch Tripped");
				break;
			}
		}
	}

	void CheckNeighboringCells(bool[,] tempGrid, Coordinates c, DungeonRoom dr){
		Coordinates[] ns = c.OrthogonalNeighbors();
		for(int i=0;i<4;i++){
			if(!ns[i].InBounds(dungeon.width,dungeon.height) || tempGrid[ns[i].x, ns[i].y] || currentFloor.GetCellAtCoord(ns[i]).room != dr){
				//do nothing
			}else{
				tempGrid[ns[i].x,ns[i].y] = true;
				CheckNeighboringCells(tempGrid, ns[i], dr);
			}
		}
	}

	void DeleteRoom(bool destroySecondarySelections=false){
		//TODO if(ApplicationSettings.warnOnDeleteRoom){WarnThem();}
		if(selectedRoom != null){
			currentFloor.DestroyRoom(selectedRoom);
			displayUpdateRequired = true;
		}
	}

	void CopyRoom(bool copySecondarySelections=false){
		if(floatingRoom != null){
			clipboard.DestroyRoom(floatingRoom);
		}

		DungeonRoom dr = new DungeonRoom(clipboard);
		for(int i=0;i<selectedRoom.cells.Count;i++){
			dr.AddCellAtCoord(selectedRoom.cells[i]);
			for(int j=0;j<4;j++){
				if(currentFloor.GetCellAtCoord(selectedRoom.cells[i]).edges[j].hasDoor){
					clipboard.GetCellAtCoord(selectedRoom.cells[i]).edges[j].hasDoor = true;
				}
			}
			//TODO, copy DungeonCell Data to clipboard
		}
		floatingRoom = dr;
		floatingRoomExtents = selectedRoom.CalculateExtents();
		floatingRoomOriginalFloor = selectedRoom.floor;
		floatingRoomOrigin = selectedRoom.origin;

	}

	void CutRoom(bool cutSecondarySelections=false){
		CopyRoom(cutSecondarySelections);
		DeleteRoom(cutSecondarySelections);
	}

	void BeginPasteRoom(){
		if(clipboard.rooms.Count > 0){
			mode = 3;	
		}
	}

	bool IsValidPlacement(Coordinates worldCoords){
		bool isValid = true;
		Coordinates localOrigin = worldCoords - floatingRoomOrigin;
		Coordinates localPosition;
		for(int i=0;i<floatingRoom.cells.Count;i++){
			localPosition = localOrigin + floatingRoom.cells[i];
			if(!localPosition.InBounds(dungeon.width, dungeon.height) || currentFloor.GetCellAtCoord(localPosition).room != null){
				isValid = false;
				break;
			}
		}
		return isValid;
	}

	void PlacePastedRoom(Coordinates worldCoords){
		DungeonRoom dr = new DungeonRoom(currentFloor);
		Coordinates offset = worldCoords - floatingRoomOrigin;
		for(int i=0;i<floatingRoom.cells.Count;i++){
			dr.AddCellAtCoord(floatingRoom.cells[i] + offset);
			for(int j=0;j<4;j++){
				if(clipboard.GetCellAtCoord(floatingRoom.cells[i]).edges[j].hasDoor){
					currentFloor.GetCellAtCoord(floatingRoom.cells[i] + offset).SetWall(j, true);
				}
			}
			//TODO: paste DungeonCell Data from clipboard
		}
		if(isMovingRoom){
			clipboard.DestroyRoom(floatingRoom);
			floatingRoom = null;
		}
		isMovingRoom = false;
		SelectRoom(dr);
		mode = 0;
		displayUpdateRequired = true;
	}

	void CancelPaste(){
		if(isMovingRoom){
			//put the room back where it was
			DungeonRoom dr = new DungeonRoom(floatingRoomOriginalFloor);
			for(int i=0;i<floatingRoom.cells.Count;i++){
				dr.AddCellAtCoord(floatingRoom.cells[i]);
				//TODO: paste DungeonCell Data from clipboard
			}
			clipboard.DestroyRoom(floatingRoom);
			SelectRoom(dr);
		}else{
			SelectRoom(null);
		}
		isMovingRoom = false;
		displayUpdateRequired = true;
	}


	public void SetCellSize(int px){
		cellSize = px;
		GridSetup();
		if(mode == 4){
			SetEdgeClickability(true);
		}else{
			SetEdgeClickability(false);
		}
		UpdateDisplay();
	}

	public void OnChangeDungeonSettings(){
		GridSetup();
	}
}
