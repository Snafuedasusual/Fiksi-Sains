using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform posMous;
    [SerializeField] private PlayerLogic playerLogic;
    private bool SI_debounce = true;

    private Vector2 moveDir;

    //Handles Move Input and Events Related.
    public event EventHandler<SendMoveInputArgs> OnMoveInput;
    public class SendMoveInputArgs : EventArgs { public Vector2 plrDir; }
    private void PlayerInputMove()
    {
        var inputVector = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            inputVector.y = Input.GetAxisRaw("Vertical");

        }
        if (Input.GetKey(KeyCode.S))
        {
            inputVector.y = Input.GetAxisRaw("Vertical");

        }
        if (Input.GetKey(KeyCode.D))
        {
            inputVector.x = Input.GetAxisRaw("Horizontal");

        }
        if (Input.GetKey(KeyCode.A))
        {
            inputVector.x = Input.GetAxisRaw("Horizontal");

        }
        OnMoveInput?.Invoke(this, new SendMoveInputArgs {plrDir = inputVector});
        moveDir = inputVector;   
        
    }
    //End Of Move Inputs----------------------------



    //Handles Mous Position Input and Events Related.
    public event EventHandler<SendMousPosInputArgs>OnMousePosInput;
    public class SendMousPosInputArgs : EventArgs { public Vector3 mousPos; }
    public void GetMousePosition()
    {
        var screenPosition = Input.mousePosition;
        screenPosition.z = cam.nearClipPlane + (cam.transform.position.y - transform.position.y);
        var mousePos = cam.ScreenToWorldPoint(screenPosition);
        var topBorder = cam.ViewportToWorldPoint(new Vector3(0.0F, 0.5F, screenPosition.z));
        var bottomBorder = cam.ViewportToWorldPoint(new Vector3(0.0F, -0.5F, screenPosition.z));
        var leftBorder = cam.ViewportToWorldPoint(new Vector3(-0.5F, 0.0F, screenPosition.z));
        var rightBorder = cam.ViewportToWorldPoint(new Vector3(0.5F, 0.0F, screenPosition.z));
        posMous.position = new Vector3(Mathf.Clamp(mousePos.x, transform.position.x - 16f, transform.position.x + 20f), 0, Mathf.Clamp(mousePos.z, transform.position.z - 10f, transform.position.z + 10f));
        OnMousePosInput?.Invoke(this, new SendMousPosInputArgs { mousPos = posMous.transform.position});
    }
    //End Of Mouse Pos Inputs--------------------------

    public event EventHandler OnInteractInput;
    private void InteractInput()
    {
        if (Input.GetKey(KeyCode.F))
        {
            OnInteractInput?.Invoke(this, EventArgs.Empty);
        }
    }
    private float SwitchInventory()
    {
        SI_debounce = true;
        var switchInventory = 0f;
        if(Input.GetKeyDown(KeyCode.E) && SI_debounce == true)
        {
            SI_debounce = false;
            switchInventory = 1f;

        }
        if (Input.GetKeyDown(KeyCode.Q) && SI_debounce == true)
        {
            SI_debounce = false;
            switchInventory = -1f;   
        }
        return switchInventory;
    }

    public float GetSwitchInventoryCode()
    {
        //Debug.Log(SwitchInventory());
        return SwitchInventory();
    }



    //Handles Shift Input and Events related.
    public event EventHandler<SendShiftHoldArgs> OnShiftHold;
    public class SendShiftHoldArgs : EventArgs { public bool keyIsPress; }
    private bool HoldToSprint()
    {
        var keyisPress = false;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            keyisPress = true;
            //playerLogic.PlayerSprintController(keyisPress);
        }
        else
        {
            keyisPress = false;
            //playerLogic.PlayerSprintController(keyisPress);
        }
        OnShiftHold?.Invoke(this, new SendShiftHoldArgs { keyIsPress = keyisPress });
        return keyisPress;
    }
    //End of Shift Input Detection-------------------------------------------------



    private void Update()
    {
        PlayerInputMove();
        GetMousePosition();
        SwitchInventory();
        HoldToSprint();
        InteractInput();
    }

}
