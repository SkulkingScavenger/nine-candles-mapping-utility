using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class MenuButton : MenuComponent, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler{

	public Text textElement;
	public Image backgroundImage;
	public string label {
		get { return textElement.text; }
		set { textElement.text = value; }
	}

	public Color defaultColor = new Color(1f, 1f, 1f, 1f);
	public Color activeColor = new Color(1f, 1f, 1f, 1f);
	public Color hoveredColor = new Color(0f, 1f, 1f, 1f);
	public Color disabledColor = new Color(0.3f, 0.3f, 0.3f, 1f);
	public Color selectedColor = new Color(1f, 1f, 1f, 1f);

	public Color defaultBackground = new Color (0.3f,0.3f,0.3f,1f);
	public Color activeBackground = new Color (0.0f,0.0f,0.0f,1f);
	public Color hoveredBackground = new Color (0.2f,0.2f,0.2f,1f);
	public Color disabledBackground = new Color (0.6f,0.6f,0.6f,1f);
	public Color selectedBackground = new Color(1f, 1f, 1f, 1f);

	void Awake(){
		textElement = transform.Find("Text").GetComponent<Text>();
		backgroundImage = GetComponent<Image>();
	}

	public override void UpdateActive(){
		UpdateDisplay();
	}

	public override void OnSubmit(){
		onClick.Invoke();
	}

	public virtual void UpdateDisplay(){
		textElement.color = GetColor();
		backgroundImage.color = GetBackgroundColor();
	}

	public Color GetColor(){
		Color output;
		if(isDisabled){
			output = disabledColor;
		}else if(isHovered){
			output = hoveredColor;
		}else{
			if(isActive){
				output = activeColor;
			}else{
				output = defaultColor;
			}
		}
		return output;
	}

	public Color GetBackgroundColor(){
		Color output;
		if(isDisabled){
			output = disabledBackground;
		}else if(isHovered){
			output = hoveredBackground;
		}else{
			if(isActive){
				output = activeBackground;
			}else{
				output = defaultBackground;
			}
		}
		return output;
	}

}
