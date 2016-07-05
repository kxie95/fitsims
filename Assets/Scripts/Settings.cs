using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
public class Settings : MonoBehaviour {

	public GameObject StartMusicBt, StopMusicBt, StartSoundBt, StopSoundBt;	//buttons
	private bool musicOn = true, soundOn = true;

	private bool fadeIn = false;
	private bool fadeOut = false;
	private bool busy = false;

	private bool isPlaying = true;
	private float musicVoume = 1;

    private AudioSource audioSource;
    public AudioClip mainMusic;
    public const int musicCollectionSize = 3;
    public AudioClip[] exerciseMusicCollection = new AudioClip[musicCollectionSize];

	private GameObject SoundFX;
	private Component soundFXSc;


	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = mainMusic;
		audioSource.Play ();
		audioSource.loop = true;
		audioSource.ignoreListenerVolume = true;

		SoundFX =  GameObject.Find("SoundFX");
		soundFXSc = SoundFX.GetComponent ("SoundFX");
	}

	public void MusicOn()
	{
		if (!fadeIn && !busy) 
		{
			fadeIn = true;
			busy = true;

			if (!isPlaying) 
			{	
				isPlaying=true;
				GetComponent<AudioSource>().Play ();
			}
			SwitchMusic ();
		}

	}
	
	public void MusicOff()
	{
		if (!fadeOut && !busy) 
		{
			fadeOut = true;
			busy = true;
			SwitchMusic ();
		}

	}

	public void SoundOn()
	{
		AudioListener.volume = 1;
		((SoundFX)soundFXSc).SoundOn ();
		SwitchSound ();
	}
	
	public void SoundOff()
	{
		AudioListener.volume = 0;
		((SoundFX)soundFXSc).SoundOff ();
		SwitchSound ();
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	// Update is called once per frame
	void Update () {
	
		if(fadeIn)
		{
			if(musicVoume<1)
			{
			musicVoume += 0.01f;
				musicVoume = Mathf.Clamp(musicVoume,0,1);
				audioSource.volume = musicVoume;
			}
			else
			{
				fadeIn = false;
				busy = false;
			}

		}
		if(fadeOut)
		{
			if(musicVoume>0)
			{
				musicVoume -= 0.01f;
				musicVoume = Mathf.Clamp(musicVoume,0,1);
				audioSource.volume = musicVoume;
			}
			else
			{
				fadeOut = false;
				busy = false;
				audioSource.Pause();
				isPlaying = false;
			}			
		}

	}

	public void SwitchMusic()
	{
		musicOn = !musicOn;
		MusicBt ();
	}

	public void SwitchSound()
	{
		soundOn = !soundOn;
		SoundBt ();
	}
	
	private void MusicBt()
	{
		StartMusicBt.SetActive (!musicOn);
		StopMusicBt.SetActive (musicOn);
	}

	private void SoundBt()
	{
		StartSoundBt.SetActive (!soundOn);
		StopSoundBt.SetActive (soundOn);
	}

    public void PlayExerciseMusic()
    {
        // Select a random track from the music collection.
        // TODO: Possibly change this to suit the exercise later.
        System.Random r = new System.Random();
        int randomNumber = r.Next(0, musicCollectionSize);
        audioSource.clip = exerciseMusicCollection[randomNumber];
        audioSource.Play();
    }

    public void PlayMainMusic()
    {
        audioSource.clip = mainMusic;
        audioSource.Play();
    }
}
