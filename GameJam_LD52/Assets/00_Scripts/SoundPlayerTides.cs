using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayerTides : MonoBehaviour
{
    //[SerializeField] List<AudioClip> audioClips = new List<AudioClip>();
    //private int currentAudioClipIndex = 0;

    [SerializeField] private AudioSource musicPlayerLowTide;
	[SerializeField] private AudioSource musicPlayerHighTide;

	private float maxVolumeLowTide;
	private float currentVolumeLowTide;

	private float maxVolumeHighTide;
	private float currentVolumeHighTide;


	private void OnDisable()
	{
		GameManager.Instance.onHighTide -= OnHighTide;
		GameManager.Instance.onLowTide -= OnLowTide;
	}


	// Start is called before the first frame update
	void Start()
    {
		//currentAudioClipIndex = 0;

		GameManager.Instance.onHighTide += OnHighTide;
		GameManager.Instance.onLowTide += OnLowTide;

		maxVolumeLowTide = musicPlayerLowTide.volume;
		currentVolumeLowTide = maxVolumeLowTide;

		maxVolumeHighTide = musicPlayerHighTide.volume;
		musicPlayerHighTide.volume = 0f;
		currentVolumeHighTide = 0f;

		//StartCoroutine(PlayMusic(audioClips[currentAudioClipIndex]));
	}

	private void Update()
	{
		musicPlayerLowTide.volume = Mathf.Lerp(musicPlayerLowTide.volume, currentVolumeLowTide, 0.5f * Time.deltaTime);

		musicPlayerHighTide.volume = Mathf.Lerp(musicPlayerHighTide.volume, currentVolumeHighTide, 0.5f * Time.deltaTime);
	}

	void OnHighTide()
	{
		currentVolumeHighTide = maxVolumeHighTide;
		currentVolumeLowTide = 0f;
	}
	void OnLowTide()
	{
		currentVolumeLowTide = maxVolumeLowTide;
		currentVolumeHighTide = 0f;
	}



	/*
	IEnumerator PlayMusic(AudioClip audioClip)
    {
        musicPlayer.clip = audioClip;
        musicPlayer.Play();
        yield return new WaitForSeconds(audioClip.length);

		if (currentAudioClipIndex + 1 < audioClips.Count)
		{
            currentAudioClipIndex += 1;
            StartCoroutine(PlayMusic(audioClips[currentAudioClipIndex]));
        }
		else
		{
            currentAudioClipIndex = 0;
            StartCoroutine(PlayMusic(audioClips[currentAudioClipIndex]));
        }
    }
	*/

}
