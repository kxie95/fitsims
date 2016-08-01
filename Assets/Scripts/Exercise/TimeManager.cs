using UnityEngine;
using System.Collections;
using System;

public class TimeManager : MonoBehaviour, BonusManager{

    public GameObject buildingCreator;
    public bool result;
    private bool counting;
    public float timeRemaining;

    public bool GetResult()
    {   
        //No longer counting
        counting = false;
        return result;
    }

    public void StartCounting()
    {
        //Determine goal time
        result = true;
        BuildingCreator creator = (BuildingCreator)buildingCreator.GetComponent("BuildingCreator");
        timeRemaining = Convert.ToInt32(creator.GetCurrentBuildingDictionary()["BonusThreshold"]);
        //Start the update
        counting = true;
    }

    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        
        if (counting && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            //print(timeRemaining);
            if (timeRemaining <= 0)
            {
                result = false;
            }
        }
	}


}
