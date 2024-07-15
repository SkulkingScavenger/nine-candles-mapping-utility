using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class MenuControl : MonoBehaviour{
	public static MenuControl Instance;
	public static Menu activeMenu = null;

	public static GameObject canvas;
	public static ConfirmationDialog confirmationDialog;
	public static NotificationDialog notificationDialog;


	InputControls controls;

	
	public static bool cancelPressed;
	public static bool submitPressed;
	public static bool cancelAltPressed;
	public static bool submitAltPressed;
	public static bool infoPressed;


	float holdDelay = 0.5f;
	public static bool[] arrowPressed = new bool[4];
	public static bool[] arrowDown = new bool[4];
	private bool[] arrowDownPrevious = new bool[4];
	public static bool[] arrowReleased = new bool[4];
	private float[] arrowTimer = {0f, 0f, 0f, 0f};
	public static Vector2 arrowkeys;


	public GameObject[] menuComponentPrefabs;


	public void Init(){
		if(Instance != null && Instance != this){
			Destroy(gameObject);
			return;
		}
		Instance = this;

		if(controls == null){
			controls = new InputControls();
		}

		canvas = GameObject.FindWithTag("Canvas");
		confirmationDialog = canvas.transform.Find("ConfirmationDialog").GetComponent<ConfirmationDialog>();
		notificationDialog = canvas.transform.Find("NotificationDialog").GetComponent<NotificationDialog>();

		// controls.UI.Enable();
		// controls.UI.Navigate.performed += context => arrowkeys = context.ReadValue<Vector2>();
	}

	void OnDisable(){
		//controls.UI.Disable();
	}

	void Update(){
		
		// cancelPressed = controls.UI.Cancel.triggered;
		// submitPressed = controls.UI.Submit.triggered;
		// cancelAltPressed = controls.UI.CancelAlt.triggered;
		// submitAltPressed = controls.UI.SubmitAlt.triggered;
		// infoPressed = controls.UI.Info.triggered;

		for(int i=0;i<4;i++){
			arrowDownPrevious[i] = arrowDown[i];
		}
		arrowDown[0] = arrowkeys.x > 0.2f;
		arrowDown[1] = arrowkeys.y > 0.2f;
		arrowDown[2] = arrowkeys.x < -0.2f;
		arrowDown[3] = arrowkeys.y < -0.2f;
		
		for(int i=0;i<4;i++){
			arrowPressed[i] = !arrowDownPrevious[i] && arrowDown[i];
			arrowReleased[i] = arrowDownPrevious[i] && !arrowDown[i];
			if(arrowDownPrevious[i] && arrowDown[i]){
				arrowTimer[i] += Time.deltaTime;
				if(arrowTimer[i] >= holdDelay){
					arrowTimer[i] -= holdDelay;
					arrowPressed[i] = true;
				}
			}else{
				arrowTimer[i] = 0f;
			}
		}
		
	}

	public void Startup(){
		canvas = GameObject.FindWithTag("Canvas");
		canvas.transform.Find("StartupMenu").GetComponent<StartupMenu>().Open();
		canvas.transform.Find("StartupMenu").GetComponent<StartupMenu>().Activate();
	}

	public static void ReloadMenus(){
		DungeonMap dm = canvas.transform.Find("DungeonMap").GetComponent<DungeonMap>();
		if(activeMenu != dm){
			dm.Open();
			dm.Activate();	
		}else{
			dm.OnDungeonReload();
		}
		
	}
}
