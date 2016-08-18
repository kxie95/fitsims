using UnityEngine;
using System.Collections;
using System;

public class TimeManager : MonoBehaviour, BonusManager{

    public GameObject buildingCreator;
    public bool result;
    private bool counting;
    public float timeRemaining;

    public GameObject bonusObject;
    public UILabel timerLabel;

    private GameObject currentCounter;
    private ExerciseCounter exerciseCounter;

    public SoundFX soundFx;

    public bool GetResult()
    {   
        //No longer counting
        counting = false;
        bonusObject.SetActive(false);
        return result;
    }

    public void StartCounting()
    {
        //Determine goal time
        result = true;
        BuildingCreator creator = (BuildingCreator)buildingCreator.GetComponent("BuildingCreator");
        currentCounter = GameObject.FindGameObjectWithTag(creator.GetCurrentBuildingDictionary()["TaskType"]);
        exerciseCounter = (ExerciseCounter)currentCounter.GetComponent("ExerciseCounter");

        // Show bonus on UI.
        bonusObject.SetActive(true);

        timeRemaining = Convert.ToInt32(creator.GetCurrentBuildingDictionary()["BonusThreshold"]);
        timerLabel.text = timeRemaining.ToString() + "s";
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
            timerLabel.text = Math.Round(timeRemaining, 2).ToString() + "s";

            // If the player has reached half way, they have received the bonus.
            if (exerciseCounter.HasCompletedAtLeast(0.5f))
            {
                soundFx.Victory();
                bonusObject.SetActive(false);
                counting = false;
            }

            //print(timeRemaining);
            if (timeRemaining <= 0)
            {
                bonusObject.SetActive(false);
                result = false;
            }
        }
	}


}
