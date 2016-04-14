using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.IO;
using System.Text;


public class HardPriceProc : MonoBehaviour {

	public TextAsset HardPriceXML;
	private List<Dictionary<string,string>> prices = new List<Dictionary<string,string>>();
	private Dictionary<string,string> dictionary;

	float totalPrice;
	private const int itemsNo = 4;
	private int currentSelection;//the button you pressed - 0,1,...itemsNo
	private int[] quantities = new int[itemsNo];//20 coins, for instance
	private float[] moneys = new float[itemsNo];//price for the 20 coins

	public UILabel[] quantitiesLb = new UILabel[itemsNo];//the labels to display the numbers above 
	public UILabel[] moneysLb = new UILabel[itemsNo]; 
	public UILabel[] pricesLb = new UILabel[itemsNo]; 

	public GameObject[] MinusBt = new GameObject[itemsNo];//the minus buttons are activated only if quantities > 0

	public UILabel totalPriceLb;//label for the total price

	public GameObject Stats; // the heads-up display game object
	private Component StatsSc;//the script for the above(component)

	// Use this for initialization
	void Start () {
		StatsSc = Stats.GetComponent ("Stats");//reads the Stats script

		GetPricesXML ();//
		InitiateLabels ();
		//UpdateLabel ();
	}

	private void GetPricesXML()
	{
		XmlDocument xmlDoc = new XmlDocument(); 
		xmlDoc.LoadXml(HardPriceXML.text); 
		XmlNodeList priceList = xmlDoc.GetElementsByTagName("Item");

		foreach (XmlNode priceInfo in priceList)
		{
			XmlNodeList priceContent = priceInfo.ChildNodes;	
			dictionary = new Dictionary<string, string>();
			
			foreach (XmlNode priceItems in priceContent) // levels itens nodes.
			{
				if(priceItems.Name == "Name")
				{
					dictionary.Add("Name",priceItems.InnerText); // put this in the dictionary.
				}		
				if(priceItems.Name == "Price")
				{
					dictionary.Add("Price",priceItems.InnerText); // put this in the dictionary.
				}
				if(priceItems.Name == "PerQuantity")
				{
					dictionary.Add("PerQuantity",priceItems.InnerText); // put this in the dictionary.
				}
			}

			prices.Add(dictionary);
		}
	}



	public void OnBuy0() { currentSelection = 0; Buy (); }//receives the buy commands
	public void OnBuy1() { currentSelection = 1; Buy (); }
	public void OnBuy2() { currentSelection = 2; Buy (); }
	public void OnBuy3() { currentSelection = 3; Buy (); }

	public void OnMinus0() { currentSelection = 0; Minus (); }//receives the minus commands
	public void OnMinus1()	{ currentSelection = 1; Minus (); }
	public void OnMinus2()	{ currentSelection = 2; Minus (); }
	public void OnMinus3() { currentSelection = 3; Minus (); }

	private void Buy()//you just pressed a buy button for one of the items; which one is passed by currentSelection
	{
		totalPrice += float.Parse (prices [currentSelection] ["Price"]);//update total price
		quantities [currentSelection] += int.Parse (prices [currentSelection] ["PerQuantity"]);//update quantity displayed on the button
		//updates the array with money spent on each item
		moneys[currentSelection] = quantities [currentSelection]* float.Parse (prices [currentSelection] ["Price"])/int.Parse (prices [currentSelection] ["PerQuantity"]);
		UpdateLabel ();
	}


	private void Minus()//buy in reverse
	{
		if(quantities [currentSelection] > 0)//only if there is something already bought
		{
			totalPrice -= float.Parse (prices [currentSelection] ["Price"]);
			quantities [currentSelection] -= int.Parse (prices [currentSelection] ["PerQuantity"]);
			moneys[currentSelection] = quantities [currentSelection]* float.Parse (prices [currentSelection] ["Price"])/int.Parse (prices [currentSelection] ["PerQuantity"]);
			UpdateLabel ();
		}
	}

	private void UpdateLabel()//updates only the label for current item, not all of them
	{
		quantitiesLb [currentSelection].text = quantities [currentSelection].ToString();
		moneysLb [currentSelection].text =  "$ " + moneys [currentSelection].ToString("0.00");
		totalPriceLb.text = "$ " + totalPrice.ToString("0.00");
		MinusBt [currentSelection].SetActive (quantities[currentSelection] > 0);//activate/deactivate minus button
	}

	private void UpdateAllLabels()
	{
		for (int i = 0; i < itemsNo; i++) 
		{							
		quantitiesLb [i].text = quantities [i].ToString();
		moneysLb [i].text =  "$ " + moneys [i].ToString("0.00");
		totalPriceLb.text = "$ " + totalPrice.ToString("0.00");
		}
	}

	private void InitiateLabels()//first time initializations, to update prices per quantities
	{
		for (int i = 0; i < pricesLb.Length; i++) 
		{
			pricesLb[i].text = "$ " + float.Parse (prices [i] ["Price"]).ToString("0.00") + " = " +
				int.Parse (prices [i] ["PerQuantity"]).ToString();
		}
		totalPriceLb.text = "$ " + totalPrice.ToString ("0.00");
	}

	public void Purchase()
	{
		//when buying resources, the max storage capacity is increased automatically and permanently; 
		//otherwise, if available storage capacity is less, the user would spend real money and get less resources than he expected
		//storage capacity is lost when specific storage/production buildings are destroyed

		if (quantities [0] > 0) //you bought dobbits
		{ 
			((Stats)StatsSc).dobbitNo += quantities [0];
		}

		if (quantities [1] > 0) //you bought some crystals
		{
			int emptyStorage = ((Stats)StatsSc).maxCrystals -((Stats)StatsSc).crystals ;//max cristals is 5, and you still have 2; available storage is 3
			if(quantities [1] > emptyStorage)//you bought more than you can store
			{
				((Stats)StatsSc).maxCrystals += quantities [1] - emptyStorage;//maxcrystal is 5; I have 3 crystals and buy 5 more; maxcrystal is increased to 8 instead of 10 (sum of max + purchase)
			}
			((Stats)StatsSc).crystals += quantities [1];//increases the actual crystals you have
		}
		if (quantities [2] > 0) 
		{
			int emptyStorage = ((Stats)StatsSc).maxStorageGold -(int)((Stats)StatsSc).gold ;

			if(quantities [2]>emptyStorage)
			{
				((Stats)StatsSc).maxStorageGold += quantities [2] - emptyStorage;
			}

			((Stats)StatsSc).gold += quantities [2];
		}
		if (quantities [3] > 0) 
		{
			int emptyStorage = ((Stats)StatsSc).maxStorageMana -(int)((Stats)StatsSc).mana ;

			if(quantities [3]>emptyStorage)
			{
			((Stats)StatsSc).maxStorageMana += quantities [3] - emptyStorage;
			}

			((Stats)StatsSc).mana += quantities [3];
		}

		((Stats)StatsSc).update = true;//this bool is used to trigger the HUD update once - there would be no point in updating the HUD with the framerate (more than 60 times a second)

		//reset all purchase menu elements to 0 
		for (int i = 0; i < itemsNo; i++) 
		{
			moneys[i]=0;
			quantities[i]=0;
			MinusBt [i].SetActive(false);
		}
		totalPrice = 0;
		UpdateAllLabels ();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
