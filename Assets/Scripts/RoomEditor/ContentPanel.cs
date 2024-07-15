using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ContentPanel : MenuSection{

	public RoomEditorMenu roomEditor;
	ContentCreationMenu contentCreationMenu;
	public Dungeon dungeon {
		get {return roomEditor.dungeon;}
		set {}
	}
	public DungeonFloor currentFloor {
		get {return roomEditor.currentFloor;}
		set {}
	}
	public DungeonRoom room {
		get {return roomEditor.room;}
		set {}
	}
	int tabIndex {
		get {return roomEditor.tabIndex;}
		set {}
	}
	public Monster selectedMonster {
		get {
			if(tabIndex != 2 || contentDisplay.selectedIndex < 0){
				return null;
			}else{
				return dungeon.monsters[contentDisplay.selectedIndex];
			}
		}
	}
	public DungeonFeature selectedDungeonFeature {
		get {
			if(tabIndex != 1 || contentDisplay.selectedIndex < 0){
				return null;
			}else{
				return dungeon.dungeonFeatures[contentDisplay.selectedIndex];
			}
		}
	}

	MenuSlotHolder contentDisplay;

	GameObject monsterPreview;

	GameObject roomDescriptionPanel;
	InputField descriptionPlayerInput;
	InputField descriptionInput;

	public void InitialSetup(){
		Transform tsf;
		tsf = transform;
		contentDisplay = tsf.Find("MenuSlotHolder").GetComponent<MenuSlotHolder>();
		contentDisplay.InitialSetup();
		contentDisplay.canEdit = true;
		contentDisplay.canDelete = true;
		contentDisplay.canAddNew = true;
		contentDisplay.canSelect = true;
		contentDisplay.onSlotEditButton.AddListener((int slotIndex) => { EditItem(slotIndex); });
		contentDisplay.onSlotDeleteButton.AddListener((int slotIndex) => { DeleteItem(slotIndex); });
		contentDisplay.onAddSlotButton.AddListener(() => { NewItem(); });
		
		contentCreationMenu = MenuControl.canvas.transform.Find("ContentCreationMenu").GetComponent<ContentCreationMenu>();
		monsterPreview = tsf.Find("MonsterPreview").gameObject;

		tsf = transform.Find("RoomDescriptionPanel");
		roomDescriptionPanel = tsf.gameObject;
		descriptionPlayerInput = tsf.Find("Input - DescriptionPlayer").GetComponent<InputField>();
		descriptionInput = tsf.Find("Input - Description").GetComponent<InputField>();

	}

	void Update(){
		if(tabIndex == 0){
			if(room.descriptionPlayer != descriptionPlayerInput.text){
				room.descriptionPlayer = descriptionPlayerInput.text;
			}
			if(room.description != descriptionInput.text){
				room.description = descriptionInput.text;
			}
		}else if(tabIndex == 1){

		}else if(tabIndex == 2){
			SetMonsterPreview();
		}
	}

	public void Reload(){
		contentDisplay.gameObject.SetActive(tabIndex != 0);
		roomDescriptionPanel.SetActive(tabIndex == 0);
		if(tabIndex == 0){
			descriptionPlayerInput.text = room.descriptionPlayer;
			descriptionInput.text = room.description;
		}else if(tabIndex == 1){
			contentDisplay.selectedIndex = -1;
			contentDisplay.SetList(GetDungeonFeaturesAsStrings());
		}else if(tabIndex == 2){
			contentDisplay.selectedIndex = -1;
			contentDisplay.SetList(GetMonstersAsStrings());
			

		}
		monsterPreview.SetActive(tabIndex == 2);
	}

	List<string> GetMonstersAsStrings(){
		List<string> output = new List<string>();
		for(int i=0;i<dungeon.monsters.Count;i++){
			output.Add(dungeon.monsters[i].name);
		}
		return output;
	}

	List<string> GetDungeonFeaturesAsStrings(){
		List<string> output = new List<string>();
		for(int i=0;i<dungeon.dungeonFeatures.Count;i++){
			output.Add(dungeon.dungeonFeatures[i].name);
		}
		return output;
	}

	void SetMonsterPreview(){
		Text title = monsterPreview.transform.Find("TitlePreview").GetComponent<Text>();
		Text statblock = monsterPreview.transform.Find("StatBlock").GetComponent<Text>();
		if(contentDisplay.selectedIndex < 0){
			title.text = "";
			statblock.text = "";
		}else{
			title.text = dungeon.monsters[contentDisplay.selectedIndex].name;
			statblock.text = dungeon.monsters[contentDisplay.selectedIndex].ToString();
		}
	}

	void EditItem(int index){
		if(tabIndex == 1){
			EditDungeonFeature(index);
		}else if(tabIndex == 2){
			EditMonster(index);
		}
	}

	void DeleteItem(int index){
		if(tabIndex == 1){
			DeleteDungeonFeature(index);
		}else if(tabIndex == 2){
			DeleteMonster(index);
		}
	}

	void NewItem(){
		if(tabIndex == 1){
			NewDungeonFeature();
		}else if(tabIndex == 2){
			NewMonster();
		}
	}

	void EditDungeonFeature(int index){
		contentCreationMenu.root = roomEditor;
		contentCreationMenu.EditDungeonFeature(dungeon.dungeonFeatures[index + contentDisplay.currentIndex]);
	}

	void DeleteDungeonFeature(int index){
		dungeon.dungeonFeatures.RemoveAt(index + contentDisplay.currentIndex);
		Reload();
	}

	void NewDungeonFeature(){
		contentCreationMenu.root = roomEditor;
		contentCreationMenu.CreateNewDungeonFeature();
	}

	void EditMonster(int index){
		contentCreationMenu.root = roomEditor;
		contentCreationMenu.EditMonster(dungeon.monsters[index + contentDisplay.currentIndex]);
	}

	void DeleteMonster(int index){
		dungeon.monsters.RemoveAt(index + contentDisplay.currentIndex);
		Reload();
	}

	void NewMonster(){
		contentCreationMenu.root = roomEditor;
		contentCreationMenu.CreateNewMonster();
	}

}
