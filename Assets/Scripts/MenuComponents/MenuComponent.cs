using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public abstract class MenuComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler{

	[HideInInspector] public UnityEvent onClick = new UnityEvent();
	[HideInInspector] public UnityEvent onRightClick = new UnityEvent();
	[HideInInspector] public UnityEvent onDoubleClick = new UnityEvent();
	[HideInInspector] public UnityEvent onHover = new UnityEvent();
	[HideInInspector] public UnityEvent onStopHover = new UnityEvent();
	[HideInInspector] public UnityEvent onActivate = new UnityEvent();
	[HideInInspector] public UnityEvent onDeactivate = new UnityEvent();
	[HideInInspector] public bool isHovered = false;
	[HideInInspector] public bool wasHovered = false;
	[HideInInspector] public bool isPressed = false;
	[HideInInspector] public bool isRightPressed = false;
	[HideInInspector] public bool isDisabled = false;
	[HideInInspector] public bool isEmpty = false;
	public Menu menu = null;
	public MenuSection section;
	public int index;
	[HideInInspector] public string tooltip = "";

	[HideInInspector] public float doubleClickSensitivity = 0.30f;
	[HideInInspector] public float doubleClickTimer = 0f;
	[HideInInspector] public bool isClicked = false;

	[HideInInspector] public bool wasActive = false;
	public bool isActive = false;
	// public bool isActive {
	// 	get {
	// 		return (menu != null && menu.activeComponent == this);
	// 	}
	// 	set {
	// 		if(value){
	// 			if(menu != null && menu.activeComponent != this){
	// 				menu.activeComponent = this;
	// 				menu.activeSection = section;
	// 			}
	// 		}else{
	// 			if(menu != null && menu.activeComponent == this){
	// 				menu.activeComponent = null;
	// 			}
	// 		}
	// 	}
	// }
	public bool isSelected = false;
	// public bool isSelected {
	// 	get {
	// 		return (section != null && section.selectedComponent == this);
	// 	}
	// 	set {
	// 		if(value){
	// 			if(section != null && section.selectedComponent != this){
	// 				section.selectedComponent = this;
	// 			}
	// 		}else{
	// 			if(section != null && section.selectedComponent == this){
	// 				section.selectedComponent = null;
	// 			}
	// 		}
	// 	}
	// }

	void OnDisable(){
		isHovered = false;
		isPressed = false;
	}

	void Update(){
		if(menu != null && menu.isActiveMenu){
			CheckDoubleClick();

			if(wasHovered != isHovered){
				if(wasHovered){
					OnStopHover();
				}else{
					OnHover();
				}
				wasHovered = isHovered;
			}

			if(wasActive != isActive){
				if(wasActive){
					OnDeactivate();
				}else{
					OnActivate();
				}
				wasActive = isActive;
			}
			UpdateActive();
		}
		UpdateBackground();
	}


	public void OnPointerEnter(PointerEventData e){
		isHovered = true;
		OnHover();
	}

	public void OnPointerExit(PointerEventData e){
		isHovered = false;
		isPressed = false;
		OnStopHover();
	}

	public void OnPointerDown(PointerEventData e){
		
		if(e.button == PointerEventData.InputButton.Right){
			isRightPressed = true;
		}else{
			isPressed = true;
		}
	}

	public void OnPointerUp(PointerEventData e){
		
		if(e.button == PointerEventData.InputButton.Right){
			if(isRightPressed){
				isRightPressed = false;
				OnRightClick();
			}
		}else{
			if(isPressed){
				isPressed = false;
				OnClick();
			}
		}
			
	}

	//Called by menu OnEnable
	public virtual void Reload(){

	}

	//Specific Behavior during Update() while Menu is Active
	public virtual void UpdateActive(){

	}

	//Specific Behavior during Update() regardless of whether Menu is Active or not
	public virtual void UpdateBackground(){

	}

	public virtual void OnHover(){
		if(menu != null && menu.isActiveMenu){
			menu.hoveredComponent = this;
			onHover.Invoke();	
		}
	}

	public virtual void OnStopHover(){
		if(menu != null && menu.isActiveMenu){
			if(menu.hoveredComponent == this){
				menu.hoveredComponent = null;
			}
			onStopHover.Invoke();	
		}
	}

	public virtual void OnClick(){
		if(menu != null && menu.isActiveMenu && !isDisabled){
			if(isClicked){
				isClicked = false;
				onDoubleClick.Invoke();
			}else{
				doubleClickTimer = 0f;
				isClicked = true;
			}
			onClick.Invoke();
		}
	}

	public virtual void OnRightClick(){
		if(menu != null && menu.isActiveMenu){
			onRightClick.Invoke();
		}
	}

	void CheckDoubleClick(){
		if(isClicked){
			doubleClickTimer += Time.deltaTime;
			if(doubleClickTimer >= doubleClickSensitivity){
				isClicked = false;
				doubleClickTimer = 0f;
			}
		}
	}

	public virtual void OnDoubleClick(){
		
	}

	public virtual void OnSubmit(){
		
	}

	public virtual void OnActivate(){
		if(menu != null && menu.isActiveMenu){
			menu.hoveredComponent = this;
			onActivate.Invoke();
		}
	}

	public virtual void OnDeactivate(){
		if(menu != null && menu.isActiveMenu){
			menu.hoveredComponent = this;
			onDeactivate.Invoke();	
		}
	}

	public virtual void ClearState(){
		isHovered = false;
		wasHovered = false;
		isClicked = false;
		isPressed = false;
		isActive = false;
		wasActive = false;
	}

}