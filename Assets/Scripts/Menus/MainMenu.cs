using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour {
	
	private const int noIFElements = 11;
	public GameObject[] InterfaceElements = new GameObject[noIFElements];
		
	private const int noScreens = 6;
	public GameObject[] Screens = new GameObject[noScreens];
	
	public GameObject gameManager;
	public bool constructionGreenlit = true;
	public GameObject BuildingCreatorOb;
	public GameObject ConfirmationScreen;
    public float DelayTime = 0.3f;
    //[HideInInspector]

    public void OnShop(){OnActivateButton (0);}
	public void OnCloseShop(){OnDeactivateButton (0);}
	public void OnCloseShopToBuild()
	{
		if (constructionGreenlit) 
		{
			Screens [0].SetActive (false);
            // Activate Buy and don't buy buttons.
            InterfaceElements[0].SetActive(true);
            InterfaceElements[1].SetActive(true);
        }
    }

	public void OnOptions(){OnActivateButton (1);}
	public void OnCloseOptions(){OnDeactivateButton (1);}

	public void OnUpgrade(){ ((Relay)gameManager.GetComponent("Relay")).pauseInput = false; OnActivateButton (2);}
	public void OnCloseUpgrade(){OnDeactivateButton (2);}

    // For controlling exercise in progress screen.
    public void onDoingExercise() { ((Relay)gameManager.GetComponent("Relay")).pauseInput = false;  OnActivateButton(3); }
    public void onCloseDoingExercise() { OnDeactivateButton(3); }

    // For controlling exercise done screen.
    public void OnExerciseDone() { ((Relay)gameManager.GetComponent("Relay")).pauseInput = false; OnActivateButton(4); }
    public void OnCloseExerciseDone (){ OnDeactivateButton(4); }

    // For help screen.
    public void OnHelp()
    {
        OnActivateButton(5);
        PlayerPrefs.SetString("HelpOpened", "Opened");
    }
    public void OnCloseHelp() { OnDeactivateButton(5); }

    // For data logging screen.
    public void OnDataLog() { OnActivateButton(6); }
    public void OnCloseDataLog() { OnDeactivateButton(6); }

    

    public void OnConfirmationScreen()	
	{
        ((Relay)gameManager.GetComponent("Relay")).pauseInput = true;
        ConfirmationScreen.SetActive (true); 
	}

	public void OnCloseConfirmationScreen() {
        StartCoroutine(SetPauseInputFalse(DelayTime));
        ConfirmationScreen.SetActive (false); }

	public void OnDestBuilding()
	{
        ConfirmationScreen.SetActive(false);
        ((BuildingCreator)BuildingCreatorOb.GetComponent("BuildingCreator")).Cancel();
        StartCoroutine(SetPauseInputFalse(DelayTime));
    }
	public void OnCancelDestBuilding()
	{
        ConfirmationScreen.SetActive(false);
        ((BuildingCreator)BuildingCreatorOb.GetComponent("BuildingCreator")).OK();
        StartCoroutine(SetPauseInputFalse(DelayTime));
    }

	void OnActivateButton(int scrno)
	{

		bool pauseInput = false;
		
		pauseInput = ((Relay)gameManager.GetComponent("Relay")).pauseInput;
        Debug.Log("pauseinput:" + pauseInput);
		if (!pauseInput) 
		{
			Screens [scrno].SetActive (true);
			((Relay)gameManager.GetComponent ("Relay")).pauseInput = true;
            DeactivateInterface();
		} 

	}

	void OnDeactivateButton(int scrno)
	{
        //((Relay)gameManager.GetComponent("Relay")).pauseInput = false;		
        StartCoroutine(SetPauseInputFalse(DelayTime));
        Screens[scrno].SetActive(false);
		ActivateInterface();
	}

	public void ActivateInterface()
	{
		for (int i = 0; i < InterfaceElements.Length; i++) 
		{
			//if(i!=9)//to disable navigation buttons
			InterfaceElements[i].SetActive(true);
		}
	}

    IEnumerator SetPauseInputFalse(float time)
    {
        yield return new WaitForSeconds(time);

        ((Relay)gameManager.GetComponent("Relay")).pauseInput = false;

    }

    public void DeactivateInterface()
    {
        for (int i = 0; i < InterfaceElements.Length; i++)
        {
            InterfaceElements[i].SetActive(false);
        }
    }

	// Use this for initialization
	void Start () {
        if (!PlayerPrefs.HasKey("HelpOpened"))
        {
            OnHelp();
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
	

	
}
