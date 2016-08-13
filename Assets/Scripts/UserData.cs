using UnityEngine;
using System.Collections;

public class UserData : MonoBehaviour {

    //Task stats
    public int TasksFinished;
    public float TimeInTasks;

    //Coins
    public int GoldEarned;
    public int GoldSpent;

    //Pets
    public int PetsBought;

    public bool InTask;
	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (InTask)
        {
            TimeInTasks += Time.deltaTime;
        }
	}

    
}
