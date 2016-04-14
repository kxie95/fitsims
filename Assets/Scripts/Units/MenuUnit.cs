using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;


//This script is active while the units menu is enabled on screen

public class MenuUnit : MenuUnitBase {
				
	//Labels
	public UILabel HintLabel;
	public UILabel TimeLabel;
	public UILabel FinishPriceLb;
	private int priceInCrystals = 0;

	private bool resetLabels = false;
	
	//[HideInInspector]	
	
	//numerics

	//button positions
	private int[] OffScreenX = new int[numberOfUnits]{-700,-550,-400,-250,-100,50,200,350,500,650,800,950};//where the que prefabs are stored
	private int[] OnScreenX = new int[maxTypes]{ -50, -200, -350 };	//the three unit in training sockets
	private int OffScreenY = 500;//Y positions ofscreen
	private int OnScreenY = 230;//y positions, on screen
	private float z = -1f;
	
	//Buttons	
	public GameObject[] BuildButtons = new GameObject[numberOfUnits];
	public GameObject[] QueButtons = new GameObject[numberOfUnits];//small icons that appear when a unit is in training
	public GameObject[] ProgBars = new GameObject[numberOfUnits];//under que buttons	

	public UILabel[] Counters = new UILabel[numberOfUnits];//0-10 numbers displayed on que buttons
	public UILabel[] CreaturePrices = new UILabel[numberOfUnits];

	private List<GameObject> activeQueList = new List<GameObject>();
		
	public GameObject UnitProc; //target game obj for processor	
	public Component unitProcScript; //hidden sript for processing 

	public TextAsset UnitsXML;
	private List<Dictionary<string,string>> units = new List<Dictionary<string,string>>();
	private Dictionary<string,string> dictionary;	

	public GameObject Stats;
	private Component StatsCo;


	//Button messages
	public void OnBuild0()	{ VerifyConditions(0);  }
	public void OnBuild1()	{ VerifyConditions(1);	}
	public void OnBuild2()	{ VerifyConditions(2);	}
	public void OnBuild3()	{ VerifyConditions(3);	}
	public void OnBuild4()	{ VerifyConditions(4);	}
	public void OnBuild5()	{ VerifyConditions(5);	}
	public void OnBuild6()	{ VerifyConditions(6);	}
	public void OnBuild7()	{ VerifyConditions(7);	}
	public void OnBuild8()	{ VerifyConditions(8);	}
	public void OnBuild9()	{ VerifyConditions(9);	}
	public void OnBuild10() { VerifyConditions(10); }
	public void OnBuild11() { VerifyConditions(11); }
	
	public void OnCancel0()  { UnBuild(0,0); }
	public void OnCancel1()  { UnBuild(1,0); }
	public void OnCancel2()  { UnBuild(2,0); }
	public void OnCancel3()  { UnBuild(3,0); }
	public void OnCancel4()  { UnBuild(4,0); }
	public void OnCancel5()  { UnBuild(5,0); }
	public void OnCancel6()  { UnBuild(6,0); }
	public void OnCancel7()  { UnBuild(7,0); }
	public void OnCancel8()  { UnBuild(8,0); }
	public void OnCancel9()  { UnBuild(9,0); }
	public void OnCancel10() { UnBuild(10,0);}
	public void OnCancel11() { UnBuild(11,0);}
	

	void Start () {
			
		//menuUnitProc = UnitProc.GetComponent("UnitProc");
		StatsCo = Stats.GetComponent ("Stats");
		unitProcScript = UnitProc.GetComponent("MenuUnitProc");		//load the remote script
		GetUnitsXML();
		UpdateXMLData ();
	}

	public void GetUnitsXML()
	{

		XmlDocument xmlDoc = new XmlDocument(); 
		xmlDoc.LoadXml(UnitsXML.text); 
		XmlNodeList unitsList = xmlDoc.GetElementsByTagName("Unit");
			
			foreach (XmlNode unitInfo in unitsList)
			{
			XmlNodeList unitsContent = unitInfo.ChildNodes;	
				dictionary = new Dictionary<string, string>();

				foreach (XmlNode unitItems in unitsContent) // levels itens nodes.
				{
				if(unitItems.Name == "ID")
				{
					dictionary.Add("ID",unitItems.InnerText); // put this in the dictionary.
				}	
				if(unitItems.Name == "Name")
				{
					dictionary.Add("Name",unitItems.InnerText); 
				}				
				if(unitItems.Name == "Description")
				{
					dictionary.Add("Description",unitItems.InnerText); 
				}
				if(unitItems.Name == "Type")
				{
					dictionary.Add("Type",unitItems.InnerText); 
				}	
				if(unitItems.Name == "PopUnitWeight")
				{
					dictionary.Add("PopUnitWeight",unitItems.InnerText); 
				}			
				if(unitItems.Name == "Time")
				{
					dictionary.Add("Time",unitItems.InnerText); 
				}
				if(unitItems.Name == "XpAward")
				{
					dictionary.Add("XpAward",unitItems.InnerText); 
				}
				if(unitItems.Name == "GoldBased")
				{
					dictionary.Add("GoldBased",unitItems.InnerText); 
				}
				if(unitItems.Name == "ResCost")
				{
					dictionary.Add("ResCost",unitItems.InnerText); 
				}
				if(unitItems.Name == "StoreCategory")
				{
					dictionary.Add("StoreCategory",unitItems.InnerText); 
				}
				if(unitItems.Name == "ObjPrereq")
				{
					dictionary.Add("ObjPrereq",unitItems.InnerText); 
				}
				}
				units.Add(dictionary);
			}
		
	}

	private void UpdateXMLData()
	{
		for (int i = 0; i < CreaturePrices.Length; i++) 
		{
			//prices
			CreaturePrices[i].text = units [i] ["ResCost"];
			//time to build
			trainingTimes[i] = int.Parse (units[i]["Time"]);

		}
	}
	private void VerifyConditions(int currentSelection)
	{

		bool canBuild = true;

		if(units [currentSelection] ["GoldBased"] == "true")//this is string, not boolean 
		{
			if(((Stats)StatsCo).gold < int.Parse (units [currentSelection] ["ResCost"]))
			{				
				canBuild = false;
				((Stats)StatsCo).userMessagesTxt = "Not enough gold";				
			}
		}
		else //if( units [currentSelection] ["GoldBased"] == "false")
		{
			if(((Stats)StatsCo).mana < int.Parse (units [currentSelection] ["ResCost"]))
			{
				canBuild = false;
				((Stats)StatsCo).userMessagesTxt = "Not enough mana";
			}
		}

		if(trainingIndexes[currentSelection] == 10)
		{
			canBuild = false;
			((Stats)StatsCo).userMessagesTxt = "Only 10 units at a time";
		}

		if (canBuild) 
		{
			if(units [currentSelection] ["GoldBased"] == "true")
			{
				((Stats)StatsCo).gold -= int.Parse (units [currentSelection] ["ResCost"]); 
			}
			else
			{
				((Stats)StatsCo).mana -= int.Parse (units [currentSelection] ["ResCost"]);
			}
			
			((Stats)StatsCo).update = true;
			Build (currentSelection);
		} 
		else
		{
			((Stats)StatsCo).initUserMessages = true;			
		}
	}

	public void PassValuestoProc()
	{			
		bool queEmpty = true;	//verify if there's anything under constuction
		
		for (int i = 0; i < trainingIndexes.Length; i++) 
		{
			if(trainingIndexes[i]>0)
			{
				queEmpty = false;
				break;
			}
		}
				
		if(!queEmpty)
		{			
			((MenuUnitProc)unitProcScript).currentSlidVal = currentSlidVal;	
			((MenuUnitProc)unitProcScript).currentTrainingTime = currentTrainingTime;			
			((MenuUnitProc)unitProcScript).queList.Clear();	//clear queIndex/trainingIndex/objIndex dictionary
																											
			for (int i = 0; i < trainingIndexes.Length; i++) 
			{			
				//((MenuUnitProc)menuUnitProc).trainingIndexes[i] = trainingIndexes[i];	//save units under construction, including 0s			
				if(trainingIndexes[i]>0)
				{
					((MenuUnitProc)unitProcScript).queList.Add(new Vector3(					// qIndex, objIndex, trainingIndex
						((QIndex)QueButtons[i].GetComponent("QIndex")).qindex,						
						((QIndex)QueButtons[i].GetComponent("QIndex")).objindex,	//same as i
					    trainingIndexes[i])); //number of units under construction
										
				}

			}
			((MenuUnitProc)unitProcScript).trainingTimes = trainingTimes;
			((MenuUnitProc)unitProcScript).start = true;//starts data processing by background script	
			EraseValues();
		}
		else
		{
			//this.transform.gameObject.SetActive(false); disable is handled from root obj
		}
	}
	private void EraseValues()
	{		
		for (int i = 0; i < trainingIndexes.Length; i++) 
		{			
			if(trainingIndexes[i]>0)
			{
				int a = trainingIndexes[i];		//while unbuilding, trainingIndexes[i] is modified - no longer valid references
				for (int j = 0; j < a; j++)
				{
					UnBuild(i,2);//UnBuild(i,0); #
				} 
			}
		}		
		currentSlidVal = 0;
		timeRemaining = 0;
		currentTimeRemaining = 0;
		hours = minutes = seconds = 0 ; //?totalTime
		queList.Clear ();
		((UILabel)HintLabel).text ="Tap on a unit to summon them and read the description.";	
			
			
	}
		
	public void LoadValuesfromProc()
	{		
		bool queEmpty = true;

		if(((MenuUnitProc)UnitProc.GetComponent("MenuUnitProc")).queList.Count > 0){queEmpty = false;}

		if(!queEmpty)
		{
			//((MenuUnitProc)unitProcScript.GetComponent("MenuUnitProc")).start = false;//			
			currentSlidVal = ((MenuUnitProc)unitProcScript).currentSlidVal;
			currentTrainingTime = ((MenuUnitProc)unitProcScript).currentTrainingTime;

			queList.Clear();
			
			for (int i = 0; i < ((MenuUnitProc)unitProcScript).queList.Count; i++) 
			{					
				queList.Add(((MenuUnitProc)unitProcScript).queList[i]);	
			}
			
			((MenuUnitProc)unitProcScript).queList.Clear();	//reset remote list
			ReBuild();
		}
	}
		
	private void ReBuild()
	{			
		queList.Sort(delegate (Vector3 v1, Vector3 v2)// qIndex, objIndex, trainingIndex
		{
			return v1.x.CompareTo(v2.x);			
		});
														
		for (int i = 0; i < queList.Count; i++) // qIndex, objIndex, trainingIndex
		{			
			for (int j = 0; j < queList[i].z; j++) 
				{
					Build((int)queList[i].y);
				}
		}
		int objIndex = ((QIndex)activeQueList[0].GetComponent("QIndex")).objindex;//the small buttons that get lined up
		((UISlider)((ProgBars[objIndex]).GetComponent("UISlider"))).value = currentSlidVal;
		//currentTimeRemaining = (1-currentSlidVal)*currentTrainingTime;
		UnitProc.SetActive(false);

		UpdateTime ();
	}
	
	void FixedUpdate()
	{
		if(typesCounter>0)
		{
			ProgressBars();	
		}		
		else if(resetLabels)
		{				
			((UILabel)TimeLabel).text ="-";
			((UILabel)FinishPriceLb).text ="-";
			resetLabels = false;			
		}
	}
	
	void Update () 
	{
		
	}
	
	void Build(int i)				
	{							
			resetLabels = true;
		
			bool iInQue = false;
						
			if(((QIndex)QueButtons[i].GetComponent("QIndex")).inque)
			{
				iInQue = true;
			}			
			
			if(iInQue && trainingIndexes[i]<10)				
			{
			trainingIndexes[i]++;
			((UILabel)Counters[i]).text = trainingIndexes[i].ToString();	
			((UILabel)HintLabel).text = units [i] ["Description"];
			//currentTrainingTime = trainingTimes[((QIndex)activeQueList[0].GetComponent("QIndex")).objindex];
			}
			
			else if(trainingIndexes[i]>=10)
			{
				((UILabel)HintLabel).text ="You can train 10 units per type.";
			}
			
			else if(!iInQue && typesCounter < maxTypes)	
			{
				QueButtons[i].SetActive(true);
				trainingIndexes[i]++;
				
				((QIndex)QueButtons[i].GetComponent("QIndex")).inque = true;
				
				((QIndex)QueButtons[i].GetComponent("QIndex")).qindex = typesCounter;			
			
				((UILabel)Counters[i]).text = trainingIndexes[i].ToString();
				
				QueButtons[i].transform.position = new Vector3(OnScreenX[typesCounter],OnScreenY,z);
					
				typesCounter++;
								
			((UILabel)HintLabel).text = units [i] ["Description"];
			//currentTrainingTime = trainingTimes[((QIndex)activeQueList[0].GetComponent("QIndex")).objindex];
			SortQue();
			}		
			else
			{
				((UILabel)HintLabel).text = "You can train 3 unit types at once.";
			}
			
		UpdateTime ();
		
	}
	
	void UnBuild(int i, int action)			// action 0 cancel 1 finished 2 exitmenu
	{	
		if(action == 0)
		{
			hours = minutes = seconds = 0;
			if(units [i] ["GoldBased"] == "true")
			{
				((Stats)StatsCo).gold += int.Parse (units [i] ["ResCost"]); 
			}
			
			else
			{
				((Stats)StatsCo).mana += int.Parse (units [i] ["ResCost"]);
			}
			
			((Stats)StatsCo).update = true;
		}

		if(trainingIndexes[i]>1)
		{
			trainingIndexes[i]--;
			((UILabel)Counters[i]).text = trainingIndexes[i].ToString();
			((UISlider)((ProgBars[i]).GetComponent("UISlider"))).value = 0;	
		}
		else
		{		
			((QIndex)QueButtons[i].GetComponent("QIndex")).inque = false;
			((QIndex)QueButtons[i].GetComponent("QIndex")).qindex = 50;			
			((UISlider)((ProgBars[i]).GetComponent("UISlider"))).value = 0;
			typesCounter--;
			trainingIndexes[i]--;				
			QueButtons[i].transform.position = new Vector3(OffScreenX[i],OffScreenY,z);			
			QueButtons[i].SetActive(false);			
			
			SortQue();
					
			for (int j = 0; j < activeQueList.Count; j++) 
			{						
				activeQueList[j].transform.position = new Vector3(OnScreenX[j],OnScreenY,z);
				((QIndex)activeQueList[j].GetComponent("QIndex")).qindex = j;
			}
		}
		
		switch (action) {
		case 0:
			((UILabel)HintLabel).text ="Training canceled.";
		break;
		case 1:
			((UILabel)HintLabel).text ="Training complete.";
		break;			
		}	
		//ResetCurrentQueTime ();
		UpdateTime ();
	} 

	private void UpdateTime()
	{
		timeRemaining = 0;
		
		for (int i = 0; i < trainingIndexes.Length; i++) 
		{
			timeRemaining += trainingIndexes[i]*trainingTimes[i];
		}
		if(activeQueList.Count>0)
		{
			currentTrainingTime = trainingTimes[((QIndex)activeQueList[0].GetComponent("QIndex")).objindex];
		}
		else
		{
			currentTrainingTime = 0;
		}
		timeRemaining -= currentSlidVal*currentTrainingTime;

		if(timeRemaining>0)//5
		{
			hours = (int)timeRemaining/60;
			minutes = (int)timeRemaining%60;
			seconds = (int)(60 - (currentSlidVal*currentTrainingTime*60)%60);			
		}		
		//hours = (int)hours;
		//minutes = (int)minutes;
		//seconds = (int)seconds;

		if(hours>0 )//&& minutes >0 && seconds>=0 
		{			
			((UILabel)TimeLabel).text = 
				hours.ToString() +" h " +
					minutes.ToString() +" m " +
					seconds.ToString() +" s ";			
		}
		else if(minutes > 0 )//&& seconds >= 0
		{
			((UILabel)TimeLabel).text = 
				minutes.ToString() +" m " +
					seconds.ToString() +" s ";			
		}
		else if(seconds > 0 )
		{
			((UILabel)TimeLabel).text = 
				seconds.ToString() +" s ";
		}
		
		if (timeRemaining >= 4320)	priceInCrystals = 150;
		else if (timeRemaining >= 2880)	priceInCrystals = 70;
		else if (timeRemaining >= 1440)	priceInCrystals = 45;
		else if (timeRemaining >= 600)	priceInCrystals = 30;
		else if (timeRemaining >= 180)	priceInCrystals = 15;
		else if (timeRemaining >= 60)	priceInCrystals = 7;
		else if (timeRemaining >= 30)	priceInCrystals = 3;
		else if (timeRemaining >= 0)	priceInCrystals = 1;
		
		((UILabel)FinishPriceLb).text = priceInCrystals.ToString();
	}
	
private void SortQue()
	{
		
		activeQueList.Clear();	
		
			for (int j = 0; j < QueButtons.Length; j++) 
			{	
				if(((QIndex)QueButtons[j].GetComponent("QIndex")).inque)
				{
					activeQueList.Add(QueButtons[j]);
				}
				
			}
			
			activeQueList.Sort(delegate(GameObject button1, GameObject button2)
			{				
				return ((QIndex)button1.GetComponent("QIndex")).qindex.CompareTo(((QIndex)button2.GetComponent("QIndex")).qindex); 
			});
	}

	private void ProgressBars()
	{
		//Time.deltaTime = 0.016; 60*Time.deltaTime = 1s ; runs at 60fps
		progCounter += Time.deltaTime*0.5f;
		if(progCounter > progTime)
		{			
			SortQue();			
			progCounter = 0;	
						
			int objIndex = ((QIndex)activeQueList[0].GetComponent("QIndex")).objindex;	
			currentTrainingTime = trainingTimes[objIndex];
			//SecMinHours();
			UpdateTime();
			((UISlider)((ProgBars[objIndex]).GetComponent("UISlider"))).value += ((Time.deltaTime)/trainingTimes[objIndex]);

			currentSlidVal = ((UISlider)((ProgBars[objIndex]).GetComponent("UISlider"))).value;
			((UISlider)((ProgBars[objIndex]).GetComponent("UISlider"))).value = 
				Mathf.Clamp(((UISlider)((ProgBars[objIndex]).GetComponent("UISlider"))).value,0,1);
			
			if(((UISlider)((ProgBars[objIndex]).GetComponent("UISlider"))).value==1)
			{ 
				FinishObject();				
			}
		}				
	}
	
	private void FinishObject()
	{		
		int objIndex = ((QIndex)activeQueList[0].GetComponent("QIndex")).objindex;		
		UnBuild(objIndex,1);	
	}

	public void FinishNow()
	{
		if (priceInCrystals <= ((Stats)StatsCo).crystals) 
		{
			((Stats)StatsCo).crystals -= priceInCrystals;	
			((Stats)StatsCo).update = true;
			((UILabel)HintLabel).text ="Training complete.";
			EraseValues();
		} 

		else if(timeRemaining > 0)
		
		{
			((Stats)StatsCo).userMessagesTxt = "Not enough crystals";
			((Stats)StatsCo).initUserMessages = true;
		}
	}
}
