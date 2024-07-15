using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RandomTableCreationSubmenu : Submenu{

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

	public RandomTable randomTable;
	public RandomTable tempRandomTable;

	InputField nameInput;
	Dropdown typeDropdown;
	Dropdown diceDropdown;

	MenuButton newEntryButton;

	MenuButton saveButton;
	MenuButton newButton;
	MenuButton exitButton;

	Text totalResultsLabel;
	Text totalResultsValue;
	Text rangePreview;
	Text entryPreview;

	public int currentIndex;
	int slotCount;
	int slotHeight = 32;
	int slotHolderOffsetY = 80;
	DiscreteScrollbar scrollbar;
	GameObject rowSection;
	GameObject rowHolder;
	GameObject rowPrefab;
	List<GameObject> rows;
	Queue<GameObject> rowPool;

	public override void InitialSetup(){
		Transform tsf;
		tsf = transform.Find("MainPanel").Find("InputSection");
		nameInput = tsf.Find("Input - Name").GetComponent<InputField>();
		typeDropdown = tsf.Find("Dropdown - Type").GetComponent<Dropdown>();
		diceDropdown = tsf.Find("Dropdown - Dice").GetComponent<Dropdown>();

		rangePreview = tsf.Find("Preview").Find("RangePreview").GetComponent<Text>();
		entryPreview = tsf.Find("Preview").Find("EntryPreview").GetComponent<Text>();

		List<string> opdatList = new List<string>();
		typeDropdown.ClearOptions();
		for(int i=0;i<RandomTable.types.Length;i++){
			opdatList.Add(RandomTable.types[i]);
		}
		typeDropdown.AddOptions(opdatList);

		opdatList.Clear();
		diceDropdown.ClearOptions();
		for(int i=0;i<Dice.types.Length;i++){
			opdatList.Add("D" + Dice.types[i]);
		}
		diceDropdown.AddOptions(opdatList);


		tsf = transform.Find("MainPanel").Find("ButtonSection");
		saveButton = tsf.Find("SaveButton").GetComponent<MenuButton>();
		newButton = tsf.Find("NewButton").GetComponent<MenuButton>();
		exitButton = tsf.Find("ExitButton").GetComponent<MenuButton>();
		saveButton.onClick.AddListener( delegate { SaveData(); } );
		newButton.onClick.AddListener( delegate { CreateNew(); } );
		exitButton.onClick.AddListener( delegate { Exit(); } );
		

		tsf = transform.Find("MainPanel").Find("RowSection");
		scrollbar = tsf.Find("DiscreteScrollbar").GetComponent<DiscreteScrollbar>();
		scrollbar.menu = this;
		scrollbar.section = tsf.GetComponent<MenuSection>();
		scrollbar.Init();
		scrollbar.onValueChanged.AddListener(delegate {OnScroll();} );

		newEntryButton = tsf.Find("AddRowButton").GetComponent<MenuButton>();
		newEntryButton.onClick.AddListener( delegate { AddNewRow(); } );

		totalResultsLabel = tsf.Find("Label - TotalResults").GetComponent<Text>();
		totalResultsValue = tsf.Find("TotalResults").GetComponent<Text>();
		rowSection = tsf.gameObject;
		rowHolder = tsf.Find("RowHolder").gameObject;

		rowPrefab = tsf.Find("RowHolder").Find("Row").gameObject;
		rowPool = new Queue<GameObject>();
		DepositRow(rowPrefab);
		rows = new List<GameObject>();
	}

	public override void OnMenuOpen(){
		if(randomTable == null){
			isEditingExisting = false;
			randomTable = new RandomTable();
			tempRandomTable = new RandomTable();
		}else{
			isEditingExisting = true;
			tempRandomTable = new RandomTable();
			tempRandomTable.CopyValuesFrom(randomTable);
		}
		ReloadFields();
		hasUnsavedChanges = false;
	}

	public override void UpdateActive(){
		if(nameInput.text != tempRandomTable.name){
			tempRandomTable.name = nameInput.text;
			hasUnsavedChanges = true;
		}
		if(typeDropdown.value != tempRandomTable.type){
			tempRandomTable.type = typeDropdown.value;
			hasUnsavedChanges = true;
		}
		if(Dice.types[diceDropdown.value] != tempRandomTable.range){
			tempRandomTable.range = Dice.types[diceDropdown.value];
			hasUnsavedChanges = true;
		}

		GameObject go;
		int rangeInput;
		string entryInput;
		for(int i=0;i<slotCount;i++){
			if(i + currentIndex < tempRandomTable.entries.Count){
				go = rows[i];
				rangeInput = Int32.Parse(go.transform.Find("Range").GetComponent<InputField>().text);
				entryInput = go.transform.Find("Entry").GetComponent<InputField>().text;
				if(rangeInput != tempRandomTable.entries[i+currentIndex].width){
					tempRandomTable.entries[i+currentIndex].width = rangeInput;
					hasUnsavedChanges = true;
				}
				if(entryInput != tempRandomTable.entries[i+currentIndex].entry){
					tempRandomTable.entries[i+currentIndex].entry = entryInput;
					hasUnsavedChanges = true;
				}
			}
		}

		int totalResults = tempRandomTable.TotalResults();
		totalResultsValue.text = totalResults.ToString() + "/" + tempRandomTable.range.ToString();
		if(IsTableValid()){
			totalResultsLabel.color = new Color(1f,1f,1f,1f);
			totalResultsValue.color = new Color(1f,1f,1f,1f);
		}else{
			totalResultsLabel.color = new Color(1f,0f,0f,1f);
			totalResultsValue.color = new Color(1f,0f,0f,1f);
		}

		DrawTablePreview();

		saveButton.isDisabled = !hasUnsavedChanges || !IsTableValid();
	}

	void ReloadFields(){
		nameInput.text = tempRandomTable.name;
		typeDropdown.value = tempRandomTable.type;
		for(int i=0;i<Dice.types.Length;i++){
			if(Dice.types[i] == tempRandomTable.range){
				diceDropdown.value = i;
				break;
			}
		}
		SetTableRows();
	}

	void OnScroll(){
		currentIndex = scrollbar.value;
		RefreshTableRows();
	}

	void SetTableRows(){
		ClearTableRows();
		int sectionHeight = (int)Mathf.Round(rowSection.GetComponent<RectTransform>().rect.height);
		
		slotCount = tempRandomTable.range;
		if(slotCount * slotHeight > sectionHeight - slotHolderOffsetY){
			slotCount = (sectionHeight - slotHolderOffsetY) / slotHeight;
		}

		int holderHeight = slotHeight * slotCount;

		RectTransform rt = rowHolder.GetComponent<RectTransform>();
		rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, holderHeight);
		float y = -(holderHeight/2) - slotHolderOffsetY;
		rt.anchoredPosition = new Vector2(272,y);

		scrollbar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, holderHeight);
		scrollbar.SetScrollbar(currentIndex, tempRandomTable.entries.Count, slotCount);
		scrollbar.GetComponent<RectTransform>().anchoredPosition = new Vector3(48,y);

		GameObject go;
		for(int i=0;i<slotCount;i++){
			int index = i;
			go = WithdrawRow();
			go.transform.Find("SortUpButton").GetComponent<MenuButton>().onClick.AddListener(delegate {SortUp(index);});
			go.transform.Find("SortDownButton").GetComponent<MenuButton>().onClick.AddListener(delegate {SortDown(index);});
			go.transform.Find("DeleteButton").GetComponent<MenuButton>().onClick.AddListener(delegate {DeleteEntry(index);});
			go.transform.SetParent(rt);
			go.transform.localPosition = new Vector3(0,-i*slotHeight + holderHeight/2 - slotHeight/2,0);
			rows.Add(go);
		}

		RefreshTableRows();
	}

	void RefreshTableRows(){
		GameObject go;
		for(int i=0;i<slotCount;i++){
			go = rows[i];
			if(i+currentIndex < tempRandomTable.entries.Count){
				go.SetActive(true);
				go.transform.Find("SortUpButton").gameObject.SetActive(i+currentIndex != 0);
				go.transform.Find("SortDownButton").gameObject.SetActive(i+currentIndex != tempRandomTable.entries.Count - 1);
				go.transform.Find("Range").GetComponent<InputField>().text = tempRandomTable.entries[i+currentIndex].width.ToString();
				go.transform.Find("Entry").GetComponent<InputField>().text = tempRandomTable.entries[i+currentIndex].entry;
			}else{
				go.SetActive(false);
			}
		}
		scrollbar.SetScrollbar(currentIndex, tempRandomTable.entries.Count, slotCount);
	}

	void SortUp(int index){
		if(index + currentIndex == 0){return;}
		tempRandomTable.SwapEntries(index+currentIndex, index+currentIndex -1);
		hasUnsavedChanges = true;
		RefreshTableRows();
	}

	void SortDown(int index){
		if(index + currentIndex == tempRandomTable.entries.Count - 1){return;}
		tempRandomTable.SwapEntries(index+currentIndex, index+currentIndex +1);
		hasUnsavedChanges = true;
		RefreshTableRows();
	}

	void AddNewRow(){
		tempRandomTable.entries.Add(new RandomTableEntry(1,""));
		hasUnsavedChanges = true;
		RefreshTableRows();
	}

	void DeleteEntry(int index){
		tempRandomTable.entries.RemoveAt(index+currentIndex);
		hasUnsavedChanges = true;
		RefreshTableRows();
	}

	void ClearTableRows(){
		for(int i=0;i<rows.Count;i++){
			DepositRow(rows[i]);
		}
	}

	bool IsTableValid(){
		return tempRandomTable.TotalResults() == tempRandomTable.range;
	}

	void DrawTablePreview(){
		Text titlePreview = transform.Find("MainPanel").Find("InputSection").Find("Preview").Find("TitlePreview").GetComponent<Text>();
		Text dicePreview = transform.Find("MainPanel").Find("InputSection").Find("Preview").Find("DicePreview").GetComponent<Text>();
		titlePreview.text = tempRandomTable.name;
		dicePreview.text = "D" + tempRandomTable.range;
		if(tempRandomTable.range == 100){
			dicePreview.text = "D%";
		}
		if(IsTableValid()){
			string entries = "";
			string ranges = "";
			List<string> entryList = tempRandomTable.GetEntriesAsStrings();
			List<string> rangeList = tempRandomTable.GetRangesAsStrings();
			for(int i=0;i<entryList.Count;i++){
				entries += entryList[i] + "\n";
				ranges += rangeList[i] + "\n";
			}
			rangePreview.text = ranges;
			entryPreview.text = entries;
		}else{
			rangePreview.text = "";
			entryPreview.text = "ERROR - missing entries";
		}
	}

	void DepositRow(GameObject go){
		go.transform.SetParent(transform);
		go.transform.localPosition = Vector3.zero;
		go.SetActive(false);
		rowPool.Enqueue(go);
	}

	GameObject WithdrawRow(){
		GameObject go;
		if(rowPool.Count == 0){
			go = Instantiate(rowPrefab);
		}else{
			go = rowPool.Dequeue();
			go.SetActive(true);
		}
		return go;
	}

	public void SaveData(){
		hasUnsavedChanges = false;
		randomTable.CopyValuesFrom(tempRandomTable);
		if(!isEditingExisting){
			dungeon.AssignSystemID(randomTable);
			dungeon.randomTables.Add(randomTable);
			isEditingExisting = true;
		}
	}

	public void CreateNew(){
		if(hasUnsavedChanges){
			OpenConfirmationDialog("This form has unsaved edits. Continue and discard changes?", (bool isConfirmed) => {
				if(isConfirmed){
					isEditingExisting = false;
					randomTable = new RandomTable();
					tempRandomTable = new RandomTable();
					ReloadFields();
					hasUnsavedChanges = false;
				}
			});
		}else{
			isEditingExisting = false;
			randomTable = new RandomTable();
			tempRandomTable = new RandomTable();
			ReloadFields();
			hasUnsavedChanges = false;
		}
	}

	public void Exit(){
		if(hasUnsavedChanges){
			OpenConfirmationDialog("This form has unsaved edits. Continue and discard changes?", (bool isConfirmed) => {
				if(isConfirmed){
					randomTable = null;
					Close();
					root.Close();
				}
			});
		}else{
			randomTable = null;
			Close();
			root.Close();
		}
	}
}
