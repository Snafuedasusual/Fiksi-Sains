using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private PlayerLogic playerLogic;

    private Vector2 moveDir;
    public Vector2 GetInputDir() { return moveDir; }

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
        //var topBorder = cam.ViewportToWorldPoint(new Vector3(0.0F, 0.5F, screenPosition.z));
        //var bottomBorder = cam.ViewportToWorldPoint(new Vector3(0.0F, -0.5F, screenPosition.z));
        //var leftBorder = cam.ViewportToWorldPoint(new Vector3(-0.5F, 0.0F, screenPosition.z));
        //var rightBorder = cam.ViewportToWorldPoint(new Vector3(0.5F, 0.0F, screenPosition.z));
        OnMousePosInput?.Invoke(this, new SendMousPosInputArgs { mousPos = mousePos});
    }
    //End Of Mouse Pos Inputs--------------------------




    //Handles Interaction Inputs and events related.
    public event EventHandler OnEInputEvent;
    private void OnEInput()
    {
        if (Input.GetKey(KeyCode.E))
        {
            OnEInputEvent?.Invoke(this, EventArgs.Empty);

        }
    }
    //Input script ends-----------------------------------



    //Handles flashlight inputs and events related.
    public event EventHandler OnFlashlightInput;
    private void FlashlightInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            OnFlashlightInput?.Invoke(this, EventArgs.Empty);
        }
    }
    //Flashligh script ends------------------------------




    //Handles Shift Input and Events related.
    public event EventHandler<SendShiftHoldArgs> OnShiftHold;
    public class SendShiftHoldArgs : EventArgs { public bool keyIsPress; }
    private void HoldToSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            OnShiftHold?.Invoke(this, new SendShiftHoldArgs { keyIsPress = true });
            return;
            //playerLogic.PlayerSprintController(keyisPress);
        }
        else if(!Input.GetKey(KeyCode.LeftShift))
        {
            OnShiftHold?.Invoke(this, new SendShiftHoldArgs { keyIsPress = false });
            return;
            //playerLogic.PlayerSprintController(keyisPress);
        }
    }
    //End of Shift Input Detection-------------------------------------------------





    //Handles Esc input and events related.
    public event EventHandler EscInputEvent;
    private bool esc_Deboune = false;
    private void EscInputDetector()
    {
        if (Input.GetKey(KeyCode.Escape) && esc_Deboune == false)
        {
            esc_Deboune = true;
            EscInputEvent?.Invoke(this, EventArgs.Empty);
        }
        if(Input.GetKey(KeyCode.Escape) && esc_Deboune == true)
        {

        }
        else if(!Input.GetKeyDown(KeyCode.Escape))
        {
            esc_Deboune= false;
        }
    }
    //Esc input ends------------------------------------------------




    private bool tabDebounce = false;
    public event EventHandler OnTabInput;
    private void TabInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && tabDebounce == false)
        {
            tabDebounce = true;
            OnTabInput?.Invoke(this, EventArgs.Empty);
        }
        if(Input.GetKeyDown(KeyCode.Tab) && tabDebounce == true)
        {

        }
        else if (!Input.GetKeyDown(KeyCode.Tab))
        {
            tabDebounce = false;
        }
    }


    public event EventHandler<OnMouse1PressedArgs> OnMouse1Pressed;
    public class OnMouse1PressedArgs : EventArgs { public bool isPressed; }
    private void Mouse1Pressed()
    {
        if(Input.GetMouseButton(0))
        {
            OnMouse1Pressed?.Invoke(this, new OnMouse1PressedArgs { isPressed = Input.GetMouseButton(0) });
        }
        if(Input.GetMouseButtonUp(0))
        {
            OnMouse1Pressed?.Invoke(this, new OnMouse1PressedArgs { isPressed = !Input.GetMouseButtonUp(0) });
        }
    }


    private void CheckInputs()
    {
        PlayerInputMove();
        GetMousePosition();
        HoldToSprint();
        OnEInput();
        FlashlightInput();
        EscInputDetector();
        TabInput();
        Mouse1Pressed();
    }


    private void Update()
    {
        CheckInputs();
    }

}
