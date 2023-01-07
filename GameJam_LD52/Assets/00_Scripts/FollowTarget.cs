using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public enum UpdateMethod { Update, FixedUpdate};

    [SerializeField] GameObject target;
    [SerializeField] UpdateMethod updateMethod = UpdateMethod.Update;
    [SerializeField] bool isLockY;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (updateMethod == UpdateMethod.Update)
		{
			if (isLockY)
			{
                this.transform.position = new Vector3(target.transform.position.x, this.transform.position.y, target.transform.position.z);
			}
			else
			{
                this.transform.position = target.transform.position;
            }
		}
    }
	private void FixedUpdate()
	{
        if (updateMethod == UpdateMethod.FixedUpdate)
        {
            if (isLockY)
            {
                this.transform.position = new Vector3(target.transform.position.x, this.transform.position.y, target.transform.position.z);
            }
            else
            {
                this.transform.position = target.transform.position;
            }
        }
    }
}
