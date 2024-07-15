using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CanvasAnimator : MonoBehaviour{

	public RuntimeAnimatorController controller;
	Image image;
	SpriteRenderer fakeRenderer;
	Sprite staticSprite;
	Animator animator;


	bool isPlaying = true;

	void Awake(){
		image = GetComponent<Image>();
		fakeRenderer = gameObject.AddComponent<SpriteRenderer>();
		fakeRenderer.enabled = false;
		animator = gameObject.AddComponent<Animator>();
		animator.runtimeAnimatorController = controller;

		isPlaying = true;
	}

	void Update(){
		if(animator.runtimeAnimatorController && isPlaying){
			image.sprite = fakeRenderer.sprite;
		}
	}
}
