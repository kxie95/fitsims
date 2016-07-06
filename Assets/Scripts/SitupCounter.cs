using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SitupCounter : MonoBehaviour {

    private AndroidJavaObject sensorPlugin;
    public Text text;

	// Use this for initialization
	void Start () {
        #if UNITY_ANDROID
        sensorPlugin = new AndroidJavaClass("jp.kshoji.unity.sensor.UnitySensorPlugin").CallStatic<AndroidJavaObject>("getInstance");
        if (sensorPlugin != null)
        {
            sensorPlugin.Call("startSensorListening", "proximity");
            sensorPlugin.Call("setSamplingPeriod", 100);
        }
        #endif
    }
	
	// Update is called once per frame
	void Update () {
        #if UNITY_ANDROID
        if (sensorPlugin != null)
        {
            float[] sensorValue = sensorPlugin.Call<float[]>("getSensorValues", "proximity");
            text.text = sensorValue[0].ToString();
        }
        #endif
    }

    // Terminate if application quits
    void OnApplicationQuit()
    {
        #if UNITY_ANDROID
        if (sensorPlugin != null)
        {
            sensorPlugin.Call("terminate");
            sensorPlugin = null;
        }
        #endif
    }
}
