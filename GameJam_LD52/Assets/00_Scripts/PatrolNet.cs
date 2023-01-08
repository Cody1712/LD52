using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolNet : MonoBehaviour
{
    [SerializeField] List<Vector3> wayPoints = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GetNewWayPoint()
	{
        //Select a random controlpoint
	}


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
		foreach (Vector3 pos in wayPoints)
		{
            Gizmos.DrawCube(pos, Vector3.one * 2);
		}
    }

#endif
}
