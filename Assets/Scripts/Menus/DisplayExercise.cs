using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class DisplayExercise : MonoBehaviour {

    public BuildingSelector buildingCreator;
    private Dictio

    public UILabel label; // Instructions in text form.
    public UISprite image; // Image shown for instruction.

	// Use this for initialization
	void Start () {
	    if (buildingCreator != null)
        {
            selectedBuildingAttr = buildingCreator.GetCurrentBuildingDictionary();
        }
	}

    public void ShowInstructionForExercise()
    {
        if (selectedBuildingAttr == null)
        {
            Debug.Log("Dictionary is null.");
            return;
        }

        label.text = selectedBuildingAttr["TaskInstruction"];
        image.spriteName = selectedBuildingAttr["TaskType"];
    }
}
