using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviour : MonoBehaviour, IInteraction
{
    [SerializeField] TextAsset text;
    public void OnDetected(Transform plr)
    {
        throw new System.NotImplementedException();
    }

    public void OnInteract(Transform plr)
    {
        SubtitleManager.instance.ActivateSubtitle(text);
    }
}
