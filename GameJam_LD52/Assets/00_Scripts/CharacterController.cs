using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] GameObject playerSprite;
    [SerializeField] GameObject Camera;

    [SerializeField] float spriteRotationSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotatePlayerSprite();
    }

    private void RotatePlayerSprite()
	{
        Vector3 rotationDir = new Vector3(Camera.transform.position.x, this.transform.position.y, Camera.transform.position.z) - this.transform.position;
        rotationDir.Normalize();

        Quaternion newRotation = Quaternion.LookRotation(rotationDir);
        playerSprite.transform.rotation = Quaternion.Slerp(playerSprite.transform.rotation, newRotation, spriteRotationSpeed * Time.deltaTime);

    }
}
