using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterSize {Fine, Diminutive, Tiny, Small, Medium, Large, Huge, Gargantuan, Colossal}

public class MonsterType{
	public string name;
	public List<MonsterAbility> traits;
}

public class MonsterAbility{
	public string name;
	public string description;

	public void CopyValuesFrom(MonsterAbility ma){
		name = ma.name;
		description = ma.description;
	}

	public List<string> ToJSON(int indentAmount){
		List<string> output = new List<string>();
		output.Add(FileUtilities.Indentation(indentAmount) + "{");
		indentAmount += 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "\"name\" : " + FileUtilities.WrapString(name) + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"description\" : " + FileUtilities.WrapString(description) + ",");
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "}");
		return output;
	}
}

public class MonsterAttack{
	public string name = "";
	public int toHitModifier = 0;
	public Dice damage = new Dice(1,6,0);

	public void CopyValuesFrom(MonsterAttack ma){
		name = ma.name;
		toHitModifier = ma.toHitModifier;
		damage = ma.damage;
	}

	public override string ToString(){
		string mod = toHitModifier < 0 ? " " : " +";
		mod += toHitModifier.ToString();
		return name + mod + " (" + damage + ")";
	}

	public List<string> ToJSON(int indentAmount){
		List<string> output = new List<string>();
		output.Add(FileUtilities.Indentation(indentAmount) + "{");
		indentAmount += 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "\"name\" : " + FileUtilities.WrapString(name) + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"toHitModifier\" : " + toHitModifier.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"damage\" : ");
		output.AddRange(damage.ToJSON(indentAmount));
		output.Add(FileUtilities.Indentation(indentAmount) + ",");
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "}");
		return output;
	}
}

public class MonsterSkill{
	public int index;
	public int modifier;
	public int ranks;
}

public class MonsterStat{
	public int index;
	public int value;
	public string str;
}

public class Monster{
	public static string[] sizes = {"Fine", "Diminutive", "Tiny", "Small", "Medium-Size", "Large", "Huge", "Gargantuan", "Colossal"};

	public uint systemID;
	public int gameSystemIndex;
	public GameSystem gameSystem {
		get { return MasterControl.gameSystems[gameSystemIndex]; }
		set {}
	}
	public string name = "";
	public string description = "";
	public Dice hitdice = new Dice(1,6,0);
	public int monsterType;
	public int size = 4;
	public int initiativeModifier = 0;
	public int[] movementSpeed = {30,0,0,0};
	public int armorClass = 10;
	public int challengeRating = 1;
	public Treasure treasure;
	public List<MonsterAttack> attacks;
	public List<MonsterAbility> specialAttacks;
	public List<MonsterAbility> specialQualities;
	public List<int> feats;
	public List<MonsterSkill> skills;
	public int[] attributes;
	public int[] savingThrows;
	public List<MonsterStat> otherStats;
	public Sprite illustration;
	public Sprite icon;
	public string illustrationFileName {
		get {return "monster_" + systemID;}
		set {}
	}
	public string iconFileName {
		get {return "monster_" + systemID;}
		set {}
	}

	public Monster(){
		gameSystemIndex = MasterControl.dungeon.gameSystemIndex;
		hitdice = new Dice(1,6,0);
		size = 4;
		initiativeModifier = 0;
		armorClass = 10;
		attacks = new List<MonsterAttack>();
		specialAttacks = new List<MonsterAbility>();
		specialQualities = new List<MonsterAbility>();
		skills = new List<MonsterSkill>();
		attributes = new int[6];
		savingThrows = new int[3];
		otherStats = new List<MonsterStat>();
		feats = new List<int>();
	}

	public Monster(int gsi){
		gameSystemIndex = gsi;
		hitdice = new Dice(1,6,0);
		size = 4;
		initiativeModifier = 0;
		armorClass = 10;
		attacks = new List<MonsterAttack>();
		specialAttacks = new List<MonsterAbility>();
		specialQualities = new List<MonsterAbility>();
		skills = new List<MonsterSkill>();
		attributes = new int[6];
		savingThrows = new int[3];
		otherStats = new List<MonsterStat>();
		feats = new List<int>();
	}

	public void CopyValuesFrom(Monster m){
		gameSystemIndex = m.gameSystemIndex;
		name = m.name;
		description = m.description;
		hitdice = m.hitdice;
		monsterType = m.monsterType;
		size = m.size;
		initiativeModifier = m.initiativeModifier;
		for(int i=0;i<m.movementSpeed.Length;i++){
			movementSpeed[i] = m.movementSpeed[i];
		}
		armorClass = m.armorClass;
		challengeRating = m.challengeRating;
		treasure = m.treasure;
		attacks.Clear();
		MonsterAttack ma;
		for(int i=0;i<m.attacks.Count;i++){
			ma = new MonsterAttack();
			ma.name = m.attacks[i].name;
			ma.toHitModifier = m.attacks[i].toHitModifier;
			ma.damage = m.attacks[i].damage;
			attacks.Add(ma);
		}
		MonsterAbility mab;
		specialAttacks.Clear();
		for(int i=0;i<m.specialAttacks.Count;i++){
			mab = new MonsterAbility();
			mab.name = m.specialAttacks[i].name;
			mab.description = m.specialAttacks[i].description;
			specialAttacks.Add(mab);
		}
		specialQualities.Clear();
		for(int i=0;i<m.specialQualities.Count;i++){
			mab = new MonsterAbility();
			mab.name = m.specialQualities[i].name;
			mab.description = m.specialQualities[i].description;
			specialQualities.Add(mab);
		}
		feats.Clear();
		for(int i=0;i<m.feats.Count;i++){
			feats.Add(m.feats[i]);
		}
		MonsterSkill ms;
		skills.Clear();
		for(int i=0;i<m.skills.Count;i++){
			ms = new MonsterSkill();
			ms.index = m.skills[i].index;
			ms.modifier = m.skills[i].modifier;
			skills.Add(ms);
		}
		for(int i=0;i<attributes.Length;i++){
			attributes[i] = m.attributes[i];
		}
		for(int i=0;i<savingThrows.Length;i++){
			savingThrows[i] = m.savingThrows[i];
		}
		MonsterStat mst;
		otherStats.Clear();
		for(int i=0;i<m.otherStats.Count;i++){
			mst = new MonsterStat();
			mst.index = m.otherStats[i].index;
			if(gameSystem.monsterTraits[mst.index].type == SystemField.FieldType.Text){
				mst.str = m.otherStats[i].str;
			}else{
				mst.value = m.otherStats[i].value;
			}
			otherStats.Add(mst);
		}

		illustration = m.illustration;
		icon = m.icon;
	}


	public List<string> ToJSON(int indentAmount){
		List<string> output = new List<string>();
		output.Add(FileUtilities.Indentation(indentAmount) + "{");
		indentAmount += 1;

		output.Add(FileUtilities.Indentation(indentAmount) + "\"systemID\" : " + systemID.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"gameSystemIndex\" : " + gameSystemIndex.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"name\" : " + FileUtilities.WrapString(name) + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"description\" : " + FileUtilities.WrapString(description) + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"hitdice\" : ");
		output.AddRange(hitdice.ToJSON(indentAmount));
		output.Add(FileUtilities.Indentation(indentAmount) + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"monsterType\" : " + monsterType.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"size\" : " + size.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"initiativeModifier\" : " + initiativeModifier.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"movementSpeed\" :[");
		indentAmount += 1;
		for(int i=0;i<movementSpeed.Length;i++){
			output.Add(FileUtilities.Indentation(indentAmount) + movementSpeed[i].ToString() + ",");
		}
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "],");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"armorClass\" : " + armorClass.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"challengeRating\" : " + challengeRating.ToString() + ",");
		

		//output.Add(FileUtilities.Indentation(indentAmount) + "\"treasure\" : " + treasure.systemID.ToString() + ",");

		output.Add(FileUtilities.Indentation(indentAmount) + "\"attacks\" :[");
		indentAmount += 1;
		for(int i=0;i<attacks.Count;i++){
			output.AddRange(attacks[i].ToJSON(indentAmount));
			if(i != attacks.Count - 1){
				output[output.Count -1] += ",";
			}
		}
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "],");


		output.Add(FileUtilities.Indentation(indentAmount) + "\"specialAttacks\" :[");
		indentAmount += 1;
		for(int i=0;i<specialAttacks.Count;i++){
			output.AddRange(specialAttacks[i].ToJSON(indentAmount));
			if(i != specialAttacks.Count - 1){
				output[output.Count -1] += ",";
			}
		}
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "],");


		output.Add(FileUtilities.Indentation(indentAmount) + "\"specialQualities\" :[");
		indentAmount += 1;
		for(int i=0;i<specialQualities.Count;i++){
			output.AddRange(specialQualities[i].ToJSON(indentAmount));
			if(i != specialQualities.Count - 1){
				output[output.Count -1] += ",";
			}
		}
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "],");


		output.Add(FileUtilities.Indentation(indentAmount) + "\"feats\" :[");
		indentAmount += 1;
		for(int i=0;i<feats.Count;i++){
			output.Add(feats[i].ToString());
			if(i != feats.Count - 1){
				output[output.Count -1] += ",";
			}
		}
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "],");


		// output.Add(FileUtilities.Indentation(indentAmount) + "\"skills\" :[");
		// indentAmount += 1;
		// for(int i=0;i<skills.Count;i++){
		// 	output.AddRange(skills[i].ToJSON(indentAmount));
		// 	if(i != skills.Count - 1){
		// 		output[output.Count -1] += ",";
		// 	}
		// }
		// indentAmount -= 1;
		// output.Add(FileUtilities.Indentation(indentAmount) + "],");


		//TO DO Feats and Skills

		output.Add(FileUtilities.Indentation(indentAmount) + "\"attributes\" :[");
		indentAmount += 1;
		for(int i=0;i<attributes.Length;i++){
			output.Add(FileUtilities.Indentation(indentAmount) + attributes[i].ToString() + ",");
		}
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "],");

		output.Add(FileUtilities.Indentation(indentAmount) + "\"savingThrows\" :[");
		indentAmount += 1;
		for(int i=0;i<savingThrows.Length;i++){
			output.Add(FileUtilities.Indentation(indentAmount) + savingThrows[i].ToString() + ",");
		}
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "],");

		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "}");
		return output;
	}

	// systemID
	// gameSystem
	// name
	// description
	// hitdice
	// monsterType
	// size
	// initiativeModifier
	// movementSpeed
	// armorClass
	// challengeRating
	// treasure
	// attacks
	// specialAttacks
	// specialQualities
	// feats
	// skills
	// attributes
	// savingThrows
	// otherStats
	// illustrationFileName
	// iconFileName

	public override string ToString(){
		string str = "";
		string sign = "";
		string[] movementTypes = {"",", fly ",", climb ",", swim "};

		str += "<b>" + Monster.sizes[size] + " " + gameSystem.monsterTypes[monsterType].name + "</b>\n";
		str += "<b>Hit Dice:</b> " + hitdice + " " + "(" + hitdice.Average() + " hp)\n";
		sign = initiativeModifier >= 0 ? "+" : "";
		str += "<b>Initiative:</b> " + sign + initiativeModifier + "\n";
		str += "<b>Speed:</b> " + movementSpeed[0] + "ft.";
		for(int i=1;i<movementSpeed.Length;i++){
			if(movementSpeed[i] > 0){
				str += movementTypes[i] + movementSpeed[i] + "ft.";
			}
		}
		str += "\n";
		str += "<b>AC:</b> " + armorClass + "\n";
		str += "<b>Attacks:</b> ";
		if(attacks.Count == 0){
			str += "None\n";
		}else{
			str += attacks[0].ToString();
			for(int i=1;i<attacks.Count;i++){
				str += ", " + attacks[i].ToString();
			}
			str += "\n";
		}

		if(specialAttacks.Count > 0){
			str += "<b>Special Attacks:</b> " + specialAttacks[0];
			for(int i=1;i<specialAttacks.Count;i++){
				str += ", " + specialAttacks[i];
			}
			str += "\n";
		}

		if(specialQualities.Count > 0){
			str += "<b>Special Qualities:</b> " + specialQualities[0];
			for(int i=1;i<specialQualities.Count;i++){
				str += ", " + specialQualities[i];
			}
			str += "\n";
		}
		sign = savingThrows[0] >= 0 ? "+" : "";
		str += "<b>Saves:</b> " + gameSystem.savingThrows[0] + " " + sign + savingThrows[0];
		for(int i=1;i<savingThrows.Length;i++){
			sign = savingThrows[i] >= 0 ? "+" : "";
			str += ", " + gameSystem.savingThrows[i] + " " + sign + savingThrows[i];
		}
		str += "\n";
		str += "<b>Abilities:</b> " + gameSystem.attributeAbbreviations[0] + " " + attributes[0];
		for(int i=1;i<attributes.Length;i++){
			str += ", " + gameSystem.attributeAbbreviations[i] + " " + attributes[i];
		}
		str += "\n";

		str += "<b>Skills:</b> ";
		if(skills.Count == 0){
			str += "None\n";
		}else{
			str += attacks[0].ToString();
			for(int i=1;i<attacks.Count;i++){
				str += ", " + attacks[i].ToString();
			}
			str += "\n";
		}

		str += "<b>Feats:</b> ";
		if(feats.Count == 0){
			str += "None\n";
		}else{
			str += gameSystem.feats[feats[0]];
			for(int i=1;i<feats.Count;i++){
				str += ", " + gameSystem.feats[feats[i]];
			}
			str += "\n";
		}
		

		return str;
	}
}