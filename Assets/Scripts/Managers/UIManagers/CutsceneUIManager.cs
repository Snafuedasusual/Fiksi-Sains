using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneUIManager : MonoBehaviour
{
    public static CutsceneUIManager instance;
    [SerializeField] GameObject uiCutsceneParent;
    bool isCutsceneActive = false;

    private void Awake()
    {
        if (instance != null && instance != this) return;
        instance = this;
    }
    public void ActivateCutsceneUI(GameObject UI)
    {
        if (isCutsceneActive == true) return;
        isCutsceneActive = true;
        UI.GetComponent<RectTransform>().SetParent(uiCutsceneParent.GetComponent<RectTransform>(), false);
    }

    public void DeactivateCutscene()
    {
        isCutsceneActive = false;
    }

    public bool GetIfCutsceneActive()
    {
        return isCutsceneActive;
    }
}
