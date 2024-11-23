using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingImage : MonoBehaviour
{
    [SerializeField] float blinkInterval = 0.5f;
    private Image Logo;
    private bool isBlinking = true;
    
    void Start()
    {
        Logo = GetComponent<Image>();
        if ( Logo != null)
        {
            StartCoroutine(Blink());
        }
        else
        {
            Debug.LogError("NoImage");
        }
    }

    private IEnumerator Blink()
    {
        while ( isBlinking )
        {
            SetAlpha(0.95f);
            yield return new WaitForSeconds(blinkInterval);

            SetAlpha(1f);
            yield return new WaitForSeconds(blinkInterval);
            //Debug.Log("Blink");
        }
    }

    private void SetAlpha(float alpha)
    {
        Color color = Logo.color;
        color.a = alpha;
        Logo.color = color;
    }

    public void StopBlink()
    {
        isBlinking = false;
        SetAlpha(1f);
    }

    public void StartBlink()
    {
        isBlinking = true;
        StartCoroutine(Blink());
    }
}
