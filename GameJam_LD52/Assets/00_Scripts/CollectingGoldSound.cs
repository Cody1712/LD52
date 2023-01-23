using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectingGoldSound : MonoBehaviour, IPooledObject
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] bool pitchVariation;

    [SerializeField][Range(0,2)] float basePitch = 1;
    [SerializeField] float minPitchChange = -0.2f;
    [SerializeField] float maxPitchChange = 0.25f;


	public void OnObjectSpawn()
	{
		if (pitchVariation)
		{
            float randomPitch = Random.Range(minPitchChange, maxPitchChange);
            audioSource.pitch = basePitch + randomPitch;
        }
        audioSource.Play();
    }
}
