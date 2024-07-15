using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileUtilities {
	public static string userPath = Application.dataPath + "/user_data";
	public static string systemPath = Application.dataPath + "/system_data";

	public static bool IsValidPath(string path){
		return Directory.Exists(path);
	}

	public static bool IsDirectory(string path){
		bool isDir = false;
		if(IsValidPath(path)){
			FileAttributes fa = File.GetAttributes(path);
			if(fa.HasFlag(FileAttributes.Directory)){
				isDir = true;
			}
		}
		return isDir;
	}

	public static string GetParent(string path){
		DirectoryInfo dir = Directory.GetParent(path);
		if(dir != null){
			return dir.FullName;
		}else{
			return null;
		}
	}

	public static List<string> GetFilesInDirectory(string path, string filter="*.*"){
		List<string> output = new List<string>();
		DirectoryInfo dir = new DirectoryInfo(path);
		FileInfo[] info = dir.GetFiles(filter);
		for(int i=0;i<info.Length;i++){
			output.Add(info[i].Name);
		}
		return output;
	}

	public static List<string> GetSubdirectoriesAtPath(string path){
		List<string> output = new List<string>();
		DirectoryInfo dir = new DirectoryInfo(path);
		DirectoryInfo[] info = dir.GetDirectories();
		for(int i=0;i<info.Length;i++){
			output.Add(info[i].Name);
		}
		return output;
	}

	public static void ValidatePaths(){
			string path;
			path = Application.dataPath;
			if(Directory.Exists(path)){
				path = userPath;
				if(!Directory.Exists(path)){
					Directory.CreateDirectory(path);
				}
				path = Path.Combine(userPath, "Dungeons");
				if(!Directory.Exists(path)){
					Directory.CreateDirectory(path);
				}
				path = Path.Combine(userPath, "Tilesets");
				if(!Directory.Exists(path)){
					Directory.CreateDirectory(path);
				}
				path = systemPath;
				if(!Directory.Exists(systemPath)){
					Directory.CreateDirectory(systemPath);
				}
			}else{
				Debug.Log("Root Path Not Found!!!");
			}
		}

	public static Texture2D LoadImageFromFile(string path){
		if(!File.Exists(path)){return null;}
		var rawData = System.IO.File.ReadAllBytes(path);
		Texture2D tex = new Texture2D(2,2);
		tex.LoadImage(rawData);
		return tex;
	}


	public static void WriteDungeonToFile(Dungeon dng){
		ValidatePaths();
		string path;
		path = Path.Combine(userPath, "Dungeons", dng.fileName);
		if(!Directory.Exists(path)){
			Directory.CreateDirectory(path);
		}
		path = Path.Combine(userPath, "Dungeons", dng.fileName, "Illustrations");
		if(!Directory.Exists(path)){
			Directory.CreateDirectory(path);
		}
		path = Path.Combine(userPath, "Dungeons", dng.fileName, "Icons");
		if(!Directory.Exists(path)){
			Directory.CreateDirectory(path);
		}
		path = Path.Combine(userPath, "Dungeons", dng.fileName, dng.fileName + ".dng");
		if(File.Exists(path)){
			File.Delete(path);
		}
		List<string> lines = dng.ToJSON(0);
		using (StreamWriter sw = new StreamWriter(path)){
			for(int i=0;i<lines.Count;i++){
				sw.WriteLine(lines[i]);
			}
		}
		WriteDungeonImagesToFiles(dng);
	}

	public static void WriteDungeonImagesToFiles(Dungeon dng){
		string basePath = Path.Combine(userPath, "Dungeons", dng.fileName);
		for(int i=0;i<dng.monsters.Count;i++){
			if(dng.monsters[i].illustration != null && dng.monsters[i].illustration.texture != null){
				WriteTextureToPNG(dng.monsters[i].illustration.texture, Path.Combine(basePath, "Illustrations"), dng.monsters[i].illustrationFileName);	
			}
			if(dng.monsters[i].icon != null && dng.monsters[i].icon.texture != null){
				WriteTextureToPNG(dng.monsters[i].icon.texture, Path.Combine(basePath, "Icons"), dng.monsters[i].illustrationFileName);	
			}
		}

		for(int i=0;i<dng.artifacts.Count;i++){
			if(dng.artifacts[i].illustration != null && dng.artifacts[i].illustration.texture != null){
				WriteTextureToPNG(dng.artifacts[i].illustration.texture, Path.Combine(basePath, "Illustrations"), dng.artifacts[i].illustrationFileName);	
			}
		}

		for(int i=0;i<dng.traps.Count;i++){
			if(dng.traps[i].illustration != null && dng.traps[i].illustration.texture != null){
				WriteTextureToPNG(dng.traps[i].illustration.texture, Path.Combine(basePath, "Illustrations"), dng.traps[i].illustrationFileName);	
			}
		}

		for(int i=0;i<dng.dungeonFeatures.Count;i++){
			if(dng.dungeonFeatures[i].illustration != null && dng.dungeonFeatures[i].illustration.texture != null){
				WriteTextureToPNG(dng.dungeonFeatures[i].illustration.texture, Path.Combine(basePath, "Illustrations"), dng.dungeonFeatures[i].illustrationFileName);	
			}
			if(dng.dungeonFeatures[i].icon != null && dng.dungeonFeatures[i].icon.texture != null){
				WriteTextureToPNG(dng.dungeonFeatures[i].icon.texture, Path.Combine(basePath, "Icons"), dng.dungeonFeatures[i].illustrationFileName);	
			}
		}
	}

	public static string WrapString(string str){
		string output = "";
		if(str == null){return "\"\"";}
		for(int i=0;i<str.Length;i++){
			if(RequiresEscapeCharacter(str[i])){
				output += '\\';
			}
			output += str[i];
		}
		output = "\"" + output + "\"";
		return output;
	}

	public static bool RequiresEscapeCharacter(char c){
		if(c == '\''){return true;}
		if(c == '\"'){return true;}
		if(c == '\\'){return true;}
		return false;
	}

	public static void WriteTextureToPNG(Texture2D tex, string directory, string filename){
		byte[] bytes = ImageConversion.EncodeToPNG(tex);
		if(Directory.Exists(directory)){
			string path = Path.Combine(directory, filename + ".png");
			if(File.Exists(path)){
				File.Delete(path);
			}
			using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write)){
				fs.Write(bytes, 0, bytes.Length);
			}
		}
	}

	public static string Indentation(int indentationAmount){
		string output = "";
		string tab = "	";
		for(int i=0;i<indentationAmount;i++){
			output += tab;
		}
		return output;
	}

	public static List<JSONObject>LoadDungeonFromJSON(string filename){
		List<JSONObject> output = new List<JSONObject>();
		ValidatePaths();
		string directory = Path.Combine(userPath, "Dungeons", filename);
		if(Directory.Exists(directory)){
			string path = Path.Combine(directory, filename + ".dng");
			if(File.Exists(path)){
				string str;
				using (StreamReader streamReader = new StreamReader(path)){
					str = streamReader.ReadToEnd();
				}
				output = StringToTokens(str);
			}else{
				Debug.Log("No such file!");
			}
		}else{
			Debug.Log("Directory Invalid!");
		}
		return output;
	}

	public static List<JSONObject> StringToTokens(string str){
		List<JSONObject> tokens = new List<JSONObject>();
		JSONObject obj;
		char c;
		string fieldName = "";
		string buffer = "";
		bool delimitByDoubleQuote = false;
		bool delimitBySingleQuote = false;
		bool readingToken = false;
		bool escapeCharacter = false;
		for(int i=0;i<str.Length;i++){
			c = str[i];

			if(!readingToken){
				if(!Char.IsWhiteSpace(c)){
					if(c == '{'){
						if(fieldName != ""){
							obj = new JSONObject(false, false);
							obj.objectObjects.Add(fieldName, ParseObject(ref str, ref i));
							tokens.Add(obj);
							fieldName = "";
							buffer = "";
						}else{
							tokens.Add(ParseObject(ref str, ref i));	
						}
					}else if(c == '['){
						if(fieldName != ""){
							obj = new JSONObject(false, true);
							obj.arrayObjects.Add(ParseObject(ref str, ref i));
							tokens.Add(obj);
							fieldName = "";
							buffer = "";
						}else{
							tokens.Add(ParseArray(ref str, ref i));	
						}
					}else if(c == ':'){
						if(!(fieldName != "")){
							readingToken = true;
						}
					}else if(c == '"'){
						delimitByDoubleQuote = true;
						readingToken = true;
						buffer = "";
					}else if(c == '\''){
						delimitBySingleQuote = true;
						readingToken = true;
						buffer = "";
					}else{
						readingToken = true;
						buffer = "" + c;
					}
				}
			}else{
				if( (delimitByDoubleQuote && c == '\"' && !escapeCharacter) || (delimitBySingleQuote && c == '\'' && !escapeCharacter) || (!delimitByDoubleQuote && !delimitBySingleQuote && Char.IsWhiteSpace(c)) ){
					readingToken = false;
					escapeCharacter = false;
					if(!(fieldName != "")){
						fieldName = buffer;
						buffer = "";
					}else{
						obj = new JSONObject(true, false);
						obj.fieldName = fieldName;
						obj.fieldValue = buffer;
						tokens.Add(obj);
						fieldName = "";
						buffer = "";
					}
				}else{
					if(c == '\\' && !escapeCharacter){
						escapeCharacter = true;
					}else{
						escapeCharacter = false;
						buffer += c;
					}

				}
			}

		}

		return tokens;
	}

	/*
	How this works: we read in the name of the field before we know if its an object, an array, or a field
		so: start by reading in characters until we find non-whitespace;
		if its a quote, single quote, or non-special character, we start reading in a new token
		we continue until we get to the end of the token (ie, we find the delimiter)
		we then dump the buffer into OBJ and look for the : 
		after we find the :, we start looking for the value, which will be either a string, an object, or an array


		pseudocode algorithm: 
		while not reading-field
			if non-whitespace
				if object -> parse object
				if array -> parse array
				else reading-field = true
					set delimiter
		while is reading-field
			if delimiter found
				break
			else
				add to buffer and continue

	*/

	public static JSONObject ParseObject(ref string str, ref int index){
		JSONObject output = new JSONObject(false, false);
		char c;
		string fieldName = "";
		JSONObject fieldValue;
		index += 1;
		for (; index<str.Length; index++){
			c = str[index];
			if(fieldName == ""){
				fieldName = GetFieldName(ref str, ref index);
				if(fieldName == "}"){
					break;
				}
			}else{
				if(c == '{'){
					output.objectObjects.Add(fieldName, ParseObject(ref str, ref index));
					fieldName = "";
				}else if(c == '['){
					output.objectObjects.Add(fieldName, ParseArray(ref str, ref index));
					fieldName = "";
				}else if(c == '}'){
					//index += 1;
					break;
				}else if(Char.IsWhiteSpace(c) || c == ':' ){
					//do nothing
				}else{
					if(c == '\"'){
						index += 1;
						fieldValue = ParseFieldValue(ref str, ref index, '\"');
					}else if(c == '\''){
						index += 1;
						fieldValue = ParseFieldValue(ref str, ref index, '\'');
					}else{
						fieldValue = ParseFieldValue(ref str, ref index, ' ');
					}
					output.objectObjects.Add(fieldName, fieldValue);
					fieldName = "";
				}
			}
		}

		return output;
	}

	public static JSONObject ParseArray(ref string str, ref int index){
		JSONObject output = new JSONObject(false, true);
		char c;
		JSONObject fieldValue;
		index+=1;
		for (; index<str.Length; index++){
			c = str[index];
			if(c == '{'){
				output.arrayObjects.Add(ParseObject(ref str, ref index));
			}else if(c == '['){
				output.arrayObjects.Add(ParseArray(ref str, ref index));
			}else if(c == ']'){
				index += 1;
				break;
			}else if(Char.IsWhiteSpace(c) || c == ',' ){
				//do nothing
			}else{
				if(c == '\"'){
					//index += 1;
					fieldValue = ParseFieldValue(ref str, ref index, '\"');
				}else if(c == '\''){
					index += 1;
					fieldValue = ParseFieldValue(ref str, ref index, '\'');
				}else{
					fieldValue = ParseFieldValue(ref str, ref index, ' ');
				}
				output.arrayObjects.Add(fieldValue);
			}
		}
		return output;
	}

	public static string GetFieldName(ref string str, ref int index){
		char c;
		string output = "";
		for (int i = index; index<str.Length; index++){
			c = str[index];
			if(!Char.IsWhiteSpace(c)){
				if(c != ','){
					if(c == '\"'){
						index += 1;
						output = GetNextToken(ref str, ref index, '\"');
					}else if(c == '\''){
						index += 1;
						output = GetNextToken(ref str, ref index, '\'');
					}else if(c == '}'){
						output = "}";
					}else if(c == ']'){
						output = "]";
					}else{
						output = GetNextToken(ref str, ref index, ' ');
					}
					break;
				}
			}
		}
		return output;
	}

	public static string GetNextToken(ref string str, ref int index, char delimiter){
		string output = "";
		char c;
		bool escapeCharacter = false;
		bool delimitByDoubleQuote = delimiter == '\"';
		bool delimitBySingleQuote = delimiter == '\'';
		bool delimitByWhiteSpace = delimiter == ' ';
		for (int i = index; index<str.Length; index++){
			c = str[index];
			if( (delimitByDoubleQuote && c == '\"' && !escapeCharacter) || (delimitBySingleQuote && c == '\'' && !escapeCharacter) || (delimitByWhiteSpace && Char.IsWhiteSpace(c)) ){
				break;
			}else if(c == '\\' && !escapeCharacter){
				escapeCharacter = true;
			}else{
				escapeCharacter = false;
				output += c;
			}
		}
		return output;
	}

	public static JSONObject ParseFieldValue(ref string str, ref int index, char delimiter){
		JSONObject output = new JSONObject(true, false);
		string buffer = "";
		char c;
		bool escapeCharacter = false;
		bool delimitByDoubleQuote = delimiter == '\"';
		bool delimitBySingleQuote = delimiter == '\'';
		bool delimitByWhiteSpace = delimiter == ' ';
		for (int i = index; index<str.Length; index++){
			c = str[index];
			if( (delimitByDoubleQuote && c == '\"' && !escapeCharacter) || (delimitBySingleQuote && c == '\'' && !escapeCharacter) || (delimitByWhiteSpace && Char.IsWhiteSpace(c) || delimitByWhiteSpace && c == ',') ){
				output.fieldValue = buffer;
				break;
			}else if(c == '\\' && !escapeCharacter){
				escapeCharacter = true;
			}else{
				escapeCharacter = false;
				buffer += c;
			}
		}
		return output;
	}
}

public class JSONObject {
	public bool isArray = false;
	public bool isField = false;
	public Dictionary<string,JSONObject> objectObjects;
	public List<JSONObject> arrayObjects;
	public string fieldName;
	public string fieldValue;

	public JSONObject(bool field, bool array){
		
		if(field){
			isField = field;
		}else if(array){
			isArray = array;
			arrayObjects = new List<JSONObject>();
		}else{
			objectObjects = new Dictionary<string,JSONObject>();
		}
	}
}
