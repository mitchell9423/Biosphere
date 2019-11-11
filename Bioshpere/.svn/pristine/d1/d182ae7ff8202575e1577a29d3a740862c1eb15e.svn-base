using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	private GameData gameData;
	private List<AudioSource> audioSource = new List<AudioSource>();
	private List<AudioSource> musicSource = new List<AudioSource>();
	private List<AudioSource> ambientSource = new List<AudioSource>();
	private List<string> validScreenNames = new List<string>();

	private void Awake()
	{
		// Loads in all audiosources attatched to AudioManager
		AudioSource[] audioArray = gameObject.GetComponents<AudioSource>();
		for (int i = 0; i < audioArray.Length; i++)
			audioSource.Add(audioArray[i]);

		validScreenNames.Add("TitleScreen");
		validScreenNames.Add("GameBoard");
		validScreenNames.Add("QuestionCard");
		validScreenNames.Add("TeamCountAndNamesMenu");
	}

	// Start is called before the first frame update
	void Start()
    {
		// Loads GameData from resources folder.
		gameData = Resources.Load("GameData") as GameData;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void PlayScreenAudio(string screenName)
	{
		switch (screenName)
		{
			case "TitleScreen":
				{
					StopSounds();
					musicSource.Add(audioSource[0]);
					musicSource[0].loop = true;
					musicSource[0].clip = gameData.GetSoundClip("TitleMusic");
					musicSource[0].Play();
					break;
				}
			case "GameBoard":
				{
					if (screenName != "TitleScreen")
					{
						StopSounds();
						musicSource.Add(audioSource[0]);
						musicSource[0].loop = true;
						musicSource[0].clip = gameData.GetSoundClip("TitleMusic");
						musicSource[0].Play();
						ambientSource.Add(audioSource[1]);
						ambientSource[0].loop = true;
						ambientSource[0].clip = gameData.GetSoundClip("AmbientZoo");
						ambientSource[0].volume = .2f;
						ambientSource[0].Play();
					}
					break;
				}
			case "QuestionCard":
				{
					StopSounds();
					break;
				}
			case "TeamCountAndNamesMenu":
				{
					StopSounds();
					break;
				}
		}
	}
	
	public void PlayOneShot(AudioClip clip)
	{
		if (clip != null)
		{
			foreach (AudioSource source in audioSource)
			{
				if (!source.isPlaying)
				{
					source.PlayOneShot(clip);
					break;
				}
			}
		}
	}

	public void PlayMusic(AudioClip clip)
	{

	}

	public void ChangeMusic(AudioClip clip)
	{

	}

	public void StopSounds()
	{
		foreach (AudioSource source in audioSource)
		{
			source.Stop();
			source.loop = false;
		}
		musicSource.Clear();
		ambientSource.Clear();
	}
}
