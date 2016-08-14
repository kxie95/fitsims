using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

public class APIConnection : MonoBehaviour {

    private string filePath;
    private string fileName = "SOMSave";
    private string fileExt = ".txt";
    private string url = "http://fitsimsdata.azurewebsites.net/api/SaveDataModels";

    // Use this for initialization
    void Start () {
        filePath = Application.persistentDataPath + "/";
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void PostData()
    {
        WWWForm form = new WWWForm();
        StreamReader sReader = new StreamReader(filePath + fileName + fileExt);
        //StringBuilder sBuilder = new StringBuilder();
        string sBuilder = "";
        while (!sReader.EndOfStream)
        {
            sBuilder = sBuilder+sReader.ReadLine()+" ";
        }

        form.AddField("DataBlob", sBuilder);
        WWW www = new WWW(url, form);

        StartCoroutine(SendSaveData(www));
    }

    IEnumerator SendSaveData(WWW www)
    {
        yield return www;

         // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
        }
        else {
            Debug.Log("WWW Error: " + www.error);
        }
    }
}
