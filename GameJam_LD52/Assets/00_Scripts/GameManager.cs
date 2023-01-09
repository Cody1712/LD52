using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : Manager<GameManager>
{
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
    public float GoldValue;



    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForNextTide(lowTideDuration));
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWaterPosition();
        ChangeTide();
    }


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
            StartCoroutine(TriggerTidalChangeEvents(4));
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
