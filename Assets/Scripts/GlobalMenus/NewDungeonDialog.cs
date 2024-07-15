using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewDungeonDialog : Menu {

	public int selectedDimensions = 0;

	InputField nameInput;
	Dropdown gameSystemDropdown;

	MenuButton confirmButton;
	MenuButton cancelButton;

	MenuComponent[] dimensionGrids = new MenuComponent[6];
	Coordinates[] dimensionList = new Coordinates[6];

	public void Awake(){

	}

	public override void InitialSetup(){
		selectedDimensions = 0;

			/*
			That one map is 40x28; graph paper is 41x31
			mini - 30x20 (half page) 3:2
			small - 40x30 (1 page) 4:3
			medium - 60x40 (2 pages) 3:2
			long - 90x40 (3 pages) 9:4
			large - 80x60 (4 pages) 4:3
			huge - 90x80 (6 pages) 9:8
			epic - 120x90 (9 pages) 4:3

			*/
		dimensionList[0] = new Coordinates(30,20);
		dimensionList[1] = new Coordinates(40,30);
		dimensionList[2] = new Coordinates(60,40);
		dimensionList[3] = new Coordinates(90,40);
		dimensionList[4] = new Coordinates(80,60);
		dimensionList[5] = new Coordinates(90,80);

		Transform tsf = transform.Find("Panel");
		nameInput = tsf.Find("Input - Name").GetComponent<InputField>();
		gameSystemDropdown = tsf.Find("Dropdown - GameSystem").GetComponent<Dropdown>();

		List<string> opdat = new List<string>();
		gameSystemDropdown.ClearOptions();
		for(int i=0;i<MasterControl.gameSystems.Count;i++){
			opdat.Add(MasterControl.gameSystems[i].name);
		}
		gameSystemDropdown.AddOptions(opdat);

		tsf = transform.Find("Panel").Find("DimensionsSection");
		for(int i=0;i<dimensionGrids.Length;i++){
			dimensionGrids[i] = tsf.Find("Grid" + i).GetComponent<MenuComponent>();
			dimensionGrids[i].menu = this;
			dimensionGrids[i].section = tsf.GetComponent<MenuSection>();
		}


		tsf = transform.Find("Panel").Find("ButtonSection");
		confirmButton = tsf.Find("ConfirmButton").GetComponent<MenuButton>();
		cancelButton = tsf.Find("CancelButton").GetComponent<MenuButton>();

		confirmButton.onClick.AddListener(delegate {CreateDungeon();} );
		cancelButton.onClick.AddListener(delegate {Cancel();} );
	}

	public override void OnMenuOpen(){
		nameInput.text = "";
		selectedDimensions = 0;
	}

	public override void UpdateActive(){

		for(int i=0;i<dimensionGrids.Length;i++){
			if(dimensionGrids[i].isHovered && selectedDimensions != i){
				dimensionGrids[i].transform.Find("Image").GetComponent<Image>().color = new Color(0.68f,0.98f,1f,1f);
				if(InputControl.mouseLeftPressed){
					selectedDimensions = i;
				}
			}else{
				if(selectedDimensions == i){
					dimensionGrids[i].transform.Find("Image").GetComponent<Image>().color = new Color(0.3f,0.2f,0.6f,1f);
				}else{
					dimensionGrids[i].transform.Find("Image").GetComponent<Image>().color = new Color(1f,1f,1f,1f);	
				}
			}
		}

		confirmButton.isDisabled = nameInput.text == "";
	}


	void CreateDungeon(){
		string name = nameInput.text;
		if(name == null){return;}
		int gameSystemIndex = gameSystemDropdown.value;
		Coordinates dimensions = dimensionList[selectedDimensions];

		Dungeon dng = new Dungeon(dimensions.x, dimensions.y, name, gameSystemIndex);
		MasterControl.SetDungeon(dng);
		MenuControl.canvas.transform.Find("DungeonMap").GetComponent<DungeonMap>().Open();
		MenuControl.canvas.transform.Find("DungeonMap").GetComponent<DungeonMap>().Activate();
		Close();
	}

	void Cancel(){
		root.Open();
		root.Activate();
		Close();
	}

}