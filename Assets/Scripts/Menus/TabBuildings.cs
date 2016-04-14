using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TabBuildings : MonoBehaviour {//controls the pages and arrows for buildings menu
		
	private const int noPages = 2;
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
			Pages [1].SetActive (false);
			Pages [0].SetActive (true);
			ArrowLeft.SetActive (false);
			ArrowRight.SetActive (true);
			noPanel--;
	}
	
	public void OnArrowRight()
	{
			Pages [0].SetActive(false);
			Pages [1].SetActive(true);
			ArrowRight.SetActive(false);
			ArrowLeft.SetActive(true);
			noPanel++;
	}
	
}
