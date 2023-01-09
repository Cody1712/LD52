using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish_Behaviour : MonoBehaviour
{

    private enum FishState { Patrol, Hunt, Hide };
    private FishState fishState = FishState.Patrol;

    [Header("Movement")]
    [SerializeField] float speed = 1;
    private Vector3 moveDir;

    [Header("Waypoints")]
    [SerializeField] List<Vector2> wayPoints = new List<Vector2>();
    [SerializeField] bool isLoop = false;
    private Vector3 activeWaypoint;
    private int wayPointListLength;
    private int activeWayPointIndex = 0;
    private int wayPointDirection = 1;
    private float distance;

    private Rigidbody rb;

	[Header("Gold")]
    [SerializeField][Tooltip("In Seconds")] float goldDropInterval = 1f;
    private float timeSinceLastDrop;



    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        wayPointListLength = wayPoints.Count;
        activeWayPointIndex = 0;
        activeWaypoint = new Vector3(wayPoints[activeWayPointIndex].x, this.transform.position.y, wayPoints[activeWayPointIndex].y);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBehaviour();
    }


    void UpdateBehaviour()
    {
        switch (fishState)
        {
            case FishState.Patrol:
                MoveToWayPoint();
                GoldTimer(1);
                break;

            case FishState.Hunt:

                GoldTimer(0.8f);
                break;

            case FishState.Hide:

                break;
        }
    }

	#region Gold
	void GoldTimer(float intervalMultiplier)
	{
        timeSinceLastDrop += Time.deltaTime;

        if (timeSinceLastDrop > (goldDropInterval * intervalMultiplier)) 
        {
            timeSinceLastDrop = 0;
            DropGold();
        }
    }

	void DropGold()
	{
        Debug.Log("Drop Gold");

        ObjectPooler.Instance.SpawnFromPool("Gold_medium",this.transform.position,null, Quaternion.identity);
    }
	#endregion

    public void GettingPicked()
	{
        if(fishState == FishState.Hide)
		{
            DropGold();
        }
    }

	void MoveToWayPoint()
    {
        moveDir = activeWaypoint - this.transform.position;
        distance = moveDir.magnitude;


        if (distance > 1)
        {
            //this.transform.position = Vector3.Slerp(this.transform.position, activeWaypoint, Time.deltaTime);
            //rb.velocity = moveDir.normalized * speed * 100 * Time.deltaTime;
            rb.AddForce(moveDir.normalized * speed, ForceMode.Force);

            Quaternion newRotation = Quaternion.LookRotation(rb.velocity);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, 8 * Time.deltaTime);
        }
        else
        {
			if (!isLoop)
			{
                if (activeWayPointIndex >= wayPoints.Count - 1)
                {
                    wayPointDirection = -1;
                }
                if (activeWayPointIndex == 0 && wayPointDirection == -1)
                {
                    wayPointDirection = 1;
                }
            }
			else
			{
				if (activeWayPointIndex == wayPoints.Count - 1)
				{
                    activeWayPointIndex = -1;
                    wayPointDirection = 1;
                }
			}

            activeWayPointIndex += wayPointDirection;
            activeWaypoint = new Vector3(wayPoints[activeWayPointIndex].x, this.transform.position.y, wayPoints[activeWayPointIndex].y);
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
		if (wayPoints.Count > 1)
		{
            for (int i = 0; i < wayPoints.Count; i++)
            {
                Gizmos.DrawCube(new Vector3(wayPoints[i].x, 0, wayPoints[i].y), Vector3.one * 2);
                if (i != 0)
                {
                    Gizmos.DrawLine(new Vector3(wayPoints[i - 1].x, 0, wayPoints[i - 1].y), new Vector3(wayPoints[i].x, 0, wayPoints[i].y));
                }
            }
            if (isLoop)
            {
                Gizmos.DrawLine(new Vector3(wayPoints[wayPoints.Count - 1].x, 0, wayPoints[wayPoints.Count - 1].y), new Vector3(wayPoints[0].x, 0, wayPoints[0].y));
            }
        }
    }

#endif

}
