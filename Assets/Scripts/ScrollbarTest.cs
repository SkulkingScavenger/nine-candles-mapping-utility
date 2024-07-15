using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;



public class ScrollbarTest : MenuSection{
	public List<Text> slots;
	public int currentIndex = 0;
	public int itemCount;
	public int slotCount {
		get {return slots.Count;}
		set {}
	}

	public DiscreteScrollbar scrollbar;

	public void InitialSetup(){
		transform.Find("UpButton").GetComponent<MenuButton>().onClick.AddListener(delegate {UP();});
		transform.Find("DownButton").GetComponent<MenuButton>().onClick.AddListener(delegate {DOWN();});

		scrollbar.menu = menu;
		scrollbar.section = this;
		scrollbar.Init();
		scrollbar.SetScrollbar(currentIndex, itemCount, slotCount);
		scrollbar.onValueChanged.AddListener(delegate {currentIndex = scrollbar.value;} );
	}

	void Update(){
		for(int i=0;i<slotCount;i++){
			slots[i].text = "Slot # " + (currentIndex + i);
		}
	}

	void UP(){
		if(currentIndex > 0){
			currentIndex -= 1;
			scrollbar.SetScrollIndex(currentIndex);
		}
	}

	void DOWN(){
		if(currentIndex < itemCount - slotCount){
			currentIndex += 1;
			scrollbar.SetScrollIndex(currentIndex);
		}
	}

}