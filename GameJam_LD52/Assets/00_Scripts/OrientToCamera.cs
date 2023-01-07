using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientToCamera : MonoBehaviour
{
    GameObject cam;
    [SerializeField] float spriteRotationSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        RotatePlayerSprite();
    }

    private void RotatePlayerSprite()
    {
        Vector3 rotationDir = new Vector3(cam.transform.position.x, this.transform.position.y, cam.transform.position.z) - this.transform.position;
        rotationDir.Normalize();

        Quaternion newRotation = Quaternion.LookRotation(rotationDir);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, spriteRotationSpeed * Time.deltaTime);

    }
}
