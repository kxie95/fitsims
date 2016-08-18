using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class ExerciseManager : MonoBehaviour {

    private MainMenu mainMenu;
    private BuildingCreator creator;

    public Stats stats;
    public UserData userData;
    public SaveLoad saveLoad;
    public Settings settings;
    public SoundFX soundFx;

    public float bonusReward;
    public int dailyBonusThreshold = 3;
    public UISprite[] bonusStars;

    // UI components
    public UILabel rewardLabel; // Reward label in ExerciseDone screen.

    public DateTime currentDate;
    public GameObject buildingCreator;
    public int claimedDailyBonus = 0;

    private GameObject currentCounter;

    void Start () {
        creator = (BuildingCreator)buildingCreator.GetComponent("BuildingCreator");
        mainMenu = (MainMenu)GameObject.Find("UIAnchor").GetComponent("MainMenu");
        //fish out current date
    }
	
	void Update () {

        //reset the claimed bonus if the date changes
        if(currentDate.Date < DateTime.Now.Date)
        {
            claimedDailyBonus = 0;
            currentDate = DateTime.Now.Date;
            UpdateDailyBonusUi(claimedDailyBonus);
        }

    }
    
    public void PerformExercise()
    {
        settings.PlayExerciseMusic();

        BuildingCreator creator = (BuildingCreator)buildingCreator.GetComponent("BuildingCreator");
        currentCounter = GameObject.FindGameObjectWithTag(creator.GetCurrentBuildingDictionary()["TaskType"]);
        Debug.Log("TASKTYPE: " + creator.GetCurrentBuildingDictionary()["TaskType"]);
        ExerciseCounter exCount = (ExerciseCounter)currentCounter.GetComponent("ExerciseCounter");

        exCount.StartCounting();
        TryStartBonus();
        userData.InTask = true;
        mainMenu.onDoingExercise();
    }

    private void TryStartBonus()
    {
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

        if (claimedDailyBonus < dailyBonusThreshold)
        {
            print("BONUS CLAIMED " + claimedDailyBonus);
            actualReward = (int)(actualReward * bonusReward);
            claimedDailyBonus++;
            UpdateDailyBonusUi(claimedDailyBonus);
        }

        // Set the text.
        rewardLabel.text = actualReward + " coins!";

        stats.gold = stats.gold + actualReward;
        stats.update = true;
        userData.GoldEarned += actualReward;
        userData.TasksFinished ++;
        userData.InTask = false;
        saveLoad.SaveGame();

        // Show the exercise done dialog.
        mainMenu.OnExerciseDone();
    }
    
    public void CompleteExercise()
    {
        ExerciseCounter exCount = (ExerciseCounter)currentCounter.GetComponent("ExerciseCounter");
        exCount.FinishTask();
    }
    
    /// <summary>
    /// Called when the player clicks on a pet to exercise with them.
    /// Loads in the correct instructions for the UI.
    /// </summary>
    public void LoadInstruction(UISprite sprite, UILabel instructionLabel)
    {
        string taskType = creator.GetCurrentBuildingDictionary()["TaskType"].ToLower();
        string taskInstruction = creator.GetCurrentBuildingDictionary()["TaskInstruction"];
        string taskAmount = creator.GetCurrentBuildingDictionary()["TaskRequirement"];
        string taskUnit = creator.GetCurrentBuildingDictionary()["TaskUnit"];

        if (sprite != null)
        {
            sprite.spriteName = taskType; // Set the image to the corresponding instruction.

        }

        if (instructionLabel != null)
        {
            instructionLabel.text = taskInstruction + "\nDo " + taskAmount + " " + taskUnit;
        }    
    }

    public void SetClaimedBonus(int claimed, DateTime lastSaveDate)
    {
        claimedDailyBonus = claimed;
        currentDate = lastSaveDate;
        UpdateDailyBonusUi(claimedDailyBonus);
    }

    private void UpdateDailyBonusUi(int bonusUsed)
    {
        hideStars();
        int bonusesLeft = dailyBonusThreshold - bonusUsed;
        for (int i = 0; i < bonusesLeft; i++)
        {
            bonusStars[i].enabled = true;
        }
    }

    private void hideStars()
    {
        for (int i = 0; i < bonusStars.Length; i++)
        {
            bonusStars[i].enabled = false;
        }
    }
}
