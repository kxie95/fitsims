using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class ExerciseManager : MonoBehaviour {

    private MainMenu mainMenu;
    public Stats stats;
    public SaveLoad saveLoad;
    public Settings settings;
    public SoundFX soundFx;
    public float bonusReward;
    public int bonusThreshold;
    public UILabel rewardLabel;
    public DateTime currentDate;
    public int claimedBonus;
    
    void Start () {
        mainMenu = (MainMenu)GameObject.Find("UIAnchor").GetComponent("MainMenu");
        claimedBonus = 0;
        //fish out current date
    }
	
	void Update () {

        //reset the claimed bonus if the date changes
        if(currentDate.Date < DateTime.Now.Date)
        {
            claimedBonus = 0;
            currentDate = DateTime.Now.Date;
        }
    }

    public void PerformExercise()
    {
        Debug.Log("Exercise performed");
        settings.PlayExerciseMusic();
        //Get the current selected object
        //Call method from object to get data on what sort of exercise/how much of that exercise
        //Start the measurement (possibly by calling and external method)
        //SceneManager.LoadScene("ChallengeScene");
        //TODO: Think about how to properly link components up.
        // mainMenu.OnDoingExercise();
        StepCounter s = (StepCounter)gameObject.GetComponent("StepCounter");
        s.StartCounting();
        //Need some sort of callback
        //Figure out currency works (just return some currency for now)
        //Get a new UI pane up (possibly) for when the service is done
        //Maybe just try to give the player cash for now 
        mainMenu.onDoingExercise();
    }

    /// <summary>
    /// Called when the player either completes the exercise or gives up on the exercise.
    /// </summary>
    /// <param name="completedValue">Amount of exercise done by the player.</param>
    /// <param name="targetDistance">Target amount of exercise for the challenge.</param>
    /// <param name="rewardValue">Total value of the reward associated with the challenge.</param>
    public void FinishExercise(int completedValue, int targetValue, int rewardValue)
    {
        settings.PlayMainMusic();
        soundFx.Victory();

        // Calculate the distance completed as a percentage.
        double percentCompleted = (double)completedValue / targetValue;
        // Multiply by the associated reward.
        int actualReward = (int)Math.Round(percentCompleted * rewardValue);
        // Set the text.
        rewardLabel.text = actualReward + " coins!";

        if (claimedBonus < bonusThreshold)
        {
            print("BONUS CLAIMED "+claimedBonus);
            actualReward = (int)(actualReward * bonusReward);
            stats.gold = stats.gold + actualReward;
            stats.update = true;
            claimedBonus++;
        }

        else {
            stats.gold = stats.gold + actualReward;
            stats.update = true;
        }
        saveLoad.SaveGame();

        // Show the exercise done dialog.
        mainMenu.OnExerciseDone();
    }
}
