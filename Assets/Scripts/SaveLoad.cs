using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.IO;
using System.Text;

public class SaveLoad : MonoBehaviour
{

    private string filePath;
    private string fileName = "SOMSave";
    private string fileExt = ".txt";

    //private const int buildingTypesNo = 11;//the building tags
    //private string[] buildingTypes = new string[buildingTypesNo]{"Academy","Barrel","Chessboard","Classroom","Forge",
    //    "Generator","Globe","Summon","Toolhouse","Vault","Workshop"};

    public GameObject[] BuildingPrefabs;
    private Dictionary<string, GameObject> BuildingPrefabsDict = new Dictionary<string, GameObject>();

    private const int grassTypesNo = 3;
    public GameObject[] GrassPrefabs = new GameObject[grassTypesNo];//three types of grass patches

    public GameObject ConstructionPrefab;

    public GameObject UnitProc; //the data for units under construction is extracted from unitproc, 
                                //that handles construction in a much simpler manner, having no graphics
    private Component unitProcSc;//script for the object above

    private List<GameObject[]> buildingList = new List<GameObject[]>();
    private GameObject[] Grass;
    private GameObject[] Construction;

    public GameObject MenuUnit;//the object that holds the MenuUnit script
    public GameObject BuildingsGroup;//object used to parent the buildings, once they are instantiated
    public GameObject BuildingCreator;//the object that holds the BuildingCreator.cs script
    public GameObject ExerciseManager;
    public GameObject Stats;//object that holds the HUD data - Heads Up Display
    public GameObject HomeExpansionMenu;
    public GameObject UserData;
    private Component statsSc;//script for the above
    private Component menuUnitSc;//script
    private const int noOfBuildings = 11;
    public Dictionary<string, int> existingBuildings = new Dictionary<string, int>();//the entire array is transfered to BuildingCreator.cs;                                                                                  //records how many buildings of each type, when they are built/game is loaded

    //lists for these elements - unknown number of elements
    private List<GameObject> LoadedBuildings = new List<GameObject>();
    private List<GameObject> LoadedConstructions = new List<GameObject>();
    private List<GameObject> LoadedGrass = new List<GameObject>();

    private int buildingZ = 1;//layer depths for building, correlate with BuildingCreator.cs
    private int grassZ = 2;//layer depths for grass, correlate with BuildingCreator.cs

    private DateTime saveDateTime, loadDateTime;//saveTime, currentTime
    private TimeSpan timeDifference;

    private bool oneLoad = true;

    // Use this for initialization
    void Start()
    {
        // Add building tags to a dictionary.
        foreach (GameObject building in BuildingPrefabs)
        {
            if (building.tag != null)
            {
                BuildingPrefabsDict.Add(building.tag, building);
            }
        }

        //filePath = Application.dataPath + "/";//doesn't work on iphone
        filePath = Application.persistentDataPath + "/";

        unitProcSc = UnitProc.GetComponent("MenuUnitProc");//UnitProc.sc script
        menuUnitSc = MenuUnit.GetComponent("MenuUnit");//MenuUnit.sc - necessary when saving only
        statsSc = Stats.GetComponent("Stats");// Stats.cs for HUD data

        loadDateTime = System.DateTime.Now;//current time

        //TODO: Add back in for full build.
        //FileInfo file = new FileInfo(filePath + fileName + fileExt);
        //if (file.Exists && file.Length != 0)
        //{
        //    LoadGame();
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void SaveGame()
    {
        StreamWriter sWriter = new StreamWriter(filePath + fileName + fileExt);
        ReadObjects();//reads all buildings/grass/under construction
                      //headers - the data structure in the file
        sWriter.WriteLine("Buildings: buildingType, buildingIndex, position.x, position.y, HP");
        sWriter.WriteLine("Grass: grassType, grassIndex, position.x, position.y");
        sWriter.WriteLine("Construction: buildingType, constructionIndex, buildingTime, remainingTime, storageIncrease, position.x, position.y");
        sWriter.WriteLine("Units: currentSlidVal, currentTrainingTime");
        sWriter.WriteLine("Units: trainingTimes array");
        sWriter.WriteLine("Units: queList = qIndex, objIndex, trainingIndex");
        sWriter.WriteLine("Stats: experience,dobbits,occupiedDobits,gold,mana,crystal,maxStorageGold,maxStorageMana,maxCrystals, productionRateGold,productionRateMana");

        sWriter.WriteLine("###Buildings###");
        for (int i = 0; i < buildingList.Count; i++)
        {
            print("Something in list");
            GameObject[] buildingArray = buildingList[i];

            for (int j = 0; j < buildingArray.Length; j++)
            {
                Component BSel = buildingArray[j].GetComponent("BuildingSelector");

				int hp = -1; // Need this so decorations don't break.
				Happiness hap = (Happiness) buildingArray [j].GetComponent ("Happiness");
				if (hap != null) {
					hp = hap.hp;
				}

                sWriter.WriteLine(((BuildingSelector)BSel).buildingType + "," +
                                  ((BuildingSelector)BSel).buildingIndex.ToString() + "," +
                                  buildingArray[j].transform.position.x + "," +
                                  buildingArray[j].transform.position.y + "," +
								  hp
                                  );
            }
        }

        sWriter.WriteLine("###Grass###");
        for (int i = 0; i < Grass.Length; i++)
        {
            Component GSel = Grass[i].GetComponent("GrassSelector");
            sWriter.WriteLine(((GrassSelector)GSel).grassType + "," +
                              ((GrassSelector)GSel).grassIndex.ToString() + "," +
                              Grass[i].transform.position.x + "," +
                              Grass[i].transform.position.y
                              );
        }

        sWriter.WriteLine("###Construction###");
        for (int i = 0; i < Construction.Length; i++)
        {
            Component CSel = Construction[i].GetComponent("ConstructionSelector");
            sWriter.WriteLine(((ConstructionSelector)CSel).buildingType + "," +
                              ((ConstructionSelector)CSel).constructionIndex.ToString() + "," +
                              ((ConstructionSelector)CSel).buildingTime + "," +
                              ((ConstructionSelector)CSel).remainingTime + "," +
                              ((ConstructionSelector)CSel).storageIncrease + "," +
                              Construction[i].transform.position.x + "," +
                              Construction[i].transform.position.y
                              );
        }

        sWriter.WriteLine("###BuildingIndex###");//the unique id for buildings/grass patches

        sWriter.WriteLine(((BuildingCreator)BuildingCreator.GetComponent("BuildingCreator")).buildingIndex);

        //sWriter.WriteLine ("###Units###");

        sWriter.WriteLine(((MenuUnitProc)unitProcSc).currentSlidVal.ToString("0.00") + "," +
                           (((MenuUnitProc)unitProcSc).currentTrainingTime)
                           );

        //  + ","+ ((MenuUnitProc)unitProcSc).timeRemaining.ToString("0.00")    
        const int numberOfUnits = 12;
        int[] trainingTimes = new int[numberOfUnits];//an array that holds training times from all units - 
                                                     //at first load, the XML will not have been read 
                                                     //int[] trainingIndexes = new int[numberOfUnits];

        trainingTimes = ((MenuUnitProc)unitProcSc).trainingTimes;//replace our empty array with the xml values, already in unitproc
                                                                 //trainingIndexes = ((MenuUnitProc)unitProcSc).trainingIndexes;

        sWriter.WriteLine(String.Join(",", new List<int>(trainingTimes).ConvertAll(i => i.ToString()).ToArray()));

        //qIndex, objIndex, trainingIndex  
        //0  5  10 
        // 0 = first position in queue ; 5 = object index - the fifth button/unit type ; 10 = number of units under construction

        List<Vector3> queList = new List<Vector3>();
        queList = ((MenuUnitProc)unitProcSc).queList;

        for (int i = 0; i < queList.Count; i++)
        {
            sWriter.WriteLine(queList[i].ToString().Trim(new Char[] { ')', '(' }));
        }

        sWriter.WriteLine("###Stats###");

        HomeExpansionMenu menu = (HomeExpansionMenu)HomeExpansionMenu.GetComponent("HomeExpansionMenu");
        BoolToKey[] cStatus = menu.ColliderStatus;
        for (int i = 0; i < cStatus.Length; i++)
        {
            print("saving line: " + cStatus[i].name + "," + cStatus[i].status);
            sWriter.WriteLine(cStatus[i].name + "," + cStatus[i].status);
        }
        sWriter.WriteLine("###HomeExpansion###");

        UserData usr = (UserData)UserData.GetComponent("UserData");

        sWriter.WriteLine("Tasks Finished," + usr.TasksFinished);
        sWriter.WriteLine("Time in Tasks," + usr.TimeInTasks);
        sWriter.WriteLine("Gold Earned," + usr.GoldEarned);
        sWriter.WriteLine("Gold Spent," + usr.GoldSpent);
        sWriter.WriteLine("Pets Bought," + usr.PetsBought);

        sWriter.WriteLine("###UserData###");

        sWriter.WriteLine(((Stats)statsSc).experience + "," +
                           ((Stats)statsSc).dobbitNo + "," +
                           ((Stats)statsSc).occupiedDobbitNo + "," +
                           ((Stats)statsSc).gold + "," +
                           ((Stats)statsSc).mana + "," +
                           ((Stats)statsSc).crystals + "," +
                           ((Stats)statsSc).maxStorageGold + "," +
                           ((Stats)statsSc).maxStorageMana + "," +
                           ((Stats)statsSc).maxCrystals + "," +
                           ((Stats)statsSc).productionRates[0] + "," +//Forge gold - production per second
                           ((Stats)statsSc).productionRates[1]//Generator mana
                           );

        sWriter.WriteLine(System.DateTime.Now);
        
        sWriter.WriteLine("###EndofFile###");

        sWriter.Flush();
        sWriter.Close();
        existingBuildings = new Dictionary<string, int>(); //reset for next save - remove if automatic
        ExerciseManager ex = (ExerciseManager)ExerciseManager.GetComponent("ExerciseManager");

        PlayerPrefs.SetInt("claimedBonus", ex.claimedDailyBonus);
        PlayerPrefs.SetString("lastSavedDate", DateTime.Today.ToLongDateString());
    }

    private void ReadObjects()//reads all buildings/grass/under construction
    {
        buildingList.Clear();

        for (int i = 0; i < BuildingPrefabs.Length; i++) //find all buildings
        {
            buildingList.Add(GameObject.FindGameObjectsWithTag(BuildingPrefabs[i].name));
        }

        Grass = GameObject.FindGameObjectsWithTag("Grass");//finds all patches of grass from underneath the buildings
        Construction = GameObject.FindGameObjectsWithTag("Construction");//find all buildings under construction
    }

    public void LoadGame()
    {
        if (!oneLoad) { return; }//prevents loading twice, since there are no safeties and the procedure should be automated at startup, not button triggered
        oneLoad = false;

        StreamReader sReader = new StreamReader(filePath + fileName + fileExt);

        LoadedBuildings.Clear();
        LoadedConstructions.Clear();
        LoadedGrass.Clear();

        string currentLine = "";

        while (currentLine != "###Buildings###")
        {
            currentLine = sReader.ReadLine();//goes through the headers, without doing anything, till it finds the ###Buildings### header
        }

        while (currentLine != "###Grass###") //Buildings - read till next header is found 
        {
            //Buildings: buildingType, buildingIndex, position.x, position.y, hp
            currentLine = sReader.ReadLine();
            if (currentLine != "###Grass###") //if next category reached, skip
            {
                string[] currentBuilding = currentLine.Split(","[0]);

                float posX = float.Parse(currentBuilding[2]);
                float posY = float.Parse(currentBuilding[3]);

				int hp = Int32.Parse (currentBuilding [4]);

                // Name of the building
                string buildingType = currentBuilding[0];

                // Instantiate the building if it is a valid building type.
                if (BuildingPrefabsDict.ContainsKey(buildingType))
                {
                    GameObject Barrel = (GameObject)Instantiate(BuildingPrefabsDict[buildingType], new Vector3(posX, posY, buildingZ), Quaternion.identity);
                    Happiness petHappiness = (Happiness)Barrel.GetComponent("Happiness");
                    petHappiness.hp = hp;
                    petHappiness.needsReinit = true;
					ProcessBuilding(buildingType, int.Parse(currentBuilding[1]));//tag + building index
                    existingBuildings.GetValueOrInitAndIncrement(buildingType);//a local array that holds how many buildings of each type
                }
            }
        }

        while (currentLine != "###Construction###") //loads the grass patches, for both buildings and underconstruction
        {
            //Grass: grassType, grassIndex, position.x, position.y
            currentLine = sReader.ReadLine();
            if (currentLine != "###Construction###")
            {
                string[] currentGrass = currentLine.Split(","[0]);//reads the line, values separated by ","

                float posX = float.Parse(currentGrass[2]);
                float posY = float.Parse(currentGrass[3]);

                switch (int.Parse(currentGrass[0]))
                {
                    case 2:
                        GameObject Grass2x = (GameObject)Instantiate(GrassPrefabs[0], new Vector3(posX, posY, grassZ), Quaternion.identity);
                        ProcessGrass(int.Parse(currentGrass[0]), int.Parse(currentGrass[1]));
                        break;
                    case 3:
                        GameObject Grass3x = (GameObject)Instantiate(GrassPrefabs[1], new Vector3(posX, posY, grassZ), Quaternion.identity);
                        ProcessGrass(int.Parse(currentGrass[0]), int.Parse(currentGrass[1]));
                        break;
                    case 4:
                        GameObject Grass4x = (GameObject)Instantiate(GrassPrefabs[2], new Vector3(posX, posY, grassZ), Quaternion.identity);
                        ProcessGrass(int.Parse(currentGrass[0]), int.Parse(currentGrass[1]));
                        break;
                }
            }
        }
        while (currentLine != "###BuildingIndex###") //Construction
        {
            //Construction: buildingType, constructionIndex, buildingTime, remainingTime, storageIncrease, position.x, position.y
            currentLine = sReader.ReadLine();
            if (currentLine != "###BuildingIndex###")
            {
                string[] currentConstruction = currentLine.Split(","[0]);//reads the line, values separated by ","

                float posX = float.Parse(currentConstruction[5]);
                float posY = float.Parse(currentConstruction[6]);

                GameObject Construction = (GameObject)Instantiate(ConstructionPrefab, new Vector3(posX, posY, buildingZ), Quaternion.identity);

                string buildingType = currentConstruction[0];

                // Instantiate building under construction.
                if (BuildingPrefabsDict.ContainsKey(buildingType))
                {
                    GameObject building = (GameObject)Instantiate(BuildingPrefabsDict[buildingType], new Vector3(posX, posY, buildingZ), Quaternion.identity);
                    ProcessConstruction(buildingType, int.Parse(currentConstruction[1]), int.Parse(currentConstruction[2]), int.Parse(currentConstruction[3]), int.Parse(currentConstruction[4]));
                    existingBuildings.GetValueOrInitAndIncrement(buildingType);
                }
            }
        }

        ParentBuildings();

        currentLine = sReader.ReadLine();
        ((BuildingCreator)BuildingCreator.GetComponent("BuildingCreator")).buildingIndex = int.Parse(currentLine);

        //UNITS
        currentLine = sReader.ReadLine();//#Add verification for empty que
        UnitProc.SetActive(true);

        string[] currentUnitinProgress = currentLine.Split(","[0]);

        ((MenuUnitProc)unitProcSc).currentSlidVal = float.Parse(currentUnitinProgress[0]);
        ((MenuUnitProc)unitProcSc).currentTrainingTime = int.Parse(currentUnitinProgress[1]);
        //((MenuUnitProc)unitProcSc).timeRemaining = float.Parse (currentUnitinProgress [2]);

        currentLine = sReader.ReadLine();
        string[] trainingTimes = currentLine.Split(","[0]);

        for (int i = 0; i < trainingTimes.Length; i++)
        {
            ((MenuUnitProc)unitProcSc).trainingTimes[i] = int.Parse(trainingTimes[i]);
        }

        ((MenuUnitProc)unitProcSc).queList.Clear();

        while (currentLine != "###Stats###")
        {
            currentLine = sReader.ReadLine();
            if (currentLine != "###Stats###")
            {
                string[] unitQue = currentLine.Split(","[0]);

                if (currentLine != "###Stats###")
                {
                    ((MenuUnitProc)unitProcSc).queList.Add(new Vector3(
                        float.Parse(unitQue[0]), float.Parse(unitQue[1]),
                        float.Parse(unitQue[2])));
                }
            }
        }
        

        HomeExpansionMenu menu = (HomeExpansionMenu)HomeExpansionMenu.GetComponent("HomeExpansionMenu");

        while (currentLine != "###HomeExpansion###")
        {
            print(currentLine);
            currentLine = sReader.ReadLine();
            if (currentLine != "###HomeExpansion###")
            {
                string[] colliderStatusList = currentLine.Split(","[0]);
                print("in the file " + colliderStatusList[0]);
                for (int i = 0; i < menu.ColliderStatus.Length; i++)
                {
                    if (menu.ColliderStatus[i].name.Equals(colliderStatusList[0]))
                    {
                        menu.ColliderStatus[i].status = Convert.ToBoolean(colliderStatusList[1]);
                        if (!Convert.ToBoolean(colliderStatusList[1]))
                        {
                            //Delete the collider
                            Destroy(GameObject.Find(colliderStatusList[0]));
                        }
                    }
                }
            }
        }

        UserData usr = (UserData)UserData.GetComponent("UserData");
        //while (currentLine != "###UserData###")
        //{
        //    currentLine = sReader.ReadLine();
        //    if (currentLine != "###UserData###")
        //    {

        //    }
        //}
        String[] s = sReader.ReadLine().Split(',');
        usr.TasksFinished = Int32.Parse(s[1]);
        s = sReader.ReadLine().Split(',');
        usr.TimeInTasks = float.Parse(s[1]);
        s = sReader.ReadLine().Split(',');
        usr.GoldEarned = Int32.Parse(s[1]);
        s = sReader.ReadLine().Split(',');
        usr.GoldSpent = Int32.Parse(s[1]);
        s = sReader.ReadLine().Split(',');
        usr.PetsBought = Int32.Parse(s[1]);

        currentLine = sReader.ReadLine();


        ((MenuUnitProc)unitProcSc).start = true;
        ((MenuUnit)menuUnitSc).unitProcScript = unitProcSc;
        ((MenuUnit)menuUnitSc).GetUnitsXML();

        //Stats: experience,dobbits,occupiedDobbit,gold,mana,crystal,,maxStorageGold,maxStroageMana,maxCrystals,forgeRates,generatorRates
        currentLine = sReader.ReadLine();

        string[] stats = currentLine.Split(","[0]);

        ((Stats)statsSc).experience = int.Parse(stats[0]);
        ((Stats)statsSc).dobbitNo = int.Parse(stats[1]);
        ((Stats)statsSc).occupiedDobbitNo = int.Parse(stats[2]);

        ((Stats)statsSc).gold = float.Parse(stats[3]);
        ((Stats)statsSc).mana = float.Parse(stats[4]);
        ((Stats)statsSc).crystals = int.Parse(stats[5]);

        ((Stats)statsSc).maxStorageGold = int.Parse(stats[6]);
        ((Stats)statsSc).maxStorageMana = int.Parse(stats[7]);
        ((Stats)statsSc).maxCrystals = int.Parse(stats[8]);

        ((Stats)statsSc).productionRates[0] = float.Parse(stats[9]);
        ((Stats)statsSc).productionRates[1] = float.Parse(stats[10]);
        ((Stats)statsSc).update = true;

        currentLine = sReader.ReadLine();
        saveDateTime = DateTime.Parse(currentLine);

        CalculateElapsedTime();

        print("current time " + loadDateTime.ToString());
        print("saved time " + saveDateTime.ToString());

        sReader.Close();

        ExerciseManager ex = (ExerciseManager)ExerciseManager.GetComponent("ExerciseManager");
        DateTime lastSave = DateTime.Parse(PlayerPrefs.GetString("lastSavedDate"));
        int claimed = PlayerPrefs.GetInt("claimedBonus");

        ex.SetClaimedBonus(claimed, lastSave);
    }



    private void ProcessBuilding(string buildingTag, int buildingIndex)
    {
        GameObject[] selectedBuildingType = GameObject.FindGameObjectsWithTag(buildingTag);

        foreach (GameObject building in selectedBuildingType)
        {
            if (((BuildingSelector)building.GetComponent("BuildingSelector")).isSelected)//isSelected is true at initialization				
            {
                ((BuildingSelector)building.GetComponent("BuildingSelector")).buildingIndex = buildingIndex;//unique int to pair buildings and the grass underneath
                ((BuildingSelector)building.GetComponent("BuildingSelector")).inConstruction = false;
                ((BuildingSelector)building.GetComponent("BuildingSelector")).isSelected = false;
                LoadedBuildings.Add(building);//the list is sorted and then used to pair the buildings and the grass by index
                                              //all grass is recorded in the same list, for both buildings and constructions; after they are sorted by index, 
                                              //the first batch goes to finished buildings, the rest goes to underconstruction
                break;
            }
        }
    }

    private void ProcessGrass(int grassType, int grassIndex)
    {
        GameObject[] selectedGrassType = GameObject.FindGameObjectsWithTag("Grass");

        foreach (GameObject grass in selectedGrassType)
        {
            if (((GrassSelector)grass.GetComponent("GrassSelector")).isSelected)
            {
                ((GrassSelector)grass.GetComponent("GrassSelector")).grassIndex = grassIndex;

                ((GrassCollider)grass.GetComponentInChildren<GrassCollider>()).selectedGrass = grass;
                ((GrassCollider)grass.GetComponentInChildren<GrassCollider>()).singleTiles = grass.GetComponentsInChildren<tk2dSprite>();
                ((GrassCollider)grass.GetComponentInChildren<GrassCollider>()).tiledTiles = grass.GetComponentsInChildren<tk2dTiledSprite>();
                ((GrassCollider)grass.GetComponentInChildren<GrassCollider>()).myGreen =
                    ((tk2dSprite)((GrassCollider)grass.GetComponentInChildren<GrassCollider>()).singleTiles[0]).color;

                ((GrassCollider)grass.GetComponentInChildren<GrassCollider>()).isMoving = false;
                grass.GetComponentInChildren<GrassCollider>().enabled = false;

                ((GrassSelector)grass.GetComponent("GrassSelector")).isSelected = false;

                LoadedGrass.Add(grass);
                break;
            }
        }

    }

    private void ProcessConstruction(string buildingTag, int buildingIndex, int buildingTime, int remainingTime, int storageIncrease)
    {
        GameObject[] selectedBuildingType = GameObject.FindGameObjectsWithTag(buildingTag);
        GameObject[] selectedConstructionType = GameObject.FindGameObjectsWithTag("Construction");

        foreach (GameObject construction in selectedConstructionType)
        {
            if (((ConstructionSelector)construction.GetComponent("ConstructionSelector")).isSelected)
            {
                ((ConstructionSelector)construction.GetComponent("ConstructionSelector")).buildingType = buildingTag;
                ((ConstructionSelector)construction.GetComponent("ConstructionSelector")).constructionIndex = buildingIndex;
                ((ConstructionSelector)construction.GetComponent("ConstructionSelector")).buildingTime = buildingTime;
                ((ConstructionSelector)construction.GetComponent("ConstructionSelector")).remainingTime = remainingTime;
                ((ConstructionSelector)construction.GetComponent("ConstructionSelector")).storageIncrease = storageIncrease;
                ((ConstructionSelector)construction.GetComponent("ConstructionSelector")).isSelected = false;
                LoadedConstructions.Add(construction);
                break;
            }
        }

        foreach (GameObject building in selectedBuildingType)
        {
            if (((BuildingSelector)building.GetComponent("BuildingSelector")).isSelected)
            {
                ((BuildingSelector)building.GetComponent("BuildingSelector")).buildingIndex = buildingIndex;
                ((BuildingSelector)building.GetComponent("BuildingSelector")).isSelected = false;
                LoadedBuildings.Add(building);
                break;
            }
        }
    }

    private void ParentBuildings()
    {
        /*
		queList.Sort(delegate (Vector3 v1, Vector3 v2)// qIndex, objIndex, trainingIndex
		             {
			return v1.x.CompareTo(v2.x);			
		});
		*/
        LoadedBuildings.Sort(delegate (GameObject g1, GameObject g2)
        {
            return ((BuildingSelector)g1.GetComponent("BuildingSelector")).buildingIndex.CompareTo(((BuildingSelector)g2.GetComponent("BuildingSelector")).buildingIndex);
        });

        LoadedConstructions.Sort(delegate (GameObject g1, GameObject g2)
        {
            return ((ConstructionSelector)g1.GetComponent("ConstructionSelector")).constructionIndex.CompareTo(((ConstructionSelector)g2.GetComponent("ConstructionSelector")).constructionIndex);
        });

        LoadedGrass.Sort(delegate (GameObject g1, GameObject g2)
        {
            return ((GrassSelector)g1.GetComponent("GrassSelector")).grassIndex.CompareTo(((GrassSelector)g2.GetComponent("GrassSelector")).grassIndex);
        });

        int constructionIndex = 0;

        for (int i = 0; i < LoadedBuildings.Count; i++)
        {
            if (!((BuildingSelector)LoadedBuildings[i].GetComponent("BuildingSelector")).inConstruction)
            {
                LoadedGrass[i].transform.parent = LoadedBuildings[i].transform;
                LoadedBuildings[i].transform.parent = BuildingsGroup.transform;
            }
            else
            {
                LoadedGrass[i].transform.parent = LoadedConstructions[constructionIndex].transform;
                LoadedBuildings[i].transform.parent = LoadedConstructions[constructionIndex].transform;
                LoadedBuildings[i].SetActive(false);
                LoadedConstructions[constructionIndex].transform.parent = BuildingsGroup.transform;
                constructionIndex++;
            }
        }
        //((BuildingCreator)BuildingCreator.GetComponent ("BuildingCreator")).buildingIndex = LoadedBuildings.Count-1;//##
        ((BuildingCreator)BuildingCreator.GetComponent("BuildingCreator")).existingBuildings = existingBuildings;
    }

    private void CalculateElapsedTime()
    {
        timeDifference = loadDateTime.Subtract(saveDateTime);

        //everything converted to minutes
        int elapsedTime = timeDifference.Days * 24 * 60 + timeDifference.Hours * 60 + timeDifference.Minutes;

        //some production buildings have finished a while ago; needed for subsequent production amount
        //int[] unfinishedProductionBuildings = new int[2];
        List<int> finishTimesGold = new List<int>();
        List<int> finishTimesMana = new List<int>();
        finishTimesGold.Clear();
        finishTimesMana.Clear();

        GameObject[] constructionsInProgress = GameObject.FindGameObjectsWithTag("Construction");

        for (int i = 0; i < constructionsInProgress.Length; i++)
        {
            int buildingTime = ((ConstructionSelector)constructionsInProgress[i].GetComponent("ConstructionSelector")).buildingTime;
            int remainingTime = ((ConstructionSelector)constructionsInProgress[i].GetComponent("ConstructionSelector")).remainingTime;

            Component slider = constructionsInProgress[i].GetComponentInChildren(typeof(UISlider));

            if (elapsedTime >= remainingTime)
            {
                ((UISlider)slider.GetComponent("UISlider")).value = 1;
                ((ConstructionSelector)constructionsInProgress[i].GetComponent("ConstructionSelector")).progCounter = 1.1f;

                if (((ConstructionSelector)constructionsInProgress[i].GetComponent("ConstructionSelector")).buildingType == "Forge")
                {
                    finishTimesGold.Add(elapsedTime - remainingTime);//add the time passed after the building was finished

                }
                else if (((ConstructionSelector)constructionsInProgress[i].GetComponent("ConstructionSelector")).buildingType == "Generator")
                {
                    finishTimesMana.Add(elapsedTime - remainingTime);//add the time passed after the building was finished
                }
                //print("elapsedTime-remainingTime = " + (elapsedTime-remainingTime).ToString());
            }
            else
            {//everything under 1 minute will appear as finished at reload - int approximation, not an error
                ((UISlider)slider.GetComponent("UISlider")).value += (float)elapsedTime / (float)buildingTime;

            }
        }

        //Calculate the progession in unit construction que

        int substractTime = elapsedTime;

        List<Vector3> queList = new List<Vector3>();    //qIndex, objIndex, trainingIndex

        queList = ((MenuUnitProc)unitProcSc).queList;
        queList.Sort(delegate (Vector3 v1, Vector3 v2) { return v1.x.CompareTo(v2.x); });

        const int numberOfUnits = 12;
        int[] trainingTimes = new int[numberOfUnits];
        trainingTimes = ((MenuUnitProc)unitProcSc).trainingTimes;

        int currentTrainingTime;

        for (int i = 0; i < queList.Count; i++)
        {
            if (substractTime > 0)
            {
                currentTrainingTime = trainingTimes[(int)queList[i].y];
                int trainingIndex = (int)queList[i].z;

                while (trainingIndex > 0)
                {
                    if (substractTime > currentTrainingTime)
                    {
                        substractTime -= currentTrainingTime;
                        trainingIndex--;
                        ((MenuUnitProc)unitProcSc).timeRemaining -= currentTrainingTime;
                        queList[i] = new Vector3(queList[i].x, queList[i].y, trainingIndex);
                    }
                    else
                    {
                        ((MenuUnitProc)unitProcSc).currentTrainingTime = currentTrainingTime;
                        ((MenuUnitProc)unitProcSc).currentSlidVal += (float)substractTime / (float)currentTrainingTime;

                        if (currentTrainingTime - substractTime > 0)
                        {
                            ((MenuUnitProc)unitProcSc).timeRemaining = currentTrainingTime - substractTime;
                        }
                        else
                        {
                            ((MenuUnitProc)unitProcSc).timeRemaining = 1;
                        }

                        queList[i] = new Vector3(queList[i].x, queList[i].y, trainingIndex);
                        substractTime = 0;
                        break;
                    }
                }
            }
            else
            { break; }
        }

        bool allZero = true;

        for (int i = 0; i < queList.Count; i++)
        {
            if ((int)queList[i].z != 0)
            {
                allZero = false;
                break;
            }
        }

        if (allZero)
        {
            ((MenuUnitProc)unitProcSc).queList.Clear();
            UnitProc.SetActive(false);

        }
        else
        {
            ((MenuUnitProc)unitProcSc).queList = queList;
            ((MenuUnitProc)unitProcSc).start = true;//##remove irrelevant
        }

        //elapsedTime - minutes

        //the existingBuildings array holds accurate finished/unfinished buildings number; substract unfinished 

        if (existingBuildings.GetValueOrInit("Forge") - finishTimesGold.Count > 0)
        {
            ((Stats)statsSc).gold += (existingBuildings.GetValueOrInit("Forge") - finishTimesGold.Count) * ((Stats)statsSc).productionRates[0] * elapsedTime * 60;
        }

        if (existingBuildings.GetValueOrInit("Generator") - finishTimesMana.Count > 0)
        {
            ((Stats)statsSc).mana += (existingBuildings.GetValueOrInit("Generator") - finishTimesMana.Count) * ((Stats)statsSc).productionRates[1] * elapsedTime * 60;
        }


        for (int i = 0; i < finishTimesGold.Count; i++)
        {
            ((Stats)statsSc).gold += finishTimesGold[i] * ((Stats)statsSc).productionRates[0] * 60;
        }

        for (int i = 0; i < finishTimesMana.Count; i++)
        {
            ((Stats)statsSc).mana += finishTimesMana[i] * ((Stats)statsSc).productionRates[1] * 60;
        }

        StartCoroutine(LateApplyMaxCaps());//some data reaches stats with latency
        ((Stats)statsSc).update = true;//updates numbers - called only after changes - once a second because of production
    }

    private IEnumerator LateApplyMaxCaps()
    {
        yield return new WaitForSeconds(0.50f);
        ((Stats)statsSc).ApplyMaxCaps();
        ((Stats)statsSc).update = true;//one-time interface update
    }

}


