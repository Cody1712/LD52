using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectingSeagull : MonoBehaviour
{
    Material seagullMaterial;    
    // Start is called before the first frame update
    void Start()
    {
        seagullMaterial = this.GetComponent<Renderer>().sharedMaterial;

        StartCoroutine(AnimateMaterial(seagullMaterial, "_ManualIndex",12,6));
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator AnimateMaterial(Material mat, string indexName, float fps, int maxFrames)
    {
        for (int i = 0; i < maxFrames; i++)
        {
            yield return new WaitForSeconds(1 / fps);
            mat.SetFloat(indexName, i);
        }
        //mat.SetFloat(indexName, 0);
        StartCoroutine(AnimateMaterial(seagullMaterial, "_ManualIndex", 12, 6));
    }

}
