using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class MasterControl : MonoBehaviour{
	bool isInitialized = false;

	PrefabManager prefabManager;
	TilesetManager tilesetManager;
	ScreenControl screenControl;
	MenuControl menuControl;
	PoolControl poolControl;

	public static EventSystem eventSystem;
	public static MasterControl Instance { get; private set; }
	public static Dungeon dungeon;
	public static List<GameSystem> gameSystems;

	void Awake(){
		if(!isInitialized){
			Init();
		}
	}

	void Init(){
		//ensure uniqueness
		if(Instance != null && Instance != this){
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(transform.gameObject);

		prefabManager = transform.Find("PrefabManager").GetComponent<PrefabManager>();
		tilesetManager = transform.Find("TilesetManager").GetComponent<TilesetManager>();
		screenControl = transform.Find("ScreenControl").GetComponent<ScreenControl>();
		menuControl = transform.Find("MenuControl").GetComponent<MenuControl>();
		poolControl = transform.Find("PoolControl").GetComponent<PoolControl>();


		prefabManager.Init();
		tilesetManager.Init();
		menuControl.Init();
		poolControl.Init();

		isInitialized = true;

		Startup();
	}

	void Update(){
		
	}

	void Startup(){
		LoadDefaultGameSystems();
		FileUtilities.ValidatePaths();
		// dungeon = new Dungeon(40, 30);
		// dungeon.gameSystem = gameSystems[0];
		menuControl.Startup();
	}

	void LoadDefaultGameSystems(){
		GameSystem gs;
		gameSystems = new List<GameSystem>();

		//DND
		gs = new GameSystem();
		gs.name = "DND 3.5";
		gs.currency = "GP";

		gs.attributes = new List<string>();
		gs.attributes.Add("Strength");
		gs.attributes.Add("Constitution");
		gs.attributes.Add("Dexterity");
		gs.attributes.Add("Intelligence");
		gs.attributes.Add("Wisdom");
		gs.attributes.Add("Charisma");

		gs.attributeAbbreviations = new List<string>();
		gs.attributeAbbreviations.Add("Str");
		gs.attributeAbbreviations.Add("Con");
		gs.attributeAbbreviations.Add("Dex");
		gs.attributeAbbreviations.Add("Int");
		gs.attributeAbbreviations.Add("Wis");
		gs.attributeAbbreviations.Add("Cha");

		gs.savingThrows = new List<string>();
		gs.savingThrows.Add("Fort");
		gs.savingThrows.Add("Ref");
		gs.savingThrows.Add("Will");

		MonsterType mt;
		gs.monsterTypes = new List<MonsterType>();
		mt = new MonsterType();
		mt.name = "Abberation";
		gs.monsterTypes.Add(mt);

		mt = new MonsterType();
		mt.name = "Humanoid";
		gs.monsterTypes.Add(mt);

		mt = new MonsterType();
		mt.name = "Monstrous Humanoid";
		gs.monsterTypes.Add(mt);

		mt = new MonsterType();
		mt.name = "Outsider";
		gs.monsterTypes.Add(mt);

		mt = new MonsterType();
		mt.name = "Magical Beast";
		gs.monsterTypes.Add(mt);

		mt = new MonsterType();
		mt.name = "Vermin";
		gs.monsterTypes.Add(mt);

		mt = new MonsterType();
		mt.name = "Undead";
		gs.monsterTypes.Add(mt);

		mt = new MonsterType();
		mt.name = "Elemental";
		gs.monsterTypes.Add(mt);

		mt = new MonsterType();
		mt.name = "Construct";
		gs.monsterTypes.Add(mt);

		mt = new MonsterType();
		mt.name = "Fae";
		gs.monsterTypes.Add(mt);

		mt = new MonsterType();
		mt.name = "Plant";
		gs.monsterTypes.Add(mt);

		mt = new MonsterType();
		mt.name = "Animal";
		gs.monsterTypes.Add(mt);

		gs.monsterTraits = new List<SystemField>();
		SystemField sf;
		sf = new SystemField();
		sf.name = "Alignment";
		sf.type = SystemField.FieldType.Discrete;
		sf.options.Add("Lawful Good");
		sf.options.Add("Neutral Good");
		sf.options.Add("Chaotic Good");
		sf.options.Add("Lawful Neutral");
		sf.options.Add("Neutral");
		sf.options.Add("Chaotic Neutral");
		sf.options.Add("Lawful Evil");
		sf.options.Add("Neutral Evil");
		sf.options.Add("Chaotic Evil");

		gs.skills = new List<string>();
		gs.skills.Add("Spot");
		gs.skills.Add("Search");
		gs.skills.Add("Hide");
		gs.skills.Add("Listen");
		gs.skills.Add("Move Silently");

		gs.feats = new List<string>();
		gs.feats.Add("Improved Initiative");
		gs.feats.Add("Toughness");

		gameSystems.Add(gs);

		//NCP
		gs = new GameSystem();
		gs.name = "NCP 4.0";
		gs.currency = "GP";

		gs.attributes = new List<string>();
		gs.attributes.Add("Strength");
		gs.attributes.Add("Dexterity");
		gs.attributes.Add("Toughness");
		gs.attributes.Add("Agility");
		gs.attributes.Add("Stamina");
		gs.attributes.Add("Alertness");
		gs.attributes.Add("Spirit");

		gs.attributeAbbreviations = new List<string>();
		gs.attributeAbbreviations.Add("Str");
		gs.attributeAbbreviations.Add("Dex");
		gs.attributeAbbreviations.Add("Tgh");
		gs.attributeAbbreviations.Add("Agi");
		gs.attributeAbbreviations.Add("Alr");
		gs.attributeAbbreviations.Add("Spi");

		gs.savingThrows = new List<string>();
		gs.savingThrows.Add("Fortitude");
		gs.savingThrows.Add("Reflex");
		gs.savingThrows.Add("Will");

		gs.monsterTypes = new List<MonsterType>();
		mt = new MonsterType();
		mt.name = "Human";
		gs.monsterTypes.Add(mt);

		mt = new MonsterType();
		mt.name = "Goblin";
		gs.monsterTypes.Add(mt);

		mt = new MonsterType();
		mt.name = "Nightmare";
		gs.monsterTypes.Add(mt);

		mt = new MonsterType();
		mt.name = "Ghoul";
		gs.monsterTypes.Add(mt);

		mt = new MonsterType();
		mt.name = "Gremlin";
		gs.monsterTypes.Add(mt);

		mt = new MonsterType();
		mt.name = "Vermin";
		gs.monsterTypes.Add(mt);

		mt = new MonsterType();
		mt.name = "Beast";
		gs.monsterTypes.Add(mt);

		mt = new MonsterType();
		mt.name = "Primordial";
		gs.monsterTypes.Add(mt);

		mt = new MonsterType();
		mt.name = "Animate Object";
		gs.monsterTypes.Add(mt);

		mt = new MonsterType();
		mt.name = "Fungus";
		gs.monsterTypes.Add(mt);

		gs.monsterTraits = new List<SystemField>();
		sf = new SystemField();
		sf.name = "Luminosity";
		sf.type = SystemField.FieldType.Numeric;

		gs.skills = new List<string>();
		gs.skills.Add("Perception"); //
		gs.skills.Add("Stealth"); //used for hiding and moving undetected
		gs.skills.Add("Deception"); //used 
		gs.skills.Add("Intuition");//or shrewdness, used to sense lies and illusions
		gs.skills.Add("Herblore");//or Foraging, used to find
		gs.skills.Add("Thieving");//slight of hand, lockpicking, disabling devices
		gs.skills.Add("Thieving");
		gs.skills.Add("Metalcraft"); //crafting and analysis of metal objects, establishing and running a forge, metal mechanisms
		gs.skills.Add("Construction"); //construction and analysis of stone and wooden objects and structures (Masonry vs Carpentry)
		gs.skills.Add("Ropework"); //making and using ropes, nets, etc.
		gs.skills.Add("Alchemy"); //brewing of potions, medicines and substances
		gs.skills.Add("Camping"); //miscellaneous survival skills such as setting alarms, pitching tents, building fires
		gs.skills.Add("Gramarye"); //skill with enchantments and magical objects and places
		gs.skills.Add("Lore"); //general knowledge about people and things


		gs.feats = new List<string>();
		gs.feats.Add("Improved Initiative"); 
		gs.feats.Add("Stout"); //grants 3 HP
		gs.feats.Add("Stubborn"); //grants 3 SAN
		gs.feats.Add("Fleetfooted"); //grants 5ft speed in light armor
		gs.feats.Add("Death Frenzy"); //grants bonuses at low HP
		gs.feats.Add("Grim Persistence"); //enables Will Save to stave off effects of injuries
		gs.feats.Add("Fast Healing"); //Decreased Recovery times for physical wounds and ailments
		gs.feats.Add("Fire Expert");//improves use of flaming oil, torches, and enables crafting of more sophisticated flammables
		gs.feats.Add("Dauntless");//allows a reroll of failed will saves 2/day
		gs.feats.Add("Unnatural Resilience");//allows a reroll of failed fortitude saves 2/day
		gs.feats.Add("Slippery");//allows a reroll of failed reflex saves 2/day
		gs.feats.Add("Berserk"); // allows the user to trade defense and accuracy for power
		gs.feats.Add("Deft Footwork"); //grants additional free 5ft step each round
		gs.feats.Add("Nimble Fighting"); //allows the user to trade power for accuracy and defense
		gs.feats.Add("Defensive Specialization"); //grants +1 AC (can never take Offensive Specialization or Precision Specialization)
		gs.feats.Add("Offensive Specialization"); //grants +1 DMG (can never take Defensive Specialization or Precision Specialization)
		gs.feats.Add("Precision Specialization"); //grants +2 ToHit (can never take Offensive Specialization or Defensive Specialization)
		gs.feats.Add("Lunging Attack"); //at cost of -2 AC, extend melee reach by 5ft for one round
		gs.feats.Add("Rapid Reload"); //decreases time to reload crossbow by one stage
		gs.feats.Add("Riposte");//Allows advanced attacks in response to enemy actions
		gs.feats.Add("Trip Attacks");//enables tripping attacks
		gs.feats.Add("Shove Attacks");//enables shoving attacks
		gs.feats.Add("Throw Opponent");//enables throwing attacks (reposition adjacent opponent to another adjacent square)
		gs.feats.Add("Combat Countermeasures");//become harder to trip, shove, and throw attempts
		gs.feats.Add("Improved Grapple");//improves grappling successs chance
		gs.feats.Add("Improved Darkvision"); //improves range of darkvision and low-light vision
		gs.feats.Add("Blindsense"); //improves use of auxiliary senses, mitigates penalties for fighting against invisible enemies
		gs.feats.Add("Locksmithing"); //allows picking and construction of mechanical locks
		gs.feats.Add("Tracking"); //allows use of perception to follow tracks
		gs.feats.Add("Mining"); // allows use of Construction to construct and analyze tunnels and earthworks
		gs.feats.Add("Mechanical Expertise");//allows disabling (thieving, metalwork) and construction (metalwork,carpentry) of mechanical devices
		gs.feats.Add("Rope Mastery");//enables advanced manuevers using ropes (lassos, nets) and improves Ropework checks used under duress (restraining struggling foes)
		gs.feats.Add("Masonry Expertise"); //enables use of construction skill to build advanced stone objects and grants +4 bonus on construction checks involving stone
		gs.feats.Add("Carpentry Expertise"); //enables use of construction skill to build advanced wooden objects and grants +4 bonus on construction checks involving wood
		gs.feats.Add("Poison Expert");//improves resistance to poison and allows use of alchemy skill to brew poisons
		gs.feats.Add("Skill Specialization"); //grants +3 on one chosen skill. can only be taken once ever
		gs.feats.Add("Skill Mastery"); //grants additional +3 on specialized skill or one other. requires Skill Specialization. can only be taken once
		gs.feats.Add("Extra Skill"); //grants additional skill point per level

		gameSystems.Add(gs);

	}

	public static void SetDungeon(Dungeon dng){
		dungeon = dng;
		MenuControl.ReloadMenus();
	}

}