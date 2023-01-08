using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDetector : MonoBehaviour
{
	private CharacterController playerController;
	public void SetPlayer(CharacterController player)
	{
		playerController = player;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Water"))
		{
			playerController.InWater();
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Water"))
		{
			playerController.OutOfWater();
		}
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(this.transform.position, 1f);
	}
#endif
}
