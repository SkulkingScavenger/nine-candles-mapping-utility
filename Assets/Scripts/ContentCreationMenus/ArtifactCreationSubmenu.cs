using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ArtifactCreationSubmenu : Submenu{

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

	public Artifact artifact;
	public Artifact tempArtifact;

	InputField nameInput;
	InputField priceInput;
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
		priceInput = tsf.Find("Input - Price").GetComponent<InputField>();
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
		if(artifact == null){
			isEditingExisting = false;
			artifact = new Artifact();
			tempArtifact = new Artifact();
		}else{
			isEditingExisting = true;
			tempArtifact = new Artifact();
			tempArtifact.CopyValuesFrom(artifact);
		}
		ReloadFields();
		hasUnsavedChanges = false;
	}

	public override void UpdateActive(){
		if(nameInput.text != tempArtifact.name){
			tempArtifact.name = nameInput.text;
			hasUnsavedChanges = true;
		}
		if(priceInput.text != tempArtifact.baseMarketPrice.ToString() && priceInput.text != "" && priceInput.text != "-"){
			tempArtifact.baseMarketPrice = Int32.Parse(priceInput.text);
			hasUnsavedChanges = true;
		}
		if(descriptionInput.text != tempArtifact.description){
			tempArtifact.description = descriptionInput.text;
			hasUnsavedChanges = true;
		}
		if(illustrationPreview.sprite != tempArtifact.illustration){
			tempArtifact.illustration = illustrationPreview.sprite;
			hasUnsavedChanges = true;
		}

		saveButton.isDisabled = !hasUnsavedChanges;
	}

	void ReloadFields(){
		nameInput.text = tempArtifact.name;
		priceInput.text = tempArtifact.baseMarketPrice.ToString();
		descriptionInput.text = tempArtifact.description;

		illustrationPreview.sprite = tempArtifact.illustration;
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
			tempArtifact.illustration = spr;
		}
	}

	public void SaveData(){
		hasUnsavedChanges = false;
		artifact.CopyValuesFrom(tempArtifact);
		if(!isEditingExisting){
			dungeon.AssignSystemID(artifact);
			dungeon.artifacts.Add(artifact);
			isEditingExisting = true;
		}
	}

	public void CreateNew(){
		if(hasUnsavedChanges){
			OpenConfirmationDialog("This form has unsaved edits. Continue and discard changes?", (bool isConfirmed) => {
				if(isConfirmed){
					isEditingExisting = false;
					artifact = new Artifact();
					tempArtifact = new Artifact();
					ReloadFields();
					hasUnsavedChanges = false;
				}
			});
		}else{
			isEditingExisting = false;
			artifact = new Artifact();
			tempArtifact = new Artifact();
			ReloadFields();
			hasUnsavedChanges = false;
		}
	}

	public void Exit(){
		if(hasUnsavedChanges){
			OpenConfirmationDialog("This form has unsaved edits. Continue and discard changes?", (bool isConfirmed) => {
				if(isConfirmed){
					artifact = null;
					Close();
					root.Close();
				}
			});
		}else{
			artifact = null;
			Close();
			root.Close();
		}
	}
}
