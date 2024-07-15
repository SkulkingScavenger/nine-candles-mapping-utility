using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MenuSlotHolder : MenuSection{

	public int currentIndex = 0;
	public int selectedIndex = -1;
	public List<MenuSlot> slots;
	public List<string> values;
	public SingleIntegerEvent onSlotDeleteButton;
	public SingleIntegerEvent onSlotEditButton;
	public UnityEvent onAddSlotButton;
	public DiscreteScrollbar scrollbar;

	public int slotHeight = 16;
	public int slotWidth = 192;

	public bool canSelect = false;
	public bool canEdit = true;
	public bool canDelete = true;
	public bool canAddNew = true;
	MenuButton addSlotButton;

	public void InitialSetup(){

		addSlotButton = transform.Find("AddSlotButton").GetComponent<MenuButton>();
		addSlotButton.onClick.AddListener(delegate {onAddSlotButton.Invoke();});
		addSlotButton.gameObject.SetActive(canAddNew);

		addSlotButton.defaultColor = new Color(0f,1f,0f,1f);

		slots = new List<MenuSlot>();

		int maxSlotCount = (int)(GetComponent<RectTransform>().rect.height / slotHeight);

		GameObject go;
		MenuSlot ms;
		float x,y;
		for(int i=0;i<maxSlotCount;i++){
			go = PoolControl.WithdrawMenuSlot();
			ms = go.GetComponent<MenuSlot>();
			ms.holder = this;
			ms.section = this;
			ms.menu = menu;
			ms.slotIndex = i;
			ms.canEdit = canEdit;
			ms.canDelete = canDelete;
			ms.InitialSetup();
			ms.SetSize(slotWidth, slotHeight);
			if(canSelect){
				int temp = i;
				ms.onClick.AddListener(delegate {SelectSlot(temp);});
			}
			go.transform.SetParent(transform);
			x = 0;
			y = -i*slotHeight + maxSlotCount*slotHeight/2 - slotHeight/2;
			go.transform.localPosition = new Vector3(x,y,0);
			
			slots.Add(ms);
		}
		RefreshList();

		scrollbar = transform.Find("DiscreteScrollbar").GetComponent<DiscreteScrollbar>();
		scrollbar.menu = menu;
		scrollbar.section = this;
		scrollbar.Init();
		scrollbar.onValueChanged.AddListener(delegate {OnScroll();} );
		//scrollbar.SetScrollbar(currentIndex, 0, maxSlotCount);
	}

	void Update(){
		if(menu.isActiveMenu){
			if(canSelect){

			}
		}
	}

	public void SetList(List<string> strs){
		values = strs;
		scrollbar.SetScrollbar(currentIndex, values.Count, slots.Count);
		RefreshList();
	}

	void RefreshList(){
		for(int i=0;i<slots.Count;i++){
			if(i + currentIndex < values.Count){
				slots[i].isEmpty = false;
				slots[i].text = values[i + currentIndex];
				if(canSelect){
					slots[i].isSelected = i+currentIndex == selectedIndex;
				}
			}else{
				slots[i].isEmpty = true;
				slots[i].text = "";
			}
			slots[i].gameObject.SetActive(!slots[i].isEmpty);
		}
	}

	public void SlotButtonDelete(int slotIndex){
		onSlotDeleteButton.Invoke(slotIndex + currentIndex);
		if(slotIndex + currentIndex == selectedIndex){
			selectedIndex = -1;
		}else if(slotIndex > slotIndex + currentIndex){
			selectedIndex -= 1;
		}
	}

	public void SlotButtonEdit(int slotIndex){
		onSlotEditButton.Invoke(slotIndex + currentIndex);
	}

	public void SelectSlot(int slotIndex){
		if(slotIndex + currentIndex < values.Count && slotIndex + currentIndex != selectedIndex){
			selectedIndex = slotIndex + currentIndex;
			RefreshList();
		}
	}

	void OnScroll(){
		currentIndex = scrollbar.value; 
		RefreshList();
	}
}
