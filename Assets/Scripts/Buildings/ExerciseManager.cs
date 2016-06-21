using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class ExerciseManager : MonoBehaviour {

    public MainMenu mainMenu;

    // Use this for initialization
    void Start () {

       
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void PerformExercise()
    {
        Debug.Log("Exercise performed");
        //Get the current selected object
        //Call method from object to get data on what sort of exercise/how much of that exercise
        //Start the measurement (possibly by calling and external method)

        //SceneManager.LoadScene("ChallengeScene");
        //TODO: Think about how to properly link components up.
        // mainMenu.OnDoingExercise();

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
        // Get text from DoingExercise dialog.

        // Convert to double.

        // Divide by assigned distance.

        // Multiply by the associated reward.

        // Set the text.

        // Show the exercise done dialog.
        mainMenu.OnExerciseDone();
    }
}
