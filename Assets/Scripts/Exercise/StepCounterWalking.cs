using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System;

/// <summary>
/// Class which tracks the number of steps taken using accelerometer readings.
/// </summary>
public class StepCounterWalking : MonoBehaviour, ExerciseCounter {

    public float thresholdLeft = 0.96f; // The weighted moving average is compared against this threshold to detect a step.
    public float thresholdRight = 0.98f;
    public int stepGoal = 100;

    private bool inQuest = false;
    private int rewardTotal;

    public AudioClip coinSoundClip;
    private Text stepCountText; // UI which shows step count.
    //private Text targetCountText; // UI which shows step count goal.
    private AudioSource coinSfx;

    public UIProgressBar exerciseBar;
    public UILabel exerciseLabel;
    private string taskType;

    private static int LIST_SIZE = 10; // Capacity of list.
    private ArrayList cosineOfAngleData = new ArrayList(); // Keeps track of last LIST_SIZE readings of cosine of angles data.
    private Vector3 previousAccel;
    private int stepCount;
    private bool isLeft = false;
    private float time;

    private MainMenu mainMenu;
    private BuildingCreator creator;
    private ExerciseManager exManager;

    // Use this for initialization
    void Start () {
        
        coinSfx = GetComponent<AudioSource>();
        exManager = (ExerciseManager)GameObject.Find("ExerciseManager").GetComponent("ExerciseManager");
        mainMenu = (MainMenu)GameObject.Find("UIAnchor").GetComponent("MainMenu");
        creator = (BuildingCreator)GameObject.Find("BuildingCreator").GetComponent("BuildingCreator");
    }
	
	// Update is called once per frame
	void Update () {
        if (inQuest)
        {
            // Get normalized acceleration vector (direction only).
            Vector3 currentAccel = Input.acceleration;
            currentAccel.Normalize();

            // Calculate the cosine of the angle between the current vector and the one before it.
            float top = (currentAccel.x * previousAccel.x) + (currentAccel.y * previousAccel.y) + (currentAccel.z * previousAccel.z);
            float currentMagnitude = (float)Mathf.Sqrt(Mathf.Pow(currentAccel.x, 2) + Mathf.Pow(currentAccel.y, 2) + Mathf.Pow(currentAccel.z, 2));
            float previousMagnitude = (float)Mathf.Sqrt(Mathf.Pow(previousAccel.x, 2) + Mathf.Pow(previousAccel.y, 2) + Mathf.Pow(previousAccel.z, 2));
            float cosineOfAngle = top / (currentMagnitude * previousMagnitude);

            addToList(cosineOfAngle);

            // If the list is full, calculate weighted moving average of the whole list.
            if (cosineOfAngleData.Count == LIST_SIZE)
            {
                float weightedSum = 0;
                float sumOfWeights = 0;

                for (int i = LIST_SIZE; i > 0; i--)
                {
                    weightedSum += i * (float)cosineOfAngleData[i - 1];
                    sumOfWeights += i;
                }

                float weightedMovingAverage = weightedSum / sumOfWeights;
                //Debug.Log(weightedSum + " / " + sumOfWeights + " = " + weightedMovingAverage);

                if (weightedMovingAverage < thresholdLeft && isLeft || weightedMovingAverage < thresholdRight && !isLeft)
                {
                    isLeft = !isLeft;
                    if ((Time.time - time) > 0.4)
                    {
                        stepCount++;

                        time = Time.time;

                        ((UISlider)exerciseBar.GetComponent("UISlider")).value = (float)stepCount / (float)stepGoal;
                        exerciseLabel.text = stepCount.ToString();

                        // If goal reached, move to victory screen.
                        if (stepCount == stepGoal)
                        {
                            FinishTask();
                        }

                        if (coinSoundClip != null && coinSfx != null)
                        {
                            coinSfx.PlayOneShot(coinSoundClip, 0.4f);
                        }
                    }
                }

            }
            previousAccel = currentAccel;
        }
    }

    /// <summary>
    /// Synchronously adds an object to the arraylist. Removes the oldest item if the list is full.
    /// </summary>
    /// <param name="acceleration"></param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    private void addToList(float item)
    {
        // If the list is full, remove the data at the beginning of the list.
        if (cosineOfAngleData.Count == LIST_SIZE)
        {
            cosineOfAngleData.RemoveAt(0);
        }
        cosineOfAngleData.Add(item);
    }

    public void FinishTask()
    {
        inQuest = false;
        //Clean up the bars; make the 0 and stuff
        mainMenu.onCloseDoingExercise();
        //Make call to the finish screen
        
        exManager.CalculateReward(stepCount, stepGoal, rewardTotal);

        ((UISlider)exerciseBar.GetComponent("UISlider")).value = 0;
        exerciseLabel.text = "0";
        stepCount = 0;
    }


    public void StartCounting()
    {
        // Set list capacity.
        cosineOfAngleData.Capacity = LIST_SIZE;

        // Initialize previous acceleration.
        previousAccel = Input.acceleration;
        previousAccel.Normalize();

        time = Time.time;

        ((UISlider)exerciseBar.GetComponent("UISlider")).value = 0;
        exerciseLabel.text = "0";
        //read in the parameters for the specific quest here
        Dictionary<string, string> build = creator.GetCurrentBuildingDictionary();
        rewardTotal = Int32.Parse(build["TaskReward"]);
        taskType = build["TaskType"];
        stepGoal = Int32.Parse(build["TaskRequirement"]);

        inQuest = true;
    }

    public bool HasCompletedAtLeast(float proportion)
    {
        int amountToCompare = (int)(stepGoal * proportion);
        return stepCount >= amountToCompare;
    }
}

