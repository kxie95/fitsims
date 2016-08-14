using UnityEngine;
using System.Collections;

public class SoundFX : MonoBehaviour {

	public AudioClip buildingFinished, move, click, close, victory;//sounds

    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//called by buttons pressed in various 2dToolkit interfaces and played near the camera
	public void BuildingFinished()
	{	
		GetComponent<AudioSource>().PlayOneShot(buildingFinished);
	}

	public void Move() { audioSource.PlayOneShot(move); }

	public void Click()	{ audioSource.PlayOneShot(click);	}

	public void Close()	{ audioSource.PlayOneShot(close);	}

    public void Victory() { audioSource.PlayOneShot(victory); }

	public void SoundOn() 
	{ 
		AudioListener.volume = 1;
	}

	public void SoundOff()
	{
		AudioListener.volume = 0;
	}
}
