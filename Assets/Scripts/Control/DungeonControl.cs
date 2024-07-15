using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonControl : MonoBehaviour{

	public static Dungeon dungeon {
		get {return MasterControl.dungeon;}
		set {}
	}

	public static void NewDungeon(){

	}


	public static void SaveDungeon(){
		FileUtilities.WriteDungeonToFile(dungeon);
	}

	public static Dungeon LoadDungeon(string filename){
		Dungeon dng;
		List<JSONObject> objects = FileUtilities.LoadDungeonFromJSON(filename);

		JSONObject dungeonObj = objects[0].objectObjects["Dungeon"];
		string name = dungeonObj.objectObjects["name"].fieldValue;
		int gameSystemIndex = Int32.Parse(dungeonObj.objectObjects["gameSystemIndex"].fieldValue);
		int width = Int32.Parse(dungeonObj.objectObjects["width"].fieldValue);
		int height = Int32.Parse(dungeonObj.objectObjects["height"].fieldValue);
		int tilesetIndex = Int32.Parse(dungeonObj.objectObjects["tilesetIndex"].fieldValue);
		dng = new Dungeon(width, height, name, gameSystemIndex);
		dng.fileName = filename;

		JSONObject arr;

		arr = dungeonObj.objectObjects["monsters"];
		for(int i=0;i<arr.arrayObjects.Count;i++){
			Monster m = LoadMonster(arr.arrayObjects[i], dng);
			dng.monsters.Add(m);
		}

		arr = dungeonObj.objectObjects["treasures"];
		for(int i=0;i<arr.arrayObjects.Count;i++){
			Treasure treasure = LoadTreasure(arr.arrayObjects[i]);
			dng.treasures.Add(treasure);
		}

		arr = dungeonObj.objectObjects["artifacts"];
		for(int i=0;i<arr.arrayObjects.Count;i++){
			Artifact art = LoadArtifact(arr.arrayObjects[i]);
			dng.artifacts.Add(art);
		}

		arr = dungeonObj.objectObjects["traps"];
		for(int i=0;i<arr.arrayObjects.Count;i++){
			Trap trap = LoadTrap(arr.arrayObjects[i]);
			dng.traps.Add(trap);
		}

		arr = dungeonObj.objectObjects["dungeonFeatures"];
		for(int i=0;i<arr.arrayObjects.Count;i++){
			DungeonFeature df = LoadDungeonFeature(arr.arrayObjects[i]);
			dng.dungeonFeatures.Add(df);
		}

		arr = dungeonObj.objectObjects["randomTables"];
		for(int i=0;i<arr.arrayObjects.Count;i++){
			RandomTable rt = LoadRandomTable(arr.arrayObjects[i]);
			dng.randomTables.Add(rt);
		}

		dng.floors.Clear();
		arr = dungeonObj.objectObjects["floors"];
		for(int i=0;i<arr.arrayObjects.Count;i++){
			DungeonFloor f = LoadFloor(arr.arrayObjects[i], dng);
			dng.floors.Add(f);
		}

		LoadImages(dng);

		return dng;
	}



	public static DungeonFloor LoadFloor(JSONObject floorObj, Dungeon dng){
		DungeonFloor output = new DungeonFloor(dng.floors.Count, dng);
		output.description = floorObj.objectObjects["description"].fieldValue;
		for(int i=0;i<floorObj.objectObjects["rooms"].arrayObjects.Count;i++){
			LoadRoom(floorObj.objectObjects["rooms"].arrayObjects[i], output);	
		}
		return output;
	}

	public static void LoadRoom(JSONObject roomObj, DungeonFloor df){
		DungeonRoom dr = new DungeonRoom(df);
		DungeonCell dc;
		int x, y;
		JSONObject cellObj;
		dr.name = roomObj.objectObjects["name"].fieldValue;
		dr.description = roomObj.objectObjects["description"].fieldValue;
		dr.descriptionPlayer = roomObj.objectObjects["descriptionPlayer"].fieldValue;
		dr.descriptionTreasure = roomObj.objectObjects["descriptionTreasure"].fieldValue;
		for(int i=0;i<roomObj.objectObjects["cells"].arrayObjects.Count;i++){
			cellObj = roomObj.objectObjects["cells"].arrayObjects[i];
			x = Int32.Parse(cellObj.objectObjects["x"].fieldValue);
			y = Int32.Parse(cellObj.objectObjects["y"].fieldValue);
			dr.cells.Add(new Coordinates(x,y));
			dc = df.cells[x,y];
			dc.room = dr;
			dc.monsterID = UInt32.Parse(cellObj.objectObjects["monsterID"].fieldValue);
			dc.dungeonFeatureID = UInt32.Parse(cellObj.objectObjects["dungeonFeatureID"].fieldValue);
			for(int j=0;j<4;j++){
				dc.edges[j].hasDoor = cellObj.objectObjects["edges"].arrayObjects[j].fieldValue == "true" ? true : false;
			}
		}
	}

	public static void LoadTileset(){

	}

	public static Monster LoadMonster(JSONObject monsterObj, Dungeon dng){
		Monster output = new Monster(dng.gameSystemIndex);
		output.systemID = UInt32.Parse(monsterObj.objectObjects["systemID"].fieldValue);
		output.name = monsterObj.objectObjects["name"].fieldValue;
		output.description = monsterObj.objectObjects["description"].fieldValue;
		output.hitdice = LoadDice(monsterObj.objectObjects["hitdice"]);
		output.monsterType = Int32.Parse(monsterObj.objectObjects["monsterType"].fieldValue);
		output.size = Int32.Parse(monsterObj.objectObjects["size"].fieldValue);
		output.initiativeModifier = Int32.Parse(monsterObj.objectObjects["initiativeModifier"].fieldValue);
		for(int i=0;i<monsterObj.objectObjects["movementSpeed"].arrayObjects.Count;i++){
			output.movementSpeed[i] = Int32.Parse(monsterObj.objectObjects["movementSpeed"].arrayObjects[i].fieldValue);	
		}
		output.armorClass = Int32.Parse(monsterObj.objectObjects["armorClass"].fieldValue);
		output.challengeRating = Int32.Parse(monsterObj.objectObjects["challengeRating"].fieldValue);
		// output.treasure = monsterObj.objectObjects[treasure];

		for(int i=0;i<monsterObj.objectObjects["attacks"].arrayObjects.Count;i++){
			output.attacks.Add(LoadMonsterAttack(monsterObj.objectObjects["attacks"].arrayObjects[i]));
		}

		for(int i=0;i<monsterObj.objectObjects["specialAttacks"].arrayObjects.Count;i++){
			output.specialAttacks.Add(LoadMonsterAbility(monsterObj.objectObjects["specialAttacks"].arrayObjects[i]));
		}

		for(int i=0;i<monsterObj.objectObjects["specialQualities"].arrayObjects.Count;i++){
			output.specialQualities.Add(LoadMonsterAbility(monsterObj.objectObjects["specialQualities"].arrayObjects[i]));
		}
		
		for(int i=0;i<monsterObj.objectObjects["feats"].arrayObjects.Count;i++){
			output.feats.Add(Int32.Parse(monsterObj.objectObjects["feats"].arrayObjects[i].fieldValue));
		}


		// output.skills = monsterObj.objectObjects[skills];


		for(int i=0;i<monsterObj.objectObjects["attributes"].arrayObjects.Count;i++){
			output.attributes[i] = Int32.Parse(monsterObj.objectObjects["attributes"].arrayObjects[i].fieldValue);	
		}
		for(int i=0;i<monsterObj.objectObjects["savingThrows"].arrayObjects.Count;i++){
			output.savingThrows[i] = Int32.Parse(monsterObj.objectObjects["savingThrows"].arrayObjects[i].fieldValue);	
		}

		
		//output.otherStats = monsterObj.objectObjects[otherStats];

		return output;
	}

	public static MonsterAttack LoadMonsterAttack(JSONObject obj){
		MonsterAttack output = new MonsterAttack();
		output.name = obj.objectObjects["name"].fieldValue;
		output.toHitModifier = Int32.Parse(obj.objectObjects["toHitModifier"].fieldValue);
		output.damage = LoadDice(obj.objectObjects["damage"]);
		return output;
	}

	public static MonsterAbility LoadMonsterAbility(JSONObject obj){
		MonsterAbility output = new MonsterAbility();
		output.name = obj.objectObjects["name"].fieldValue;
		output.description = obj.objectObjects["description"].fieldValue;
		return output;
	}

	public static Artifact LoadArtifact(JSONObject obj){
		Artifact output = new Artifact();
		output.systemID = UInt32.Parse(obj.objectObjects["systemID"].fieldValue);
		output.name = obj.objectObjects["name"].fieldValue;
		output.description = obj.objectObjects["description"].fieldValue;
		output.baseMarketPrice = Int32.Parse(obj.objectObjects["baseMarketPrice"].fieldValue);
		return output;
	}

	public static Treasure LoadTreasure(JSONObject obj){
		Treasure output = new Treasure();
		output.systemID = UInt32.Parse(obj.objectObjects["systemID"].fieldValue);
		output.name = obj.objectObjects["name"].fieldValue;
		output.description = obj.objectObjects["description"].fieldValue;

		return output;
	}

	public static Trap LoadTrap(JSONObject obj){
		Trap output = new Trap();
		output.systemID = UInt32.Parse(obj.objectObjects["systemID"].fieldValue);
		output.name = obj.objectObjects["name"].fieldValue;
		output.searchDC = Int32.Parse(obj.objectObjects["searchDC"].fieldValue);
		output.disableDC = Int32.Parse(obj.objectObjects["disableDC"].fieldValue);
		output.description = obj.objectObjects["description"].fieldValue;
		return output;
	}

	public static DungeonFeature LoadDungeonFeature(JSONObject obj){
		DungeonFeature output = new DungeonFeature();
		output.systemID = UInt32.Parse(obj.objectObjects["systemID"].fieldValue);
		output.name = obj.objectObjects["name"].fieldValue;
		output.description = obj.objectObjects["description"].fieldValue;
		return output;
	}

	public static RandomTable LoadRandomTable(JSONObject obj){
		RandomTable output = new RandomTable();
		output.systemID = UInt32.Parse(obj.objectObjects["systemID"].fieldValue);
		output.name = obj.objectObjects["name"].fieldValue;
		output.type = Int32.Parse(obj.objectObjects["type"].fieldValue);
		output.range = Int32.Parse(obj.objectObjects["range"].fieldValue);
		List<JSONObject> entryArray = obj.objectObjects["entries"].arrayObjects;
		for(int i=0;i<entryArray.Count;i++){
			output.entries.Add(LoadRandomTableEntry(entryArray[i]));
		}
		return output;
	}

	public static RandomTableEntry LoadRandomTableEntry(JSONObject obj){
		RandomTableEntry output = new RandomTableEntry();
		output.width = Int32.Parse(obj.objectObjects["width"].fieldValue);
		output.entry = obj.objectObjects["entry"].fieldValue;
		return output;
	}

	public static void LoadImages(Dungeon dng){
		string basePath = Path.Combine(FileUtilities.userPath, "Dungeons", dng.fileName);
		for(int i=0;i<dng.monsters.Count;i++){
			dng.monsters[i].illustration = LoadSprite(Path.Combine(basePath, "Illustrations", dng.monsters[i].illustrationFileName) + ".png");
			dng.monsters[i].icon = LoadSprite(Path.Combine(basePath, "Icons", dng.monsters[i].iconFileName) + ".png");
		}

		for(int i=0;i<dng.artifacts.Count;i++){
			dng.artifacts[i].illustration = LoadSprite(Path.Combine(basePath, "Illustrations", dng.artifacts[i].illustrationFileName) + ".png");
		}

		for(int i=0;i<dng.traps.Count;i++){
			dng.traps[i].illustration = LoadSprite(Path.Combine(basePath, "Illustrations", dng.traps[i].illustrationFileName) + ".png");
		}

		for(int i=0;i<dng.dungeonFeatures.Count;i++){
			dng.dungeonFeatures[i].illustration = LoadSprite(Path.Combine(basePath, "Illustrations", dng.dungeonFeatures[i].illustrationFileName) + ".png");
			dng.dungeonFeatures[i].icon = LoadSprite(Path.Combine(basePath, "Icons", dng.dungeonFeatures[i].iconFileName) + ".png");
		}

		//TODO get images of others stuff too
	}

	public static Dice LoadDice(JSONObject diceObj){
		int dc = Int32.Parse(diceObj.objectObjects["diceCount"].fieldValue);
		int fc = Int32.Parse(diceObj.objectObjects["faceCount"].fieldValue);
		int m = Int32.Parse(diceObj.objectObjects["modifier"].fieldValue);
		return new Dice(dc,fc,m);
	}

	public static Sprite LoadSprite(string path){
		Texture2D tex = FileUtilities.LoadImageFromFile(path);
		if(tex == null){
			return null;
		}
		Rect rect = new Rect(0,0,tex.width,tex.height);
		Vector2 pivot = new Vector2(tex.width/2f,tex.height/2f);
		Sprite spr = Sprite.Create(tex, rect, pivot, 64f);
		return spr;
	}


}