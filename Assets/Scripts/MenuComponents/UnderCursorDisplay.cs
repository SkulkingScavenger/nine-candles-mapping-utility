using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnderCursorDisplay : MonoBehaviour{
	Image img;
	RectTransform rect;
	public Vector3 offset;
	public int width {
		get {return (int)rect.rect.width;}
		set {SetSize(value, height);}
	}
	public int height {
		get {return (int)rect.rect.height;}
		set {SetSize(width, value);}
	}

	public Sprite sprite {
		get {return img.sprite;}
		set {img.sprite = value;}
	}

	public Material material {
		get {return img.material;}
		set {img.material = value;}
	}

	void Start(){
		img = GetComponent<Image>();
		rect = GetComponent<RectTransform>();
	}

	void Update(){
		transform.position = new Vector3(InputControl.mousePosition.x, InputControl.mousePosition.y, 0) + offset;
	}

	public void SetSize(int w, int h){
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,w);
		rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,h);
	}
}