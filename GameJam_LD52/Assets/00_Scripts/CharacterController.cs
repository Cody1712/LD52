using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 1f;
    [SerializeField] float deselerationTime = 1f;
    [SerializeField] float characterDirectionFlipSpeed = 2f;
    private GameObject cam;
    private Rigidbody rb;
    private Vector3 movementInputDirection;
    //left is -1 right is 1
    private float currentRotation = 1;


    private Renderer renderer;
    private AnimationState animState;
    public enum AnimationState { Idle, Walk, Pick };
    private bool isInteracting = false;
    [SerializeField] GameObject playerModel;
    [SerializeField] Material idleMaterial;
    [SerializeField] Material walkMaterial;
    [SerializeField] Material pickMaterial;




    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cam = Camera.main.gameObject;
        rb = this.GetComponent<Rigidbody>();

        renderer = playerModel.GetComponent<MeshRenderer>();
        animState = AnimationState.Idle;
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
        Move();
    }

    private void HandleInput()
	{
        movementInputDirection = GetMovementInputDirection();
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
            moveDir = moveDir * movementSpeed;
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
        }
    }

    public void StartInteraction(AnimationState newAnimState, float duration)
	{
        isInteracting = true;
	}

    IEnumerator WaitForInteraction(float duration)
	{
        yield return new WaitForSeconds(duration);
        isInteracting = false;
    }

	#endregion
}
