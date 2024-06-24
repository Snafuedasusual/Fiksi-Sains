using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform posMous;
    [SerializeField] private PlayerLogic playerLogic;
    private bool SI_debounce = true;

    private Vector2 moveDir;

    private void PlayerInputMove()
    {
        Vector2 inputVector = new Vector2(0, 0);
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

        playerLogic.PlayerMovement();
        moveDir = inputVector;   
        
    }

    public void GetMousePosition()
    {
        Vector3 screenPosition = Input.mousePosition;
        screenPosition.z = cam.nearClipPlane + 12.5f;
        Vector3 mousePos = cam.ScreenToWorldPoint(screenPosition);
        posMous.position = mousePos;
        playerLogic.PlayerRotate(mousePos);
    }
    
    public Vector2 GetMoveDir()
    {
        return moveDir.normalized;
    }

    private int Mouse1Press()
    {
        int mouse1press = 0;
        if (Input.GetKey(KeyCode.Mouse0))
        {
            mouse1press = 1;
        }

        return mouse1press;
    }

    public int GetMouse1Press()
    {
        return Mouse1Press();
    }

    private float SwitchInventory()
    {
        SI_debounce = true;
        float switchInventory = 0f;
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
        //Debug.Log(SI_debounce);
        return switchInventory;
    }

    public float GetSwitchInventoryCode()
    {
        //Debug.Log(SwitchInventory());
        return SwitchInventory();
    }

    private bool HoldToSprint()
    {
        bool keyisPress = false;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            keyisPress = true;
            playerLogic.PlayerSprintController(keyisPress);
            return keyisPress;
        }
        else
        {
            keyisPress = false;
            playerLogic.PlayerSprintController(keyisPress);
            return keyisPress;
        }

    }

    public bool SprintIsPressed()
    {
        return HoldToSprint();
    }

    private void Update()
    {
        PlayerInputMove();
        GetMousePosition();
        GetMouse1Press();
        SwitchInventory();
        HoldToSprint();
    }

}
