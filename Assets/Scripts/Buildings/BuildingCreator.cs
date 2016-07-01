using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;

public class BuildingCreator : MonoBehaviour {
	
	private const int noOfBuildings = 11;//number of existing buildings ingame 
	private string currentSelection = "";//when a construct building button is pressed, this determines which one
	private int gridx = 256;//necessary to adjust the middle screen "target" to the exact grid X position
	private int gridy = 181;//necessary to adjust the middle screen "target" to the exact grid Y position
	
	
	public UILabel HintText;// a top screen label that displays user messages when necessary
                            //public UILabel UserMessages;
    public NamedPriceLabels[] BuildingPriceLabels;

    //public GameObject[] BuildingPrefabs = new GameObject[noOfBuildings];
    public GameObject ConstructionPrefab;//the building "under construction" sand and materials prefab
	public GameObject BuildingSelectedPanel; //menu that appears when you reselect a finished building - buttons: upgrade, move, ok, cancel

	//public int[] existingBuildings = new int[noOfBuildings]; // necessary to keep track of each buiding type number and enforce conditions
    public Dictionary<string, int> existingBuildings = new Dictionary<string, int>();

    public GameObject gameManager;
	public GameObject UIAnchor;//MainMenu script is there
	public GameObject DummyObj;// a crosshair that follows the middle of the screen, for placing a new building; position is adjusted to exact grid middle point
	public GameObject BuildingsGroup;// to keep all buildings in one place in the ierarchy, they are parented to this empty object
	public GameObject MovingPad;// the arrow pad - when created/selected+move, the buildings are parented to this object that can move

	public GameObject StatsPad;//displays relevant info when a building is reselected
	public UILabel StatsName;//label with the building name(type)
	public UILabel StatsDescription;//text description on StatsPad
	public GameObject StatsCoin;//coin icon
	public GameObject StatsMana;//mana icon
	public GameObject ProductionLabel;//if the building is producing any resource (mana/gold)
	public UILabel StatsGoldProduction;//amount of gold produced per second
	public UILabel StatsManaProduction;//amount of mana produced per second

	public GameObject Grass2x,Grass3x,Grass4x;// the grass prefabs
		
	private GameObject selectedBuilding;//current selected building
	private GameObject selectedGrass;//current selected grass
	private GameObject selectedConstruction;//current selected "under construction" prefab
	
	public bool isReselect = false;//building is under construction or reselected
	public int buildingIndex = -1;//associates the underlying grass with the building on top, so they can be reselected together

	private GameObject[] selectedBuildingType;//necessary to select all buildings of a certain type by tag
	private GameObject[] selectedGrassType;// necessary to select all patches of grass of a certain type by tag
	//private Vector3 selectedPosition;stats
	
	private bool inCollision = false;// prevents placing the building in inaccessible areas/on top of another building
		
	private int buildingZ = 1;//layer depths for building, 3 after parenting; 0 instantiated, 2 after parenting
	private int padZ = -2;//moving pad
	private int grassZ = 2;//grass2
	private bool pivotCorrection = false;//adjusts the position to match the grid
	private bool displacedonZ = false;

	public TextAsset BuildingsXML;//variables for loading building characteristics from XML
	private Dictionary<string,Dictionary<string,string>> buildings = new Dictionary<string, Dictionary<string, string>>();
	private Dictionary<string,string> dictionary;

	public GameObject Stats;//the stats object
	private Component StatsCo;//the stats script

	private GameObject SoundFX;//sound source attached to the camera
	private Component soundFXSc;

    public SaveLoad saveLoad;

	// Use this for initialization
	void Start () {
		GetBuildingsXML();//reads Buildings.xml
		UpdatePrices ();//updates price labels on building buttons
		StatsCo = Stats.GetComponent ("Stats");//conects to Stats script
		SoundFX =  GameObject.Find("SoundFX"); //connects to SoundFx - a sound source near the camera
		soundFXSc = SoundFX.GetComponent ("SoundFX");// gets the SoundFx script
	}

    public Dictionary<string, string> GetCurrentBuildingDictionary()
    {
        return buildings[currentSelection];
    }

	private void GetBuildingsXML()//reads buildings XML
	{
		XmlDocument xmlDoc = new XmlDocument(); 
		xmlDoc.LoadXml(BuildingsXML.text); 
		XmlNodeList buildingsList = xmlDoc.GetElementsByTagName("Building");

		foreach (XmlNode buildingInfo in buildingsList)
		{
			XmlNodeList buildingsContent = buildingInfo.ChildNodes;	
			dictionary = new Dictionary<string, string>();
            string key = "";

			foreach (XmlNode buildingItems in buildingsContent) // levels itens nodes.
			{
                Debug.Log(buildingItems.Name);
                //read everything into the dictionary

                if (buildingItems.Name != "#comment")
                {   if(buildingItems.Name == "Key")
                    {
                        Debug.Log("Key found: " + buildingItems.InnerText);
                        key = buildingItems.InnerText;
                    }
                    dictionary.Add(buildingItems.Name, buildingItems.InnerText);
                }
              
            }
            buildings.Add(key, dictionary);
		}
	}

	private void UpdatePrices()//updates price labels on building buttons
	{
		//for (int i = 0; i < noOfBuildings; i++) 
		//{   //TODO: Figure this out
		//	BuildingPriceLbs[i].text = buildings [i] ["ResCost"];
		//}
        foreach (NamedPriceLabels n in BuildingPriceLabels)
        {
            n.label.text = buildings[n.name]["ResCost"];
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
	//receive a NGUI button message to build
    //the prefab names must be correct the whole way through
    public void OnBuild(GameObject newPet) {
        Debug.Log(newPet.name);
        currentSelection = newPet.name.Trim();
        VerifyConditions(newPet);
    }
	
	//receive a Tk2d button message to select an existing building; the button is in the middle of each building prefab and is invisible 
    public void OnReselect(string selection)
    {
        currentSelection = selection;
        StartCoroutine(ReselectObject(selection));
    }

	public void CancelObject()//cancel construction, or reselect building and destroy/cancel
	{	
		if (!isReselect) 
		{
			((Stats)StatsCo).occupiedDobbitNo--;//frees the dobbit
			if (buildings [currentSelection] ["GoldBased"] == "true")//refunds the gold/mana 
			{
				((Stats)StatsCo).gold += int.Parse (buildings [currentSelection] ["ResCost"]);				
			} 
			else 
			{
				((Stats)StatsCo).mana += int.Parse (buildings [currentSelection] ["ResCost"]);				
			}
			((Stats)StatsCo).update = true;//update stats interface
		}

		else 
		{   //TODO: Remove these later when stripping out functionality
			if (currentSelection == "Barrel") //1=Barrel (mana)
			{
				DecreaseStorage(2);
			}
			else if (currentSelection == "Forge") //4=forge
			{
				((Stats)StatsCo).productionBuildings[0]--;
				DecreaseStorage(1);
			} 
			else if (currentSelection == "Generator") //5=Generator (mana)
			{
				((Stats)StatsCo).productionBuildings[1]--;	
				DecreaseStorage(2);
			}

			else if (currentSelection == "Vault") //9=Vault gold
			{
				DecreaseStorage(1);
			}

			DeactivateStatsPad ();
        }

		((Stats)StatsCo).experience -= int.Parse (buildings [currentSelection] ["XpAward"]);

		Destroy(selectedBuilding);
		existingBuildings [currentSelection]--;//decreases the array which counts how many buildings of each type you have 
		Destroy(selectedGrass);

		//selectedBuilding = null;
		MovingPad.SetActive(false);//deactivates the arrow building moving platform
		BuildingSelectedPanel.SetActive (false);//deactivates the buttons move/upgrade/place/cancel, at the bottom of the screen
		((Relay)gameManager.GetComponent("Relay")).pauseInput = false;//while the building is selected, pressing other buttons has no effect
        if(isReselect){isReselect = false; } //end the reselect state
	
    }

	private void DecreaseStorage(int restype)//when a building is reselected and destroyed, the gold/mana storage capacity decrease; 
	{
		if(restype==1)//gold
		{
			((Stats)StatsCo).maxStorageGold -= int.Parse (buildings [currentSelection] ["StoreCap"]);//the destroyed building storage cap
			if(((Stats)StatsCo).gold > ((Stats)StatsCo).maxStorageGold)//more gold than max storage?
			{
				((Stats)StatsCo).gold = ((Stats)StatsCo).maxStorageGold;//discards resources exceeding storage capacity

			}
		}
		else if (restype==2) //mana
		{
			((Stats)StatsCo).maxStorageMana -= int.Parse (buildings [currentSelection] ["StoreCap"]);
			if(((Stats)StatsCo).mana > ((Stats)StatsCo).maxStorageMana)
			{
				((Stats)StatsCo).mana = ((Stats)StatsCo).maxStorageMana;

			}
		}
		((Stats)StatsCo).update = true;//updates the interface numbers
	}

	//  verifies if the building can be constructed:
	//  exceeds max number of buildings / enough gold/mana/free dobbits to build?
	//  pays the price to Stats; updates the Stats interface numbers
	private void VerifyConditions(GameObject newPet)
	{
		bool canBuild = true;//must pass as true through all verifications
        //max allowed buildings ok?
        if (int.Parse (buildings [currentSelection] ["MaxCap"]) > 0 && //there is a maximum number of buildings permitted for this one; 0 means irrelevant, you can have as many as you want
existingBuildings.GetValueOrInit(currentSelection) >= int.Parse(buildings [currentSelection] ["MaxCap"]))//max already reached
		{					
			canBuild = false;
			((Stats)StatsCo).userMessagesTxt = "Maximum " + buildings [currentSelection] ["MaxCap"] + 
			                                          " buildings of type " +
			                                          buildings [currentSelection] ["Name"];//displays the hint - you can have only 3 buildings of this type
		}

		//enough gold?
		if (buildings [currentSelection] ["GoldBased"] == "true") //this needs gold
		{
			if (((Stats)StatsCo).gold < int.Parse (buildings [currentSelection] ["ResCost"])) 
				{
					canBuild = false;
					((Stats)StatsCo).userMessagesTxt = "Not enough gold";//updates hint text
				}
		} 
		else  //this needs mana; enough mana?
		{
			if(((Stats)StatsCo).mana < int.Parse (buildings [currentSelection] ["ResCost"]))
			{
				canBuild = false;
				((Stats)StatsCo).userMessagesTxt = "Not enough mana";//updates hint text
			}
		}

		if (((Stats)StatsCo).occupiedDobbitNo >= ((Stats)StatsCo).dobbitNo) //dobbit available?
		{
			canBuild = false;
			((Stats)StatsCo).userMessagesTxt = "You need more dobbits.";//updates hint text
		}

		if (canBuild) 
		{
			((MainMenu)UIAnchor.GetComponent("MainMenu")).constructionGreenlit = true;//ready to close menus and place the building; 
			//constructionGreenlit bool necessary because the command is sent by pressing the button anyway
			existingBuildings.GetValueOrInitAndIncrement(currentSelection);//an array that keeps track of how many buildings of each type exist

			((Stats)StatsCo).maxPopulation += int.Parse (buildings [currentSelection] ["PopBonus"]); //increase maxPopulation ; not displayed on interface/taken into account yet - no units
			((Stats)StatsCo).experience += int.Parse (buildings [currentSelection] ["XpAward"]); //increases Stats experience

			//pays the gold/mana price to Stats
			if(buildings [currentSelection] ["GoldBased"] == "true")
			{
				((Stats)StatsCo).gold -= int.Parse (buildings [currentSelection] ["ResCost"]); 
			}
			else
			{
				((Stats)StatsCo).mana -= int.Parse (buildings [currentSelection] ["ResCost"]);
			}

			((Stats)StatsCo).update=true;//tells stats to update the interface - otherwise new numbers are updated but not displayed

            LoadBuilding (newPet);
        } 
		else 
		{
			((MainMenu)UIAnchor.GetComponent("MainMenu")).constructionGreenlit = false;//halts construction - the button message is sent anyway, but ignored
			((Stats)StatsCo).initUserMessages = true; // the hint message is breefly displayed - why can not build
		}
	}

    private void LoadBuilding(GameObject newBuilding)//instantiates the building and grass prefabs
    {
        ((Stats)StatsCo).occupiedDobbitNo++;//get one dobbit

        ((Relay)gameManager.GetComponent("Relay")).pauseInput = true;//pause all other input - the user starts moving the building

        pivotCorrection = false;//used to flag necessary correction so all buildings are centered on the grid square
        
        GameObject NewPet = (GameObject)Instantiate(newBuilding, new Vector3(0, 0, buildingZ), Quaternion.identity);
        string grass = buildings[currentSelection]["GrassSize"];
        if (grass == "2")
        {
            GameObject NewPetGrass = (GameObject)Instantiate(Grass2x, new Vector3(0, 0, grassZ), Quaternion.identity);
        }
        else if (grass == "3")
        {
            GameObject NewPetGrass = (GameObject)Instantiate(Grass3x, new Vector3(0, 0, grassZ), Quaternion.identity);
        }
        else
        {
            GameObject NewPetGrass = (GameObject)Instantiate(Grass4x, new Vector3(0, 0, grassZ), Quaternion.identity);
        }
        pivotCorrection = true;
        SelectObject(currentSelection);
              
    }
    
    private void SelectObject(string buildingTag) //after the grass/building prefabs are instantiated, they must be selected from the existing buildings on the map
	{
		BuildingSelectedPanel.SetActive (true);// the move/upgrade/place/cancel, at the bottom of the screen
		selectedBuildingType = GameObject.FindGameObjectsWithTag(buildingTag);//finds all existing buildings with the apropriate string tag(ex “Forge”)	
		selectedGrassType = GameObject.FindGameObjectsWithTag("Grass");//finds all grass	
			
		//both the grass and buildings are instantiated with the isSelected bool variable as true; 
		//this allows us to find them on the map, as being the new/latest ones.
		foreach (GameObject grass in selectedGrassType) 
		{
		if(((GrassSelector)grass.GetComponent("GrassSelector")).isSelected)				
			{
				selectedGrass = grass;
				break;//found it. exit foreach loop
			}
		}
				
		foreach (GameObject building in selectedBuildingType) 
		{
		if(((BuildingSelector)building.GetComponent("BuildingSelector")).isSelected)				
			{
				//print ("object found");
				
				selectedBuilding = building; //the selected building is registered for the entire class				
				int posX = (int) (DummyObj.transform.position.x-//calculates the middle of the screen - the DummyObj position,
				DummyObj.transform.position.x%gridx); //and adjusts it to match the grid
				int posY = (int)(DummyObj.transform.position.y-
					DummyObj.transform.position.y%gridy);
								
				MovingPad.SetActive(true);//activates the arrow move platform
								
				if(pivotCorrection)					
				{

					selectedBuilding.transform.position = new Vector3(posX+gridx/2, posY, buildingZ-2);//moves the building to position				
					selectedGrass.transform.position = new Vector3(posX+gridx/2, posY, grassZ);	//grass
					MovingPad.transform.position = new Vector3(posX+gridx/2, posY, padZ);//move pad
				}
				
				else
				{
					selectedBuilding.transform.position = new Vector3(posX, posY, buildingZ-2);//the building must appear in front				
					selectedGrass.transform.position = new Vector3(posX, posY, grassZ);	
					MovingPad.transform.position = new Vector3(posX, posY, padZ);
				}
			
				selectedBuilding.transform.parent = MovingPad.transform;//parents the selected building to the arrow moving platform
				selectedGrass.transform.parent = MovingPad.transform;//parents the grass to the move platform
				((Relay)gameManager.GetComponent("Relay")).pauseInput = true;//pause other input so the user can move the building	
				break;//exit foreach loop
			}
		}
	}

	public void ActivateMovingPad()//move pad activated and translated into position
	{
		if (!MovingPad.activeSelf) 
		{
			MovingPad.SetActive (true);
			DeactivateStatsPad ();

			selectedBuilding.transform.parent = MovingPad.transform;
			selectedGrass.transform.parent = MovingPad.transform;

			if (isReselect) 
			{
			selectedGrass.transform.position = new Vector3 (selectedGrass.transform.position.x,
		                                       selectedGrass.transform.position.y,
		                                       selectedGrass.transform.position.z - 0.5f);//move to front

			selectedBuilding.transform.position = new Vector3 (selectedBuilding.transform.position.x,
			                                   selectedBuilding.transform.position.y,
			                                   selectedBuilding.transform.position.z - 2);//move to front
			displacedonZ = true;
			}
		}
	}

	public void ActivateStatsPad()//displays the small stats window
	{
		StatsPad.SetActive (true);

		StatsName.text = buildings [currentSelection] ["Name"];
		StatsDescription.text = buildings [currentSelection] ["Description"];

		ProductionLabel.SetActive (false);
		StatsCoin.SetActive (false);
		StatsMana.SetActive (false);

		if (buildings [currentSelection] ["Name"] == "Gold Forge") 
		{
			ProductionLabel.SetActive (true);
			StatsCoin.SetActive (true);
			StatsGoldProduction.text = buildings [currentSelection] ["ProdPerSec"];
		} 
		else if (buildings [currentSelection] ["Name"] == "Mana Generator") 
		{
			ProductionLabel.SetActive (true);
			StatsMana.SetActive (true);
			StatsManaProduction.text = buildings [currentSelection] ["ProdPerSec"];
		}

		StatsPad.transform.position = selectedBuilding.transform.position;
		//StatsPad.transform.parent = selectedBuilding.transform;
	}

	public void DeactivateStatsPad()
	{
		StatsPad.SetActive (false);
	}

	public void DeactivateStatsPadSound()
	{
		((SoundFX)soundFXSc).Close();
		StatsPad.SetActive (false);
	}


	private IEnumerator ReselectObject(string type)//building reselect
	{	
		((SoundFX)soundFXSc).Click();
		yield return new WaitForSeconds(0.25f);

		BuildingSelectedPanel.SetActive (true);

		selectedBuildingType = GameObject.FindGameObjectsWithTag(type);		
		selectedGrassType = GameObject.FindGameObjectsWithTag("Grass");	
				
		foreach (GameObject building in selectedBuildingType) 
		{
		if(((BuildingSelector)building.GetComponent("BuildingSelector")).isSelected)				
			{
				//print ("object found");

				selectedBuilding = building;

				MovingPad.transform.position = 
					new Vector3(building.transform.position.x,
						building.transform.position.y, padZ);
					
				//MovingPad.SetActive(true);					
				//selectedBuilding.transform.parent = MovingPad.transform;
				((Relay)gameManager.GetComponent("Relay")).pauseInput = true;		
				break;
			}
		}
		
		foreach (GameObject grass in selectedGrassType) 
		{
		if(((GrassSelector)grass.GetComponent("GrassSelector")).grassIndex ==
				((BuildingSelector)selectedBuilding.GetComponent("BuildingSelector")).buildingIndex)				
			{
				selectedGrass = grass;
				//selectedGrass.transform.parent = MovingPad.transform;
				selectedGrass.GetComponentInChildren<GrassCollider>().enabled = true;
				((GrassCollider)selectedGrass.GetComponentInChildren<GrassCollider>()).isMoving = true;
				break;
			}
		}

		ActivateStatsPad ();

	}
	
	public void MoveNW(){Move(0);}//-+
	public void MoveNE(){Move(1);}//++
	public void MoveSE(){Move(2);}//+-
	public void MoveSW(){Move(3);}//--
	public void Cancel(){CancelObject(); saveLoad.SaveGame(); }
	public void OK()
	{
		inCollision = selectedGrass.GetComponentInChildren<GrassCollider>().inCollision;
		if (!inCollision) 
		{
			if(isReselect){	DeactivateStatsPad();}
			PlaceObject ();
            saveLoad.SaveGame();
        }
    }
	
	private void Move(int i)
	{
		((SoundFX)soundFXSc).Move();
		//128x64
		switch (i) 
		{
			case 0:
				MovingPad.transform.position += new Vector3(-gridx/2,gridy/2,0);//NW	
			break;
			
			case 1:
				MovingPad.transform.position += new Vector3(gridx/2,gridy/2,0);//NE		
			break;
			
			case 2:
				MovingPad.transform.position += new Vector3(gridx/2,-gridy/2,0);//SE		
			break;
			
			case 3:
				MovingPad.transform.position += new Vector3(-gridx/2,-gridy/2,0);//SW		
			break;
			
		default:
		break;
		}	
		
		
	}
	
	public void PlaceObject()
	{
		if(!isReselect)
		{
			buildingIndex++;//unique number for pairing the buildings and the grass patches underneath
			((BuildingSelector)selectedBuilding.GetComponent("BuildingSelector")).buildingIndex = buildingIndex;
			((GrassSelector)selectedGrass.GetComponent("GrassSelector")).grassIndex = buildingIndex;


			//instantiates the construction prefab and pass the relevant info;
			GameObject Construction = (GameObject)Instantiate(ConstructionPrefab, selectedBuilding.transform.position, Quaternion.identity);
			selectedConstruction = Construction;
			((ConstructionSelector)selectedConstruction.GetComponent("ConstructionSelector")).constructionIndex = buildingIndex;
			((ConstructionSelector)selectedConstruction.GetComponent("ConstructionSelector")).buildingTime =
				int.Parse (buildings [currentSelection] ["Time"]);
			((ConstructionSelector)selectedConstruction.GetComponent("ConstructionSelector")).storageIncrease=
				int.Parse (buildings [currentSelection] ["StoreCap"]);
			((ConstructionSelector)selectedConstruction.GetComponent("ConstructionSelector")).buildingType=
				((BuildingSelector)selectedBuilding.GetComponent("BuildingSelector")).buildingType;
		}
		
		((Relay)gameManager.GetComponent("Relay")).pauseInput = false;
		((BuildingSelector)selectedBuilding.GetComponent("BuildingSelector")).isSelected = false;
		((GrassSelector)selectedGrass.GetComponent("GrassSelector")).isSelected = false;
		
		((GrassCollider)selectedGrass.GetComponentInChildren<GrassCollider>()).isMoving = false;		
		selectedGrass.GetComponentInChildren<GrassCollider>().enabled = false;		
				
		//-->
		if(!isReselect)
		{
			selectedConstruction.transform.parent = BuildingsGroup.transform;
			selectedGrass.transform.parent = selectedConstruction.transform;

			selectedBuilding.transform.position = new Vector3(selectedBuilding.transform.position.x, selectedBuilding.transform.position.y, selectedBuilding.transform.position.z+2); 

			selectedBuilding.transform.parent = selectedConstruction.transform;
			selectedBuilding.SetActive(false);
		}
		else if(displacedonZ)
		{
			//send the buildings 1 z unit to the background
			selectedGrass.transform.position = new Vector3 (selectedGrass.transform.position.x,
			                                                selectedGrass.transform.position.y,
			                                                selectedGrass.transform.position.z + 0.5f);//move to front
			selectedBuilding.transform.position = new Vector3(selectedBuilding.transform.position.x, 
			                                                  selectedBuilding.transform.position.y, 
			                                                  selectedBuilding.transform.position.z+2); 
			selectedBuilding.transform.parent = BuildingsGroup.transform;
			selectedGrass.transform.parent = selectedBuilding.transform;	
			displacedonZ = false;
		}
		//<--		
		MovingPad.SetActive(false);
		BuildingSelectedPanel.SetActive (false);
		isReselect = false;
		//((UISprite)BkActive[currentSelection]).color = Color.white;	

	}

    //TODO:Remove this?
    public void CollectTaskReward()
    {
        if (buildings[currentSelection]["GoldReward"] == "true")
        {
            ((Stats)StatsCo).gold += int.Parse(buildings[currentSelection]["TaskReward"]);
        }
        else
        {
            ((Stats)StatsCo).mana += int.Parse(buildings[currentSelection]["TaskReward"]);
        }
    }
}