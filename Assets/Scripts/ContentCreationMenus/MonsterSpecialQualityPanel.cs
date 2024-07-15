using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MonsterSpecialQualityPanel : MenuSection{

	public MonsterCreationSubmenu monsterMenu;
	public MonsterAbility monsterAbility;
	public MonsterAbility tempAbility;
	public bool isEditingExisting;

	public delegate void OnCloseFunc(bool isCancelled, bool editingExisting, MonsterAbility ma);
	public OnCloseFunc onClose;

	InputField nameInput;
	InputField descriptionInput;

	MenuButton saveButton;
	MenuButton cancelButton;

	bool hasUnsavedChanges = false;

	public void InitialSetup(){
		Transform tsf;
		tsf = transform.Find("Panel");
		nameInput = tsf.Find("Input - Name").GetComponent<InputField>();
		descriptionInput = tsf.Find("Input - Description").GetComponent<InputField>();

		tsf = transform.Find("Panel").Find("ButtonSection");
		saveButton = tsf.Find("SaveButton").GetComponent<MenuButton>();
		cancelButton = tsf.Find("CancelButton").GetComponent<MenuButton>();
		
		saveButton.onClick.AddListener( delegate {Save();} );
		cancelButton.onClick.AddListener( delegate {Cancel();} );

		tempAbility = new MonsterAbility();
	}

	public void Open(MonsterAbility ma){
		if(ma == null){
			isEditingExisting = false;
			monsterAbility = new MonsterAbility();
		}else{
			isEditingExisting = true;
			monsterAbility = ma;
		}
		tempAbility.CopyValuesFrom(monsterAbility);
		SetValues();
		hasUnsavedChanges = false;
		gameObject.SetActive(true);
	}

	void SetValues(){
		nameInput.text = tempAbility.name;
		descriptionInput.text = tempAbility.description;
	}

	void Update(){
		if(nameInput.text != tempAbility.name){
			tempAbility.name = nameInput.text;
			hasUnsavedChanges = true;
		}
		if(descriptionInput.text != tempAbility.description){
			tempAbility.description = descriptionInput.text;
			hasUnsavedChanges = true;
		}

		saveButton.isDisabled = !hasUnsavedChanges;
	}

	void Save(){
		monsterAbility.CopyValuesFrom(tempAbility);
		onClose(false, isEditingExisting, monsterAbility);
		Close();
	}

	void Cancel(){
		onClose(true, false, null);
		Close();
	}

	void Close(){
		gameObject.SetActive(false);
	}
}
