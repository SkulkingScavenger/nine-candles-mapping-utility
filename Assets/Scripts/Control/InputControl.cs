using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControl : MonoBehaviour{

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


	public static Vector2 mousePosition;
	public static bool mouseLeftDown;
	public static bool mouseLeftPressed; //triggered on click and every [holdDelay] seconds
	private float mouseLeftTimer = 0f;
	private bool mouseLeftDownPrevious = false;
	public static bool mouseLeftClicked; //triggers exactly once per click
	public static bool mouseLeftReleased;

	public static bool mouseRightDown;
	public static bool mouseRightPressed; //triggered on click and every [holdDelay] seconds
	private float mouseRightTimer = 0f;
	private bool mouseRightDownPrevious = false;
	public static bool mouseRightClicked; //triggers exactly once per click
	public static bool mouseRightReleased;

	public static bool mouseScrolled = false;
	public static float mouseScrollAmount;

	public static bool floorAscendPressed;
	public static bool floorAscendDown;
	private bool floorAscendPrevious = false;
	public static bool floorAscendReleased;

	public static bool floorDescendPressed;
	public static bool floorDescendDown;
	private bool floorDescendPrevious = false;
	public static bool floorDescendReleased;

	public static bool ctrlPressed;
	public static bool ctrlDown;
	private bool ctrlPrevious = false;
	public static bool ctrlReleased;

	public static bool shiftPressed;
	public static bool shiftDown;
	private bool shiftPrevious = false;
	public static bool shiftReleased;

	public static bool tabPressed;
	public static bool tabDown;
	private bool tabPrevious = false;
	public static bool tabReleased;

	public static bool newPressed;
	public static bool newDown;
	private bool newPrevious = false;
	public static bool newReleased;

	public static bool delPressed;
	public static bool delDown;
	private bool delPrevious = false;
	public static bool delReleased;

	public static bool movePressed;
	public static bool moveDown;
	private bool movePrevious = false;
	public static bool moveReleased;

	public static bool copyPressed;
	public static bool copyDown;
	private bool copyPrevious = false;
	public static bool copyReleased;

	public static bool cutPressed;
	public static bool cutDown;
	private bool cutPrevious = false;
	public static bool cutReleased;

	public static bool pastePressed;
	public static bool pasteDown;
	private bool pastePrevious = false;
	public static bool pasteReleased;

	public static bool edgePressed;
	public static bool edgeDown;
	private bool edgePrevious = false;
	public static bool edgeReleased;

	// Start is called before the first frame update
	void Awake()
	{
		controls = new InputControls();
	}

	void OnEnable(){
		controls.UI.Enable();
		controls.UI.Navigate.performed += context => arrowkeys = context.ReadValue<Vector2>();
		controls.UI.Click.performed += context => mouseLeftDown = context.ReadValue<float>() > 0.2f;
		controls.UI.RightClick.performed += context => mouseRightDown = context.ReadValue<float>() > 0.2f;
		controls.UI.Point.performed += context => mousePosition = context.ReadValue<Vector2>();
		controls.UI.ScrollWheel.performed += context => mouseScrollAmount = context.ReadValue<Vector2>().normalized.y;
		controls.UI.FloorAscend.performed += context => floorAscendDown = context.ReadValue<float>() > 0.2f;
		controls.UI.FloorDescend.performed += context => floorDescendDown = context.ReadValue<float>() > 0.2f;
		controls.UI.Ctrl.performed += context => ctrlDown = context.ReadValue<float>() > 0.2f;
		controls.UI.Shift.performed += context => shiftDown = context.ReadValue<float>() > 0.2f;
		controls.UI.Tab.performed += context => tabDown = context.ReadValue<float>() > 0.2f;
		controls.UI.New.performed += context => newDown = context.ReadValue<float>() > 0.2f;
		controls.UI.Delete.performed += context => delDown = context.ReadValue<float>() > 0.2f;
		controls.UI.Move.performed += context => moveDown = context.ReadValue<float>() > 0.2f;
		controls.UI.Copy.performed += context => copyDown = context.ReadValue<float>() > 0.2f;
		controls.UI.Cut.performed += context => cutDown = context.ReadValue<float>() > 0.2f;
		controls.UI.Paste.performed += context => pasteDown = context.ReadValue<float>() > 0.2f;
		controls.UI.EdgeEdit.performed += context => edgeDown = context.ReadValue<float>() > 0.2f;
	}

	// Update is called once per frame
	void Update(){
		UpdateInputs();
	}

	void UpdateInputs(){
		cancelPressed = controls.UI.Cancel.triggered;
		submitPressed = controls.UI.Submit.triggered;

		floorAscendPressed = false;
		floorAscendReleased = false;
		if(floorAscendDown && !floorAscendPrevious){
			floorAscendPressed = true;
		}else if(!floorAscendDown && floorAscendPrevious){
			floorAscendReleased = true;
		}
		floorAscendPrevious = floorAscendDown;

		floorDescendPressed = false;
		floorDescendReleased = false;
		if(floorDescendDown && !floorDescendPrevious){
			floorDescendPressed = true;
		}else if(!floorDescendDown && floorDescendPrevious){
			floorDescendReleased = true;
		}
		floorDescendPrevious = floorDescendDown;

		ctrlPressed = false;
		ctrlReleased = false;
		if(ctrlDown && !ctrlPrevious){
			ctrlPressed = true;
		}else if(!ctrlDown && ctrlPrevious){
			ctrlReleased = true;
		}
		ctrlPrevious = ctrlDown;

		shiftPressed = false;
		shiftReleased = false;
		if(shiftDown && !shiftPrevious){
			shiftPressed = true;
		}else if(!shiftDown && shiftPrevious){
			shiftReleased = true;
		}
		shiftPrevious = shiftDown;

		tabPressed = false;
		tabReleased = false;
		if(tabDown && !tabPrevious){
			tabPressed = true;
		}else if(!tabDown && tabPrevious){
			tabReleased = true;
		}
		tabPrevious = tabDown;

		newPressed = false;
		newReleased = false;
		if(newDown && !newPrevious){
			newPressed = true;
		}else if(!newDown && newPrevious){
			newReleased = true;
		}
		newPrevious = newDown;

		delPressed = false;
		delReleased = false;
		if(delDown && !delPrevious){
			delPressed = true;
		}else if(!delDown && delPrevious){
			delReleased = true;
		}
		delPrevious = delDown;

		movePressed = false;
		moveReleased = false;
		if(moveDown && !movePrevious){
			movePressed = true;
		}else if(!moveDown && movePrevious){
			moveReleased = true;
		}
		movePrevious = moveDown;

		copyPressed = false;
		copyReleased = false;
		if(copyDown && !copyPrevious){
			copyPressed = true;
		}else if(!copyDown && copyPrevious){
			copyReleased = true;
		}
		copyPrevious = copyDown;

		cutPressed = false;
		cutReleased = false;
		if(cutDown && !cutPrevious){
			cutPressed = true;
		}else if(!cutDown && cutPrevious){
			cutReleased = true;
		}
		cutPrevious = cutDown;

		pastePressed = false;
		pasteReleased = false;
		if(pasteDown && !pastePrevious){
			pastePressed = true;
		}else if(!pasteDown && pastePrevious){
			pasteReleased = true;
		}
		pastePrevious = pasteDown;

		edgePressed = false;
		edgeReleased = false;
		if(edgeDown && !edgePrevious){
			edgePressed = true;
		}else if(!edgeDown && edgePrevious){
			edgeReleased = true;
		}
		edgePrevious = edgeDown;

		//Left Mouse Button
		mouseLeftPressed = false;
		mouseLeftClicked = false;
		mouseLeftReleased = false;
		if(mouseLeftDown){
			if(mouseLeftDownPrevious){
				mouseLeftTimer += Time.deltaTime;
				if(mouseLeftTimer >= holdDelay){
					mouseLeftTimer -= holdDelay;
					mouseLeftPressed = true;
				}
			}else{
				mouseLeftPressed = true;
				mouseLeftClicked = true;
			}
		}else{
			if(mouseLeftDownPrevious){
				mouseLeftReleased = true;
				mouseLeftTimer = 0f;
			}
		}
		mouseLeftDownPrevious = mouseLeftDown;

		//Right Mouse Button
		mouseRightPressed = false;
		mouseRightClicked = false;
		mouseRightReleased = false;
		if(mouseRightDown){
			if(mouseRightDownPrevious){
				mouseRightTimer += Time.deltaTime;
				if(mouseRightTimer >= holdDelay){
					mouseRightTimer -= holdDelay;
					mouseRightPressed = true;
				}
			}else{
				mouseRightPressed = true;
				mouseRightClicked = true;
			}
		}else{
			if(mouseRightDownPrevious){
				mouseRightReleased = true;
				mouseRightTimer = 0f;
			}
		}
		mouseRightDownPrevious = mouseRightDown;

		//Mouse Scroll Wheel
		mouseScrolled = mouseScrollAmount != 0f;
		// if(!mouseScrolled){mouseScrollAmount = 0f;}
		if(mouseScrollAmount < 0){

		}else if(mouseScrollAmount > 0){

		}

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

}