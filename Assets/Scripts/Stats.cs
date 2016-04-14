using UnityEngine;
using System.Collections;

public class Stats : MonoBehaviour {

	public bool update = true;

	public int currentPopulation = 1;
	public int maxPopulation = 50;

	public int dobbitNo = 1;
	public int occupiedDobbitNo = 0;

	public UIProgressBar experienceBar;
	public UIProgressBar dobbitsBar;
	public UIProgressBar goldBar;
	public UIProgressBar manaBar;
	public UIProgressBar crystalsBar;

	public int experience = 0;
	public float gold = 5000;//5000;
	public float mana = 500;//500;
	public int crystals = 5;//5

	//when user hard buys resources, storage capacity permanently increases 
	public int maxExperience = 1132570; //from design document; many elements add to this
	public int maxStorageGold = 5000;
	public int maxStorageMana = 500;
	public int maxCrystals = 5;

	public UILabel curPopLb;
	public UILabel maxPopLb;
	public UILabel dobbitLb;
	public UILabel xpLb;
	public UILabel goldLb;
	public UILabel manaLb;
	public UILabel crystalsLb;

	public UILabel UserMessages;
	private bool displayUserMessages = false;
	public bool initUserMessages = false;
	public string userMessagesTxt;

	//0 gold 1 mana
	public int[] productionBuildings;//0 gold 1 mana
	public float[] productionRates;

	private const int noOfBuildings = 11;//correlate with building creator
	public int[] existingBuildings = new int[noOfBuildings]; 

	// Use this for initialization
	void Start () {
		productionBuildings = new int[2];
		productionRates = new float[2];
		InvokeRepeating ("Production", 1, 1);	
	}

	private void Production()
	{
		bool isProducing = false;

		if (productionBuildings [0] > 0) 
		{
			if(gold<maxStorageGold)
			{
				gold += productionBuildings [0] * productionRates [0];
				isProducing=true;
			}
			else
			{
				userMessagesTxt= "Increase Gold storage capacity.";
				initUserMessages = true;
			}
		}
		if (productionBuildings [1] > 0) 
		{
			if(mana<maxStorageMana)
			{
				mana += productionBuildings [1] * productionRates [1];
				isProducing=true;
			}
			else
			{	userMessagesTxt = "Increase Mana storage capacity.";
				initUserMessages = true;
			}
		}
		if (isProducing)
		update = true;

	}
	// Update is called once per frame
	void Update () {	

		if (update) 
		{
			//ApplyMaxCaps();
			UpdateUI();
			update = false;
		}
		if(initUserMessages)
		{
			UserMessages.text = userMessagesTxt;
			displayUserMessages = true;
			initUserMessages = false;
		}

		if (displayUserMessages) 
		{
			StartCoroutine(DisplayUserMessages());
		}

	}

	public void ApplyMaxCaps()//cannot exceed storage+bought capacity
	{
		if (gold > maxStorageGold) { gold = maxStorageGold; }
		if (mana > maxStorageMana) { mana = maxStorageMana; }
	}

	private void UpdateUI()//updates numbers and progress bars
	{
		((UISlider)experienceBar.GetComponent ("UISlider")).value = (float)experience/(float)maxExperience;
		((UISlider)dobbitsBar.GetComponent ("UISlider")).value = 1-((float)occupiedDobbitNo/(float)dobbitNo);
		((UISlider)goldBar.GetComponent ("UISlider")).value = (float)gold/(float)maxStorageGold;
		((UISlider)manaBar.GetComponent ("UISlider")).value = (float)mana/(float)maxStorageMana;
		((UISlider)crystalsBar.GetComponent ("UISlider")).value = (float)crystals/(float)maxCrystals;
			
		curPopLb.text = currentPopulation.ToString ();
		maxPopLb.text = maxPopulation.ToString ();
		xpLb.text = experience.ToString ();
		dobbitLb.text = (dobbitNo-occupiedDobbitNo).ToString () + " / " + dobbitNo.ToString ();
		goldLb.text = ((int)gold).ToString ();
		manaLb.text = ((int)mana).ToString ();
		crystalsLb.text = crystals.ToString ();
	}

	private IEnumerator DisplayUserMessages()//displays the hint- "maximum 3 buildings of type x allowed"
	{
		displayUserMessages = false;
		yield return new WaitForSeconds(1.50f);
		UserMessages.text = " ";
	}

}
