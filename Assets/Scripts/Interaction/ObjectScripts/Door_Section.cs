using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Section : MonoBehaviour, F_Interaction
{
    public bool canFinish = false;
    public void OnInteract(Transform plr)
    {
        if (canFinish == true)
        {
            GameManagers.instance.OnLevelChange(plr);
        }
    }
}
