using UnityEngine;
using System.Collections;

public class BuildingSelector : MonoBehaviour {//attached to each building as an invisible 2dtoolkit button
	
	public bool isSelected = true;
	public bool inConstruction = true;//only for load/save

	public int buildingIndex = -1;	
	public string buildingType;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void ReSelect()
	{
        //((Relay)gameManager.GetComponent("Relay")).buildingFloating
        print("reselect called");
		GameObject gameManager = GameObject.Find("GameManager");
		GameObject buildingCreator = GameObject.Find("BuildingCreator");
        GameObject uiAnchor = GameObject.Find("UIAnchor");

        //MainMenu mainMenu = (MainMenu)uiAnchor.GetComponent("MainMenu");
        //mainMenu.DeactivateInterface();
        
        Component relayScript = (Relay)gameManager.GetComponent("Relay");
		Component buildingCreatorScript = (BuildingCreator)buildingCreator.GetComponent("BuildingCreator");
		
		if(!((BuildingCreator)buildingCreatorScript).isReselect &&
			!((Relay)relayScript).pauseInput)
		{
			isSelected = true;				
			((BuildingCreator)buildingCreatorScript).isReselect = true;

            ((BuildingCreator)buildingCreatorScript).OnReselect(buildingType);
        }
	}
}
