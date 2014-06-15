using UnityEngine;
using System.Collections;

public class RadioScript : MonoBehaviour
{
	public GameObject MusicPlayer;
	public AudioClip[] RadioClips;
	private int currentClip;

	private float musicVolume, interruptVolume;
	private float radioInterruptTimer, radioInterruptDelay;
	private float fadeTimer, fadeDelay;
	private bool fadeIn;
	
	// Use this for initialization
	void Start ()
	{
		currentClip = Random.Range (0, RadioClips.Length);
		musicVolume = 0.4f;
		interruptVolume = 0.8f;
		radioInterruptTimer = 0;
		radioInterruptDelay = 0;
		fadeDelay = 1.5f;
		fadeTimer = fadeDelay;
		fadeIn = true;

		this.audio.volume = interruptVolume;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (this.audio.isPlaying == true)
		{
			// Run the interrupt timer.
			fadeTimer -= Time.deltaTime;
			if (fadeTimer >= 0)
			{
				MusicPlayer.audio.volume = fadeTimer / fadeDelay * musicVolume;

				if (radioInterruptTimer != 0)
				{
					ChooseNextRadioInterrupt();
				}
			}
			else
			{
				fadeTimer = 0;
			}
		}
		else
		{
			radioInterruptTimer += Time.deltaTime;
			if (radioInterruptTimer >= radioInterruptDelay)
			{
				this.audio.clip = RadioClips[currentClip];
				currentClip = (currentClip + 1) % RadioClips.Length;
				this.audio.Play ();
			}
			else
			{
				fadeTimer += Time.deltaTime;
				if (fadeTimer <= fadeDelay)
				{
					MusicPlayer.audio.volume = fadeTimer / fadeDelay * musicVolume;
				}
				else
				{
					fadeTimer = fadeDelay;
				}
			}
		}
	}

	void ChooseNextRadioInterrupt()
	{
		radioInterruptDelay = Random.Range (60, 240);
		//radioInterruptDelay = 5;
		radioInterruptTimer = 0;
	}
}
