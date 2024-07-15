using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TrapCreationSubmenu : Submenu{

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

	public Trap trap;
	public Trap tempTrap;

	InputField nameInput;
	InputField searchInput;
	InputField disableInput;
	InputField descriptionInput;

	Image illustrationPreview;
	MenuButton uploadImageButton;

	MenuButton saveButton;
	MenuButton newButton;
	MenuButton exitButton;
	


	public override void InitialSetup(){
		Transform tsf;
		tsf = transform.Find("MainPanel").Find("InputSection");
		nameInput = tsf.Find("Input - Name").GetComponent<InputField>();
		searchInput = tsf.Find("Input - SearchDC").GetComponent<InputField>();
		disableInput = tsf.Find("Input - DisableDC").GetComponent<InputField>();
		descriptionInput = tsf.Find("Input - Description").GetComponent<InputField>();

		tsf = transform.Find("MainPanel").Find("ImageSection");
		illustrationPreview = tsf.Find("IllustrationPreview").GetComponent<Image>();
		uploadImageButton = tsf.Find("UploadImageButton").GetComponent<MenuButton>();
		uploadImageButton.onClick.AddListener( delegate { UploadImage(); } );

		tsf = transform.Find("MainPanel").Find("ButtonSection");
		saveButton = tsf.Find("SaveButton").GetComponent<MenuButton>();
		newButton = tsf.Find("NewButton").GetComponent<MenuButton>();
		exitButton = tsf.Find("ExitButton").GetComponent<MenuButton>();
		saveButton.onClick.AddListener( delegate { SaveData(); } );
		newButton.onClick.AddListener( delegate { CreateNew(); } );
		exitButton.onClick.AddListener( delegate { Exit(); } );
	}

	public override void OnMenuOpen(){
		if(trap == null){
			isEditingExisting = false;
			trap = new Trap();
			tempTrap = new Trap();
		}else{
			isEditingExisting = true;
			tempTrap = new Trap();
			tempTrap.CopyValuesFrom(trap);
		}
		ReloadFields();
		hasUnsavedChanges = false;
	}

	public override void UpdateActive(){
		if(nameInput.text != tempTrap.name){
			tempTrap.name = nameInput.text;
			hasUnsavedChanges = true;
		}
		if(descriptionInput.text != tempTrap.description){
			tempTrap.description = descriptionInput.text;
			hasUnsavedChanges = true;
		}
		if(Int32.Parse(searchInput.text) != tempTrap.searchDC && searchInput.text != "" && searchInput.text != "-"){
			tempTrap.searchDC = Int32.Parse(searchInput.text);
			hasUnsavedChanges = true;
		}
		if(Int32.Parse(disableInput.text) != tempTrap.disableDC && disableInput.text != "" && disableInput.text != "-"){
			tempTrap.disableDC = Int32.Parse(disableInput.text);
			hasUnsavedChanges = true;
		}
		if(illustrationPreview.sprite != tempTrap.illustration){
			tempTrap.illustration = illustrationPreview.sprite;
			hasUnsavedChanges = true;
		}

		saveButton.isDisabled = !hasUnsavedChanges;
	}

	void ReloadFields(){
		nameInput.text = tempTrap.name;
		descriptionInput.text = tempTrap.description;

		illustrationPreview.sprite = tempTrap.illustration;
	}

	void UploadImage(){
		FilePickerDialog fpd;
		fpd = MenuControl.canvas.transform.Find("FilePickerDialog").GetComponent<FilePickerDialog>();
		fpd.root = root;
		fpd.callback = UploadImageCallback;	
		fpd.Open();
		fpd.Activate();
	}

	void UploadImageCallback(bool isCancelled, string path){
		if(!isCancelled){
			Texture2D tex = FileUtilities.LoadImageFromFile(path);
			Rect rect = new Rect(0,0,tex.width,tex.height);
			Vector2 pivot = new Vector2(tex.width/2f,tex.height/2f);
			Sprite spr = Sprite.Create(tex, rect, pivot, 64f);
			illustrationPreview.sprite = spr;
			tempTrap.illustration = spr;
		}
	}

	public void SaveData(){
		hasUnsavedChanges = false;
		trap.CopyValuesFrom(tempTrap);
		if(!isEditingExisting){
			dungeon.AssignSystemID(trap);
			dungeon.traps.Add(trap);
			isEditingExisting = true;
		}
	}

	public void CreateNew(){
		if(hasUnsavedChanges){
			OpenConfirmationDialog("This form has unsaved edits. Continue and discard changes?", (bool isConfirmed) => {
				if(isConfirmed){
					isEditingExisting = false;
					trap = new Trap();
					tempTrap = new Trap();
					ReloadFields();
					hasUnsavedChanges = false;
				}
			});
		}else{
			isEditingExisting = false;
			trap = new Trap();
			tempTrap = new Trap();
			ReloadFields();
			hasUnsavedChanges = false;
		}
	}

	public void Exit(){
		if(hasUnsavedChanges){
			OpenConfirmationDialog("This form has unsaved edits. Continue and discard changes?", (bool isConfirmed) => {
				if(isConfirmed){
					trap = null;
					Close();
					root.Close();
				}
			});
		}else{
			trap = null;
			Close();
			root.Close();
		}
	}
}
