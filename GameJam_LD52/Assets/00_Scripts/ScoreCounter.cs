using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;


    // Update is called once per frame
    void Update()
    {
        text.text = GameManager.Instance.goldInventory.ToString();
    }
}
