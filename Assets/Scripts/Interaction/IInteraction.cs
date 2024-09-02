using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteraction
{
    public void OnInteract(Transform plr);

    public void OnDetected(Transform plr);

}
