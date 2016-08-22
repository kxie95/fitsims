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

    public float maxSadPet = 5;
    public float bonusReward;
    public int dailyBonusThreshold = 3;
    public UISprite[] bonusStars;

    // UI components
    public UILabel rewardLabel; // Reward label in ExerciseDone screen.

    public DateTime currentDate;
    public GameObject buildingCreator;
    public int claimedDailyBonus = 0;

    public float numSadPets;

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
        double actualReward = Math.Round(percentCompleted * rewardValue)/2;
        //double actualReward = 400 / 2;
        double actualRewardUnchanged = actualReward;
        double exerciseTaskBonus = 0;
        double dailyTaskBonus = 0;

        GameObject exManager = GameObject.FindGameObjectWithTag(creator.GetCurrentBuildingDictionary()["BonusType"]);
        //Check if this works
        BonusManager bonus = (BonusManager)exManager.GetComponent("BonusManager");

        //Set the task bonus gold
        if (Convert.ToBoolean(creator.GetCurrentBuildingDictionary()["HasBonus"]))
        {
            if (bonus.GetResult())
            {
                double taskBonus = double.Parse(creator.GetCurrentBuildingDictionary()["BonusAmount"]) - 1.0;
                exerciseTaskBonus = (actualReward * taskBonus);
                print("CLAIMED TASKBONUS: " + exerciseTaskBonus + "REWARD: "+actualReward + "BONUS AMOUNT: "+taskBonus);
            }
        }
        //Set the daily bonus gold
        if (claimedDailyBonus < dailyBonusThreshold)
        {
            print("CLAIMED DAILYBONUS " + claimedDailyBonus);
            //actualReward = (int)(actualReward * bonusReward);
            dailyTaskBonus = actualRewardUnchanged * bonusReward;
            claimedDailyBonus++;
            UpdateDailyBonusUi(claimedDailyBonus);
        }
        if (numSadPets <= maxSadPet)
        {
            actualReward = actualReward * ((maxSadPet - numSadPets) / maxSadPet);
        }
        else
        {
            actualReward = 0;
        }
        

        print("Half nonminused: "+actualRewardUnchanged+" Half minused: "+actualReward+" Daily bonus amount: "+ dailyTaskBonus + " Task bonus amount: "+ exerciseTaskBonus);

        int totalReward = (int)(actualRewardUnchanged + actualReward + dailyTaskBonus + exerciseTaskBonus);

        // Set the text.
        //rewardLabel.text = totalReward + " coins!";
        rewardLabel.text = "Murakami’s home is highly functional with a calm and polished atmosphere. It’s not only \n simple but also a product of ingenuity. This isn’t only accomplished for her workspace \n but also for her live-in partner, Amanosuke the cat. \nOur interview begins while also being considerate of Amanosuke’s wariness of our \n\n\n - Please tell us about your workspace.";
        stats.gold = stats.gold + totalReward;
        stats.update = true;
        ((Happiness)creator.selectedBuilding.GetComponent("Happiness")).IncreaseHP();

        //Update user data
        userData.GoldEarned += totalReward;
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

    public void DecrementNumSadPets()
    {
        numSadPets--;
        if (numSadPets < 0)
        {
            numSadPets = 0;
        }
    }
}
