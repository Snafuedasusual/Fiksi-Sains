using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitBarManager : MonoBehaviour, ICloseAllMenus
{
    public static WaitBarManager instance;
    void Awake()
    {
        if (instance != null & instance != this) return;
        if (instance == null) instance = this;
    }

    [SerializeField] GameObject waitBar;
    [SerializeField] Slider slider;

    public void ActivateWaitBar()
    {
        waitBar.SetActive(true);
    }

    public void DeactivateWaitBar()
    {
        waitBar.SetActive(false);
    }

    public void UpdateBarValue(float value)
    {
        if (waitBar.activeSelf == false) return;
        slider.value = value;
    }

    public void CloseMenu()
    {
        DeactivateWaitBar();
    }
}
