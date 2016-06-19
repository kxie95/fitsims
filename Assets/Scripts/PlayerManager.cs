using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

    private GameObject Player;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void MovePlayerToSelected()
    {
        if(Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }
        Transform target = BuildingCreator.selectedBuilding.transform.Find("PlayerTarget");
        Player.transform.position = target.position;
    }
    
}
