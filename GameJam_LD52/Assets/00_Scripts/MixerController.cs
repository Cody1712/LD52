using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class MixerController : MonoBehaviour
{
	[SerializeField] private AudioMixer audioMixer;
	[SerializeField] private Slider slider;

	public void SetVolume()
	{
		Debug.Log(slider.value);
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(slider.value) * 20);

    }
}
