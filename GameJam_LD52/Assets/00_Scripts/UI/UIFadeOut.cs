using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFadeOut : MonoBehaviour
{
    private CanvasGroup image;
    [SerializeField] private float fadeOutSpeed;

    private bool fadeOut = false;
    
    // Start is called before the first frame update
    void Start()
    {
        image = this.GetComponent<CanvasGroup>();
        StartCoroutine(WaitForFade());
    }

    // Update is called once per frame
    void Update()
    {
		if (fadeOut)
		{
            image.alpha = Mathf.Lerp(image.alpha, 0, Time.deltaTime * fadeOutSpeed);
        }
    }

    IEnumerator WaitForFade()
	{
        yield return new WaitForSeconds(2);
        fadeOut = true;

    }
}
