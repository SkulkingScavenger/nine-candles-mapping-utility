using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenDungeonDialog : Menu {

	public string currentDirectory;
	public string selectedFile = "";
	public string fileName = "";
	MenuSlotHolder slotHolder;
	InputField fileNameInput;
	Text title;
	MenuButton confirmButton;
	MenuButton cancelButton;
	public bool saveMode = false;
	int previousSelectionIndex = -1;

	public delegate void CallbackFunc(bool isCancelled, string path);
	public CallbackFunc callback;

	public void Awake(){

	}

	public override void InitialSetup(){
		currentDirectory = Path.Combine(FileUtilities.userPath, "Dungeons");

		Transform tsf = transform;
		fileNameInput = tsf.Find("FileNameInput").GetComponent<InputField>();
		title = tsf.Find("Title").GetComponent<Text>();
		slotHolder = tsf.Find("MenuSlotHolder").GetComponent<MenuSlotHolder>();

		slotHolder.menu = this;
		slotHolder.canSelect = true;
		slotHolder.canAddNew = false;
		slotHolder.canEdit = false;
		slotHolder.canDelete = false;
		slotHolder.InitialSetup();

		tsf = transform.Find("ButtonSection");
		confirmButton = tsf.Find("ConfirmButton").GetComponent<MenuButton>();
		cancelButton = tsf.Find("CancelButton").GetComponent<MenuButton>();

		confirmButton.onClick.AddListener(delegate {Confirm();} );
		cancelButton.onClick.AddListener(delegate {Cancel();} );
	}

	public override void OnMenuOpen(){
		if(saveMode && MasterControl.dungeon != null && MasterControl.dungeon.fileName != ""){
			fileName = MasterControl.dungeon.fileName;
		}else{
			fileName = "";	
		}
		fileNameInput.text = fileName;
		previousSelectionIndex = -1;
		slotHolder.selectedIndex = -1;
		if(saveMode){
			confirmButton.transform.Find("Text").GetComponent<Text>().text = "Save";
			title.text = "SAVE DUNGEON";
		}else{
			confirmButton.transform.Find("Text").GetComponent<Text>().text = "Open";
			title.text = "OPEN DUNGEON";
		}
		RefreshSlots();
	}

	public override void UpdateActive(){
		if(fileName != fileNameInput.text){
			if(fileNameInput.text != ""){
				fileName = fileNameInput.text;
			}
		}

		if(slotHolder.selectedIndex != previousSelectionIndex){
			fileName = slotHolder.values[slotHolder.selectedIndex];
			previousSelectionIndex = slotHolder.selectedIndex;
			fileNameInput.text = fileName;
		}
		if(saveMode){
			confirmButton.isDisabled = fileName == "";
		}else{
			confirmButton.isDisabled = !FileExists(fileName);
		}
		
	}

	void RefreshSlots(){
		List<string> dirs = FileUtilities.GetSubdirectoriesAtPath(currentDirectory);
		slotHolder.SetList(dirs);
	}

	bool FileExists(string str){
		bool output = false;
		for(int i=0;i<slotHolder.values.Count;i++){
			if(slotHolder.values[i] == str){
				output = true;
				break;
			}
		}
		return output;
	}

	void Confirm(){
		if(saveMode){
			Save();
		}else{
			Load();
		}
	}

	void Save(){
		if(FileExists(fileName)){
			OpenConfirmationDialog("A dungeon named \"" + fileName + "\" already exists. Continue and overwrite the existing dungeon?", (bool isConfirmed) => {
				if(isConfirmed){
					MasterControl.dungeon.fileName = fileName;
					DungeonControl.SaveDungeon();
					root.Activate();
					Close();
				}
			});
		}else{
			MasterControl.dungeon.fileName = fileName;
			DungeonControl.SaveDungeon();
			root.Activate();
			Close();
		}
	}

	void Load(){
		if(File.Exists(Path.Combine(currentDirectory,fileName,fileName + ".dng"))){
			Dungeon dng = DungeonControl.LoadDungeon(fileName);
			MasterControl.SetDungeon(dng);
			MenuControl.canvas.transform.Find("DungeonMap").GetComponent<DungeonMap>().Open();
			MenuControl.canvas.transform.Find("DungeonMap").GetComponent<DungeonMap>().Activate();
			Close();
		}else{
			OpenNotificationDialog("The dungeon \"" + fileName + "\" could not be opened. File may be corrupt", ()=>{});
		}
	}

	void Cancel(){
		root.Open();
		root.Activate();
		Close();
	}

}