using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MenuShop : MonoBehaviour {//controls the building/units tabs
	
	private const int noTabs = 4;
	
	public GameObject[] LabelsSelected = new GameObject[noTabs];
	public GameObject[] LabelsIdle = new GameObject[noTabs];
	public GameObject[] Tabs = new GameObject[noTabs];
	
	public GameObject GameManager;
	private Component relayScript;
	
	public int curSelection = 1;
	public int prevSelection = 1;
		
	public void OnTreasureTab(){
		curSelection=0;	
		SelectTab();
	}
	
	public void OnBuildingsTab(){
		curSelection=1;
		SelectTab();
	}
	
	public void OnCreaturesTab(){
		curSelection=2;	
		SelectTab();
	}
	
	public void OnArtifactsTab(){
		curSelection=3;
		SelectTab();
	}
	
	// Use this for initialization
	void Start () {
		relayScript = GameManager.GetComponent("Relay");	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
	private void SelectTab()
	{
		if(curSelection != prevSelection)
		{	
			((Relay)relayScript).currentTab = curSelection;
			
			LabelsIdle[prevSelection].SetActive(true);
			LabelsSelected[prevSelection].SetActive(false);
			
			LabelsSelected[curSelection].SetActive(true);
			LabelsIdle[curSelection].SetActive(false);
						
			SwitchTab();
			
			prevSelection = curSelection;			
		}
	}

	private void SwitchTab()
	{
		if(prevSelection == 2)
		{
			((Relay)relayScript).ActivateMenuUnitProc();
		}
		else if(curSelection==2)
		{
			((Relay)relayScript).ActivateMenuUnit();
		}
		
		Tabs[curSelection].SetActive(true);
		Tabs[prevSelection].SetActive(false);
	}
}
