using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DungeonContent {
	public uint systemID = 0;
	public abstract List<string> ToJSON(int indentAmount);
}

public class Artifact{
	public uint systemID = 0;
	public string name;
	public string description;
	public int baseMarketPrice; //-1 denotes N/A
	public Sprite illustration;
	public string illustrationFileName {
		get {return "artifact_" + systemID;}
		set {}
	}

	public void CopyValuesFrom(Artifact a){
		name = a.name;
		description = a.description;
		baseMarketPrice = a.baseMarketPrice;
		illustration = a.illustration;
	}

	public List<string> ToJSON(int indentAmount){
		List<string> output = new List<string>();
		output.Add(FileUtilities.Indentation(indentAmount) + "{");
		indentAmount += 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "\"systemID\" : " + systemID.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"name\" : " + FileUtilities.WrapString(name) + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"description\" : " + FileUtilities.WrapString(description) + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"baseMarketPrice\" : " + baseMarketPrice.ToString() + ",");
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "}");
		return output;
	}
}

public class Treasure {
	public uint systemID = 0;
	public string name;
	public string description;

	// public List<CurrencyAmount> guaranteedCurrency;
	// public List<Artifact> guaranteedItems;
	// public List<CurrencyAmount> randomCurrency;
	// public List<RandomTable> randomTables;

	public List<string> ToJSON(int indentAmount){
		List<string> output = new List<string>();
		output.Add(FileUtilities.Indentation(indentAmount) + "{");
		indentAmount += 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "\"systemID\" : " + systemID.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"name\" : " + FileUtilities.WrapString(name) + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"description\" : " + FileUtilities.WrapString(description) + ",");
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "}");
		return output;
	}

	public void CopyValuesFrom(Treasure t){
		name = t.name;
		description = t.description;

		// CurrencyAmount ca;
		// Artifact a;
		// RandomTable rt;

		// guaranteedCurrency.Clear();
		// guaranteedItems.Clear();
		// randomCurrency.Clear();
		// randomTables.Clear();

		// for(int i=0;i<t.guaranteedCurrency.Count;i++){
		// 	ca = new CurrencyAmount();
		// 	ca.CopyValuesFrom(t.guaranteedCurrency[i]);
		// 	guaranteedCurrency.Add(ca);
		// }

		// for(int i=0;i<t.guaranteedItems.Count;i++){
		// 	a = new Artifact();
		// 	a.CopyValuesFrom(t.guaranteedItems[i]);
		// 	guaranteedItems.Add(a);
		// }

		// for(int i=0;i<t.randomCurrency.Count;i++){
		// 	ca = new CurrencyAmount();
		// 	ca.CopyValuesFrom(t.randomCurrency[i]);
		// 	randomCurrency.Add(ca);
		// }

		// for(int i=0;i<t.randomTables.Count;i++){
		// 	rt = new RandomTable();
		// 	rt.CopyValuesFrom(t.randomTables[i]);
		// 	randomTables.Add(rt);
		// }
	}
}

public class CurrencyAmount {
	public string name = "gold coins";
	public int guaranteedQuantity = 0;
	public Dice randomQuantity;

	public void CopyValuesFrom(CurrencyAmount ca){
		name = ca.name;
		guaranteedQuantity = ca.guaranteedQuantity;
		randomQuantity = ca.randomQuantity;
	}
}

public class Door {
	public Trap trap;
	public bool hasTrap {
		get { return trap != null; }
		set {}
	}
	public bool isLocked = false;
	public bool isStuck = false;
	public int hitpoints = 0;
	public int hardness = 0;
}

public class Trap {
	public uint systemID = 0;
	public string name;
	public int searchDC;
	public int disableDC;
	public string description;
	public Sprite illustration;
	public string illustrationFileName {
		get {return "trap_" + systemID;}
		set {}
	}

	public void CopyValuesFrom(Trap t){
		name = t.name;
		searchDC = t.searchDC;
		disableDC = t.disableDC;
		description = t.description;
		illustration = t.illustration;
	}

	public List<string> ToJSON(int indentAmount){
		List<string> output = new List<string>();
		output.Add(FileUtilities.Indentation(indentAmount) + "{");
		indentAmount += 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "\"systemID\" : " + systemID.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"name\" : " + FileUtilities.WrapString(name) + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"searchDC\" : " + searchDC.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"disableDC\" : " + disableDC.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"description\" : " + FileUtilities.WrapString(description) + ",");
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "}");
		return output;
	}
}


public class DungeonFeature {
	public uint systemID = 0;
	public string name;
	public string description;
	public Sprite icon;
	public Sprite illustration;
	public string illustrationFileName {
		get {return "feature_" + systemID;}
		set {}
	}
	public string iconFileName {
		get {return "feature_" + systemID;}
		set {}
	}

	public void CopyValuesFrom(DungeonFeature df){
		name = df.name;
		description = df.description;
		icon = df.icon;
		illustration = df.illustration;
	}

	public List<string> ToJSON(int indentAmount){
		List<string> output = new List<string>();
		output.Add(FileUtilities.Indentation(indentAmount) + "{");
		indentAmount += 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "\"systemID\" : " + systemID.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"name\" : " + FileUtilities.WrapString(name) + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"description\" : " + FileUtilities.WrapString(description) + ",");
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "}");
		return output;
	}
}


public class RandomTable {
	public static string[] types = {
		"Wandering Monster",
		"Random Trap",
		"Random Treasure",
		"Random Artifact",
		"Miscellaneous"
	};

	public uint systemID = 0;
	public string name = "Random Table";
	public int type = 0;
	public int range = 12;
	public List<RandomTableEntry> entries;

	public RandomTable (){
		range = 10;
		entries = new List<RandomTableEntry>();
	}

	public void CopyValuesFrom(RandomTable rt){
		name = rt.name;
		type = rt.type;
		range = rt.range;
		RandomTableEntry rte;
		entries.Clear();
		for(int i=0;i<rt.entries.Count;i++){
			rte = new RandomTableEntry();
			rte.CopyValuesFrom(rt.entries[i]);
			entries.Add(rte);
		}
	}

	public void SwapEntries(int a, int b){
		RandomTableEntry temp = entries[a];
		entries[a] = entries[b];
		entries[b] = temp;
	}

	public int TotalResults(){
		int width = 0;
		for(int i=0;i<entries.Count;i++){
			if(!entries[i].isEmpty){
				width += entries[i].width;	
			}
		}
		return width;
	}

	public List<string> GetEntriesAsStrings(){
		List<string> output = new List<string>();
		for(int i=0;i<entries.Count;i++){
			if(!entries[i].isEmpty){
				output.Add(entries[i].entry);
			}
		}
		return output;
	}

	public List<string> GetRangesAsStrings(){
		List<string> output = new List<string>();
		int counter = 1;
		if(range == 100){counter = 0;}
		string str;
		for(int i=0;i<entries.Count;i++){
			if(!entries[i].isEmpty){
				if(entries[i].width == 1){
					str = counter.ToString();
					counter += 1;
				}else{
					str = counter + "-" + (counter+entries[i].width-1);
					counter += entries[i].width;
				}
				output.Add(str);
			}
		}
		return output;
	}

	public List<string> ToJSON(int indentAmount){
		List<string> output = new List<string>();
		output.Add(FileUtilities.Indentation(indentAmount) + "{");
		indentAmount += 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "\"systemID\" : " + systemID.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"name\" : " + FileUtilities.WrapString(name) + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"type\" : " + type.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"range\" : " + range.ToString() + ",");

		output.Add(FileUtilities.Indentation(indentAmount) + "\"entries\" : [");
		indentAmount += 1;
		for(int i=0;i<entries.Count;i++){
			output.AddRange(entries[i].ToJSON(indentAmount));
			if(i != entries.Count -1){
				output[output.Count - 1] += ",";
			}
		}
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "]");

		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "}");
		return output;
	}
}

public class RandomTableEntry {
	public int width = 1;
	public string entry = "";
	public bool isEmpty {
		get { return entry == "" || width == 0;}
		set { if(value){entry = ""; width = 0;} }
	}

	public RandomTableEntry (int w=1, string str=""){
		width = w;
		entry = str;
	}

	public void CopyValuesFrom(RandomTableEntry rte){
		width = rte.width;
		entry = rte.entry;
	}

	public List<string> ToJSON(int indentAmount){
		List<string> output = new List<string>();
		output.Add(FileUtilities.Indentation(indentAmount) + "{");
		indentAmount += 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "\"entry\" : " + FileUtilities.WrapString(entry) + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"width\" : " + width.ToString() + ",");
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "}");
		return output;
	}
}