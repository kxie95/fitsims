using UnityEngine;
using System.Collections;

public class Draggable : MonoBehaviour {

    public CameraController controller;

    // Use this for initialization[RequireComponent(typeof(BoxCollider))]
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private Vector3 screenPoint;
    private Vector3 offset;

    void OnMouseDown()
    {
        //disable screen movement
        controller.touchActive = false;

        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

    }

    void OnMouseUp()
    {
        //Set the camera to move with touch again
        controller.touchActive = true;
    }

    void OnMouseDrag()
    {

        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;

    }
}
