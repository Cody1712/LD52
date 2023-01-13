using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish_Behaviour : MonoBehaviour
{

    private enum FishState { Patrol, Hunt, Hide };
    private FishState fishState = FishState.Patrol;

    private Rigidbody rb;
    private Renderer renderer;

    [Header("Movement")]
    [SerializeField] float speed = 1;
    [SerializeField] float acceleration = 1;
    private Vector3 moveDir;
    private Vector3 hidingSpot;
    [SerializeField] private float fishHight = - 1;

    private float newSpeed = 1f;
    private float currentSpeed = 1f;

    [Header("Hunt")]
    [SerializeField] GameObject player;
    [SerializeField] float huntSpeed = 4f;
    [SerializeField] float agroRadius = 4f;
    [SerializeField] GameObject signal;
    [SerializeField] LayerMask playerLayer;
    private bool canHunt = true;

    [Header("Waypoints")]
    [SerializeField] List<Transform> wayTransforms = new List<Transform>();
    List<Vector2> wayPoints = new List<Vector2>();
    [SerializeField] bool isLoop = false;
    private Vector3 activeWaypoint;
    private int wayPointListLength;
    private int activeWayPointIndex = 0;
    private int wayPointDirection = 1;
    private float distance;


	[Header("Gold")]
    [SerializeField][Tooltip("In Seconds")] float goldDropInterval = 1f;
    [SerializeField] float goldDropHight = -2.1f;
    private float timeSinceLastDrop;

	private void Awake()
	{
		foreach (Transform t in wayTransforms)
		{
            wayPoints.Add(new Vector2(t.transform.position.x, t.transform.position.z));
        }
	}

	// Start is called before the first frame update
	void Start()
    {
        renderer = this.GetComponent<Renderer>();
        rb = this.GetComponent<Rigidbody>();
        wayPointListLength = wayPoints.Count;
        activeWayPointIndex = 0;
        activeWaypoint = new Vector3(wayPoints[activeWayPointIndex].x, fishHight, wayPoints[activeWayPointIndex].y);
        this.transform.position = activeWaypoint;

        fishState = FishState.Hide;
        hidingSpot = new Vector3(this.transform.position.x, -2.2f, this.transform.position.z);

        GameManager.Instance.onHighTide += OnHighTide;
        GameManager.Instance.onLowTide += OnLowTide;

        newSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBehaviour();

        currentSpeed = Mathf.Lerp(currentSpeed, newSpeed, acceleration * Time.deltaTime);
    }


    void UpdateBehaviour()
    {
        switch (fishState)
        {
            case FishState.Patrol:
                newSpeed = speed;
                signal.SetActive(false);
                MoveToWayPoint();
                GoldTimer(1);
                CheckPlayerDistance();
                break;

            case FishState.Hunt:
                newSpeed = huntSpeed;
                signal.SetActive(true);
                Hunt();
                GoldTimer(0.8f);
                break;

            case FishState.Hide:
                newSpeed = speed;
                signal.SetActive(false);
                Hide();
                break;
        }
    }

    private void OnHighTide()
	{
        fishState = FishState.Patrol;
	}
    private void OnLowTide()
	{
        fishState = FishState.Hide;
        hidingSpot = new Vector3(this.transform.position.x, -2.2f, this.transform.position.z);
    }

    private void Hunt()
	{
		if (RaycastCheck() && canHunt)
		{
            Debug.Log("Hunting!!!");

            Vector3 newPlayerPosition = new Vector3(player.transform.position.x, fishHight, player.transform.position.z);

            moveDir = newPlayerPosition - this.transform.position;

			if (moveDir.magnitude < 1.5f)
			{
                player.GetComponent<CharacterController>().Hurt();
                GameManager.Instance.Hurt() ;
                canHunt = false;
                StartCoroutine(WaitforNextHunt(3));
			}

            moveDir = moveDir.normalized;

            rb.AddForce(moveDir * currentSpeed * Time.deltaTime, ForceMode.VelocityChange);

            Quaternion newRotation = Quaternion.LookRotation(rb.velocity);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, 8 * Time.deltaTime);
        }
		else
		{
            fishState = FishState.Patrol;
		}

    }

    IEnumerator WaitforNextHunt(float waitTime)
	{
        yield return new WaitForSeconds(waitTime);
        canHunt = true;
	}

    private void CheckPlayerDistance()
	{
        float distance = Vector3.Distance(player.transform.position, this.transform.position);

		if (distance < agroRadius)
		{
			if (RaycastCheck())
			{
				if (canHunt)
				{
                    fishState = FishState.Hunt;
                }
			}
		}
	}

    private bool RaycastCheck()
	{
        Vector3 newPlayerPosition = new Vector3(player.transform.position.x, fishHight, player.transform.position.z);
        Vector3 rayDir = newPlayerPosition - this.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, rayDir, out hit, agroRadius, playerLayer))
		{
			if (hit.collider.CompareTag("Player"))
			{
                return true;
            }
		}
        return false;
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
        int randomNumber = Random.Range(1, 3);
		if (randomNumber == 1)
		{
            float randomOffset = Random.Range(0f,1f);

            Vector3 dropPosition= new Vector3(this.transform.position.x + randomOffset, goldDropHight, this.transform.position.z + randomOffset);
            
            ObjectPooler.Instance.SpawnFromPool("Gold_big", dropPosition, null, Quaternion.identity);
        }
        else if (randomNumber == 2)
		{
            float randomOffset = Random.Range(0f, 1f);
            Vector3 dropPosition = new Vector3(this.transform.position.x + randomOffset, goldDropHight, this.transform.position.z + randomOffset);
            ObjectPooler.Instance.SpawnFromPool("Gold_medium", dropPosition, null, Quaternion.identity);
        }
        else if (randomNumber == 3)
		{
            float randomOffset = Random.Range(0f, 1f);
            Vector3 dropPosition = new Vector3(this.transform.position.x + randomOffset, goldDropHight, this.transform.position.z + randomOffset);
            ObjectPooler.Instance.SpawnFromPool("Gold_small", dropPosition, null, Quaternion.identity);
        }
    }

    void SpawnGold(Vector3 pos)
    {
        float randomOffset = Random.Range(0f, 0.5f);

        Vector3 dropPosition = new Vector3(pos.x + randomOffset, goldDropHight, pos.z + randomOffset);

        ObjectPooler.Instance.SpawnFromPool("Gold_big", dropPosition, null, Quaternion.identity);
    }
    #endregion

    public void GettingPicked()
	{
        if(fishState == FishState.Hide)
		{
            Vector3 spawnPos = this.transform.position - player.transform.position;
            spawnPos = spawnPos.normalized;
            SpawnGold(this.transform.position + (spawnPos * 2f));
		}
	}

    void Hide()
	{
        this.transform.position = Vector3.Lerp(this.transform.position, hidingSpot, 2 * Time.deltaTime);

        Vector3 lookVector = new Vector3(hidingSpot.x, -20, hidingSpot.z);
        Quaternion newRotation = Quaternion.LookRotation(lookVector);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, 8 * Time.deltaTime);
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
            rb.AddForce(moveDir * currentSpeed * Time.deltaTime, ForceMode.VelocityChange);

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
            activeWaypoint = new Vector3(wayPoints[activeWayPointIndex].x, fishHight, wayPoints[activeWayPointIndex].y);
        }
    }


	private void OnDisable()
	{
        GameManager.Instance.onHighTide -= OnHighTide;
        GameManager.Instance.onLowTide -= OnLowTide;
    }


#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
		if (wayTransforms.Count > 1)
		{
            for (int i = 0; i < wayTransforms.Count; i++)
            {
                Gizmos.DrawCube(new Vector3(wayTransforms[i].transform.position.x, fishHight, wayTransforms[i].transform.position.z), Vector3.one * 2);
                if (i != 0)
                {
                    Gizmos.DrawLine(new Vector3(wayTransforms[i - 1].transform.position.x, fishHight, wayTransforms[i - 1].transform.position.z), new Vector3(wayTransforms[i].transform.position.x, fishHight, wayTransforms[i].transform.position.z));
                }
            }
            if (isLoop)
            {
                Gizmos.DrawLine(new Vector3(wayTransforms[wayTransforms.Count - 1].transform.position.x, fishHight, wayTransforms[wayTransforms.Count - 1].transform.position.z), new Vector3(wayTransforms[0].transform.position.x, fishHight, wayTransforms[0].transform.position.z));
            }
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, agroRadius);
    }

#endif

}
