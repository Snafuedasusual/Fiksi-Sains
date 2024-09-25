using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveTextManager : MonoBehaviour
{
    public static ObjectiveTextManager instance;
    [SerializeField] TextMeshProUGUI objectiveText;
    public string GetObjectiveText()
    {
        if (objectiveText.text != string.Empty) return objectiveText.text;
        else return string.Empty;
    }

    private void Awake()
    {
        if(instance != null && instance != this)
        {

        }
        else
        {
            instance = this;
        }
    }

    public void UpdateText(string text)
    {
        objectiveText.text = text;
    }

    public void EmptyText()
    {
        objectiveText.text = string.Empty;
    }
}
