using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorNavigationSlot : MenuButton{

	Image img;
	public FloorNavigationPanel panel;

	void Awake(){
		textElement = transform.Find("Label").GetComponent<Text>();
		img = GetComponent<Image>();
		disabledColor = new Color(0.15f,0.12f,0.08f,1f);
		activeColor = new Color(0.92f,0.61f,0.27f,1f);
		hoveredColor = new Color(0.75f,0.50f,0.22f,1f);
		defaultColor = new Color(0.49f,0.32f,0.14f,1f);
	}


	public override void UpdateActive(){
		if(isEmpty){
			img.color = disabledColor;
			
		}else if(index + panel.currentIndex == panel.map.floorIndex){
			img.color = activeColor;
			textElement.color = new Color(0f,0f,0f,1f);
		}else if(isHovered){
			img.color = hoveredColor;
			textElement.color = new Color(1f,1f,1f,1f);
		}else{
			img.color = defaultColor;
			textElement.color = new Color(1f,1f,1f,1f);
		}
	}

	public override void OnClick(){
		if(!isEmpty && index + panel.currentIndex != panel.map.floorIndex){
			panel.SetFloorIndex(index + panel.currentIndex);
		}
	}
}
