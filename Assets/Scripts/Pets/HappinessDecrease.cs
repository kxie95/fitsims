using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Decreases the happiness bar of a pet every thirty minutes
/// by the amount specified in the pet's XML attributes.
/// </summary>
public class HappinessDecrease : MonoBehaviour {

	public BuildingCreator creator;
	public GameObject pet;
	public UISlider happinessBar;
	public UISprite happinessIndicator;

	private DateTime previousTime;
	private DateTime previousTimePlus30;

	// Pet happiness stats.
	private int maxHp;
	private int hp;
	private int decreaseAmount; // amount that happiness decreases by every 30 minutes.

	// Use this for initialization
	void Start () 
	{
		// Get pet stats.
		Dictionary<string, string> petAttributes = creator.GetBuildingDictionary (pet.tag);
		decreaseAmount = Int32.Parse(petAttributes ["happinessDecrease"]);
		maxHp = Int32.Parse(petAttributes ["maxHp"]);
		hp = maxHp;

		// Initialise timer.
		previousTime = DateTime.UtcNow;
		previousTimePlus30 = previousTime.AddMinutes (30);
	}
	
	// Update is called once per frame
	void Update () 
	{
		DateTime now = DateTime.UtcNow;

		// Check if 30 minutes has passed.
		if (previousTimePlus30 >= now) 
		{
			// Decrease the slider by the amount obtained from attributes.
			if (hp - decreaseAmount >= 0) 
			{
				happinessBar.value -= (float)(hp - decreaseAmount)/(float)maxHp;
			} else 
			{
				happinessBar.value = 0;
			}

			// Change happiness indicator to sad if below 50%.
			if (!IsHappy()) 
			{
				happinessIndicator.spriteName = "sad";
			}

			previousTime = now;
			previousTimePlus30 = previousTime.AddMinutes (30);
		}
			
		if (IsHappy())
		{
			happinessIndicator.spriteName = "happy";
		}
	}
		
	private bool IsHappy() {
		return (float)hp / (float)maxHp >= 0.5;
	}
}
