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
    public int dailyBonusThreshold;
    public UILabel rewardLabel;
    public DateTime currentDate;
    public GameObject buildingCreator;
    public int claimeDailyBonus;

    private GameObject currentCounter;

    void Start () {
        mainMenu = (MainMenu)GameObject.Find("UIAnchor").GetComponent("MainMenu");
        claimeDailyBonus = 0;
        //fish out current date
    }
	
	void Update () {

        //reset the claimed bonus if the date changes
        if(currentDate.Date < DateTime.Now.Date)
        {
            claimeDailyBonus = 0;
            currentDate = DateTime.Now.Date;
        }
    }
    
    public void PerformExercise()
    {
        Debug.Log("Exercise performed");
        settings.PlayExerciseMusic();

        BuildingCreator creator = (BuildingCreator)buildingCreator.GetComponent("BuildingCreator");
        currentCounter = GameObject.FindGameObjectWithTag(creator.GetCurrentBuildingDictionary()["TaskType"]);
        ExerciseCounter exCount = (ExerciseCounter)currentCounter.GetComponent("ExerciseCounter");

        exCount.StartCounting();
        TryStartBonus();
        mainMenu.onDoingExercise();
    }

    private void TryStartBonus()
    {
        BuildingCreator creator = (BuildingCreator)buildingCreator.GetComponent("BuildingCreator");
        if (Convert.ToBoolean(creator.GetCurrentBuildingDictionary()["HasBonus"]))
        {
            GameObject exManager = GameObject.FindGameObjectWithTag(creator.GetCurrentBuildingDictionary()["BonusType"]);
            //Check if this works                
            BonusManager bonus = (BonusManager)exManager.GetComponent("BonusManager");
            bonus.StartCounting();
            //Call the startCounting on the bonus manager
        }
    }

    /// <summary>
    /// Called when the player either completes the exercise or gives up on the exercise.
    /// </summary>
    /// <param name="completedValue">Amount of exercise done by the player.</param>
    /// <param name="targetDistance">Target amount of exercise for the challenge.</param>
    /// <param name="rewardValue">Total value of the reward associated with the challenge.</param>
    public void CalculateReward(int completedValue, int targetValue, int rewardValue)
    {
        settings.PlayMainMusic();
        soundFx.Victory();

        // Calculate the distance completed as a percentage.
        double percentCompleted = (double)completedValue / targetValue;
        // Multiply by the associated reward.
        int actualReward = (int)Math.Round(percentCompleted * rewardValue);

        BuildingCreator creator = (BuildingCreator)buildingCreator.GetComponent("BuildingCreator");
        GameObject exManager = GameObject.FindGameObjectWithTag(creator.GetCurrentBuildingDictionary()["BonusType"]);
        //Check if this works
        BonusManager bonus = (BonusManager)exManager.GetComponent("BonusManager");

        if (Convert.ToBoolean(creator.GetCurrentBuildingDictionary()["HasBonus"]))
        {
            if (bonus.GetResult())
            {
                float taskBonus = float.Parse(creator.GetCurrentBuildingDictionary()["BonusAmount"]);
                actualReward = (int)(actualReward * taskBonus);
                print("CLAIMED TASKBONUS: " + taskBonus+ "REWARD: "+actualReward);
            }
        }

        if (claimeDailyBonus < dailyBonusThreshold)
        {
            print("BONUS CLAIMED " + claimeDailyBonus);
            actualReward = (int)(actualReward * bonusReward);
            claimeDailyBonus++;
        }
        // Set the text.
        rewardLabel.text = actualReward + " coins!";

        stats.gold = stats.gold + actualReward;
        stats.update = true;
        saveLoad.SaveGame();

        // Show the exercise done dialog.
        mainMenu.OnExerciseDone();
    }

    public void CompleteExercise()
    {
        ExerciseCounter exCount = (ExerciseCounter)currentCounter.GetComponent("ExerciseCounter");
        exCount.FinishTask();
    }

}
