using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartupMenu : Menu {

	MenuButton defaultDungeonButton;
	MenuButton newDungeonButton;
	MenuButton loadDungeonButton;

	public void Awake(){

	}

	public override void InitialSetup(){
		Transform tsf;
		tsf = transform.Find("Panel").Find("ButtonSection");
		defaultDungeonButton = tsf.Find("DefaultButton").GetComponent<MenuButton>();
		newDungeonButton = tsf.Find("NewButton").GetComponent<MenuButton>();
		loadDungeonButton = tsf.Find("LoadButton").GetComponent<MenuButton>();

		defaultDungeonButton.onClick.AddListener(delegate {LoadDefaults();} );
		newDungeonButton.onClick.AddListener(delegate {OpenNewDungeonDialog();} );
		loadDungeonButton.onClick.AddListener(delegate {OpenLoadDungeonDialog();} );
	}

	public override void OnMenuOpen(){
		//TODO load recents
	}

	public override void UpdateActive(){

		//Check for recents to load
	}


	void LoadDefaults(){
		Dungeon dungeon = new Dungeon(30, 20, "The Dungeon", 0);
		MasterControl.dungeon = dungeon;
		MenuControl.canvas.transform.Find("DungeonMap").GetComponent<DungeonMap>();
		MenuControl.canvas.transform.Find("DungeonMap").GetComponent<DungeonMap>().Open();
		MenuControl.canvas.transform.Find("DungeonMap").GetComponent<DungeonMap>().Activate();
		Close();
	}

	void OpenNewDungeonDialog(){
		NewDungeonDialog ndd = MenuControl.canvas.transform.Find("NewDungeonDialog").GetComponent<NewDungeonDialog>();
		ndd.root = this;
		ndd.Open();
		ndd.Activate();
		Close();
	}

	void OpenLoadDungeonDialog(){
		OpenDungeonDialog odd = MenuControl.canvas.transform.Find("OpenDungeonDialog").GetComponent<OpenDungeonDialog>();
		odd.saveMode = false;
		odd.root = this;
		odd.Open();
		odd.Activate();
		Close();
	}

}