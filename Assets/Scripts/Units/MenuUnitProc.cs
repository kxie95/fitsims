﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuUnitProc : MenuUnitBase {//when the units menu is closed(deactivated), this takes over
										  //also used to extract/update data when saving/loading	
	
	public GameObject MenuUnit;
	public bool start = false;
	
	// Use this for initialization
	void Start () {

	}

	void FixedUpdate()
	{
		if(queList.Count > 0)
		{
			Progress();						
		}	

		if(start)//one-shot initialization
		{			
			Initialize();
			start = false;
		}

	}

	private void Initialize()
	{		
			SortList();
	}
	
	private void SortList()//sorts the list based on index in queue
	{
		queList.Sort(delegate (Vector3 v1, Vector3 v2)// qIndex, objIndex, trainingIndex
		{
			return v1.x.CompareTo(v2.x);			
		});	
	}

	private void Progress()
	{
		progCounter += Time.deltaTime*0.5f;

		if(progCounter > progTime)
		{	
			progCounter = 0;	
			currentSlidVal += (Time.deltaTime)/currentTrainingTime;

			//adjusts timeRemaining
			currentTimeRemaining = (1 - currentSlidVal)*currentTrainingTime;
			currentTimeRemaining = Mathf.Clamp(currentTimeRemaining,0,currentTrainingTime);

			currentSlidVal = Mathf.Clamp(currentSlidVal,0,1);
		
		if(currentSlidVal==1)
			{
				if(queList[0].z>1)//still more than one unit
				{			
					int newIndex = (int)queList[0].z;
					newIndex--;
					queList[0]= new Vector3(queList[0].x, queList[0].y, newIndex);	
				}
				else if(queList[0].z==1)//just built the last unit in first position
				{
					queList.RemoveAt(0);
					SortList();
				}
				if(queList.Count>0)//there were other units in queue
				{
					currentTrainingTime = trainingTimes[(int)queList[0].y];
					currentTimeRemaining = currentTrainingTime;
				}			
				currentSlidVal = 0;
			}	
		}
	}
	
	
}
