using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubInteractObjectives : MonoBehaviour, IInteraction
{
    [SerializeField] GameObject interactionObj;
    [SerializeField] TextAsset textAsset;
    [SerializeField] string notif;
    public event EventHandler OnInteractActive;
    public event EventHandler OnInteractDeactive;

    public void OnDetected(Transform plr)
    {
        throw new NotImplementedException();
    }

    public void OnInteract(Transform plr)
    {
        if (interactionObj == null) return;
        if (!interactionObj.TryGetComponent(out IInteraction interact)) return;
        interact.OnInteract(plr);
        OnInteractActive?.Invoke(this, EventArgs.Empty);
        gameObject.SetActive(false);
        if (textAsset == null) return;
        SubtitleManager.instance.ActivateSubtitle(textAsset);
    }

    public string UpdateNotif()
    {
        return notif;
    }
}