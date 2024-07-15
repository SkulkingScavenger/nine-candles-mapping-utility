using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon {
	public string fileName;
	public string name;
	public int width;
	public int height;
	public int tilesetIndex = 0;
	public Tileset tileset {
		get { return TilesetManager.GetAtIndex(tilesetIndex); }
		set {}
	}
	public int gameSystemIndex = 0;
	public GameSystem gameSystem {
		get { return MasterControl.gameSystems[gameSystemIndex]; }
		set {}
	}
	public List<DungeonFloor> floors;

	public IList[] contentLists = new IList[6];
	public List<Monster> monsters;
	public List<Treasure> treasures;
	public List<Trap> traps;
	public List<Artifact> artifacts;
	public List<DungeonFeature> dungeonFeatures;
	public List<RandomTable> randomTables;
	public int floorCount {
		get {return floors.Count;}
		set {}
	}

	public Dungeon(int w, int h, string n, int gsi){
		fileName = "";
		name = n;
		gameSystemIndex = gsi;
		width = w;
		height = h;
		tilesetIndex = 0;

		floors = new List<DungeonFloor>();
		floors.Add(new DungeonFloor(0, this));

		monsters = new List<Monster>();
		artifacts = new List<Artifact>();
		treasures = new List<Treasure>();
		traps = new List<Trap>();
		dungeonFeatures = new List<DungeonFeature>();
		randomTables = new List<RandomTable>();

		contentLists[0] = (IList)artifacts;
	}

	public void SetDimensions(int w, int h){
		//adjust everything
	}

	public DungeonCell GetCellAtCoord(int x, int y, int z){
		DungeonFloor df = floors[z];
		return df.GetCellAtCoord(x,y);
	}

	public void AssignSystemID(Monster monster){
		bool flag = true;
		bool isAvailable = true;
		uint prospectiveID = 0;
		while(flag){
			prospectiveID += 1;
			isAvailable = true;
			for(int i=0;i<monsters.Count;i++){
				if(monsters[i].systemID == prospectiveID){
					isAvailable = false;
					break;
				}
			}
			if(isAvailable){
				monster.systemID = prospectiveID;
				flag = false;
			}
		}
	}

	public void AssignSystemID(Artifact artifact){
		bool flag = true;
		bool isAvailable = true;
		uint prospectiveID = 0;
		while(flag){
			prospectiveID += 1;
			isAvailable = true;
			for(int i=0;i<artifacts.Count;i++){
				if(artifacts[i].systemID == prospectiveID){
					isAvailable = false;
					break;
				}
			}
			if(isAvailable){
				artifact.systemID = prospectiveID;
				flag = false;
			}
		}
	}

	public void AssignSystemID(Treasure treasure){
		bool flag = true;
		bool isAvailable = true;
		uint prospectiveID = 0;
		while(flag){
			prospectiveID += 1;
			isAvailable = true;
			for(int i=0;i<treasures.Count;i++){
				if(treasures[i].systemID == prospectiveID){
					isAvailable = false;
					break;
				}
			}
			if(isAvailable){
				treasure.systemID = prospectiveID;
				flag = false;
			}
		}
	}

	public void AssignSystemID(Trap trap){
		bool flag = true;
		bool isAvailable = true;
		uint prospectiveID = 0;
		while(flag){
			prospectiveID += 1;
			isAvailable = true;
			for(int i=0;i<traps.Count;i++){
				if(traps[i].systemID == prospectiveID){
					isAvailable = false;
					break;
				}
			}
			if(isAvailable){
				trap.systemID = prospectiveID;
				flag = false;
			}
		}
	}

	public void AssignSystemID(DungeonFeature dungeonFeature){
		bool flag = true;
		bool isAvailable = true;
		uint prospectiveID = 0;
		while(flag){
			prospectiveID += 1;
			isAvailable = true;
			for(int i=0;i<dungeonFeatures.Count;i++){
				if(dungeonFeatures[i].systemID == prospectiveID){
					isAvailable = false;
					break;
				}
			}
			if(isAvailable){
				dungeonFeature.systemID = prospectiveID;
				flag = false;
			}
		}
	}

	public void AssignSystemID(RandomTable randomTable){
		bool flag = true;
		bool isAvailable = true;
		uint prospectiveID = 0;
		while(flag){
			prospectiveID += 1;
			isAvailable = true;
			for(int i=0;i<randomTables.Count;i++){
				if(randomTables[i].systemID == prospectiveID){
					isAvailable = false;
					break;
				}
			}
			if(isAvailable){
				randomTable.systemID = prospectiveID;
				flag = false;
			}
		}
	}

	public DungeonFeature GetDungeonFeatureByID(uint id){
		if(id == 0){return null;}
		for(int i=0;i<dungeonFeatures.Count;i++){
			if(dungeonFeatures[i].systemID == id){
				return dungeonFeatures[i];
			}
		}
		return null;
	}

	public Monster GetMonsterByID(uint id){
		if(id == 0){return null;}
		for(int i=0;i<monsters.Count;i++){
			if(monsters[i].systemID == id){
				return monsters[i];
			}
		}
		return null;
	}

	public List<string> ToJSON(int indentAmount){
		List<string> output = new List<string>();
		output.Add(FileUtilities.Indentation(indentAmount) + "\"Dungeon\" : {");
		indentAmount += 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "\"name\" : " + FileUtilities.WrapString(name) + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"gameSystemIndex\" : " + gameSystemIndex.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"width\" : " + width.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"height\" : " + height.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"tilesetIndex\" : " + tilesetIndex.ToString() + ",");

		//floors
		output.Add(FileUtilities.Indentation(indentAmount) + "\"floors\" : [");
		indentAmount += 1;
		for(int i=0;i<floors.Count;i++){
			output.AddRange(floors[i].ToJSON(indentAmount));
			if(i != floors.Count -1){
				output[output.Count - 1] += ",";
			}
		}
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "],");

		//monsters
		output.Add(FileUtilities.Indentation(indentAmount) + "\"monsters\" : [");
		indentAmount += 1;
		for(int i=0;i<monsters.Count;i++){
			output.AddRange(monsters[i].ToJSON(indentAmount));
			if(i != monsters.Count -1){
				output[output.Count - 1] += ",";
			}
		}
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "],");

		//artifacts
		output.Add(FileUtilities.Indentation(indentAmount) + "\"artifacts\" : [");
		indentAmount += 1;
		for(int i=0;i<artifacts.Count;i++){
			output.AddRange(artifacts[i].ToJSON(indentAmount));
			if(i != artifacts.Count -1){
				output[output.Count - 1] += ",";
			}
		}
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "],");

		//treasures
		output.Add(FileUtilities.Indentation(indentAmount) + "\"treasures\" : [");
		indentAmount += 1;
		for(int i=0;i<treasures.Count;i++){
			output.AddRange(treasures[i].ToJSON(indentAmount));
			if(i != treasures.Count -1){
				output[output.Count - 1] += ",";
			}
		}
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "],");

		//traps
		output.Add(FileUtilities.Indentation(indentAmount) + "\"traps\" : [");
		indentAmount += 1;
		for(int i=0;i<traps.Count;i++){
			output.AddRange(traps[i].ToJSON(indentAmount));
			if(i != traps.Count -1){
				output[output.Count - 1] += ",";
			}
		}
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "],");


		//dungeonFeatures
		output.Add(FileUtilities.Indentation(indentAmount) + "\"dungeonFeatures\" : [");
		indentAmount += 1;
		for(int i=0;i<dungeonFeatures.Count;i++){
			output.AddRange(dungeonFeatures[i].ToJSON(indentAmount));
			if(i != dungeonFeatures.Count -1){
				output[output.Count - 1] += ",";
			}
		}
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "],");
		
		//randomTables
		output.Add(FileUtilities.Indentation(indentAmount) + "\"randomTables\" : [");
		indentAmount += 1;
		for(int i=0;i<randomTables.Count;i++){
			output.AddRange(randomTables[i].ToJSON(indentAmount));
			if(i != randomTables.Count -1){
				output[output.Count - 1] += ",";
			}
		}
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "],");


		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "}");
		return output;
	}
}

public class DungeonFloor {
	public Dungeon dungeon;
	public DungeonCell[,] cells;
	public int index;
	public string description = "";
	public List<DungeonRoom> rooms;

	public DungeonFloor(int floorIndex, Dungeon dngn){
		dungeon = dngn;
		index = floorIndex;
		rooms = new List<DungeonRoom>();
		cells = new DungeonCell[dngn.width, dngn.height];
		for(int i=0; i<dngn.height; i++){
			for(int j=0; j<dngn.width; j++){
				cells[j,i] = new DungeonCell(j,i,this);
				cells[j,i].coords = new Coordinates(j,i);
				cells[j,i].dungeon = dungeon;
				cells[j,i].UpdateGraphics();
				cells[j,i].edges[0] = new DungeonCellEdge(); 
				if(i == 0){
					cells[j,i].edges[1] = new DungeonCellEdge();
				}else{
					cells[j,i].edges[1] = cells[j,i-1].edges[3];
				}
				if(j == 0){
					cells[j,i].edges[2] = new DungeonCellEdge(); //edges[2] = WEST; west = east of previous
				}else{
					cells[j,i].edges[2] = cells[j-1,i].edges[0];
				}
				cells[j,i].edges[3] = new DungeonCellEdge();

			}
		}
	}

	public DungeonCell GetCellAtCoord(int x, int y){
		Coordinates c = new Coordinates(x,y);
		return GetCellAtCoord(c);
	}

	public DungeonCell GetCellAtCoord(Coordinates c){
		return c.InBounds(dungeon.width,dungeon.height) ? cells[c.x,c.y] : null;
	}

	public void DestroyRoom(DungeonRoom room){
		DungeonRoom dr;
		for (int i=0;i<rooms.Count;i++){
			dr = rooms[i];
			if (dr == room){
				for(int j=0;j<dr.cells.Count;j++){
					GetCellAtCoord(dr.cells[j]).room = null;
				}
				rooms.RemoveAt(i);
				//TO DO: renumber rooms
			}
		}
	}

	public List<string> ToJSON(int indentAmount){
		List<string> output = new List<string>();
		output.Add(FileUtilities.Indentation(indentAmount) + "{");
		indentAmount += 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "\"description\" : " + FileUtilities.WrapString(description) + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"rooms\" : " + "[");
		indentAmount += 1;
		for(int i=0;i<rooms.Count;i++){
			output.AddRange(rooms[i].ToJSON(indentAmount));
			if(i != rooms.Count - 1){
				output[output.Count - 1] += ",";
			}
		}
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "]");
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "}");
		return output;
	}

}

public class DungeonRoom {
	public DungeonFloor floor;
	public List<Coordinates> cells;
	public string name;
	public int index;
	public string description;
	public string descriptionPlayer;
	public string descriptionTreasure;
	public Coordinates origin;

	public DungeonRoom(DungeonFloor df){
		floor = df;
		cells = new List<Coordinates>();
		floor.rooms.Add(this);
	}

	public void AddCellAtCoord(Coordinates c){
		DungeonCell dc = floor.GetCellAtCoord(c);
		if (dc.room != null){
			dc.room.RemoveCellAtCoord(c,this);
		}else{
			dc.room = this;	
		}
		cells.Add(c);
		
	}

	public void RemoveCellAtCoord(Coordinates c, DungeonRoom newRoom = null){
		for(int i=0; i<cells.Count;i++){
			if(cells[i] == c){
				floor.GetCellAtCoord(cells[i]).room = newRoom;
				cells.RemoveAt(i);
				if(cells.Count == 0){
					floor.DestroyRoom(this);
				}
				break;
			}
		}
	}

	public Coordinates CalculateExtents(){
		int minW, maxW, minH, maxH;
		minW = maxW = cells[0].x;
		minH = maxH = cells[0].y;
		for(int i=1;i<cells.Count;i++){
			if(cells[i].x < minW){
				minW = cells[i].x;
			}
			if(cells[i].x > maxW){
				maxW = cells[i].x;
			}
			if(cells[i].y < minH){
				minH = cells[i].y;
			}
			if(cells[i].y > maxH){
				maxH = cells[i].y;
			}
		}
		//NOTE: extents of zero mean width of 1. width 0 rooms can't exist
		int w = Math.Abs(maxW-minW);
		int h = Math.Abs(maxH-minH);

		origin = new Coordinates(minW, minH);
		return new Coordinates(w,h);
	}

	public List<string> ToJSON(int indentAmount){
		List<string> output = new List<string>();
		output.Add(FileUtilities.Indentation(indentAmount) + "{");
		indentAmount += 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "\"name\" : " + FileUtilities.WrapString(name) + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"description\" : " + FileUtilities.WrapString(description) + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"descriptionPlayer\" : " + FileUtilities.WrapString(descriptionPlayer) + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"descriptionTreasure\" : " + FileUtilities.WrapString(descriptionTreasure) + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"cells\" : " + "[");
		indentAmount += 1;
		for(int i=0;i<cells.Count;i++){
			output.AddRange(floor.GetCellAtCoord(cells[i]).ToJSON(indentAmount));
			if(i != cells.Count - 1){
				output[output.Count - 1] += ",";
			}
		}
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "]");
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "}");
		return output;
	}
}

public class DungeonCell {
	public Dungeon dungeon;
	public DungeonFloor floor;
	public Coordinates coords;
	public uint dungeonFeatureID = 0;
	public uint monsterID = 0;
	private DungeonRoom _room = null;
	public DungeonRoom room {
		get {return _room;}
		set {SetRoom(value);}
	}
	public float rot = 0;
	public Sprite spr = null;
	public DungeonCellEdge[] edges = new DungeonCellEdge[4];

	public int x {
		get {return coords.x;}
		set {coords = new Coordinates(value, y);}
	}
	public int y {
		get {return coords.y;}
		set {coords = new Coordinates(x, value);}
	}

	public DungeonFeature dungeonFeature {
		get {
			return dungeon.GetDungeonFeatureByID(dungeonFeatureID);
		}
		set {
			if(value == null){
				dungeonFeatureID = 0;
			}else{
				dungeonFeatureID = value.systemID;
			}
		}
	}

	public Monster monster {
		get {
			return dungeon.GetMonsterByID(monsterID);
		}
		set {
			if(value == null){
				monsterID = 0;
			}else{
				monsterID = value.systemID;
			}
		}
	}

	public DungeonCell(int a, int b, DungeonFloor f){
		coords = new Coordinates(a,b);
		floor = f;
	}

	public void UpdateGraphics(){
		spr = dungeon.tileset.RandomTile();
		rot = (float)(UnityEngine.Random.Range(0, 4)*90);
	}

	public void SetRoom(DungeonRoom dr){
		if(dr == _room){return;}
		if(_room == null){ //add any room-specific data
			
		}else if(dr == null){ //cleardata
			
		}else{

		}
		_room = dr;
		RemoveInvalidDoors();
	}

	public void RemoveInvalidDoors(){
		Coordinates[] ns = coords.OrthogonalNeighbors();
		DungeonCell dc;
		bool bothNull;
		bool sameRoom;
		for(int i=0;i<4;i++){
			dc = floor.GetCellAtCoord(ns[i]);
			bothNull = (dc == null || dc.room == null) && room == null;
			sameRoom = dc != null && dc.room == room;
			if(bothNull || sameRoom){
				SetWall(i, false);
			}
		}
	}

	public void SetWall(int edgeIndex, bool hasDoor){
		edges[edgeIndex].hasDoor = hasDoor;
	}

	public List<string> ToJSON(int indentAmount){
		List<string> output = new List<string>();
		output.Add(FileUtilities.Indentation(indentAmount) + "{");
		indentAmount += 1;

		output.Add(FileUtilities.Indentation(indentAmount) + "\"x\" : " + x.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"y\" : " + y.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"dungeonFeatureID\" : " + dungeonFeatureID.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"monsterID\" : " + monsterID.ToString() + ",");

		output.Add(FileUtilities.Indentation(indentAmount) + "\"edges\" : [");
		indentAmount += 1;
		for(int i=0;i<4;i++){
			string str = edges[i].hasDoor ? "true" : "false";
			if(i != 3){
				str += ",";
			}
			output.Add(FileUtilities.Indentation(indentAmount) + str);
		}
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "]");

		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "}");
		return output;
	}
}

public class DungeonCellEdge {
	public bool hasDoor = false;
}

[Serializable]
public class Tileset {
	public Sprite[] tileSprites;
	public Sprite[] borderSprites;
	public Sprite backgroundSprite;
	public Sprite doorSprite;
	public Sprite stairSprite;

	public Sprite RandomTile(){
		return tileSprites[UnityEngine.Random.Range(0, tileSprites.Length)];
	}
}