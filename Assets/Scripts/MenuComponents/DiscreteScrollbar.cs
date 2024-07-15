using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;



public class DiscreteScrollbar : MenuComponent{

	[SerializeField]
	bool isHorizontal = false;
	[SerializeField]
	int currentIndex = 0;
	public UnityEvent onValueChanged = new UnityEvent();
	public int slotCount = 1;
	public int itemCount = 1;
	public int value {
		get{ 
			if(isHorizontal){
				return ClampValue(itemCount - slotCount - currentIndex);
			}else{
				return ClampValue(currentIndex); 
			}
		}
		set{
			if(isHorizontal){
				currentIndex = ClampValue(itemCount - slotCount - value);
			}else{
				currentIndex = ClampValue(value);
			}
			
		}
	}
	GameObject handle;

	RectTransform rt;
	[SerializeField]
	int numberOfSteps;
	float height {
		get {
			if(isHorizontal){
				return rt.rect.width;
			}else{
				return rt.rect.height;
			}
			
		}
		set {}
	}
	//int pixelHeightOfStep;
	float pixelHeightOfStep;
	bool isDragging = false;
	float mousePos, mousePreviousPos;
	int startingPosition;
	public float sensitivity;

	public void Init(){
		rt = GetComponent<RectTransform>();
		handle = transform.Find("SlidingArea").Find("Handle").gameObject;
	}

	public override void UpdateActive(){
		int valuePrevious = value;
		if((isHovered || section.isHovered) && itemCount > slotCount){
			if(InputControl.mouseScrollAmount > 0f && !isHorizontal){
				if(currentIndex > 0){
					currentIndex -= 1;
				}
			}else if(InputControl.mouseScrollAmount < 0f && !isHorizontal){
				if(currentIndex < numberOfSteps -1){
					currentIndex += 1;
				}
			}else if(InputControl.mouseLeftDown && !isDragging && isHovered){
				isDragging = true;
				startingPosition = currentIndex;
				mousePos = isHorizontal ? InputControl.mousePosition.x : InputControl.mousePosition.y;
				mousePreviousPos = mousePos;
			}
		}
		if(isDragging){
			if(InputControl.mouseLeftReleased || itemCount <= slotCount){
				isDragging = false;
				currentIndex = ClampValue(currentIndex);
			}else{

				mousePos = isHorizontal ? InputControl.mousePosition.x : InputControl.mousePosition.y;
				float deltaAmount = mousePreviousPos - mousePos;
				int movementInSteps = (int)Mathf.Round(deltaAmount / pixelHeightOfStep);
				currentIndex = startingPosition + movementInSteps;
				currentIndex = ClampValue(currentIndex);
			}
		}
		if(value != valuePrevious){
			onValueChanged.Invoke();
		}
		if(itemCount > slotCount){
			SetHandlePosition();	
		}else{
			//hideHandle
		}
		
	}

	public void SetScrollIndex(int newIndex){
		if(isHorizontal){
			currentIndex = ClampValue(itemCount - slotCount - value);
		}else{
			currentIndex = newIndex;	
		}
		SetHandlePosition();
	}

	public override void Reload(){
		RecalculateScrollSteps();
	}

	public void SetScrollbar(int ci, int ic, int sc){
		if(isHorizontal){
			currentIndex = ClampValue(itemCount - slotCount - ci);
		}else{
			currentIndex = ci;	
		}
		itemCount = ic;
		slotCount = sc;
		if(itemCount <= slotCount){
			gameObject.SetActive(false);
		}else{
			gameObject.SetActive(true);
		}
		RecalculateScrollSteps();
	}

	void RecalculateScrollSteps(){
		if(itemCount <= slotCount){
			numberOfSteps = 1;
		}else{
			numberOfSteps = itemCount - slotCount + 1;
		}
		pixelHeightOfStep = height / numberOfSteps;
		SetHandleHeight();
		SetHandlePosition();
	}

	void SetHandleHeight(){
		if(isHorizontal){
			handle.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pixelHeightOfStep);
		}else{
			handle.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pixelHeightOfStep);
		}
	}

	void SetHandlePosition(){
		float handlePos = -currentIndex * pixelHeightOfStep - pixelHeightOfStep/2f + (height/2); //if current index is 0, it should 
		if(isHorizontal){
			handle.transform.localPosition = new Vector3(handlePos,0,0);
		}else{
			handle.transform.localPosition = new Vector3(0,handlePos,0);
		}
	}

	int ClampValue(int val){
		if(val < 0){
			val = 0;
		}
		if(val > numberOfSteps - 1){
			val = numberOfSteps - 1;
		}
		return val;
	}
}