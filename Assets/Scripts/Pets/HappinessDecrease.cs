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

	private float maxValue;
	private int decreaseAmount; // amount that happiness decreases by every 30 minutes.
	private DateTime previousTime;
	private DateTime previousTimePlus30;

	// Use this for initialization
	void Start () 
	{
		Dictionary<string, string> petAttributes = creator.GetBuildingDictionary (pet.tag);
		decreaseAmount = Int32.Parse(petAttributes ["happinessDecrease"]);
		previousTime = DateTime.UtcNow;
		previousTimePlus30 = previousTime.AddMinutes (30);
		maxValue = happinessBar.value;
	}
	
	// Update is called once per frame
	void Update () 
	{
		DateTime now = DateTime.UtcNow;

		// Check if 30 minutes has passed.
		if (previousTimePlus30 >= now) 
		{
			// Decrease the slider by the amount obtained from attributes.
			if (happinessBar.value - decreaseAmount >= 0) 
			{
				happinessBar.value -= decreaseAmount;
			} else 
			{
				happinessBar.value = 0;
			}

			// Change happiness indicator to sad if below 50%.
			if (happinessBar.value < (maxValue / 2.0)) 
			{
				happinessIndicator.spriteName = "sad";
			}

			previousTime = now;
			previousTimePlus30 = previousTime.AddMinutes (30);
		}
			
		if (happinessBar.value >= (maxValue / 2.0))
		{
			happinessIndicator.spriteName = "happy";
		}
	}
}
