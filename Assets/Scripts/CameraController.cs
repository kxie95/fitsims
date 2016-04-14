using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CameraController : MonoBehaviour {
	
	private float speed = 2.0f;
	public bool MoveNb, MoveEb, MoveSb, MoveWb, ZoomInb, ZoomOutb;//bools for button controlled moving Move North, East, South, West
	//private bool active = false;
	//private float zoom; 
	
	private float zoomMax = 1;	//caps for zoom
	private float zoomMin = 0.2f;
	private float zoom; // the current camera zoom factor

	private float currentFingerDistance;
	private float previousFingerDistance;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	private IEnumerator Reset()
	{		
		//arrow activated move stops after 5 secs or at second move press
		yield return new WaitForSeconds(5.0f);
		MoveNb = MoveEb = MoveSb = MoveWb = ZoomInb = ZoomOutb = false;
	}
		
	public void MoveN()
	{
		MoveNb = !MoveNb;
		StartCoroutine(Reset());
	}
	
	public void MoveE()
	{	
		MoveEb = !MoveEb;
		StartCoroutine(Reset());
	}
	
	public void MoveS()
	{	
		MoveSb = !MoveSb;
		StartCoroutine(Reset());
	}
	
	public void MoveW()
	{	
		MoveWb = !MoveWb;
		StartCoroutine(Reset());
	}
	
	public void ZoomIn()
	{	
		ZoomInb = !ZoomInb;	
		StartCoroutine(Reset());
	}
	
	public void ZoomOut()
	{	
		ZoomOutb = !ZoomOutb;
		StartCoroutine(Reset());
	}
	
	// conditions keep the camera from going off-map, leaving a margin for zoom-in/out
	void FixedUpdate ()
	{			
		if(MoveNb 			
			&& transform.position.y < 4200){transform.position+=new Vector3(0,10,0);}
			//&& transform.position.x > -5200
		if (MoveEb
			&& transform.position.x < 5200){transform.position+=new Vector3(10,0,0);}
			//&& transform.position.y < 4200		
		if(MoveSb
			//&& transform.position.x < 5200
			&& transform.position.y > -4300){transform.position+=new Vector3(0,-10,0);}		
		if (MoveWb
			&& transform.position.x > -5200){transform.position+=new Vector3(-10,0,0);}
			//&& transform.position.y > -4300
		
		zoom = ((tk2dCamera)this.GetComponent("tk2dCamera")).ZoomFactor;

		//zoom in/out
		if(ZoomInb && zoom<zoomMax){((tk2dCamera)this.GetComponent("tk2dCamera")).ZoomFactor += 0.01f;}
		else if(ZoomOutb && zoom>zoomMin){((tk2dCamera)this.GetComponent("tk2dCamera")).ZoomFactor -= 0.01f;}
		
	}
	
	void Update()
	{
			
			zoom = ((tk2dCamera)this.GetComponent("tk2dCamera")).ZoomFactor;
		
			if (Input.touchCount > 1 && Input.GetTouch(0).phase == TouchPhase.Moved//chech for 2 fingers on screen
				&& Input.GetTouch(1).phase == TouchPhase.Moved) {
            	
			Vector2 touchPosition0 = Input.GetTouch(0).position;//positions for both fingers for pinch zoom in/out
			Vector2 touchPosition1 = Input.GetTouch(1).position;
				
			currentFingerDistance = Vector2.Distance(touchPosition0,touchPosition1);//distance between fingers
				
			if (currentFingerDistance>previousFingerDistance && zoom<zoomMax)
				{
				((tk2dCamera)this.GetComponent("tk2dCamera")).ZoomFactor += 0.05f;//0.02f;
				}
			else if(zoom>zoomMin)
				{
				((tk2dCamera)this.GetComponent("tk2dCamera")).ZoomFactor -= 0.05f;//0.02f;
				}
				
			previousFingerDistance = currentFingerDistance;
		
			}
			
			//else, if one finger on screen - scroll
			else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
                  		
			
			if(touchDeltaPosition.x < 0)
			{
				if( transform.position.x < 5000)
				{
						transform.Translate(-touchDeltaPosition.x*speed, 0, 0);	
				}
			}
			else if( transform.position.x > -5000)
				{
						transform.Translate(-touchDeltaPosition.x*speed, 0, 0);	
				}
			
			
			if(touchDeltaPosition.y < 0)
			{
				if( transform.position.y < 4000)
				{
						transform.Translate(0, -touchDeltaPosition.y*speed, 0);	
				}
			}
			
			else if( transform.position.y > -4000)
				{
						transform.Translate(0, -touchDeltaPosition.y*speed, 0);	
				}
		
		}
	}
}
