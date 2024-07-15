using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public enum SelectionBehavior {None, Unique, Multiple}

public class MenuSection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{

	public Menu menu;
	public int sectionIndex;
	public bool isGrid;
	public int width = 1;
	public int height = 1;
	public bool isVertical;
	public SelectionBehavior selectionBehavior;
	public List<MenuComponent> components;
	public bool isInitialized = false;
	public bool isHovered = false;
	public MenuComponent activeComponent{
		get {
			if(menu.activeSection == this){
				if(menu.activeComponent != null){
					return menu.activeComponent;
				}else{
					menu.activeComponent = components[0];
					return menu.activeComponent;
				}
			}else{
				return null;
			}
		}
		set {
			menu.activeComponent = value;
		}
	}
	public MenuComponent selectedComponent;

	public virtual void Init(){
		if(!isInitialized){
			MenuComponent[] results;
			results = GetComponentsInChildren<MenuComponent>();
			for(int i=0;i<results.Length;i++){
				components.Add(results[i]);
			}
			ResetIndices();
			isInitialized = true;
		}
	}

	void Update(){
		if(menu.isActiveMenu){
			if(menu.activeSection == this){
				if(isGrid){
					if(height > 1){
						if(MenuControl.arrowPressed[1]){
							NavigateGrid(false,false);
						}else if(MenuControl.arrowPressed[3]){
							NavigateGrid(true,false);
						}
					}
					if(width > 1){
						if(MenuControl.arrowPressed[2]){
							NavigateGrid(false,true);
						}else if(MenuControl.arrowPressed[0]){
							NavigateGrid(true,true);
						}
					}
				}else{
					if(height > 1){
						if(MenuControl.arrowPressed[1]){
							DecrementActiveIndex();
						}else if(MenuControl.arrowPressed[3]){
							IncrementActiveIndex();
						}
					}
					if(width > 1){
						if(MenuControl.arrowPressed[2]){
							DecrementActiveIndex();
						}else if(MenuControl.arrowPressed[0]){
							IncrementActiveIndex();
						}
					}
				}
			}
		}
	}

	public void NavigateGrid(bool ascending, bool horizontal){
		int x;
		int y;
		int newIndex;
		int startIndex;
		if(activeComponent != null && activeComponent.gameObject.activeSelf){
			startIndex = activeComponent.index;
			x = startIndex % width;
			y = startIndex / width;
			do {
				if(horizontal){
					if(ascending){
						x += 1;
						if(x == width){
							x = 0;
						}
					}else{
						x -= 1;
						if(x == -1){
							x = width-1;
						}
					}
					newIndex = y*width + x;
					activeComponent = components[newIndex];
				}else{
					if(ascending){
						y += 1;
						if(y == height){
							y = 0;
						}
					}else{
						y -= 1;
						if(y == -1){
							y = height-1;
						}
					}
					newIndex = y*width + x;
					activeComponent = components[newIndex];
				}
			} while (!ActiveComponentIsValid() && newIndex != startIndex);
		}
	}

	public void IncrementActiveIndex(){
		//int output = 0;
		if(activeComponent != null && activeComponent.gameObject.activeSelf){
			ActivateNextValidComponent(true);
		}
		//activeComponent = components[output];
	}

	public void DecrementActiveIndex(){
		//int output = 0;
		if(activeComponent != null && activeComponent.gameObject.activeSelf){
			ActivateNextValidComponent(false);
		}
		//activeComponent = components[output];
	}

	public void ResetIndices(){
		for(int i=0; i<components.Count; i++){
			components[i].index = i;
			components[i].menu = menu;
			components[i].section = this;
		}
	}

	public bool ActiveComponentIsValid(){
		return activeComponent != null && activeComponent.gameObject.activeSelf && !activeComponent.isEmpty;
	}

	void ActivateNextValidComponent(bool ascending){
		int startIndex = activeComponent.index;
		int index = startIndex;
		do {
			if(ascending){
				index += 1;
				if(index == components.Count){
					index = 0;
				}
			}else{
				index -= 1;
				if(index < 0){
					index = components.Count -1;
				}
			}
			activeComponent = components[index];
		} while (!ActiveComponentIsValid() && index != startIndex);
	}

	public void OnPointerEnter(PointerEventData e){
		isHovered = true;
		//OnHover();
	}

	public void OnPointerExit(PointerEventData e){
		isHovered = false;
		//OnStopHover();
	}

	public virtual void OnActivate(){

	}

	public virtual void OnDeactivate(){
		for(int i=0;i<components.Count;i++){
			components[i].ClearState();
		}
	}
}