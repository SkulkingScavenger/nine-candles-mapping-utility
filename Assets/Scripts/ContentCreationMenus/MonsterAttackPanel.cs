using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MonsterAttackPanel : MenuSection{

	public MonsterCreationSubmenu monsterMenu;
	public MonsterAttack attack;
	public MonsterAttack tempAttack;
	public bool isEditingExisting;

	public delegate void OnCloseFunc(bool isCancelled, bool editingExisting, MonsterAttack ma);
	public OnCloseFunc onClose;

	InputField nameInput;
	InputField toHitInput;
	InputField damageCountInput;
	Dropdown damageFacesDropdown;
	InputField damageModifierInput;

	MenuButton saveButton;
	MenuButton cancelButton;

	bool hasUnsavedChanges = false;

	public void InitialSetup(){
		Transform tsf;
		tsf = transform.Find("Panel");
		nameInput = tsf.Find("Input - Name").GetComponent<InputField>();
		toHitInput = tsf.Find("Input - ToHit").GetComponent<InputField>();
		damageCountInput = tsf.Find("Input - DamageCount").GetComponent<InputField>();
		damageFacesDropdown = tsf.Find("Dropdown - DamageSides").GetComponent<Dropdown>();
		damageModifierInput = tsf.Find("Input - DamageModifier").GetComponent<InputField>();

		List<string> opdatList = new List<string>();
		damageFacesDropdown.ClearOptions();
		for(int i=0;i<Dice.types.Length;i++){
			opdatList.Add("D" + Dice.types[i]);
		}
		damageFacesDropdown.AddOptions(opdatList);
		opdatList.Clear();

		tsf = transform.Find("Panel").Find("ButtonSection");
		saveButton = tsf.Find("SaveButton").GetComponent<MenuButton>();
		cancelButton = tsf.Find("CancelButton").GetComponent<MenuButton>();
		
		saveButton.onClick.AddListener( delegate {Save();} );
		cancelButton.onClick.AddListener( delegate {Cancel();} );
		

		tempAttack = new MonsterAttack();
	}

	public void Open(MonsterAttack ma){
		if(ma == null){
			isEditingExisting = false;
			attack = new MonsterAttack();
		}else{
			isEditingExisting = true;
			attack = ma;
		}
		tempAttack.CopyValuesFrom(attack);
		SetValues();
		hasUnsavedChanges = false;
		gameObject.SetActive(true);
	}

	void SetValues(){
		nameInput.text = tempAttack.name;
		toHitInput.text = tempAttack.toHitModifier.ToString();
		damageCountInput.text = tempAttack.damage.diceCount.ToString();
		for(int i=0;i<Dice.types.Length;i++){
			if(Dice.types[i] == tempAttack.damage.faceCount){
				damageFacesDropdown.value = i;
				break;
			}
		}
		damageModifierInput.text = tempAttack.damage.modifier.ToString();
	}

	void Update(){
		if(nameInput.text != tempAttack.name){
			tempAttack.name = nameInput.text;
			hasUnsavedChanges = true;
		}
		if(Int32.Parse(toHitInput.text) != tempAttack.toHitModifier){
			tempAttack.toHitModifier = Int32.Parse(toHitInput.text) ;
			hasUnsavedChanges = true;
		}
		if(Int32.Parse(damageCountInput.text) != tempAttack.damage.diceCount){
			tempAttack.damage = new Dice(Int32.Parse(damageCountInput.text), tempAttack.damage.faceCount, tempAttack.damage.modifier);
			hasUnsavedChanges = true;	
		}
		if(Int32.Parse(damageModifierInput.text) != tempAttack.damage.modifier){
			tempAttack.damage = new Dice(tempAttack.damage.diceCount, tempAttack.damage.faceCount, Int32.Parse(damageModifierInput.text));
			hasUnsavedChanges = true;	
		}
		if(Dice.types[damageFacesDropdown.value] != tempAttack.damage.faceCount){
			tempAttack.damage = new Dice(tempAttack.damage.diceCount, Dice.types[damageFacesDropdown.value], tempAttack.damage.modifier);
		}

		saveButton.isDisabled = !hasUnsavedChanges;
	}

	void Save(){
		attack.CopyValuesFrom(tempAttack);
		onClose(false, isEditingExisting, attack);
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
