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
        BuildingCreator creator = (BuildingCreator)GameObject.Find("BuildingCreator").GetComponent("BuildingCreator");
        Transform target = creator.selectedBuilding.transform.Find("PlayerTarget");
        print(target.position.ToString());
        Player.transform.position = target.position;
    }

}