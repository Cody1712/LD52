using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
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
    //TODO get slower in high water




    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cam = Camera.main.gameObject;
        rb = this.GetComponent<Rigidbody>();

        renderer = playerModel.GetComponent<Renderer>();
        animState = AnimationState.Idle;

        waterDetector.SetPlayer(this);
    }

	// Update is called once per frame
	private void Update()
	{
        HandleInput();
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
            walkMaterial.SetFloat("_Speed_in_Fps", 14f);
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
                StartCoroutine(AnimateMaterial(pickMaterial, "_ManualIndex",5,6));
                isPicking = false;

            }
		}


        switch (animState)
        {
            case AnimationState.Idle:
                renderer.sharedMaterial = idleMaterial;
                break;
            case AnimationState.Walk:
                renderer.sharedMaterial = walkMaterial;
                break;
            case AnimationState.Pick:
                renderer.sharedMaterial = pickMaterial;
                break;
            case AnimationState.Hurt:
                renderer.sharedMaterial = hurtMaterial;
                break;
        }
    }

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


    IEnumerator AnimateMaterial(Material mat, string indexName, float fps, int maxFrames)
	{
		for (int i = 0; i< maxFrames; i++)
		{
            yield return new WaitForSeconds(1 / fps);
            mat.SetFloat(indexName, i);
            Debug.Log("Animation Frame "+ i);
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


}
