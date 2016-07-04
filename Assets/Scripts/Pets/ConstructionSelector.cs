using UnityEngine;
using System.Collections;

public class ConstructionSelector : MonoBehaviour {//controls the behaviour of a "building under construction"

	private bool inConstruction = true;
	public bool isSelected = true;

	public float progTime = 0.57f;
	public float progCounter = 0;
	public int buildingTime = 1;
	private GameObject gameManager;

	public int remainingTime = 1;
	public GameObject Price;
	private GameObject Stats;
	private Component StatsCo;
	//public UILabel UserMessages;
	public int priceno;
	public int storageIncrease;

	public int constructionIndex = -1;	
	public GameObject ProgressBarObj;
	private GameObject[] selectedGrassType;	
	private GameObject BuildingsGroup;
	public string buildingType; //what kind of building is hosting

	public UILabel TimeCounterLb;
	int hours,minutes,seconds;

	private GameObject SoundFXOb;
	private Component soundFXSc;
	// Use this for initialization
	void Start () 
	{
		BuildingsGroup = GameObject.Find("BuildingsGroup");
		gameManager = GameObject.Find("GameManager");
		Stats = GameObject.Find("Stats");
		StatsCo = Stats.GetComponent ("Stats");

		SoundFXOb = GameObject.Find("SoundFX");
		soundFXSc = SoundFXOb.GetComponent("SoundFX");

		//init price so user can't click fast on price 0
		remainingTime = buildingTime * (1 - (int)((UISlider)ProgressBarObj.GetComponent("UISlider")).value);
		UpdatePrice (remainingTime);
		//UpdateTimeCounter(remainingTime); 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate()
	{
		if(inConstruction)
		{
			ProgressBar();
		}
	}
	
	private void ProgressBar()
	{		
		progCounter += Time.deltaTime*0.5f;
		if(progCounter > progTime)
		{
			progCounter = 0;
			
			((UISlider)ProgressBarObj.GetComponent("UISlider")).value += (float)(Time.deltaTime/buildingTime);
			
			((UISlider)ProgressBarObj.GetComponent("UISlider")).value=
			Mathf.Clamp(((UISlider)ProgressBarObj.GetComponent("UISlider")).value,0,1);

			remainingTime = (int)(buildingTime * (1 - ((UISlider)ProgressBarObj.GetComponent("UISlider")).value));

			UpdatePrice (remainingTime);
			UpdateTimeCounter(remainingTime);

			if(((UISlider)ProgressBarObj.GetComponent("UISlider")).value == 1)
			{
				((SoundFX)soundFXSc).BuildingFinished();
				((Stats)StatsCo).occupiedDobbitNo--;
				((Stats)StatsCo).update = true;

				if(buildingType=="Barrel")
				{
					((Stats)StatsCo).maxStorageMana += storageIncrease;
				}
				else if(buildingType=="Forge")
				{
					((Stats)StatsCo).productionBuildings[0]++;
					((Stats)StatsCo).maxStorageGold += storageIncrease;
				}
				else if(buildingType=="Generator")
				{
					((Stats)StatsCo).productionBuildings[1]++;
					((Stats)StatsCo).maxStorageMana += storageIncrease;
				}
				else if(buildingType=="Vault")
				{
					((Stats)StatsCo).maxStorageGold += storageIncrease;
				}

				foreach (Transform child in transform) 
				{					
					if(child.gameObject.tag == buildingType)
					{
						child.gameObject.SetActive(true);
						((BuildingSelector)child.gameObject.GetComponent("BuildingSelector")).inConstruction = false;
						foreach (Transform childx in transform) 
						{	
							if(childx.gameObject.tag == "Grass")
							{									
								childx.gameObject.transform.parent = child.gameObject.transform;
								child.gameObject.transform.parent = BuildingsGroup.transform;
								break;
							}	
						}
					}
				}

				Destroy(this.gameObject);
				inConstruction = false;	

			}
			
		}
		
		
	}

	private void UpdateTimeCounter(int remainingTime)
	{
		hours = (int)remainingTime/60; //93 1
		minutes = (int)remainingTime%60;//33
		seconds = (int)(60 - (((UISlider)ProgressBarObj.GetComponent("UISlider")).value*buildingTime*60)%60);			
		UpdateTimeLabel ();
	}

	private void UpdateTimeLabel()
	{
		if(hours>0 && minutes >0 && seconds>=0 )
		{			

			((UILabel)TimeCounterLb).text = 
				hours.ToString() +" h " +
					minutes.ToString() +" m " +
					seconds.ToString() +" s ";			
		}
		else if(minutes > 0 && seconds >= 0)
		{
			((UILabel)TimeCounterLb).text = 
				minutes.ToString() +" m " +
					seconds.ToString() +" s ";
			
		}
		else if(seconds > 0 )
		{
			((UILabel)TimeCounterLb).text = 
				seconds.ToString() +" s ";
		}

	}


	private void UpdatePrice(int remainingTime)
	{
		/*
		0		30		1
		30		60		3
		60		180		7
		180		600		15
		600		1440	30
		1440	2880	45
		2880	4320	70
		4320			150
		 */

		if (remainingTime >= 4320) { priceno = 150; }
		else if (remainingTime >= 2880) { priceno = 70; }
		else if (remainingTime >= 1440) { priceno = 45;	}
		else if (remainingTime >= 600)	{ priceno = 30;	}
		else if (remainingTime >= 180) { priceno = 15; }
		else if (remainingTime >= 60) { priceno = 7; }
		else if (remainingTime >= 30) {	priceno = 3; }
		else if(remainingTime >= 0) { priceno = 1; }

		((tk2dTextMesh)Price.GetComponent("tk2dTextMesh")).text = priceno.ToString();
	}

	public void Finish()
	{
		if (!((Relay)gameManager.GetComponent ("Relay")).pauseInput) 
		{
			((SoundFX)soundFXSc).Click();
			if (((Stats)StatsCo).crystals >= priceno) 
			{
				((Stats)StatsCo).crystals -= priceno;
				((Stats)StatsCo).update = true;
				((UISlider)ProgressBarObj.GetComponent ("UISlider")).value = 1;
			} 
			else 
			{
				((Stats)StatsCo).userMessagesTxt = "Not enough crystals";
				((Stats)StatsCo).initUserMessages = true;
			}
		}
	}
}
