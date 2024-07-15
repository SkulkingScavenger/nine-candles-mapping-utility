using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainToolbar : MonoBehaviour{
	public DungeonMap map;
	Dungeon dungeon {
		get {return MasterControl.dungeon;}
		set {}
	}

	ContentCreationMenu contentCreationMenu;


	DropdownMenu fileDropdown;
	DropdownMenu contentDropdown;

	void Awake(){
		contentCreationMenu = MenuControl.canvas.transform.Find("ContentCreationMenu").GetComponent<ContentCreationMenu>();

		fileDropdown = transform.Find("FileDropdown").GetComponent<DropdownMenu>();
		fileDropdown.transform.Find("NewButton").GetComponent<Button>().onClick.AddListener(delegate { New(); } );
		fileDropdown.transform.Find("OpenButton").GetComponent<Button>().onClick.AddListener(delegate { Open(); } );
		fileDropdown.transform.Find("SaveButton").GetComponent<Button>().onClick.AddListener(delegate { Save(); } );
		fileDropdown.transform.Find("SaveAsButton").GetComponent<Button>().onClick.AddListener(delegate { SaveAs(); } );

		contentDropdown = transform.Find("ContentDropdown").GetComponent<DropdownMenu>();
		contentDropdown.transform.Find("MonsterButton").GetComponent<Button>().onClick.AddListener(delegate { OpenMonsterMenu(); } );
		contentDropdown.transform.Find("ArtifactButton").GetComponent<Button>().onClick.AddListener(delegate { OpenArtifactMenu(); } );
		contentDropdown.transform.Find("TreasureButton").GetComponent<Button>().onClick.AddListener(delegate { OpenTreasureMenu(); } );
		contentDropdown.transform.Find("TrapButton").GetComponent<Button>().onClick.AddListener(delegate { OpenTrapMenu(); } );
		contentDropdown.transform.Find("FeatureButton").GetComponent<Button>().onClick.AddListener(delegate { OpenFeatureMenu(); } );
		contentDropdown.transform.Find("RandomTableButton").GetComponent<Button>().onClick.AddListener(delegate { OpenRandomTableMenu(); } );

		transform.Find("FileButton").GetComponent<Button>().onClick.AddListener(delegate {OpenFileDropdown();});
		transform.Find("ContentButton").GetComponent<Button>().onClick.AddListener(delegate {OpenContentDropdown();});
	}

	void Update(){
		// if(InputControl.newDungeonPressed){
		// 	New();
		// }else if(InputControl.openDungeonPressed){
		// 	Open();
		// }else if(InputControl.saveDungeonPressed){
		// 	Save();
		// }else if(InputControl.saveDungeonAsPressed){
		// 	SaveAs();
		// }
	}

	public void OpenFileDropdown(){
		fileDropdown.Open();
	}

	public void OpenContentDropdown(){
		contentDropdown.Open();
	}

	public void OpenDataCreationMenu(){
		ContentCreationMenu ccm = MenuControl.canvas.transform.Find("ContentCreationMenu").GetComponent<ContentCreationMenu>();
		ccm.root = map;
		ccm.Open();
		ccm.Activate();
	}


	public void New(){
		NewDungeonDialog ndd = MenuControl.canvas.transform.Find("NewDungeonDialog").GetComponent<NewDungeonDialog>();
		ndd.root = MenuControl.activeMenu;
		ndd.Open();
		ndd.Activate();
	}

	public void Open(){
		OpenDungeonDialog odd = MenuControl.canvas.transform.Find("OpenDungeonDialog").GetComponent<OpenDungeonDialog>();
		odd.saveMode = false;
		odd.root = MenuControl.activeMenu;
		odd.Open();
		odd.Activate();
	}

	public void Save(){
		if(dungeon.fileName == null || dungeon.fileName == ""){
			SaveAs();
		}else{
			DungeonControl.SaveDungeon();
		}
	}

	public void SaveAs(){
		OpenDungeonDialog odd = MenuControl.canvas.transform.Find("OpenDungeonDialog").GetComponent<OpenDungeonDialog>();
		odd.saveMode = true;
		odd.root = MenuControl.activeMenu;
		odd.Open();
		odd.Activate();
	}

	public void OpenMonsterMenu(){
		contentCreationMenu.root = MenuControl.activeMenu;
		contentCreationMenu.CreateNewMonster();
		contentDropdown.Close();
	}

	public void OpenArtifactMenu(){
		contentCreationMenu.root = MenuControl.activeMenu;
		contentCreationMenu.CreateNewArtifact();
		contentDropdown.Close();
	}

	public void OpenTreasureMenu(){
		contentCreationMenu.activeSubmenuIndex = 2;
		contentCreationMenu.root = MenuControl.activeMenu;
		contentCreationMenu.Open();
		contentCreationMenu.Activate();
		contentDropdown.Close();
	}

	public void OpenTrapMenu(){
		contentCreationMenu.activeSubmenuIndex = 3;
		contentCreationMenu.root = MenuControl.activeMenu;
		contentCreationMenu.Open();
		contentCreationMenu.Activate();
		contentDropdown.Close();
	}

	public void OpenFeatureMenu(){
		contentCreationMenu.activeSubmenuIndex = 4;
		contentCreationMenu.root = MenuControl.activeMenu;
		contentCreationMenu.Open();
		contentCreationMenu.Activate();
		contentDropdown.Close();
	}

	public void OpenRandomTableMenu(){
		contentCreationMenu.activeSubmenuIndex = 5;
		contentCreationMenu.root = MenuControl.activeMenu;
		contentCreationMenu.Open();
		contentCreationMenu.Activate();
		contentDropdown.Close();
	}
}
