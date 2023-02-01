using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectingSeagull : MonoBehaviour
{
    Material seagullMaterial;
    [SerializeField] private Transform goldFlyToPosition;
    [SerializeField] private Transform seagullStart;
    [SerializeField] private Transform seagullEnd;

    private Rigidbody rb;
    private bool hasCollected;


    [Header("Waypoints")]
    [SerializeField] private float speed = 1;
    [SerializeField] private List<Transform> wayPoints = new List<Transform>();
    private Vector3 activeWaypoint;
    private int wayPointListLength;
    private int activeWayPointIndex = 0;
    private int wayPointDirection = 1;
    private float distance;


    Vector3 moveDir;

    // Start is called before the first frame update
    void Start()
    {
        seagullMaterial = this.GetComponent<Renderer>().sharedMaterial;
        rb = this.GetComponent<Rigidbody>();
        StartCoroutine(AnimateMaterial(seagullMaterial, "_ManualIndex",12,6));

        activeWayPointIndex = 0;
        activeWaypoint = wayPoints[activeWayPointIndex].position;
        this.transform.position = activeWaypoint;

        hasCollected = true;
    }

    // Update is called once per frame
    void Update()
    {
		if (!hasCollected)
		{
            MoveToWayPoint();
        }
    }

    public void Collect()
    {
        activeWayPointIndex = 0;
        activeWaypoint = wayPoints[activeWayPointIndex].position;
        this.transform.position = activeWaypoint;
        hasCollected = false;
    }

    void MoveToWayPoint()
    {
        moveDir = activeWaypoint - this.transform.position;
        distance = moveDir.magnitude;
        moveDir = moveDir.normalized;


        if (distance > 1)
        {
            //this.transform.position = Vector3.Slerp(this.transform.position, activeWaypoint, Time.deltaTime);
            //rb.velocity = moveDir.normalized * speed * 100 * Time.deltaTime;
            //rb.AddForce(moveDir * Time.deltaTime, ForceMode.VelocityChange);

            this.transform.position = Vector3.Slerp(this.transform.position, activeWaypoint, speed * Time.deltaTime);

            //Quaternion newRotation = Quaternion.LookRotation(rb.velocity);
            //this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, 8 * Time.deltaTime);
        }
        else
        {
			if (activeWayPointIndex == 1)
			{
                GameManager.Instance.CollectedGold();
			}

			if (activeWayPointIndex < wayPoints.Count -1)
			{
                activeWayPointIndex += 1;
                activeWaypoint = wayPoints[activeWayPointIndex].position;
            }
			else
			{
                FinishedCollection();
            }
            
        }
    }

    void FinishedCollection()
	{
        hasCollected = true;

    }


    IEnumerator AnimateMaterial(Material mat, string indexName, float fps, int maxFrames)
    {
        for (int i = 0; i < maxFrames; i++)
        {
            yield return new WaitForSeconds(1 / fps);
            mat.SetFloat(indexName, i);
        }
        //mat.SetFloat(indexName, 0);
        StartCoroutine(AnimateMaterial(seagullMaterial, "_ManualIndex", 12, 6));
    }

}
