using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager instance;
    [SerializeField] GameObject loadingPrefab;
    [SerializeField] GameObject loadingPrefabWorld;

    private void Awake()
    {
        if (instance != null && instance != this) return;
        if (instance == null) instance = this;
    }

    public void ActivateLoading()
    {
        if (loadingPrefabWorld != null) return;
        loadingPrefabWorld = Instantiate(loadingPrefab, UIManager.instance.GetCanvas().transform);
        loadingPrefabWorld.transform.SetAsLastSibling();
    }

    public void DeactivateLoading()
    {
        if (loadingPrefabWorld != null) Destroy(loadingPrefabWorld);
    }
}
