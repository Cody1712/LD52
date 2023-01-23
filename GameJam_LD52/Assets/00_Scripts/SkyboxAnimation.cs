using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxAnimation : MonoBehaviour
{
	//[SerializeField] Material titleSkyboxMaterial;
	[SerializeField] float rotationSpeed;

	private void Start()
	{
		//titleSkyboxMaterial.SetInt
	}

	private void Update()
	{
		RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
	}
}
