using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerToUILogic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public event EventHandler OnOpenInventory;
    private void OpenInventory()
    {
        if (GameManagers.instance.GetGameState() == GameManagers.GameState.Playing)
        {
            OnOpenInventory?.Invoke(this, EventArgs.Empty);
        }
        else
        {

        }
    }
}
