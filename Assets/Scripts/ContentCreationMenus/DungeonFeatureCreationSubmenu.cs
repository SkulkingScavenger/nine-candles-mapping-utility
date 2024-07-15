using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DungeonFeatureCreationSubmenu : Submenu{

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

	public DungeonFeature dungeonFeature;
	public DungeonFeature tempDungeonFeature;

	InputField nameInput;
	InputField descriptionInput;

	Image illustrationPreview;
	Image iconPreview;
	MenuButton uploadImageButton;
	MenuButton uploadIconButton;

	MenuButton saveButton;
	MenuButton newButton;
	MenuButton exitButton;
	


	public override void InitialSetup(){
		Transform tsf;
		tsf = transform.Find("MainPanel").Find("InputSection");
		nameInput = tsf.Find("Input - Name").GetComponent<InputField>();
		descriptionInput = tsf.Find("Input - Description").GetComponent<InputField>();

		tsf = transform.Find("MainPanel").Find("ImageSection");
		illustrationPreview = tsf.Find("IllustrationPreview").GetComponent<Image>();
		iconPreview = tsf.Find("IconPreview").GetComponent<Image>();
		uploadImageButton = tsf.Find("UploadImageButton").GetComponent<MenuButton>();
		uploadImageButton.onClick.AddListener( delegate { UploadImage(true); } );
		uploadIconButton = tsf.Find("UploadIconButton").GetComponent<MenuButton>();
		uploadIconButton.onClick.AddListener( delegate { UploadImage(false); } );

		tsf = transform.Find("MainPanel").Find("ButtonSection");
		saveButton = tsf.Find("SaveButton").GetComponent<MenuButton>();
		newButton = tsf.Find("NewButton").GetComponent<MenuButton>();
		exitButton = tsf.Find("ExitButton").GetComponent<MenuButton>();
		saveButton.onClick.AddListener( delegate { SaveData(); } );
		newButton.onClick.AddListener( delegate { CreateNew(); } );
		exitButton.onClick.AddListener( delegate { Exit(); } );
	}

	public override void OnMenuOpen(){
		if(dungeonFeature == null){
			isEditingExisting = false;
			dungeonFeature = new DungeonFeature();
			tempDungeonFeature = new DungeonFeature();
		}else{
			isEditingExisting = true;
			tempDungeonFeature = new DungeonFeature();
			tempDungeonFeature.CopyValuesFrom(dungeonFeature);
		}
		ReloadFields();
		hasUnsavedChanges = false;
	}

	public override void UpdateActive(){
		if(nameInput.text != tempDungeonFeature.name){
			tempDungeonFeature.name = nameInput.text;
			hasUnsavedChanges = true;
		}
		if(descriptionInput.text != tempDungeonFeature.description){
			tempDungeonFeature.description = descriptionInput.text;
			hasUnsavedChanges = true;
		}
		if(illustrationPreview.sprite != tempDungeonFeature.illustration){
			tempDungeonFeature.illustration = illustrationPreview.sprite;
			hasUnsavedChanges = true;
		}
		if(iconPreview.sprite != tempDungeonFeature.icon){
			tempDungeonFeature.icon = iconPreview.sprite;
			hasUnsavedChanges = true;
		}

		saveButton.isDisabled = !hasUnsavedChanges;
	}

	void ReloadFields(){
		nameInput.text = tempDungeonFeature.name;
		descriptionInput.text = tempDungeonFeature.description;

		illustrationPreview.sprite = tempDungeonFeature.illustration;
	}

	void UploadImage(bool isIllustration){
		FilePickerDialog fpd;
		fpd = MenuControl.canvas.transform.Find("FilePickerDialog").GetComponent<FilePickerDialog>();
		fpd.root = root;
		if(isIllustration){
			fpd.callback = UploadIllustrationCallback;	
		}else{
			fpd.callback = UploadIconCallback;
		}
		fpd.Open();
		fpd.Activate();
	}

	void UploadIllustrationCallback(bool isCancelled, string path){
		if(!isCancelled){
			Texture2D tex = FileUtilities.LoadImageFromFile(path);
			Rect rect = new Rect(0,0,tex.width,tex.height);
			Vector2 pivot = new Vector2(tex.width/2f,tex.height/2f);
			Sprite spr = Sprite.Create(tex, rect, pivot, 64f);
			illustrationPreview.sprite = spr;
			tempDungeonFeature.illustration = spr;
		}
	}

	void UploadIconCallback(bool isCancelled, string path){
		if(!isCancelled){
			Texture2D tex = FileUtilities.LoadImageFromFile(path);
			Rect rect = new Rect(0,0,tex.width,tex.height);
			Vector2 pivot = new Vector2(tex.width/2f,tex.height/2f);
			Sprite spr = Sprite.Create(tex, rect, pivot, 64f);
			iconPreview.sprite = spr;
			tempDungeonFeature.icon = spr;
		}
	}

	public void SaveData(){
		hasUnsavedChanges = false;
		dungeonFeature.CopyValuesFrom(tempDungeonFeature);
		if(!isEditingExisting){
			dungeon.AssignSystemID(dungeonFeature);
			dungeon.dungeonFeatures.Add(dungeonFeature);
			isEditingExisting = true;
		}
	}

	public void CreateNew(){
		if(hasUnsavedChanges){
			OpenConfirmationDialog("This form has unsaved edits. Continue and discard changes?", (bool isConfirmed) => {
				if(isConfirmed){
					isEditingExisting = false;
					dungeonFeature = new DungeonFeature();
					tempDungeonFeature = new DungeonFeature();
					ReloadFields();
					hasUnsavedChanges = false;
				}
			});
		}else{
			isEditingExisting = false;
			dungeonFeature = new DungeonFeature();
			tempDungeonFeature = new DungeonFeature();
			ReloadFields();
			hasUnsavedChanges = false;
		}
	}

	public void Exit(){
		if(hasUnsavedChanges){
			OpenConfirmationDialog("This form has unsaved edits. Continue and discard changes?", (bool isConfirmed) => {
				if(isConfirmed){
					dungeonFeature = null;
					Close();
					root.Close();
				}
			});
		}else{
			dungeonFeature = null;
			Close();
			root.Close();
		}
	}
}
