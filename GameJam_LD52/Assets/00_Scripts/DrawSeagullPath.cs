using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawSeagullPath : MonoBehaviour
{
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
                Gizmos.DrawCube(this.transform.GetChild(i).transform.position, Vector3.one * 2);
                if (i != 0)
                {
                    Gizmos.DrawLine(this.transform.GetChild(i - 1).transform.position, this.transform.GetChild(i).transform.position);
                }
            }
        }
    }
#endif
}
