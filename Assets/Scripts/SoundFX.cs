using UnityEngine;
using System.Collections;

public class SoundFX : MonoBehaviour {

	public AudioClip buildingFinished, move, click, close;//sounds

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//called by buttons pressed in various 2dToolkit interfaces and played near the camera
	public void BuildingFinished()
	{	
		GetComponent<AudioSource>().PlayOneShot(buildingFinished);
	}

	public void Move() { GetComponent<AudioSource>().PlayOneShot(move); }

	public void Click()	{ GetComponent<AudioSource>().PlayOneShot(click);	}

	public void Close()	{ GetComponent<AudioSource>().PlayOneShot(close);	}

	public void SoundOn() 
	{ 
		AudioListener.volume = 1;
	}

	public void SoundOff()
	{
		AudioListener.volume = 0;
	}
}
