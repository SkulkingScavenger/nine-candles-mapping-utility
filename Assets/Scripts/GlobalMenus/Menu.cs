using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public abstract class Menu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{

	public List<MenuSection> sections;
	public MenuComponent activeComponent;
	public Menu root;
	int _activeSubmenuIndex = -1;
	public int activeSubmenuIndex {
		get { return _activeSubmenuIndex; }
		set { SetActiveSubmenu(value); }
	}
	public Submenu activeSubmenu {
		get {
			if(_activeSubmenuIndex >= 0){
				return submenus[_activeSubmenuIndex];
			}else{
				return null;
			}
		}
		set {}
	}
	bool _isOpen = false;
	public bool isOpen {
		get { return _isOpen; }
		set {
			if(value){
				Open();
			}else{
				Close();
			}
		}
	}
	public Submenu[] submenus;
	public bool isHovered;
	protected bool isInitialized = false;
	[HideInInspector] public MenuComponent hoveredComponent;
	public MenuSection activeSection {
		get {
			if(activeComponent == null){
				return null;
			}else{
				return activeComponent.section;
			}
		}
		set {
			activeComponent = value.components[0];
		}
	}
	public virtual bool isActiveMenu{
		get {return MenuControl.activeMenu == this;}
		set {
			if(value){
				if(MenuControl.activeMenu != this){
					Activate();
				}
			}else{
				if(MenuControl.activeMenu == this){
					Deactivate();
				}
			}
		}
	}

	int _heldSlotIndex;
	public virtual int heldSlotIndex {
		get { return _heldSlotIndex; }
		set { _heldSlotIndex = value; }
	}
	MenuSection _heldSlotSource;
	public virtual MenuSection heldSlotSource {
		get { return _heldSlotSource; }
		set { _heldSlotSource = value; }
	}
	UnderCursorDisplay _underCursorDisplay;
	public virtual UnderCursorDisplay underCursorDisplay {
		get { return _underCursorDisplay; }
		set { _underCursorDisplay = value; }
	}

	void Start(){
		Init();
	}

	public void Init(){
		if(!isInitialized){
			
			for(int i=0;i<submenus.Length;i++){
				submenus[i].root = this;
				submenus[i].submenuIndex = i;
			}

			InitialSetup();

			for(int i=0;i<submenus.Length;i++){
				submenus[i].SubmenuStart();
			}

			MenuSection[] results;
			results = GetComponentsInChildren<MenuSection>();
			for(int i=0;i<results.Length;i++){
				if(!results[i].isInitialized){
					results[i].menu = this;
					results[i].sectionIndex = i;
					results[i].Init();
					//for(int j=0j<components.Length;j++){}
					sections.Add(results[i]);
					results[i].isInitialized = true;
				}
			}

			isInitialized = true;
		}
	}

	void Update(){
		if(isActiveMenu){
			UpdateActive();
			// for(int i=0;i<submenus.Length;i++){
			// 	submenus[i].UpdateActive();
			// }
			if(activeSubmenu != null){
				activeSubmenu.UpdateActive();
			}
		}
		UpdateBackground();
		for(int i=0;i<submenus.Length;i++){
			submenus[i].UpdateBackground();
		}
	}

	public void Open(bool openAsActive=true){
		if(_isOpen){
			return;
		}
		if(!isInitialized){
			Init();
		}
		gameObject.SetActive(true);
		OnMenuOpen();
		_isOpen = true;
		for(int i=0;i<sections.Count;i++){
			sections[i].OnActivate();
		}
		if(activeSubmenuIndex >= 0){
			submenus[activeSubmenuIndex].Open();
		}
	}

	public void Close(){
		Deactivate();
		gameObject.SetActive(false);
		for(int i=0;i<submenus.Length;i++){
			submenus[i].OnMenuClose();
		}
		for(int i=0;i<sections.Count;i++){
			sections[i].OnDeactivate();
		}
		OnMenuClose();
		_isOpen = false;
	}

	public void Activate(){
		if(!isActiveMenu && gameObject.activeSelf){
			MenuControl.activeMenu = this;
			OnActivate();
			if(activeSubmenuIndex >= 0){
				submenus[activeSubmenuIndex].Activate();
			}
		}
	}

	public void Deactivate(){
		if(isActiveMenu){
			root.Activate();
			for(int i=0;i<sections.Count;i++){
				for(int j=0;j<sections[i].components.Count;j++){
					sections[i].components[j].ClearState();
				}
			}
			for(int i=0;i<submenus.Length;i++){
				submenus[i].Deactivate();
			}
			OnDeactivate();
		}
	}

	public void OnPointerEnter(PointerEventData e){
		isHovered = true;
	}

	public void OnPointerExit(PointerEventData e){
		isHovered = false;
	}

	public virtual void InitialSetup(){

	}

	public virtual void OnMenuOpen(){

	}

	public virtual void OnMenuClose(){

	}

	public virtual void OnActivate(){

	}

	public virtual void OnDeactivate(){

	}

	public virtual void Reload(){

	}

	public virtual void UpdateActive(){

	}

	public virtual void UpdateBackground(){

	}

	public virtual void FieldClicked(int index){

	}

	void SetActiveSubmenu(int index){
		if(index >= 0){
			if(_activeSubmenuIndex != index){
				if(_activeSubmenuIndex >= 0){
					if(isOpen){
						submenus[_activeSubmenuIndex].Close();	
					}
				}
				_activeSubmenuIndex = index;
				if(isOpen){
					submenus[_activeSubmenuIndex].Open();
					if(isActiveMenu){
						submenus[_activeSubmenuIndex].Activate();
					}
				}
			}
		}else{
			if(_activeSubmenuIndex >= 0){
				if(isOpen){
					submenus[_activeSubmenuIndex].Close();	
				}
			}
			_activeSubmenuIndex = -1;
		}
	}

	public bool ClickedOutside(){
		return InputControl.mouseLeftClicked && !isHovered;
	}

	public void OpenConfirmationDialog(string label, ConfirmationDialog.CallbackFunc callback){
		MenuControl.confirmationDialog.label = label;
		MenuControl.confirmationDialog.gameObject.SetActive(true);
		MenuControl.confirmationDialog.root = MenuControl.activeMenu;
		MenuControl.confirmationDialog.isActiveMenu = true;
		MenuControl.confirmationDialog.callback = callback;
	}

	public void OpenNotificationDialog(string message, NotificationDialog.CallbackFunc callback){
		MenuControl.notificationDialog.label.text = message;
		MenuControl.notificationDialog.gameObject.SetActive(true);
		MenuControl.notificationDialog.root = MenuControl.activeMenu;
		MenuControl.notificationDialog.isActiveMenu = true;
		MenuControl.notificationDialog.callback = callback;
	}
}