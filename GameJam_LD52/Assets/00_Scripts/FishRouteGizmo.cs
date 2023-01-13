using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishRouteGizmo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
	{
		foreach (Transform t in this.transform)
		{
            Gizmos.DrawCube(t.transform.position, Vector3.one * 2);
        }
	}
#endif
}
