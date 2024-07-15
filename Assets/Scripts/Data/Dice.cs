using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Dice{
	public static int[] types = {3,4,6,8,10,12,20,100};

	public int diceCount;
	public int faceCount;
	public int modifier;

	public Dice(int count, int faces, int mod){
		diceCount = count;
		faceCount = faces;
		modifier = mod;
	}

	public override string ToString(){
		string mod = "";
		if(modifier < 0){mod = "" + modifier;}
		if(modifier > 0){mod = "+" + modifier;}
		return diceCount + "d" + faceCount + mod;
	}

	public int Roll(){
		int result = 0;
		for(int i=0;i<diceCount;i++){
			result += UnityEngine.Random.Range(0, faceCount) + 1;
		}
		return result + modifier;
	}

	public int Average(){
		return (diceCount*(faceCount+1))/2 + modifier;
	}

	public List<string> ToJSON(int indentAmount){
		List<string> output = new List<string>();
		output.Add(FileUtilities.Indentation(indentAmount) + "{");
		indentAmount += 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "\"diceCount\" : " + diceCount.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"faceCount\" : " + faceCount.ToString() + ",");
		output.Add(FileUtilities.Indentation(indentAmount) + "\"modifier\" : " + modifier.ToString());
		indentAmount -= 1;
		output.Add(FileUtilities.Indentation(indentAmount) + "}");
		return output;
	}
}
