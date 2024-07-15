using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuSlot : MenuComponent{
	public MenuSlotHolder holder;
	RectTransform rt;
	Text textElement;
	Image background;
	public int slotIndex;
	public string text {
		get {return textElement.text;}
		set {textElement.text = value;}
	}
	MenuButton editButton;
	MenuButton deleteButton;

	public Color selectedColor = new Color (1f,0.1f,0f,1f);
	public Color defaultColor = new Color (1f,1f,1f,1f);
	public Color hoveredBackgroundColor = new Color (0.25f,0.2f,0.15f,1f);
	public Color defaultBackgroundColor = new Color (0f,0f,0f,1f);

	public bool canSelect {
		get { return holder.canSelect; }
		set {}
	}
	public bool canEdit = true;
	public bool canDelete = true;

	public void SetSize(int w, int h){
		rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
		rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
	}

	public void InitialSetup(){
		rt = GetComponent<RectTransform>();
		background = GetComponent<Image>();
		textElement = transform.Find("Text").GetComponent<Text>();
		editButton = transform.Find("EditButton").GetComponent<MenuButton>();
		deleteButton = transform.Find("DeleteButton").GetComponent<MenuButton>();

		editButton.menu = menu;
		editButton.section = section;
		deleteButton.menu = menu;
		deleteButton.section = section;

		editButton.onClick.AddListener(delegate {holder.SlotButtonEdit(slotIndex);});
		deleteButton.onClick.AddListener(delegate {holder.SlotButtonDelete(slotIndex);});
		editButton.gameObject.SetActive(canEdit);
		deleteButton.gameObject.SetActive(canDelete);
	}

	public override void UpdateActive(){
		if(canSelect){
			if(isSelected){
				textElement.color = selectedColor;
			}else{
				textElement.color = defaultColor;
			}
			if(isHovered){
				background.color = hoveredBackgroundColor;
			}else{
				background.color = defaultBackgroundColor;
			}
		}
	}
}
