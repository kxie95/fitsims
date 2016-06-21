using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {
	
	private const int noIFElements = 13;
	public GameObject[] InterfaceElements = new GameObject[noIFElements];
		
	private const int noScreens = 6;
	public GameObject[] Screens = new GameObject[noScreens];
	
	public GameObject gameManager;
	public bool constructionGreenlit = true;
	public GameObject BuildingCreatorOb;
	public GameObject ConfirmationScreen;
    

    //[HideInInspector]

    public void OnShop(){OnActivateButton (0);}
	public void OnCloseShop(){OnDeactivateButton (0);}
	public void OnCloseShopToBuild()
	{
		if (constructionGreenlit) 
		{
			Screens [0].SetActive (false);
			ActivateInterface ();
		}
	}

	public void OnOptions(){OnActivateButton (1);}
	public void OnCloseOptions(){OnDeactivateButton (1);}

	public void OnCompetition(){OnActivateButton (2);}
	public void OnCloseCompetition(){OnDeactivateButton (2);}

	public void OnInvite(){OnActivateButton (3);}
	public void OnCloseInvite(){OnDeactivateButton (3);}

	public void OnPurchase(){OnActivateButton (4);}
	public void OnClosePurchase(){OnDeactivateButton (4);}

	public void OnUpgrade(){OnActivateButton (5);}
	public void OnCloseUpgrade(){OnDeactivateButton (5);}

    // For controlling exercise in progress frame.
    public void onDoingExercise() { OnActivateButton(6); }
    public void onCloseDoingExercise() { OnDeactivateButton(6); }

    // For controlling exercise done frame.
    public void OnExerciseDone() { OnActivateButton(6); }
    public void OnCloseExerciseDone (){ OnDeactivateButton(6); }



	public void OnConfirmationScreen()	
	{ 
		ConfirmationScreen.SetActive (true); 
	}

	public void OnCloseConfirmationScreen() { ConfirmationScreen.SetActive (false); }

	public void OnDestBuilding()
	{
		((BuildingCreator)BuildingCreatorOb.GetComponent("BuildingCreator")).Cancel();
		ConfirmationScreen.SetActive (false);
	}
	public void OnCancelDestBuilding()
	{
		((BuildingCreator)BuildingCreatorOb.GetComponent("BuildingCreator")).OK();
		ConfirmationScreen.SetActive (false);
	}

	void OnActivateButton(int scrno)
	{
		bool pauseInput = false;
		
		pauseInput = ((Relay)gameManager.GetComponent("Relay")).pauseInput;
		
		if (!pauseInput) 
		{
			Screens [scrno].SetActive (true);
			((Relay)gameManager.GetComponent ("Relay")).pauseInput = true;
			for (int i = 0; i < InterfaceElements.Length; i++) 
			{
				InterfaceElements [i].SetActive (false);
			}
		} 

	}

	void OnDeactivateButton(int scrno)
	{	
		((Relay)gameManager.GetComponent("Relay")).pauseInput = false;		
		Screens[scrno].SetActive(false);
		ActivateInterface();
	}

	private void ActivateInterface()
	{
		for (int i = 0; i < InterfaceElements.Length; i++) 
		{
			//if(i!=9)//to disable navigation buttons
			InterfaceElements[i].SetActive(true);
		}
	}

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	
	}

	
	
	

	
}
