using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] List<AudioClip> audioClips = new List<AudioClip>();
    private int currentAudioClipIndex = 0;

    private AudioSource musicPlayer;

    // Start is called before the first frame update
    void Start()
    {
        currentAudioClipIndex = 0;
        musicPlayer = this.GetComponent<AudioSource>();
		StartCoroutine(PlayMusic(audioClips[currentAudioClipIndex]));
    }

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

}
