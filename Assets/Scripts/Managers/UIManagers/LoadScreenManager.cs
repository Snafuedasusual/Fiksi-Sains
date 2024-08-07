using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScreenManager : MonoBehaviour
{
    public static LoadScreenManager loadScreenManager;

    private void Awake()
    {
        if(loadScreenManager != null && loadScreenManager != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            loadScreenManager = this;
        }
    }

    [Header("LoadScreen")]
    [SerializeField] GameObject loadScreenPrefab;
    [SerializeField] GameObject loadScreenWorld;

    [Header("Canvas")]
    [SerializeField] GameObject canvas;

    public void StartLoadScreen()
    {
        loadScreenWorld = Instantiate(loadScreenPrefab, canvas.transform);
        SetInFront();

    }

    private void SetInFront()
    {
        if(canvas.transform.childCount > 0)
        {

        }
        else
        {
            loadScreenWorld.transform.SetSiblingIndex(canvas.transform.childCount - 1);
        }
    }

    public void EndLoadScreen()
    {
        if(loadScreenWorld != null)
        {
            Destroy(loadScreenWorld.gameObject);
        }
        else
        {

        }
    }
}
