using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Manager<GameManager>
{
	[Header("Positioning")]
    [SerializeField] GameObject waterPlane;
    [SerializeField] float highTideHight = 1f;
    [SerializeField] float lowTideHight = -1f;

    [Header("Timing")]
    [SerializeField] float highTideDuration = 10f;
    [SerializeField] float lowTideDuration = 20f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

#if UNITY_EDITOR

	private void OnDrawGizmos()
	{
        Vector3 highTidePosition = new Vector3(0, highTideHight, 0);
        Vector3 lowTidePosition = new Vector3(0, lowTideHight, 0);
        Vector3 gizmoSize = new Vector3(2f,0.1f,2f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(highTidePosition, lowTidePosition);

        Gizmos.DrawCube(highTidePosition, gizmoSize);
        Gizmos.DrawCube(lowTidePosition, gizmoSize);
	}
#endif
}
