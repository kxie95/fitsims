using UnityEngine;
using System.Collections;

public class BuildingSelector : MonoBehaviour {//attached to each building as an invisible 2dtoolkit button
	
	public bool isSelected = true;
	public bool inConstruction = true;//only for load/save
    public int reSelectNumber;

	public int buildingIndex = -1;	
	public string buildingType;
    //This is the position of this particular building in the building.xml file
    //This solution isnt the greatest but works for now, and I think its better than having a whole bunch of methods
    public int selectionIndex;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void ReSelect()
	{
		//((Relay)gameManager.GetComponent("Relay")).buildingFloating
		GameObject gameManager = GameObject.Find("GameManager");
		GameObject buildingCreator = GameObject.Find("BuildingCreator");
        GameObject playerManager = GameObject.Find("PlayerManager");


		Component relayScript = (Relay)gameManager.GetComponent("Relay");
		Component buildingCreatorScript = (BuildingCreator)buildingCreator.GetComponent("BuildingCreator");
        Component playerManagerScript = (PlayerManager)playerManager.GetComponent("PlayerManager");

        

		if(!((BuildingCreator)buildingCreatorScript).isReselect &&
			!((Relay)relayScript).pauseInput)
		{
			isSelected = true;				
			((BuildingCreator)buildingCreatorScript).isReselect = true;

            ((BuildingCreator)buildingCreatorScript).OnReselect(reSelectNumber, buildingType);

            //WHY
            //switch (buildingType) 
            //{
            //	case "Academy":
            //	((BuildingCreator)buildingCreatorScript).OnReselect0();
            //	break;

            //	case "Barrel":
            //	((BuildingCreator)buildingCreatorScript).OnReselect1();
            //	break;

            //	case "Chessboard":
            //	((BuildingCreator)buildingCreatorScript).OnReselect2();
            //	break;

            //	case "Classroom":
            //	((BuildingCreator)buildingCreatorScript).OnReselect3();
            //	break;

            //	case "Forge":
            //	((BuildingCreator)buildingCreatorScript).OnReselect4();
            //	break;

            //	case "Generator":
            //	((BuildingCreator)buildingCreatorScript).OnReselect5();
            //	break;

            //	case "Globe":
            //	((BuildingCreator)buildingCreatorScript).OnReselect6();
            //	break;

            //	case "Summon":
            //	((BuildingCreator)buildingCreatorScript).OnReselect7();
            //	break;			

            //	case "Toolhouse":
            //	((BuildingCreator)buildingCreatorScript).OnReselect8();
            //	break;

            //	case "Vault":
            //	((BuildingCreator)buildingCreatorScript).OnReselect9();
            //	break;

            //	case "Workshop":
            //	((BuildingCreator)buildingCreatorScript).OnReselect10();
            //	break;

            //	default:
            //	break;
            //}
            ((PlayerManager)playerManagerScript).MovePlayerToSelected();
        }
	}
}
