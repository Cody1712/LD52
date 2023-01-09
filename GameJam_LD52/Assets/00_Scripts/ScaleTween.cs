using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScaleTween : MonoBehaviour
{
    [SerializeField] float scaleFactor = 1;
    // Start is called before the first frame update
    void Start()
    {
        this.transform.DOScale(this.transform.localScale + Vector3.one * scaleFactor, 0.8f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
}
