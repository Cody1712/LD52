using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GoldNugget : MonoBehaviour, IPooledObject
{
    [SerializeField] float bounceHight = 0.8f;
    private Collider col;
    // Start is called before the first frame update
    void Start()
    {
        col = this.GetComponent<Collider>();
        this.transform.DOMoveY(this.transform.position.y + bounceHight, 1).SetEase(Ease.InOutSine).SetLoops(-1,LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnObjectSpawn()
	{
        
    }

    IEnumerator WaitUntilCollectable(float waitTime)
	{
        yield return new WaitForSeconds(waitTime);
        col.enabled = true;
    }
}
