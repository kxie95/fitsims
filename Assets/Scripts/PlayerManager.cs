using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour
{

    public GameObject Player;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MovePlayerToSelected()
    {
        print("moving player to selected");
        BuildingCreator creator = (BuildingCreator)GameObject.Find("BuildingCreator").GetComponent("BuildingCreator");
        Transform target = creator.selectedBuilding.transform.Find("PlayerTarget");
        Player.transform.position = target.position;
    }

}