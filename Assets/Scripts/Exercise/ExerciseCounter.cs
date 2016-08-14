using UnityEngine;
using System.Collections;

public interface ExerciseCounter {

    void StartCounting();
    void FinishTask();
    bool HasCompletedAtLeast(float proportion); // Checks if counter is at least the proportion (specified) of the goal.
}
