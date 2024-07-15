using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Submenu : Menu {

	public int submenuIndex;
	public override bool isActiveMenu{
		get {return root.isActiveMenu && root.activeSubmenuIndex == submenuIndex;}
		set {}
	}

	public override int heldSlotIndex {
		get { return root.heldSlotIndex; }
		set { root.heldSlotIndex = value; }
	}

	public override MenuSection heldSlotSource {
		get { return root.heldSlotSource; }
		set { root.heldSlotSource = value; }
	}

	public override UnderCursorDisplay underCursorDisplay {
		get { return root.underCursorDisplay; }
		set { root.underCursorDisplay = value; }
	}

	void Start(){

	}

	public void SubmenuStart(){
		if(!isInitialized){
			InitialSetup();
			isInitialized = true;	
		}

		for(int i=0;i<submenus.Length;i++){
			submenus[i].root = this;
			submenus[i].submenuIndex = i;
			submenus[i].SubmenuStart();
		}

		MenuSection[] results;
		results = GetComponentsInChildren<MenuSection>();
		for(int i=0;i<results.Length;i++){
			if(!results[i].isInitialized){
				results[i].menu = this;
				results[i].sectionIndex = i;
				results[i].Init();
				sections.Add(results[i]);
				results[i].isInitialized = true;
			}
		}
	}

	void Update(){

	}
}
