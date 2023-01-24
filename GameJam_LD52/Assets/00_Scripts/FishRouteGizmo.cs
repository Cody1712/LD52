using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishRouteGizmo : MonoBehaviour
{
    [SerializeField] float gizmoDisplayHieght = 0.5f;

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
		//foreach (Transform t in this.transform)
		//{
        //    Gizmos.DrawCube(t.transform.position, Vector3.one * 2);
        //}

		if (this.transform.childCount > 1)
		{
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Gizmos.DrawCube(new Vector3(this.transform.GetChild(i).transform.position.x, gizmoDisplayHieght, this.transform.GetChild(i).transform.position.z), Vector3.one * 2);
                if (i != 0)
                {
                    Gizmos.DrawLine(new Vector3(this.transform.GetChild(i- 1).transform.position.x, gizmoDisplayHieght, this.transform.GetChild(i - 1).transform.position.z), new Vector3(this.transform.GetChild(i).transform.position.x, gizmoDisplayHieght, this.transform.GetChild(i).transform.position.z));
                }
            }
        }
    }
#endif
}
