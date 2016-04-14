using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuUnitBase : MonoBehaviour {
	
	
	//public bool stateEnabled;
	
	public int hours, minutes, seconds;	
	public int maxUnits = 10;//maximum number of units of each type that can be trained	
	public const int maxTypes = 3;// the 3 sockets for units under construction	
	public int typesCounter = 0;//keeps track of different types of units being constructed
	//public float timeAcceleration = 1f; 
	
	//Time.deltaTime = 0.016; 60*Time.deltaTime = 1s ; runs at 60fps
	
	
	public float progTime = 0.57f; //current unit progress bars timer, also correction.57
	public float progCounter = 0;//progress counter, reaches construction time, then is reset to 0
	
	public float currentSlidVal = 0; //current slider value
	public int currentTrainingTime = 0;
	
	//public int totalTime;
	public float currentTimeRemaining;//seconds+minutes+hours, necessary for establishing hard price
	public float timeRemaining;//full

	public const int numberOfUnits = 12;
	public int[] trainingTimes = new int[numberOfUnits];//{1,2,3,4,5,6,7,8,9,10,11,12};//time to finish training for each unit- values are replaced with XML	
	public int[] trainingIndexes = new int[numberOfUnits];//how many units are under construction - from 0 to 10 maximum 
	
	  
	public List<Vector3> queList = new List<Vector3>();// qIndex, objIndex, trainingIndex

}
