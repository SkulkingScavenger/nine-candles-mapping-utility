using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MonsterCreationSubmenu : Submenu{

	public Texture2D tex;

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

	public Monster monster;
	public Monster tempMonster;

	MenuButton saveButton;
	MenuButton newButton;
	MenuButton exitButton;

	InputField nameInput;
	InputField initiativeModifierInput;
	InputField[] movementInputs = new InputField[4];
	InputField armorClassInput;
	InputField challengeRatingInput;
	InputField[] attributeInputs = new InputField[6];
	InputField[] saveInputs = new InputField[3];
	InputField hitdiceCountInput;
	InputField hitdiceModifierInput;
	InputField descriptionInput;
	
	Dropdown hitdiceDropdown;
	Dropdown monsterTypeDropdown;
	Dropdown sizeDropdown;

	MenuSlotHolder attacksHolder;
	MenuSlotHolder specialAttacksHolder;
	MenuSlotHolder featsHolder;
	MenuSlotHolder specialQualitiesHolder;
	MenuSlotHolder skillsHolder;

	MenuButton newAttackButton;
	MenuButton newSpecialAttackButton;
	MenuButton newFeatButton;
	MenuButton newSpecialQualityButton;
	MenuButton newSkillButton;

	MenuButton uploadImageButton;
	MenuButton uploadIconButton;
	Image illustrationPreview;
	Image iconPreview;

	Text gameSystemValue;
	Text[] attributeLabels = new Text[6];
	Text[] saveLabels = new Text[3];

	MonsterAttackPanel attackPanel;
	MonsterSpecialAttackPanel specialAttackPanel;
	MonsterFeatPanel featPanel;
	MonsterSpecialQualityPanel specialQualityPanel;
	MonsterSkillPanel skillPanel;

	string[] monsterSizes = {"Fine","Diminutive","Tiny","Small","Medium","Large","Huge","Gargantuan","Colossal"};

	public override void InitialSetup(){
		Transform tsf;
		tsf = transform.Find("MainPanel");
		gameSystemValue = tsf.Find("Value - GameSystem").GetComponent<Text>();
		nameInput = tsf.Find("Input - Name").GetComponent<InputField>();
		initiativeModifierInput = tsf.Find("Input - InitiativeModifier").GetComponent<InputField>();
		for(int i=0;i<4;i++){
			movementInputs[i] = tsf.Find("Input - Movement" + i).GetComponent<InputField>();	
		}
		armorClassInput = tsf.Find("Input - ArmorClass").GetComponent<InputField>();
		challengeRatingInput = tsf.Find("Input - ChallengeRating").GetComponent<InputField>();
		hitdiceCountInput = tsf.Find("Input - HitdiceCount").GetComponent<InputField>();
		hitdiceModifierInput = tsf.Find("Input - HitdiceModifier").GetComponent<InputField>();
		for(int i=0;i<attributeInputs.Length;i++){
			attributeInputs[i] = tsf.Find("Input - Attribute" + i).GetComponent<InputField>();
			attributeLabels[i] = tsf.Find("Label - Attribute" + i).GetComponent<Text>();
		}
		for(int i=0;i<saveInputs.Length;i++){
			saveInputs[i] = tsf.Find("Input - Saves" + i).GetComponent<InputField>();
			saveLabels[i] = tsf.Find("Label - Saves" + i).GetComponent<Text>();
		}
		descriptionInput = tsf.Find("Input - Description").GetComponent<InputField>();

		hitdiceDropdown = tsf.Find("Dropdown - Hitdice").GetComponent<Dropdown>();
		monsterTypeDropdown = tsf.Find("Dropdown - MonsterType").GetComponent<Dropdown>();
		sizeDropdown = tsf.Find("Dropdown - Size").GetComponent<Dropdown>();

		List<string> opdatList = new List<string>();
		hitdiceDropdown.ClearOptions();
		for(int i=0;i<Dice.types.Length;i++){
			opdatList.Add("D" + Dice.types[i]);
		}
		hitdiceDropdown.AddOptions(opdatList);
		opdatList.Clear();

		monsterTypeDropdown.ClearOptions();
		for(int i=0;i<gameSystem.monsterTypes.Count;i++){
			opdatList.Add(gameSystem.monsterTypes[i].name);
		}
		monsterTypeDropdown.AddOptions(opdatList);
		opdatList.Clear();

		sizeDropdown.ClearOptions();
		for(int i=0;i<monsterSizes.Length;i++){
			opdatList.Add(monsterSizes[i]);
		}
		sizeDropdown.AddOptions(opdatList);
		opdatList.Clear();


		tsf = transform;
		attackPanel = tsf.Find("AttackPanel").GetComponent<MonsterAttackPanel>();
		specialAttackPanel = tsf.Find("SpecialAttackPanel").GetComponent<MonsterSpecialAttackPanel>();
		specialQualityPanel = tsf.Find("SpecialQualityPanel").GetComponent<MonsterSpecialQualityPanel>();
		featPanel = tsf.Find("FeatPanel").GetComponent<MonsterFeatPanel>();
		skillPanel = tsf.Find("SkillPanel").GetComponent<MonsterSkillPanel>();

		attackPanel.monsterMenu = this;
		attackPanel.InitialSetup();
		attackPanel.onClose = (bool isCancelled, bool isEditing, MonsterAttack ma) => {
			if(!isCancelled){
				if(!isEditing){
					tempMonster.attacks.Add(ma);
				}
				attacksHolder.SetList(GetAttacksAsStrings());
				hasUnsavedChanges = true;
			}
		};

		specialAttackPanel.monsterMenu = this;
		specialAttackPanel.InitialSetup();
		specialAttackPanel.onClose = (bool isCancelled, bool isEditing, MonsterAbility ma) => {
			if(!isCancelled){
				if(!isEditing){
					tempMonster.specialAttacks.Add(ma);
				}
				specialAttacksHolder.SetList(GetSpecialAttacksAsStrings());
				hasUnsavedChanges = true;
			}
		};

		specialQualityPanel.monsterMenu = this;
		specialQualityPanel.InitialSetup();
		specialQualityPanel.onClose = (bool isCancelled, bool isEditing, MonsterAbility ma) => {
			if(!isCancelled){
				if(!isEditing){
					tempMonster.specialQualities.Add(ma);
				}
				specialQualitiesHolder.SetList(GetSpecialQualitiesAsStrings());
				hasUnsavedChanges = true;
			}
		};


		tsf = transform.Find("MainPanel").Find("AttacksHolder");
		attacksHolder = tsf.Find("SlotHolder").GetComponent<MenuSlotHolder>();
		attacksHolder.menu = this;
		attacksHolder.InitialSetup();
		attacksHolder.onAddSlotButton.AddListener(delegate {attackPanel.Open(null);});
		attacksHolder.onSlotEditButton.AddListener((int index)=>{
			attackPanel.Open(tempMonster.attacks[index]);
		});

		tsf = transform.Find("MainPanel").Find("SpecialAttacksHolder");
		specialAttacksHolder = tsf.Find("SlotHolder").GetComponent<MenuSlotHolder>();
		specialAttacksHolder.menu = this;
		specialAttacksHolder.InitialSetup();
		specialAttacksHolder.onAddSlotButton.AddListener(delegate {specialAttackPanel.Open(null);});
		specialAttacksHolder.onSlotEditButton.AddListener((int index)=>{
			specialAttackPanel.Open(tempMonster.specialAttacks[index]);
		});

		tsf = transform.Find("MainPanel").Find("SpecialQualitiesHolder");
		specialQualitiesHolder = tsf.Find("SlotHolder").GetComponent<MenuSlotHolder>();
		specialQualitiesHolder.menu = this;
		specialQualitiesHolder.InitialSetup();
		specialQualitiesHolder.onAddSlotButton.AddListener(delegate {specialQualityPanel.Open(null);});
		specialQualitiesHolder.onSlotEditButton.AddListener((int index)=>{
			specialQualityPanel.Open(tempMonster.specialQualities[index]);
		});

		tsf = transform.Find("MainPanel").Find("FeatsHolder");
		featsHolder = tsf.Find("SlotHolder").GetComponent<MenuSlotHolder>();
		featsHolder.menu = this;
		featsHolder.InitialSetup();
		featsHolder.onAddSlotButton.AddListener(delegate {featPanel.Open();});
		featsHolder.canEdit = false;

		tsf = transform.Find("MainPanel").Find("SkillsHolder");
		skillsHolder = tsf.Find("SlotHolder").GetComponent<MenuSlotHolder>();
		skillsHolder.menu = this;
		skillsHolder.InitialSetup();
		// attacksHolder.onAddSlotButton.AddListener(delegate {attackPanel.Open(null);});
		// attacksHolder.onSlotEditButton.AddListener((int index)=>{
		// 	attackPanel.Open(tempMonster.attacks[index]);
		// });

		tsf = transform.Find("MainPanel").Find("ImageSection");
		uploadImageButton = tsf.Find("UploadImageButton").GetComponent<MenuButton>();
		uploadImageButton.onClick.AddListener(delegate {UploadImage(true);});
		uploadIconButton = tsf.Find("UploadIconButton").GetComponent<MenuButton>();
		uploadIconButton.onClick.AddListener(delegate {UploadImage(false);});

		illustrationPreview = tsf.Find("IllustrationPreview").GetComponent<Image>();
		iconPreview = tsf.Find("IconPreview").Find("Mask").Find("Image").GetComponent<Image>();

		tsf = transform.Find("ButtonSection");
		saveButton = tsf.Find("SaveButton").GetComponent<MenuButton>();
		newButton = tsf.Find("NewButton").GetComponent<MenuButton>();
		exitButton = tsf.Find("ExitButton").GetComponent<MenuButton>();
		saveButton.onClick.AddListener(delegate {SaveData();});
		newButton.onClick.AddListener(delegate {CreateNew();});
		exitButton.onClick.AddListener(delegate {Exit();});

		
	}

	public override void OnMenuOpen(){
		if(monster == null){
			monster = new Monster();
			tempMonster = new Monster();
			tempMonster.gameSystem = gameSystem;
			isEditingExisting = false;
		}else{
			LoadData();
			isEditingExisting = true;
		}
		
		ReloadFields();
	}

	public override void UpdateActive(){
		if(nameInput.text != tempMonster.name){
			tempMonster.name = nameInput.text;
			hasUnsavedChanges = true;	
		}
		if(Int32.Parse(hitdiceCountInput.text) != tempMonster.hitdice.diceCount && hitdiceCountInput.text != "" && hitdiceCountInput.text != "-"){
			tempMonster.hitdice = new Dice(Int32.Parse(hitdiceCountInput.text),tempMonster.hitdice.faceCount,tempMonster.hitdice.modifier);
			hasUnsavedChanges = true;	
		}
		if(Int32.Parse(hitdiceModifierInput.text) != tempMonster.hitdice.modifier && hitdiceModifierInput.text != "" && hitdiceModifierInput.text != "-"){
			tempMonster.hitdice = new Dice(tempMonster.hitdice.diceCount,tempMonster.hitdice.faceCount,Int32.Parse(hitdiceModifierInput.text));
			hasUnsavedChanges = true;	
		}
		if(Int32.Parse(initiativeModifierInput.text) != tempMonster.initiativeModifier && initiativeModifierInput.text != "" && initiativeModifierInput.text != "-"){
			tempMonster.initiativeModifier = Int32.Parse(initiativeModifierInput.text);
			hasUnsavedChanges = true;	
		}
		for(int i=0;i<4;i++){
			if(Int32.Parse(movementInputs[i].text) != tempMonster.movementSpeed[i] && movementInputs[i].text != "" && movementInputs[i].text != "-"){
				tempMonster.movementSpeed[i] = Int32.Parse(movementInputs[i].text);
				hasUnsavedChanges = true;
			}
		}
		if(Int32.Parse(armorClassInput.text) != tempMonster.armorClass && armorClassInput.text != "" && armorClassInput.text != "-"){
			tempMonster.armorClass = Int32.Parse(armorClassInput.text);
			hasUnsavedChanges = true;	
		}
		if(Int32.Parse(challengeRatingInput.text) != tempMonster.challengeRating && challengeRatingInput.text != "" && challengeRatingInput.text != "-"){
			tempMonster.challengeRating = Int32.Parse(challengeRatingInput.text);
			hasUnsavedChanges = true;	
		}
		for(int i=0;i<attributeInputs.Length;i++){
			if(Int32.Parse(attributeInputs[i].text) != tempMonster.attributes[i] && attributeInputs[i].text != "" && attributeInputs[i].text != "-"){
				tempMonster.attributes[i] = Int32.Parse(attributeInputs[i].text);
				hasUnsavedChanges = true;	
			}
		}
		for(int i=0;i<saveInputs.Length;i++){
			if(Int32.Parse(saveInputs[i].text) != tempMonster.savingThrows[i] && saveInputs[i].text != "" && saveInputs[i].text != "-"){
				tempMonster.savingThrows[i] = Int32.Parse(saveInputs[i].text);
				hasUnsavedChanges = true;	
			}
		}

		if(descriptionInput.text != tempMonster.description){
			tempMonster.description = descriptionInput.text;
			hasUnsavedChanges = true;	
		}


		if(monsterTypeDropdown.value != tempMonster.monsterType){
			tempMonster.monsterType = monsterTypeDropdown.value;
			hasUnsavedChanges = true;
		}
		if(sizeDropdown.value != tempMonster.size){
			tempMonster.size = sizeDropdown.value;
			hasUnsavedChanges = true;
		}
		if(Dice.types[hitdiceDropdown.value] != tempMonster.hitdice.faceCount){
			tempMonster.hitdice = new Dice(tempMonster.hitdice.diceCount, Dice.types[hitdiceDropdown.value], tempMonster.hitdice.modifier);
		}

		//TODO Treasure Dropdown

		saveButton.isDisabled = !hasUnsavedChanges;
		
	}

	public void ReloadFields(){
		gameSystemValue.text = tempMonster.gameSystem.name;
		nameInput.text = tempMonster.name;
		initiativeModifierInput.text = tempMonster.initiativeModifier.ToString();
		for(int i=0;i<4;i++){
			movementInputs[i].text = tempMonster.movementSpeed[i].ToString();	
		}
		armorClassInput.text = tempMonster.armorClass.ToString();
		challengeRatingInput.text = tempMonster.challengeRating.ToString();
		hitdiceCountInput.text = tempMonster.hitdice.diceCount.ToString();
		hitdiceModifierInput.text = tempMonster.hitdice.modifier.ToString();
		for(int i=0;i<attributeInputs.Length;i++){
			attributeInputs[i].text = tempMonster.attributes[i].ToString();
		}
		for(int i=0;i<saveInputs.Length;i++){
			saveInputs[i].text = tempMonster.savingThrows[i].ToString();
		}

		monsterTypeDropdown.value = tempMonster.monsterType;
		sizeDropdown.value = tempMonster.size;

		for(int i=0;i<Dice.types.Length;i++){
			if(Dice.types[i] == tempMonster.hitdice.faceCount){
				hitdiceDropdown.value = i;
				break;
			}
		}
		

		attacksHolder.SetList(GetAttacksAsStrings());
		specialAttacksHolder.SetList(GetSpecialAttacksAsStrings());
		specialQualitiesHolder.SetList(GetSpecialQualitiesAsStrings());

		illustrationPreview.sprite = tempMonster.illustration;
		iconPreview.sprite = tempMonster.icon;
		

		descriptionInput.text = tempMonster.description;
	}

	public void LoadData(){
		tempMonster = new Monster();
		tempMonster.CopyValuesFrom(monster);
	}

	public void SaveData(){
		hasUnsavedChanges = false;
		monster.CopyValuesFrom(tempMonster);
		if(!isEditingExisting){
			dungeon.AssignSystemID(monster);
			dungeon.monsters.Add(monster);
			isEditingExisting = true;
		}
	}

	public void CreateNew(){
		if(hasUnsavedChanges){
			OpenConfirmationDialog("This form has unsaved edits. Continue and discard changes?", (bool isConfirmed) => {
				if(isConfirmed){
					isEditingExisting = false;
					monster = new Monster();
					tempMonster = new Monster();
					ReloadFields();
					hasUnsavedChanges = false;
				}
			});
		}else{
			isEditingExisting = false;
			monster = new Monster();
			tempMonster = new Monster();
			ReloadFields();
			hasUnsavedChanges = false;
		}
	}

	public void Exit(){
		if(hasUnsavedChanges){
			OpenConfirmationDialog("This form has unsaved edits. Continue and discard changes?", (bool isConfirmed) => {
				if(isConfirmed){
					monster = null;
					Close();
					root.Close();
					hasUnsavedChanges = false;
				}
			});
		}else{
			monster = null;
			Close();
			root.Close();
			hasUnsavedChanges = false;
		}
	}

	List<string> GetAttacksAsStrings(){
		List<string> output = new List<string>();
		for(int i=0;i<tempMonster.attacks.Count;i++){
			output.Add(tempMonster.attacks[i].ToString());
		}
		return output;
	}

	List<string> GetSpecialAttacksAsStrings(){
		List<string> output = new List<string>();
		for(int i=0;i<tempMonster.specialAttacks.Count;i++){
			output.Add(tempMonster.specialAttacks[i].name);
		}
		return output;
	}

	List<string> GetSpecialQualitiesAsStrings(){
		List<string> output = new List<string>();
		for(int i=0;i<tempMonster.specialQualities.Count;i++){
			output.Add(tempMonster.specialQualities[i].name);
		}
		return output;
	}

	List<string> GetFeatsAsStrings(){
		List<string> output = new List<string>();
		for(int i=0;i<tempMonster.feats.Count;i++){
			output.Add(gameSystem.feats[tempMonster.feats[i]]);
		}
		return output;
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
			tex = FileUtilities.LoadImageFromFile(path);
			Rect rect = new Rect(0,0,tex.width,tex.height);
			Vector2 pivot = new Vector2(tex.width/2f,tex.height/2f);
			Sprite spr = Sprite.Create(tex, rect, pivot, 64f);
			illustrationPreview.sprite = spr;
			tempMonster.illustration = spr;
			hasUnsavedChanges = true;
		}
	}

	void UploadIconCallback(bool isCancelled, string path){
		if(!isCancelled){
			tex = FileUtilities.LoadImageFromFile(path);
			Rect rect = new Rect(0,0,tex.width,tex.height);
			Vector2 pivot = new Vector2(tex.width/2f,tex.height/2f);
			Sprite spr = Sprite.Create(tex, rect, pivot, 64f);
			iconPreview.sprite = spr;
			tempMonster.icon = spr;
			hasUnsavedChanges = true;
		}
	}


}
