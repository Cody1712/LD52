using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectingGoldSound : MonoBehaviour, IPooledObject
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] bool pitchVariation;

    [SerializeField][Range(0,2)] float basePitch = 1;


	public void OnObjectSpawn()
	{
		if (pitchVariation)
		{
            float randomPitch = Random.Range(-0.2f, 0.25f);
            audioSource.pitch = basePitch + randomPitch;
        }
        audioSource.Play();
    }
}
