using System;
using System.Collections;
using System.Collections.Generic;

public class GameSystem{
	public string name;
	public List<string> feats;
	public List<string> skills;
	public List<string> attributes;
	public List<string> attributeAbbreviations;
	public List<string> savingThrows;
	public List<SystemField> monsterTraits;
	public List<MonsterType> monsterTypes;
	public string currency;
}


public class SystemField{
	public enum FieldType {Discrete, Numeric, Text};
	public string name;
	public FieldType type;
	public int value;
	public List<string> options;

	public SystemField(){
		options = new List<string>();
	}
}