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

	public GameObject pet;
	public UISlider happinessBar;
	public UISprite happinessIndicator;

    public int decreaseTimeMins = 30;

	private DateTime previousTime;
	private DateTime previousTimePlus;
    
	// Pet happiness stats.
	private int maxHp;
	public int hp = -1;
	private int decreaseAmount; // amount that happiness decreases by every decreaseTimeMins.

    private bool initialised = false;

	// Use this for initialization
	void Start () 
	{
        creator = (BuildingCreator)GameObject.Find("BuildingCreator").GetComponent("BuildingCreator");
		saveLoad = (SaveLoad)GameObject.Find ("SaveLoad").GetComponent ("SaveLoad");
		Initialise ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!initialised) {
			Initialise ();
		} else {
			return;
		}

		DateTime now = DateTime.Now;

		// Check if decreaseTimeMins has passed.
		if (now >= previousTimePlus) 
		{
			// Decrease the slider by the amount obtained from attributes.
			if (hp - decreaseAmount >= 0) 
			{
                hp -= decreaseAmount;
				happinessBar.value = (float)(hp) / (float)maxHp;
			} else 
			{
                hp = 0;
                happinessBar.value = 0;
			}

			previousTime = now;
			previousTimePlus = previousTime.AddMinutes (decreaseTimeMins);
		}
			
		// Update happiness indicator.
		if (IsHappy ()) {
			happinessIndicator.spriteName = "happy";
		} else {
			happinessIndicator.spriteName = "sad";
		}
	}

    void OnApplicationPause(bool isGamePause)
    {
        if (isGamePause)
        {
			PlayerPrefs.SetString("HappinessTimePrevious", previousTime.ToString());
			saveLoad.SaveGame ();
        }
    }

    void OnApplicationFocus(bool isGameFocus)
    {
		if (isGameFocus) {
			saveLoad.LoadGame ();
			Initialise ();
		}
    }

    void OnApplicationExit(bool isGameExit)
    {
        if (isGameExit)
        {
			PlayerPrefs.SetString("HappinessTimePrevious", previousTime.ToString());
			saveLoad.SaveGame ();
        }
    }

    private bool IsHappy()
    {
        return (float)hp / (float)maxHp >= 0.5;
    }

    private void Initialise()
    {
		if (creator.isBuildingsInitialised)
		{
			Dictionary<string, string> petAttributes = creator.GetBuildingDictionary(pet.tag);
			decreaseAmount = Int32.Parse(petAttributes["HappinessDecrease"]);
			maxHp = Int32.Parse(petAttributes["MaxHp"]);

			previousTime = DateTime.Now;
			previousTimePlus = previousTime.AddMinutes(decreaseTimeMins);
			if (hp == -1) {
				hp = maxHp;
			}
			initialised = true;
		}
    }

	/// <summary>
	/// Called by SaveLoad if the pet has been initialised before.
	/// </summary>
	public void ReInitialise() 
	{
		if (!PlayerPrefs.HasKey("HappinessTimePrevious"))
		{
			return;
		}

		DateTime savedTime = DateTime.Parse(PlayerPrefs.GetString("HappinessTimePrevious"));
		double minutesPassed = (DateTime.Now - savedTime).TotalMinutes;
		int numDecreaseTimeMinsPassed = (int)(minutesPassed / decreaseTimeMins);

		hp -= decreaseAmount * numDecreaseTimeMinsPassed;

		previousTime = previousTime.AddMinutes(decreaseTimeMins * numDecreaseTimeMinsPassed);
		previousTimePlus = previousTime.AddMinutes(decreaseTimeMins);
	}
}
