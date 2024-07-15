using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilePickerDialog : Menu {

	public string currentDirectory;
	public string selectedFile = "";

	MenuSlotHolder slotHolder;
	InputField pathDisplay;
	Text selectedFileDisplay;
	MenuButton selectButton;
	MenuButton cancelButton;
	MenuButton upOneDirectoryButton;

	public delegate void CallbackFunc(bool isCancelled, string path);
	public CallbackFunc callback;

	public void Awake(){

	}

	public override void InitialSetup(){
		currentDirectory = FileUtilities.userPath;

		Transform tsf = transform;
		pathDisplay = tsf.Find("PathDisplay").GetComponent<InputField>();
		selectedFileDisplay = tsf.Find("SelectedFileDisplay").GetComponent<Text>();
		slotHolder = tsf.Find("MenuSlotHolder").GetComponent<MenuSlotHolder>();

		slotHolder.menu = this;
		slotHolder.canAddNew = false;
		slotHolder.canEdit = false;
		slotHolder.canDelete = false;
		slotHolder.InitialSetup();

		tsf = transform.Find("ButtonSection");
		selectButton = tsf.Find("SelectButton").GetComponent<MenuButton>();
		cancelButton = tsf.Find("CancelButton").GetComponent<MenuButton>();
		upOneDirectoryButton = tsf.Find("UpOneDirectoryButton").GetComponent<MenuButton>();

		selectButton.onClick.AddListener(delegate {SelectFile();} );
		cancelButton.onClick.AddListener(delegate {Cancel();} );
		upOneDirectoryButton.onClick.AddListener(delegate {UpOneLevel();} );
	}

	public override void OnMenuOpen(){
		pathDisplay.text = currentDirectory;
	}

	public override void UpdateActive(){
		if(currentDirectory != pathDisplay.text){
			if(FileUtilities.IsValidPath(pathDisplay.text)){
				currentDirectory = pathDisplay.text;
				RefreshSlots();
			}
		}

		for(int i=0;i<slotHolder.slots.Count;i++){
			if(slotHolder.slots[i].isHovered && !slotHolder.slots[i].isEmpty){
				slotHolder.slots[i].transform.Find("Text").GetComponent<Text>().color = new Color(1f,0f,0f,1f);
				if(InputControl.mouseLeftPressed){
					string fullPath = Path.Combine(currentDirectory,slotHolder.slots[i].transform.Find("Text").GetComponent<Text>().text);
					if(FileUtilities.IsDirectory(fullPath)){
						SetDirectory(fullPath);	
					}else{
						selectedFile = fullPath;
						selectedFileDisplay.text = selectedFile;
					}
				}
			}else{
				slotHolder.slots[i].transform.Find("Text").GetComponent<Text>().color = new Color(1f,1f,1f,1f);
			}
		}

		selectButton.isDisabled = selectedFile == "";
	}

	void RefreshSlots(){
		List<string> files = FileUtilities.GetFilesInDirectory(currentDirectory,"*.png");
		List<string> dirs = FileUtilities.GetSubdirectoriesAtPath(currentDirectory);
		dirs.AddRange(files);
		slotHolder.SetList(dirs);
	}

	void UpOneLevel(){
		string newDir = FileUtilities.GetParent(currentDirectory);
		if(newDir != null){
			SetDirectory(newDir);
		}
	}

	void SetDirectory(string newDir){
		currentDirectory = newDir;
		pathDisplay.text = currentDirectory;
		RefreshSlots();
	}


	void SelectFile(){
		callback(false, selectedFile);
		root.isActiveMenu = true;
		Close();
	}

	void Cancel(){
		callback(true, "");
		root.isActiveMenu = true;
		Close();
	}

}