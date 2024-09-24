using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsManager : MonoBehaviour
{
    public static CreditsManager instance;
    private void Awake()
    {
        if (instance != null && instance != this) return;
        instance = this;
    }

    [SerializeField] GameObject canvas;
    [SerializeField] GameObject prefab;
    [SerializeField] GameObject prefabWorld;

    public void ActivateCredits()
    {
        if (prefabWorld != null) return;
        prefabWorld = Instantiate(prefab, canvas.transform);
        prefabWorld.transform.SetAsLastSibling();
    }

}
