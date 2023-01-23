using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SwitchUI : MonoBehaviour
{
	[SerializeField] GameObject newUI;
	[SerializeField] List<GameObject> objectsToTurnOff = new List<GameObject>();

	public void SwitchUIOut()
	{
		newUI.SetActive(true);
		foreach (GameObject g in objectsToTurnOff)
		{
			g.SetActive(false);
		}
	}

	public void LoadTitle()
	{
		SceneManager.LoadScene(0);
	}

	public void Quit()
	{
		Application.Quit();
		Debug.Log("Quit");
	}
}
