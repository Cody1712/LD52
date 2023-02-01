using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class GameManager : Manager<GameManager>
{
    public enum GameState { RUNNING, PAUSE, DIALOGUE};
    public GameState gameState = GameState.RUNNING;

    [Header("Positions")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject bobTheBird;
    [SerializeField] Transform goldFlyToPosition;
    [SerializeField] GameObject seagull;
    List<GameObject> droppedGold = new List<GameObject>();


    [Header("UI")]
    [SerializeField] List<GameObject> gameUIElements = new List<GameObject>();
    [SerializeField] List<GameObject> menuUIElements = new List<GameObject>();
    [SerializeField] GameObject baseGameElement;
    [SerializeField] GameObject baseMenuElement;

    [SerializeField] GameObject gameVolume;
    [SerializeField] GameObject menuVolume;

    [Header("Positioning")]
    [SerializeField] GameObject waterPlane;
    [SerializeField] float highTideHight = 1f;
    [SerializeField] float lowTideHight = -1f;
    Vector3 newWaterPosition;

    [Header("Timing")]
    [SerializeField] float waterSinkingTime = 0.3f;
    [SerializeField] float waterRisingTime = 0.3f;
    [SerializeField] float highTideDuration = 10f;
    [SerializeField] float lowTideDuration = 20f;
    private bool isHighTide = false;

    //[Header("Fish Control")]
    public event Action onHighTide;
    public event Action onLowTide;


    [Header("Gold")]
    public int goldInventory;
    public int goldStash;


    public void Hurt()
	{
        goldInventory -= 4;
        if(goldInventory < 0)
		{
            goldInventory = 0;
		}
	}

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(WaitForNextTide(lowTideDuration));
        CloseMenu();
    }

    // Update is called once per frame
    void Update()
    {
		if (gameState == GameState.RUNNING)
		{
            UpdateWaterPosition();
            ChangeTide();
        }

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (gameState == GameState.RUNNING)
			{
                OpenMenu();
            }
            else if (gameState == GameState.PAUSE)
            {
                CloseMenu();
            }
		}
    }

    public void SpendGold()
	{
		if (goldInventory != 0)
		{
            StartCoroutine(SpendGoldOverTime(goldInventory));
        }
    }

    public void CollectedGold()
	{
		foreach (GameObject g in droppedGold)
		{
            g.SetActive(false);
		}
        droppedGold.Clear();
    }

    IEnumerator SpendGoldOverTime(int oldGoldInventory)
	{
        for (int i = 0; i < oldGoldInventory; i++)
        {
            goldStash += 1;
            goldInventory -= 1;

            int randomNumber = UnityEngine.Random.Range(1, 3);
            string randomGoldVisual = "";
            if (randomNumber == 1)
            {
                randomGoldVisual = "Gold_Visual_01";
            }
            else if (randomNumber == 2)
            {
                randomGoldVisual = "Gold_Visual_02";
            }
            else
            {
                randomGoldVisual = "Gold_Visual_03";
            }
            Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(0, 0.5f), UnityEngine.Random.Range(0, 0.5f), UnityEngine.Random.Range(0, 0.5f));
            GameObject newGold = ObjectPooler.Instance.SpawnFromPool(randomGoldVisual, player.transform.position + Vector3.up, null, Quaternion.identity);
            newGold.transform.DOMove(goldFlyToPosition.position + randomOffset, 0.5f).SetEase(Ease.OutSine);

            droppedGold.Add(newGold);

            //GameObject newSeagull = ObjectPooler.Instance.SpawnFromPool("Seagull", seagullStart.position, null, Quaternion.identity);
            //newSeagull.transform.DOMove(seagullEnd.position,1);

            yield return new WaitForSeconds(0.1f);
        }

        seagull.GetComponent<CollectingSeagull>().Collect();
        goldInventory = 0;
    }

	#region Menu
	private void OpenMenu()
	{
		foreach (GameObject g in gameUIElements)
		{
            g.SetActive(false);
		}
        foreach (GameObject g in menuUIElements)
        {
            g.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        gameVolume.SetActive(false);
        menuVolume.SetActive(true);

        baseMenuElement.SetActive(true);

        gameState = GameState.PAUSE;
    }

    private void CloseMenu()
	{
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (GameObject g in gameUIElements)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in menuUIElements)
        {
            g.SetActive(false);
        }

        menuVolume.SetActive(false);
        gameVolume.SetActive(true);

        baseGameElement.SetActive(true);

        gameState = GameState.RUNNING;
    }
	#endregion

	#region Tidle change
	void UpdateWaterPosition()
	{
		if (isHighTide)
		{
            waterPlane.transform.position = Vector3.Slerp(waterPlane.transform.position, newWaterPosition, waterRisingTime * Time.deltaTime);
        }
		else
		{
            waterPlane.transform.position = Vector3.Slerp(waterPlane.transform.position, newWaterPosition, waterSinkingTime * Time.deltaTime);
        }
        
    }

    void ChangeTide()
	{
		if (isHighTide)
		{
            newWaterPosition = new Vector3(waterPlane.transform.position.x, highTideHight, waterPlane.transform.position.z);
        }
		else
		{
            newWaterPosition = new Vector3(waterPlane.transform.position.x, lowTideHight, waterPlane.transform.position.z);
        }
	}

    

    IEnumerator WaitForNextTide(float duration)
	{
        yield return new WaitForSeconds(duration);
        isHighTide = !isHighTide;

		if (isHighTide)
		{
            StartCoroutine(TriggerTidalChangeEvents(6));
            StartCoroutine(WaitForNextTide(highTideDuration));
        }
		else
		{
            StartCoroutine(TriggerTidalChangeEvents(0));
            StartCoroutine(WaitForNextTide(lowTideDuration));
        }
    }

    IEnumerator TriggerTidalChangeEvents(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (isHighTide)
        {
            onHighTide.Invoke();
        }
        else
        {
            onLowTide.Invoke();
        }
    }
	#endregion



#if UNITY_EDITOR

	private void OnDrawGizmos()
	{
        Vector3 highTidePosition = new Vector3(0, highTideHight, 0);
        Vector3 lowTidePosition = new Vector3(0, lowTideHight, 0);
        Vector3 gizmoSize = new Vector3(2f,0.1f,2f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(highTidePosition, lowTidePosition);

        Gizmos.DrawCube(highTidePosition, gizmoSize);
        Gizmos.DrawCube(lowTidePosition, gizmoSize);
	}

#endif
}
