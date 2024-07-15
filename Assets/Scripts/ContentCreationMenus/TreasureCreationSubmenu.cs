using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TreasureCreationSubmenu : Submenu{

	public ContentCreationMenu contentCreationMenu;
	public Dungeon dungeon {
		get {return contentCreationMenu.dungeon;}
		set {}
	}
	public GameSystem gameSystem {
		get {return dungeon.gameSystem;}
		set {}
	}
	public bool isEditingExisting {
		get { return contentCreationMenu.isEditingExisting; }
		set { contentCreationMenu.isEditingExisting = value; }
	}
	public bool hasUnsavedChanges {
		get { return contentCreationMenu.hasUnsavedChanges; }
		set { contentCreationMenu.hasUnsavedChanges = value; }
	}

	public Treasure treasure;
	public Treasure tempTreasure;

	InputField nameInput;
	InputField descriptionInput;

	MenuButton saveButton;
	MenuButton newButton;
	MenuButton exitButton;
	


	public override void InitialSetup(){
		Transform tsf;
		tsf = transform.Find("MainPanel").Find("InputSection");
		nameInput = tsf.Find("Input - Name").GetComponent<InputField>();
		descriptionInput = tsf.Find("Input - Description").GetComponent<InputField>();

		tsf = transform.Find("MainPanel").Find("ButtonSection");
		saveButton = tsf.Find("SaveButton").GetComponent<MenuButton>();
		newButton = tsf.Find("NewButton").GetComponent<MenuButton>();
		exitButton = tsf.Find("ExitButton").GetComponent<MenuButton>();
		saveButton.onClick.AddListener( delegate { SaveData(); } );
		newButton.onClick.AddListener( delegate { CreateNew(); } );
		exitButton.onClick.AddListener( delegate { Exit(); } );
	}

	public override void OnMenuOpen(){
		if(treasure == null){
			isEditingExisting = false;
			treasure = new Treasure();
			tempTreasure = new Treasure();
		}else{
			isEditingExisting = true;
			tempTreasure = new Treasure();
			tempTreasure.CopyValuesFrom(treasure);
		}
		ReloadFields();
		hasUnsavedChanges = false;
	}

	public override void UpdateActive(){
		if(nameInput.text != tempTreasure.name){
			tempTreasure.name = nameInput.text;
			hasUnsavedChanges = true;
		}

		if(descriptionInput.text != tempTreasure.description){
			tempTreasure.description = descriptionInput.text;
			hasUnsavedChanges = true;
		}

		saveButton.isDisabled = !hasUnsavedChanges;
	}

	void ReloadFields(){
		nameInput.text = tempTreasure.name;
		descriptionInput.text = tempTreasure.description;
	}

	public void SaveData(){
		hasUnsavedChanges = false;
		treasure.CopyValuesFrom(tempTreasure);
		if(!isEditingExisting){
			dungeon.AssignSystemID(treasure);
			dungeon.treasures.Add(treasure);
			isEditingExisting = true;
		}
	}

	public void CreateNew(){
		if(hasUnsavedChanges){
			OpenConfirmationDialog("This form has unsaved edits. Continue and discard changes?", (bool isConfirmed) => {
				if(isConfirmed){
					isEditingExisting = false;
					treasure = new Treasure();
					tempTreasure = new Treasure();
					ReloadFields();
					hasUnsavedChanges = false;
				}
			});
		}else{
			isEditingExisting = false;
			treasure = new Treasure();
			tempTreasure = new Treasure();
			ReloadFields();
			hasUnsavedChanges = false;
		}
	}

	public void Exit(){
		if(hasUnsavedChanges){
			OpenConfirmationDialog("This form has unsaved edits. Continue and discard changes?", (bool isConfirmed) => {
				if(isConfirmed){
					treasure = null;
					Close();
					root.Close();
				}
			});
		}else{
			treasure = null;
			Close();
			root.Close();
		}
	}
}
