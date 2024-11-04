using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadableUI : MonoBehaviour, IReadableUI
{
    [SerializeField] GameObject textUI;
    public GameObject GetTextVersion()
    {
        return textUI;
    }
}
