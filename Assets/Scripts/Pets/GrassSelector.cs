using UnityEngine;
using System.Collections;

public class GrassSelector : MonoBehaviour {
	
	public bool isSelected = true;
	public bool inCollision = false;
	public int grassIndex = -1;	
	public int grassType = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void ReSelect()
	{
		isSelected = true;		
	}
	
}
