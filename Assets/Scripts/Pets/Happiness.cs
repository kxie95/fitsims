using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Decreases the happiness bar of a pet every thirty minutes
/// by the amount specified in the pet's XML attributes.
/// </summary>
public class Happiness : MonoBehaviour {

	private BuildingCreator creator;
    private ExerciseManager exManager;
	public GameObject pet;
	public UISlider happinessBar;
	public UISprite happinessIndicator;

    public int hpGained = 80;
    public int decreaseTimeMins = 1;

	private DateTime previousTime;
	private DateTime previousTimePlus;
    
	// Pet happiness stats.
	public int maxHp;
	public int hp;
	public int decreaseAmount; // amount that happiness decreases by every decreaseTimeMins.

    private bool initialised = false;

	// Use this for initialization
	void Start () 
	{
        creator = (BuildingCreator)GameObject.Find("BuildingCreator").GetComponent("BuildingCreator");
        exManager = (ExerciseManager)GameObject.Find("ExerciseManager").GetComponent("ExerciseManager");

        // Get pet stats.
        Debug.Log(pet.tag);
        Debug.Log("Current time" + DateTime.Now.ToString());

        if (creator.isBuildingsInitialised)
        {
            Initialise();
        }
	}
	
	// Update is called once per frame
	void Update () 
	{
        if (!initialised)
        {
            if (creator.isBuildingsInitialised)
            {
                Initialise();
            } else
            {
                return;
            }
        } 

		DateTime now = DateTime.Now;

		// Check if decreaseTimeMins has passed.
		if (now >= previousTimePlus) 
		{
            //Starts happy
            if (IsHappy())
            {
                // Decrease the slider by the amount obtained from attributes.
                if (hp - decreaseAmount >= 0)
                {
                    hp -= decreaseAmount;
                    happinessBar.value = (float)(hp) / (float)maxHp;
                }
                else
                {
                    hp = 0;
                    happinessBar.value = 0;
                }

                // Change happiness indicator to sad if below 50%.
                if (!IsHappy())
                {
                    happinessIndicator.spriteName = "sad";
                    exManager.numSadPets++;
                }
            }
            //Starts sad
            else
            {
                // Decrease the slider by the amount obtained from attributes.
                if (hp - decreaseAmount >= 0)
                {
                    hp -= decreaseAmount;
                    happinessBar.value = (float)(hp) / (float)maxHp;
                }
                else
                {
                    hp = 0;
                    happinessBar.value = 0;
                }
            }

			previousTime = now;
			previousTimePlus = previousTime.AddMinutes (decreaseTimeMins);
		}
			
		if (IsHappy())
		{
			happinessIndicator.spriteName = "happy";
		}
	}

    void OnApplicationPause(bool isGamePause)
    {
        if (isGamePause)
        {
            PlayerPrefs.SetString("HappinessTimePrevious", previousTime.ToString());
        }
    }

    void OnApplicationFocus(bool isGameFocus)
    {
        if (isGameFocus)
        {
            if (!PlayerPrefs.HasKey("HappinessTimePrevious"))
            {
                return;
            }
            DateTime savedTime = DateTime.Parse(PlayerPrefs.GetString("HappinessTimePrevious"));
            double minutesPassed = (DateTime.Now - savedTime).TotalMinutes;
            if ((int)minutesPassed == 0)
            {
                return;
            }
            int numDecreaseTimeMinsPassed = (int)(minutesPassed / decreaseTimeMins);

            hp -= decreaseAmount * numDecreaseTimeMinsPassed;
            previousTime = previousTime.AddMinutes(decreaseTimeMins * numDecreaseTimeMinsPassed);
            previousTimePlus = previousTime.AddMinutes(decreaseTimeMins);
        }
    }

    void OnApplicationExit(bool isGameExit)
    {
        if (isGameExit)
        {
            PlayerPrefs.SetString("HappinessTimePrevious", previousTime.ToString());
        }
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
                hp = +hpGained;
            }

            happinessBar.value = (float)(hp) / (float)maxHp;

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
                hp = +hpGained;
            }

            happinessBar.value = (float)(hp) / (float)maxHp;
        }
        
    }

    private bool IsHappy()
    {
        return (float)hp / (float)maxHp >= 0.5;
    }

    private void Initialise()
    {
        Dictionary<string, string> petAttributes = creator.GetBuildingDictionary(pet.tag);
        decreaseAmount = Int32.Parse(petAttributes["HappinessDecrease"]);
        maxHp = Int32.Parse(petAttributes["MaxHp"]);
        hp = maxHp;

        // Initialise timer.
        previousTime = DateTime.Now;
        previousTimePlus = previousTime.AddMinutes(decreaseTimeMins);

        initialised = true;
    }
}
