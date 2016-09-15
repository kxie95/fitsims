using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Decreases the happiness bar of a pet every thirty minutes
/// by the amount specified in the pet's XML attributes.
/// </summary>
public class Happiness : MonoBehaviour {

	private SaveLoad saveLoad;
	private BuildingCreator creator;
    private ExerciseManager exManager;
    
	public GameObject pet;
	public UISlider happinessBar;
	public UISprite happinessIndicator;

    public int hpGained = 80;
    public int decreaseTimeMins = 1;

	public DateTime previousTime;
	private DateTime previousTimePlus;
    public DateTime savedTime;
    
    // Pet happiness stats.
    public int maxHp;
	public int hp = -1;
	public int decreaseAmount; // amount that happiness decreases by every decreaseTimeMins.

    public bool initialised = false;
    public bool needsReinit = false;
    public bool needsUiUpdate = false;

	// Use this for initialization
	void Start () 
	{
        creator = (BuildingCreator)GameObject.Find("BuildingCreator").GetComponent("BuildingCreator");
        exManager = (ExerciseManager)GameObject.Find("ExerciseManager").GetComponent("ExerciseManager");
		saveLoad = (SaveLoad)GameObject.Find ("SaveLoad").GetComponent ("SaveLoad");
		Initialise ();
	}
	
	// Update is called once per frame
	void Update () 
	{
        if (!initialised)
        {
			Initialise (); // May still not initialise if pet dictionary not initialised.
            return;
		}

        if (needsReinit)
        {
            Reinitialise();
        }

		DateTime now = DateTime.Now;

		// Check if decreaseTimeMins has passed. Decreases HP. Updates new time.
		if (DateTime.Compare(previousTimePlus, now) <= 0) 
		{
            if (IsHappy())
            {
                Debug.Log("1 min passed");
                // Decrease the slider by the amount obtained from attributes.
                DecreaseHappiness();

                previousTime = now;
                previousTimePlus = previousTime.AddMinutes(decreaseTimeMins);

                needsUiUpdate = true;

                if (!IsHappy())
                {
                    exManager.numSadPets++;
                }
            }
            else {
                Debug.Log("1 min passed");
                // Decrease the slider by the amount obtained from attributes.
                DecreaseHappiness();
                previousTime = now;
                previousTimePlus = previousTime.AddMinutes(decreaseTimeMins);
                needsUiUpdate = true;
            }
        }

        if (needsUiUpdate)
        {
            UpdateUi();
        }
    }


    private void DecreaseHappiness()
    {
        if (hp - decreaseAmount >= 0)
        {
            hp -= decreaseAmount;
        }
        else
        {
            hp = 0;
        }
        needsUiUpdate = true;
    }

    private void UpdateUi()
    {
        happinessBar.value = (float)(hp) / (float)maxHp;

        if (IsHappy())
        {
            happinessIndicator.spriteName = "happy";
        }
        else
        {
            happinessIndicator.spriteName = "sad";
        }
        needsUiUpdate = false;
    }

    private bool IsHappy()
    {
        return (float)hp / (float)maxHp >= 0.5;
    }

    private void Initialise()
    {
        if (creator.isBuildingsInitialised)
        {
            Debug.Log("in init");
            Dictionary<string, string> petAttributes = creator.GetBuildingDictionary(pet.tag);
            decreaseAmount = Int32.Parse(petAttributes["HappinessDecrease"]);
            maxHp = Int32.Parse(petAttributes["MaxHp"]);

            previousTime = DateTime.Now;
            previousTimePlus = previousTime.AddMinutes(decreaseTimeMins);

            if (hp == -1)
            {
                Debug.Log("setting hp as maxhp");
                hp = maxHp;
            }
            initialised = true;
        }
    }

    /// <summary>
    /// Called by SaveLoad if the pet has been initialised before.
    /// </summary>
    public void Reinitialise()
    {
        //DateTime savedTime = DateTime.Parse(PlayerPrefs.GetString(PREV_TIME));
        if (savedTime == null)
        {
            return;
        }

        double minutesPassed = (DateTime.Now - savedTime).TotalMinutes;
        int numDecreaseTimeMinsPassed = (int)(minutesPassed / decreaseTimeMins);

        hp -= decreaseAmount * numDecreaseTimeMinsPassed;
        if (hp < 0)
        {
            hp = 0;
        }
        previousTime = previousTime.AddMinutes(decreaseTimeMins * numDecreaseTimeMinsPassed);
        previousTimePlus = previousTime.AddMinutes(decreaseTimeMins);

        needsReinit = false;
        needsUiUpdate = true;

        Debug.Log("Happiness reinit");
    }

    public void IncreaseHP()
    {
        if (!IsHappy()) {
       
            //Check if higher than maximum
            if (hp + hpGained > maxHp)
            {
                hp = maxHp;
            }
            else
            {
                hp += hpGained;
            }

            if (IsHappy())
            {
                exManager.DecrementNumSadPets();
            }
        }
        else
        {
            //Check if higher than maximum
            if (hp + hpGained > maxHp)
            {
                hp = maxHp;
            }
            else
            {
                hp += hpGained;
            }
        }
        needsUiUpdate = true;
    }

    void OnApplicationPause(bool isGamePause)
    {
        if (isGamePause)
        {
            Debug.Log("Happiness Saving pause");
			saveLoad.Save ();
        }
    }

    void OnApplicationExit(bool isGameExit)
    {
        if (isGameExit)
        {
            Debug.Log("Happiness Saving exit");
			saveLoad.Save ();
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("Happiness Saving quit");
        saveLoad.Save();
    }
}
