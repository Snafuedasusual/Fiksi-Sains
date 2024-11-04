using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteraction
{
    public event EventHandler OnInteractActive;
    public event EventHandler OnInteractDeactive;
    public void OnInteract(Transform plr);

    public void OnDetected(Transform plr);

    public string UpdateNotif();

}
