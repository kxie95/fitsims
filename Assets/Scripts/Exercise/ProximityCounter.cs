using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Class for counting exercises using the proximity sensor (eg. situps and pushups).
/// </summary>
public class ProximityCounter : MonoBehaviour, ExerciseCounter {

    private AndroidJavaObject sensorPlugin;

    // Counter controls
    private int counter = 0;
    private bool inQuest = false;
    public float proximityThreshold = 0f;
    private float previousValue = 3f;

    // Task attributes
    private int rewardTotal = 0;
    private string taskType = "";
    private int goal = 0;

    // UI components
    public UIProgressBar exerciseBar;
    public UILabel exerciseLabel;

    private MainMenu mainMenu;
    private BuildingCreator creator;
    private ExerciseManager exManager;

    // Use this for initialization
    void Start () {
        exManager = (ExerciseManager)GameObject.Find("ExerciseManager").GetComponent("ExerciseManager");
        mainMenu = (MainMenu)GameObject.Find("UIAnchor").GetComponent("MainMenu");
        creator = (BuildingCreator)GameObject.Find("BuildingCreator").GetComponent("BuildingCreator");

        #if UNITY_ANDROID
            // Initialise plugin.
            sensorPlugin = new AndroidJavaClass("jp.kshoji.unity.sensor.UnitySensorPlugin").CallStatic<AndroidJavaObject>("getInstance");
            sensorPlugin.Call("startSensorListening", "proximity");
            sensorPlugin.Call("setSamplingPeriod", 100000);
        #endif

    }
	
	// Update is called once per frame
	void Update () {
        if (inQuest)
        {
            #if UNITY_ANDROID
                if (sensorPlugin != null)
                {
                    float[] sensorValue = sensorPlugin.Call<float[]>("getSensorValues", "proximity");
                    if (sensorValue != null)
                    {
                        float actualValue = sensorValue[0];
                        if (actualValue <= proximityThreshold && previousValue > proximityThreshold)
                        {
                            counter = counter +5;
                            previousValue = actualValue;
                            exerciseLabel.text = counter.ToString();
                            ((UISlider)exerciseBar.GetComponent("UISlider")).value = (float)counter / (float)goal;
                            
                            if (counter >= goal)
                            {
                                FinishTask();
                            }

                        }
                        previousValue = actualValue;
                }                
            }
            #endif
        }

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

    public void StartCounting()
    {
        // Get task attributes from pet.
        Dictionary<String, String> petDictionary = creator.GetCurrentBuildingDictionary();
        rewardTotal = Int32.Parse(petDictionary["TaskReward"]);
        taskType = petDictionary["TaskType"];
        goal = Int32.Parse(petDictionary["TaskRequirement"]);

        // Set everything to zero.
        reset();

        inQuest = true;
    }

    public void FinishTask()
    {
        inQuest = false;
        mainMenu.onCloseDoingExercise();
        exManager.CalculateReward(counter, goal, rewardTotal);
        reset();
    }

    private void reset()
    {
        ((UISlider)exerciseBar.GetComponent("UISlider")).value = 0;
        exerciseLabel.text = "0";
        counter = 0;
    }

    public bool HasCompletedAtLeast(float proportion)
    {
        int amountToCompare = (int)(goal * proportion);
        return counter >= amountToCompare;
    }
}
