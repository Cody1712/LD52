using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] Transform pickPosition;

	[Header("Movement")]    
    [SerializeField] float movementSpeed = 1f;
    [SerializeField] float sprintMultiplier = 1f;
    private float currentSprintMultiplier = 1f;
    [SerializeField] float waterMultiplier = 1f;
    private float currentWaterMultiplier = 1f;
    [SerializeField] float deselerationTime = 1f;
    [SerializeField] float characterDirectionFlipSpeed = 2f;
    private GameObject cam;
    private Rigidbody rb;
    private Vector3 movementInputDirection;
    //left is -1 right is 1
    private float currentRotation = 1;

    [Header("Animation")]
    [SerializeField] GameObject playerModel;
    [SerializeField] Material idleMaterial;
    [SerializeField] Material walkMaterial;
    [SerializeField] Material pickMaterial;
    [SerializeField] Material hurtMaterial;
    private Renderer renderer;
    private AnimationState animState;
    public enum AnimationState { Idle, Walk, Pick, Hurt };
    private bool isInteracting = false;

    private bool isPicking = false;

    [Header("Water Interaction")]
    [SerializeField] WaterDetector waterDetector;

	[Header("Footstep Audio")]
	[SerializeField] private AudioClip waterFootSteps;
	[SerializeField] private AudioClip SandFootSteps;
    [SerializeField] private LayerMask footSoundLayers;
    private AudioSource walkAudioPlayer;
    private bool isOnSand;






    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.gameObject;
        rb = this.GetComponent<Rigidbody>();

        walkAudioPlayer = this.GetComponent<AudioSource>();
        renderer = playerModel.GetComponent<Renderer>();
        animState = AnimationState.Idle;

        waterDetector.SetPlayer(this);
    }

	// Update is called once per frame
	private void Update()
	{
		if (GameManager.Instance.gameState == GameManager.GameState.RUNNING)
		{
            HandleInput();
        }
		else
		{
            movementInputDirection = Vector3.zero;
        }

        CheckRotation();

        HandleAnimation();
    }

	void FixedUpdate()
    {
        if (isInteracting)
            return;

        Move();
    }

    private void HandleInput()
	{
        movementInputDirection = GetMovementInputDirection();

		if (Input.GetKey(KeyCode.LeftShift))
		{
            currentSprintMultiplier = sprintMultiplier;
            walkMaterial.SetFloat("_Speed_in_Fps", 11f);
        }
		else
		{
            currentSprintMultiplier = 1;
            walkMaterial.SetFloat("_Speed_in_Fps", 9f);
        }

		if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
		{
			if (!isInteracting)
			{
                isInteracting = true;
                isPicking = true;

                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                pickMaterial.SetFloat("_Speed_in_Fps", 0f);
            }
        }
    }


    #region Movement
    private Vector3 GetMovementInputDirection()
    {
        return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }

    private void Move()
	{
        if (movementInputDirection == Vector3.zero)
		{
            Vector3 zeroVector = new Vector3(0, rb.velocity.y, 0);
            rb.velocity = Vector3.Slerp(rb.velocity, zeroVector, deselerationTime);

        }
		else
		{
            float targetAngle = Mathf.Atan2(movementInputDirection.x, movementInputDirection.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            moveDir = moveDir.normalized;
            moveDir = moveDir * movementSpeed * currentSprintMultiplier * currentWaterMultiplier;
            moveDir.y = rb.velocity.y;

            rb.velocity = moveDir;
        }
    }

    private void CheckRotation()
	{
		if (movementInputDirection.x < 0)
		{
			if (currentRotation != 1)
			{
                currentRotation = 1f;
			}
		}
		if (movementInputDirection.x > 0)
		{
            if (currentRotation != -1)
            {
                currentRotation = -1f;
            }
        }

        ChangeRotation(currentRotation);
    }

    private void ChangeRotation(float direction)
	{
        this.transform.localScale = Vector3.Lerp(this.transform.localScale, new Vector3(direction, this.transform.localScale.y, this.transform.localScale.z), characterDirectionFlipSpeed * Time.deltaTime);
	}

    public void InWater()
	{
        currentWaterMultiplier = waterMultiplier;
    }

    public void OutOfWater()
	{
        currentWaterMultiplier = 1f;
    }

    #endregion

    #region Animation

    public void Hurt()
    {
        isInteracting = true;
        rb.velocity = new Vector3(0, rb.velocity.y, 0);

        animState = AnimationState.Hurt;
        StartCoroutine(AnimateMaterial(hurtMaterial, "_ManualIndex", 5, 3));
    }

    void Pick()
    {
        bool detectedFish = false;

        //RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, 1.5f, transform.forward);
        Collider[] hitColliders = Physics.OverlapSphere(pickPosition.position, 1.5f);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Fish"))
            {
                hitCollider.GetComponent<Fish_Behaviour>().GettingPicked();
                ObjectPooler.Instance.SpawnFromPool("Pick_Fish_Sound", Vector3.zero, null, Quaternion.identity);
                detectedFish = true;
            }
        }

		if (!detectedFish)
		{
            //Check what sound to play
            RaycastHit floorhit;
            if (Physics.Raycast(this.transform.position + (Vector3.up * 3f), Vector3.down, out floorhit, 3.2f, footSoundLayers))
            {
                if (floorhit.collider.CompareTag("Terrain"))
                {
                    ObjectPooler.Instance.SpawnFromPool("Pick_Sand_Sound", Vector3.zero, null, Quaternion.identity);
                }
                else if (floorhit.collider.CompareTag("Water"))
                {
                    ObjectPooler.Instance.SpawnFromPool("Pick_Water_Sound", Vector3.zero, null, Quaternion.identity);
                }
            }
        }
    }

    private void HandleAnimation()
    {
		if (movementInputDirection == Vector3.zero && !isInteracting)
		{
            animState = AnimationState.Idle;
        }
		else if(movementInputDirection != Vector3.zero && !isInteracting)
		{
            animState = AnimationState.Walk;
        }


        //Animations for if the player is interacting
		if (isInteracting)
		{
            if (isPicking)
            {
                animState = AnimationState.Pick;
                Pick();
                StartCoroutine(AnimateMaterial(pickMaterial, "_ManualIndex",6,6));
                isPicking = false;

            }
		}


        switch (animState)
        {
            case AnimationState.Idle:
                renderer.sharedMaterial = idleMaterial;
                walkAudioPlayer.enabled = false;
                break;
            case AnimationState.Walk:
                renderer.sharedMaterial = walkMaterial;
                //Check for what terrain is below character
                walkAudioPlayer.enabled = true;
                FloorCheck();
                SwitchAudio();
                break;
            case AnimationState.Pick:
                walkAudioPlayer.enabled = false;
                renderer.sharedMaterial = pickMaterial;
                break;
            case AnimationState.Hurt:
                walkAudioPlayer.enabled = false;
                renderer.sharedMaterial = hurtMaterial;
                break;
        }
    }
	#region unused
	public void StartInteraction(AnimationState newAnimState, float duration)
	{
        isInteracting = true;
	}

    void AnimatePick(float fps, float maxFrames)
    {
        float currentFrame = 0;
        currentFrame = (Time.time * fps);
        currentFrame = currentFrame % maxFrames;
        currentFrame = Mathf.Floor(currentFrame);

        pickMaterial.SetFloat("_ManualIndex", currentFrame);
    }
	#endregion

    void SwitchAudio()
	{
		if (isOnSand)
		{
			if (walkAudioPlayer.clip != SandFootSteps)
			{
                walkAudioPlayer.clip = SandFootSteps;
                walkAudioPlayer.Play();
            }
        }
		else if(walkAudioPlayer.clip != waterFootSteps)
		{
            walkAudioPlayer.clip = waterFootSteps;
            walkAudioPlayer.Play();
        }
	}

    void FloorCheck()
	{
        RaycastHit hit;

        if (Physics.Raycast(this.transform.position + (Vector3.up * 3f), Vector3.down, out hit, 3.2f, footSoundLayers))
		{
            if (hit.collider.CompareTag("Terrain")) 
            {
                isOnSand = true;
            }
			else if(hit.collider.CompareTag("Water"))
			{
                isOnSand = false;
            }
		}
		else
		{
            Debug.Log("Nothing Detected");
        }
	}



	IEnumerator AnimateMaterial(Material mat, string indexName, float fps, int maxFrames)
	{
		for (int i = 0; i<= maxFrames; i++)
		{
            yield return new WaitForSeconds(1 / fps);
            mat.SetFloat(indexName, i);
        }
        isInteracting = false;
        mat.SetFloat(indexName, 0);
    }

    IEnumerator WaitForInteraction(float duration)
	{
        yield return new WaitForSeconds(duration);
        isInteracting = false;
    }

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
	{
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position + (Vector3.up * 3f), this.transform.position + (Vector3.down * 3.2f));
	}
#endif
}
