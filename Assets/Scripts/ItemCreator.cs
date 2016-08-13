using UnityEngine;
using System.Xml;
using System.Collections.Generic;

/// <summary>
/// Handles items and their attributes.
/// </summary>
public class ItemCreator : MonoBehaviour {

    public TextAsset itemsXml; //for loading item attributes from XML.
    private Dictionary<string, Dictionary<string, string>> buildings = new Dictionary<string, Dictionary<string, string>>();
    private Dictionary<string, string> dictionary;

    // Use this for initialization
    void Start () {
        GetItemsFromXml();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void GetItemsFromXml()//reads buildings XML
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(itemsXml.text);
        XmlNodeList buildingsList = xmlDoc.GetElementsByTagName("Item");

        foreach (XmlNode buildingInfo in buildingsList)
        {
            XmlNodeList buildingsContent = buildingInfo.ChildNodes;
            dictionary = new Dictionary<string, string>();
            string key = "";

            foreach (XmlNode buildingItems in buildingsContent) 
            {
                Debug.Log(buildingItems.Name);

                if (buildingItems.Name != "#comment")
                {
                    if (buildingItems.Name == "Key")
                    {
                        key = buildingItems.InnerText;
                    }
                    dictionary.Add(buildingItems.Name, buildingItems.InnerText);
                }

            }
            buildings.Add(key, dictionary);
        }
    }
}
