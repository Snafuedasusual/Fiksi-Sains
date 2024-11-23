using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonHover : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] float hoverAlpha = 0.5f;
    [SerializeField] float normalAlpha = 1.0f;
    [SerializeField] TextMeshProUGUI text;

    //private void Start()
    //{
    //    if (button == null)
    //    {
    //        button = GetComponent<Button>();
    //    }
    //    if (button != null)
    //    {
    //        text = button.GetComponent<TextMeshProUGUI>();
    //    }
    //}

    public void OnPointerEnter()
    {
        SetButtonAlpha(normalAlpha);
    }

    public void OnPointerExit()
    {
        SetButtonAlpha(hoverAlpha);
    }

    private void SetButtonAlpha(float alpha)
    {
        if (button != null)
        {
            Color color = text.color;
            color.a = alpha;
            text.color = color;
        }
    }
}
