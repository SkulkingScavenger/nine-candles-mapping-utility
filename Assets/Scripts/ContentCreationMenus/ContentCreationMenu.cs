using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ContentCreationMenu : Menu{

	public Dungeon dungeon {
		get {return MasterControl.dungeon;}
		set {}
	}

	Text title;

	public bool isEditingExisting = false;
	public bool hasUnsavedChanges = false;
	public Type[] submenuTypes = new Type[6];

	string[] submenuTitles = {
		"MONSTER",
		"ARTIFACT",
		"TREASURE",
		"TRAP",
		"DUNGEON FEATURE",
		"RANDOM TABLE"
	};

	public override void OnMenuOpen(){

	}

	public override void InitialSetup(){
		title = transform.Find("Title").GetComponent<Text>();

		submenuTypes[0] = typeof(MonsterCreationSubmenu);
		submenuTypes[1] = typeof(ArtifactCreationSubmenu);
		submenuTypes[2] = typeof(TreasureCreationSubmenu);
		submenuTypes[3] = typeof(TrapCreationSubmenu);
		submenuTypes[4] = typeof(DungeonFeatureCreationSubmenu);
		submenuTypes[5] = typeof(RandomTableCreationSubmenu);

		submenus[0].GetComponent<MonsterCreationSubmenu>().contentCreationMenu = this;
		submenus[1].GetComponent<ArtifactCreationSubmenu>().contentCreationMenu = this;
		submenus[2].GetComponent<TreasureCreationSubmenu>().contentCreationMenu = this;
		submenus[3].GetComponent<TrapCreationSubmenu>().contentCreationMenu = this;
		submenus[4].GetComponent<DungeonFeatureCreationSubmenu>().contentCreationMenu = this;
		submenus[5].GetComponent<RandomTableCreationSubmenu>().contentCreationMenu = this;
	}

	public override void UpdateActive(){
		if(activeSubmenuIndex >= 0){
			string str;
			str = isEditingExisting ? "EDIT " : "CREATE NEW ";
			str += submenuTitles[activeSubmenuIndex];
			title.text = str;
		}
	}

	public override void OnMenuClose(){
		hasUnsavedChanges = false;
		root.Open();
		root.Activate();
	}

	public void CreateNewMonster(){
		EditMonster(null);
	}

	public void CreateNewArtifact(){
		EditArtifact(null);
	}

	public void CreateNewTreasure(){
		EditTreasure(null);
	}

	public void CreateNewTrap(){
		EditTrap(null);
	}

	public void CreateNewDungeonFeature(){
		EditDungeonFeature(null);
	}

	public void CreateNewRandomTable(){
		EditRandomTable(null);
	}

	public void EditMonster(Monster monster){
		if(hasUnsavedChanges){
			OpenConfirmationDialog("This form has unsaved edits. Continue and discard changes?", (bool isConfirmed) => {
				if(isConfirmed){
					submenus[0].GetComponent<MonsterCreationSubmenu>().monster = monster;
					activeSubmenuIndex = 0;
					Open();
					Activate();
				}
			});
		}else{
			submenus[0].GetComponent<MonsterCreationSubmenu>().monster = monster;
			activeSubmenuIndex = 0;
			Open();
			Activate();
		}

	}

	public void EditArtifact(Artifact artifact){
		if(hasUnsavedChanges){
			OpenConfirmationDialog("This form has unsaved edits. Continue and discard changes?", (bool isConfirmed) => {
				if(isConfirmed){
					submenus[1].GetComponent<ArtifactCreationSubmenu>().artifact = artifact;
					activeSubmenuIndex = 1;
					Open();
					Activate();
				}
			});
		}else{
			submenus[1].GetComponent<ArtifactCreationSubmenu>().artifact = artifact;
			activeSubmenuIndex = 1;
			Open();
			Activate();
		}

	}

	public void EditTreasure(Treasure treasure){
		if(hasUnsavedChanges){
			OpenConfirmationDialog("This form has unsaved edits. Continue and discard changes?", (bool isConfirmed) => {
				if(isConfirmed){
					submenus[2].GetComponent<TreasureCreationSubmenu>().treasure = treasure;
					activeSubmenuIndex = 2;
					Open();
					Activate();
				}
			});
		}else{
			submenus[2].GetComponent<TreasureCreationSubmenu>().treasure = treasure;
			activeSubmenuIndex = 2;
			Open();
			Activate();
		}

	}

	public void EditTrap(Trap trap){
		if(hasUnsavedChanges){
			OpenConfirmationDialog("This form has unsaved edits. Continue and discard changes?", (bool isConfirmed) => {
				if(isConfirmed){
					submenus[3].GetComponent<TrapCreationSubmenu>().trap = trap;
					activeSubmenuIndex = 3;
					Open();
					Activate();
				}
			});
		}else{
			submenus[3].GetComponent<TrapCreationSubmenu>().trap = trap;
			activeSubmenuIndex = 3;
			Open();
			Activate();
		}

	}

	public void EditDungeonFeature(DungeonFeature dungeonFeature){
		if(hasUnsavedChanges){
			OpenConfirmationDialog("This form has unsaved edits. Continue and discard changes?", (bool isConfirmed) => {
				if(isConfirmed){
					submenus[4].GetComponent<DungeonFeatureCreationSubmenu>().dungeonFeature = dungeonFeature;
					activeSubmenuIndex = 4;
					Open();
					Activate();
				}
			});
		}else{
			submenus[4].GetComponent<DungeonFeatureCreationSubmenu>().dungeonFeature = dungeonFeature;
			activeSubmenuIndex = 4;
			Open();
			Activate();
		}

	}

	public void EditRandomTable(RandomTable randomTable){
		if(hasUnsavedChanges){
			OpenConfirmationDialog("This form has unsaved edits. Continue and discard changes?", (bool isConfirmed) => {
				if(isConfirmed){
					submenus[5].GetComponent<RandomTableCreationSubmenu>().randomTable = randomTable;
					activeSubmenuIndex = 5;
					Open();
					Activate();
				}
			});
		}else{
			submenus[5].GetComponent<RandomTableCreationSubmenu>().randomTable = randomTable;
			activeSubmenuIndex = 5;
			Open();
			Activate();
		}
		
	}

}
