using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TabBuildings : MonoBehaviour {//controls the pages and arrows for buildings menu
		
	private const int noPages = 3;
	public GameObject[] Pages = new GameObject[noPages];
	private int noPanel = 0;	
	public GameObject ArrowLeft;
	public GameObject ArrowRight;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnArrowLeft()
	{
		Pages [noPanel].SetActive (false);
		Pages [noPanel-1].SetActive (true);
            
        if (noPanel-1 == 0) {
            ArrowLeft.SetActive(false);       
        }

        ArrowRight.SetActive(true);
       
        noPanel--;
	}
	
	public void OnArrowRight()
	{
		Pages [noPanel].SetActive(false);
		Pages [noPanel+1].SetActive(true);

        if (noPanel+1 == noPages-1)
        {
            ArrowRight.SetActive(false);
        }
        
        ArrowLeft.SetActive(true);

        noPanel++;
	}
	
}
