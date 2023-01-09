using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCollector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gold"))
        {
            CollectGold(other.gameObject);
        }
    }

    void CollectGold(GameObject goldNugget)
    {
        GameManager.Instance.goldInventory += 1;
        goldNugget.transform.position = new Vector3(0, -100, 0);
        goldNugget.SetActive(false);
    }
}
