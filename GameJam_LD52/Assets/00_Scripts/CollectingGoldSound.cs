using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectingGoldSound : MonoBehaviour, IPooledObject
{
    [SerializeField] private AudioSource audioSource;
    

    public void OnObjectSpawn()
	{
        float randomPitch = Random.Range(0.8f,1.25f);
        audioSource.pitch = randomPitch;
        audioSource.Play();
    }
}
