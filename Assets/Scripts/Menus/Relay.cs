using UnityEngine;
using System.Collections;

public class Relay : MonoBehaviour {//coordinates elements that are alternately activated
	
	public GameObject MenuUnit;
	public GameObject MenuUnitProc;
	
	//Relayed variables
	public int currentTab = 1;
	public bool pauseInput = false;	
	//
	
	
	public void ActivateMenuUnit()
	{
		if(currentTab == 2)
		{
			MenuUnit.SetActive(true);
			((MenuUnit)MenuUnit.GetComponent("MenuUnit")).LoadValuesfromProc();
		}		
	}
	
	public void ActivateMenuUnitProc()
	{
		MenuUnitProc.SetActive(true);	
		((MenuUnit)MenuUnit.GetComponent("MenuUnit")).PassValuestoProc();		
	}
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
