using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static PlayerInput;

public class PlayerLogic : MonoBehaviour, IHealthInterface
{
    [Header("Script References")]
    [SerializeField] EntityHealthController healthController;
    [SerializeField] PlayerInput plrInp;
    
    [Header("Components")]
    [SerializeField] InventorySystem inventorySystem;
    [SerializeField] Transform mousePosition;

    [Header("Variables")]
    [SerializeField] float plrSpdBase = 5f;
    [SerializeField] float plrSprintBase = 10f;
    [SerializeField] float plrSprintApplied = 0f;
    [SerializeField] float plrStamina = 100f;
    [SerializeField] float centerBody = 1.1f;
    [SerializeField] Transform targetInteract;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform mousPosTrans;
    [SerializeField] private ItemUses equippedItem;

    [Header("LayerMasks")]
    [SerializeField] private LayerMask interactableObjs;
    [SerializeField] private LayerMask enemyLyr;


    float playerDirection;


    [Header("PlayerStates")]
    [SerializeField] PlayerStates defaultPlrState;
    public enum PlayerStates
    {
        Null,
        Idle,
        Walking,
        Sprinting,
        Attacking,
        InteractingToggle,
        InteractingHold,
        Hiding,
        InVent,
        Dead
    }

    public enum PlrAnimations
    {
        MOVEMENT_WALK,
        MOVEMENT_RUN,
        NONE
    }

    public event EventHandler<PlayThisMovementAnimArgs> PlayThisMovementAnim;
    public class PlayThisMovementAnimArgs : EventArgs { public PlrAnimations playThisAnim; public float xAxis; public float yAxis; }

    public PlayerStates plrState;



    public void InitializeScript()
    {
        plrInp.OnMoveInput += OnMoveInputDetector;
        plrInp.OnMousePosInput += OnMousePosInputDetector;
        plrInp.OnShiftHold += OnShiftHoldDetector;
        plrInp.OnEInputEvent += OnInteractInputDetector;
        healthController.SendDmgToLogic += SendDmgToLogicReceiver;
        plrInp.OnFlashlightInput += OnFlashlightInputDetector;
        plrInp.OnMouse1Pressed += OnMouse1PressedReceiver;
        inventorySystem.EquipItemEvent += InventorySystem_EquipItemEvent;
    }

    

    public void DeInitializeScript()
    {
        plrInp.OnMoveInput -= OnMoveInputDetector;
        plrInp.OnMousePosInput -= OnMousePosInputDetector;
        plrInp.OnShiftHold -= OnShiftHoldDetector;
        plrInp.OnEInputEvent -= OnInteractInputDetector;
        healthController.SendDmgToLogic -= SendDmgToLogicReceiver;
        plrInp.OnFlashlightInput -= OnFlashlightInputDetector;
    }

    private void ForceStopAllCoroutines()
    {
        StopCoroutine(StaminaDrainer());
        IsStaminaDrainerRunning = null;
        StopCoroutine(StaminaRefillController());
        IsStaminaRefillRunning = null;
    }

    private void OnEnable()
    {
        InitializeScript();
    }

    private void OnDisable()
    {
        DeInitializeScript();
    }

    private void OnDestroy()
    {
        DeInitializeScript();
    }



    //Handles Movement When Input Detected
    private Vector2 plrDirection;
    private void OnMoveInputDetector(object sender, SendMoveInputArgs e)
    {
        PlayerMovement(e.plrDir);
    }
    private Vector3 PlayerMovement(Vector2 dir)
    {
        var plrSpd = plrSpdBase + plrSprintApplied;

        var plrMoveDir = new Vector3(dir.x, 0f, dir.y).normalized;

        plrDirection = new Vector2(plrMoveDir.x, plrMoveDir.z);

        var walkSound = 0.3f;
        if(GameManagers.instance.GetGameState() != GameManagers.GameState.Playing)
        {
            StopAllCoroutines();
        }
        else
        {
            if (plrState == PlayerStates.InteractingToggle || plrState == PlayerStates.Hiding || plrState == PlayerStates.InteractingHold || plrState == PlayerStates.InVent || plrState == PlayerStates.Dead)
            {

            }
            else
            {
                rb.MovePosition(rb.position + plrMoveDir * plrSpd * Time.deltaTime);

                var localMoveDir = (transform.forward * plrMoveDir.z) + (transform.right * plrMoveDir.x);

                //rb.MovePosition(rb.position + transform.forward * plrMoveDir.z * plrSpd * Time.deltaTime);
                //rb.MovePosition(rb.position + transform.right * plrMoveDir.x * plrSpd * Time.deltaTime);

                playerDirection = Vector3.Dot(transform.forward, plrMoveDir);
                //playerDirection = Vector3.Dot(transform.forward.normalized, localMoveDir.normalized);

                

                if (plrMoveDir != Vector3.zero)
                {
                    plrState = PlayerStates.Walking;
                    MakeSound(walkSound);
                }
                else if(plrMoveDir == Vector3.zero)
                {
                    MakeSound(0f);
                    plrState = PlayerStates.Idle;
                }

            }
            PlayMovementAnim();
        }
        return plrMoveDir;
    }
    public Vector2 GetPlayerMovement()
    {
        return plrDirection;
    }
    //End Of Movement Script----------------------------------------------------



    //Handles Sprint when Input detected.
    private void OnShiftHoldDetector(object sender, SendShiftHoldArgs e)
    {
        PlayerSprintController(e.keyIsPress);
    }
    public void PlayerSprintController(bool spaceIsPressed)
    {
        var sprintSound = 10f;
        if(GameManagers.instance.GetGameState() != GameManagers.GameState.Playing)
        {
            ForceStopAllCoroutines();
        }
        else
        {
            if (plrState == PlayerStates.Dead || plrState == PlayerStates.InVent || plrState == PlayerStates.Hiding)
            {

            }
            else
            {
                if(spaceIsPressed == true && plrStamina > 0 && playerDirection >= 0.3f && plrState == PlayerStates.Walking)
                {
                    plrSprintApplied = plrSprintBase;
                    plrState = PlayerStates.Sprinting;
                    MakeSound(sprintSound);
                    DrainStamina();
                }
                if(spaceIsPressed == true && plrStamina <= 0 && playerDirection >= 0.3f && plrState == PlayerStates.Walking)
                {
                    plrSprintApplied = 0f;
                    DrainStamina();
                    MakeSound(sprintSound);

                }
                else if(playerDirection <= 0)
                {
                    plrSprintApplied = 0f;
                    RefillStamina();
                }
                else if(spaceIsPressed == false)
                {
                    plrSprintApplied = 0f;
                    RefillStamina();

                }
                PlayMovementAnim();
            }
        } 
    }
    //End of Sprint Script-----------------------------------------------------



    void PlayMovementAnim()
    {
        if(plrState == PlayerStates.Idle || plrState == PlayerStates.Walking)
        {
            PlayThisMovementAnim?.Invoke(this, new PlayThisMovementAnimArgs { playThisAnim = PlrAnimations.MOVEMENT_WALK, xAxis = plrDirection.x, yAxis = plrDirection.y });
        }
        if(plrState == PlayerStates.Sprinting)
        {
            PlayThisMovementAnim?.Invoke(this, new PlayThisMovementAnimArgs { playThisAnim = PlrAnimations.MOVEMENT_RUN, xAxis = plrDirection.x, yAxis = plrDirection.y });
        }
    }


    //Handles Stamina Bar Drain and Regen.
    public event EventHandler<StaminaBarToUIArgs> StaminaBarToUI;
    public class StaminaBarToUIArgs : EventArgs { public float staminaBarValue; }
    private void StaminaBarToUIFunc(float staminaBarValue)
    {
        StaminaBarToUI?.Invoke(this, new StaminaBarToUIArgs { staminaBarValue = plrStamina });
    }

    private void DrainStamina()
    {
        if(GameManagers.instance.GetGameState() != GameManagers.GameState.Playing)
        {
            ForceStopAllCoroutines();
            IsStaminaDrainerRunning = null;
        }
        else
        {
            if (IsStaminaDrainerRunning == null)
            {
                StartCoroutine(StaminaDrainer());
            }
            else
            {

            }
        } 
    }
    IEnumerator IsStaminaDrainerRunning;
    IEnumerator StaminaDrainer()
    {
        var staminaTime = 0f;
        var staminaRate = 0.35f;
        if (IsStaminaDrainerRunning != null)
        {

        }
        else
        {
            if (plrState == PlayerStates.Sprinting )
            {
                IsStaminaDrainerRunning = StaminaDrainer();
                while (plrState == PlayerStates.Sprinting)
                {
                    staminaTime = 0f;
                    while (staminaTime < staminaRate)
                    {
                        staminaTime += Time.deltaTime * 15f;
                        yield return null;
                    }
                    if (plrStamina <= 0)
                    {
                        StopCoroutine(StaminaDrainer());
                        IsStaminaDrainerRunning = null;
                    }
                    else if (plrStamina > 0 && playerDirection >= 0.3f)
                    {
                        plrStamina--;

                    }
                    else if(playerDirection <= 0)
                    {
                        StopCoroutine(IsStaminaDrainerRunning);
                        IsStaminaDrainerRunning = null;
                    }
                    StaminaBarToUIFunc(plrStamina);
                }
            }
            else
            {

            }
        }
        IsStaminaDrainerRunning = null;
    }

    private void RefillStamina()
    {
        if(GameManagers.instance.GetGameState() != GameManagers.GameState.Playing)
        {
            ForceStopAllCoroutines();
            IsStaminaRefillRunning = null;
        }
        else
        {
            if (IsStaminaRefillRunning == null)
            {
                StartCoroutine(StaminaRefillController());
            }
        }
    }
    IEnumerator IsStaminaRefillRunning;
    IEnumerator StaminaRefillController()
    {
        var staminaTime = 0f;
        var staminaRate = 0.45f;
        if(IsStaminaRefillRunning != null)
        {

        }
        else
        {
            if(plrState != PlayerStates.Sprinting)
            {
                
                IsStaminaRefillRunning = StaminaRefillController();
                while (plrState != PlayerStates.Sprinting)
                {
                    staminaTime = 0f;
                    while (staminaTime < staminaRate)
                    {
                        staminaTime += Time.deltaTime * 7.5f;
                        yield return null;
                    }
                    if (plrStamina == 100)
                    {

                    }
                    else if (plrStamina < 100)
                    {
                        plrStamina++;
                    }
                    StaminaBarToUIFunc(plrStamina);
                }
            }
            else
            {
                
            }
            
        }
        IsStaminaRefillRunning = null;
    }
    //End of Stamina Bar script-----------------------------



    //Handles Player Look At When Mouse Pos Input Detected.
    private void OnMousePosInputDetector(object sender, SendMousPosInputArgs e)
    {
        CameraFollow(e.mousPos);
    }

    private void CameraFollow(Vector3 mousPos)
    {
        if(GameManagers.instance.GetGameState() != GameManagers.GameState.Playing)
        {
            ForceStopAllCoroutines();
        }
        else
        {
            mousePosition.position = new Vector3(Mathf.Clamp(mousPos.x, transform.position.x - 20f, transform.position.x + 20f), 0, Mathf.Clamp(mousPos.z, transform.position.z - 10f, transform.position.z + 10f));
            PlayerRotate(mousePosition.position);
        } 
    }

    private void PlayerRotate(Vector3 mousPos)
    {
        if(GameManagers.instance.GetGameState() != GameManagers.GameState.Playing)
        {
            ForceStopAllCoroutines();
        }
        else
        {
            if (plrState == PlayerStates.InteractingToggle || plrState == PlayerStates.Hiding || plrState == PlayerStates.InteractingHold || plrState == PlayerStates.Dead)
            {

            }
            else
            {
                transform.LookAt(new Vector3(mousPos.x, transform.position.y, mousPos.z));
            }
        }
        
    }
    //End Of Player Look At Script----------------------------------------------------



    //Handles Interactions when Input detected.
    private void OnInteractInputDetector(object sender, EventArgs e)
    {
        PlayerInteracts();
    }
    public event EventHandler<InteractNotifArgs> InteractNotif;
    public class InteractNotifArgs : EventArgs { public Transform target; }
    private void PlayerInteracts()
    {
        if(GameManagers.instance.GetGameState() == GameManagers.GameState.Playing || GameManagers.instance.GetGameState() == GameManagers.GameState.OnUI)
        {
            if (targetInteract == null)
            {
                InteractNotif?.Invoke(this, new InteractNotifArgs { target = null });
            }
            else
            {
                if (targetInteract.TryGetComponent(out IInteraction interact))
                {
                    interact.OnInteract(transform);

                }
            }
        }
        else
        {
            ForceStopAllCoroutines();
        }
    }
    private void InteractionDetector()
    {
        var highestPoint = transform.position + Vector3.up * 3f;
        var playerWidth = 0.35f;
        var playerArmLength = 1.75f;
        if (GameManagers.instance.GetGameState() != GameManagers.GameState.Playing)
        {
            ForceStopAllCoroutines();
        }
        else
        {
            if (RotaryHeart.Lib.PhysicsExtension.Physics.CapsuleCast(transform.position, highestPoint, playerWidth, transform.forward, out RaycastHit hit, playerArmLength, interactableObjs, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.None))
            {
                targetInteract = hit.transform;
                InteractNotif?.Invoke(this, new InteractNotifArgs { target = targetInteract });


            }
            else
            {
                targetInteract = null;
                InteractNotif?.Invoke(this, new InteractNotifArgs { target = null });
            }
        } 
    }
    // End of Interaction script--------------------------------------



    //Checking Health Status
    private void SendDmgToLogicReceiver(object sender, EntityHealthController.SendDmgToLogicArgs e)
    {
        CheckStatus(e.currentHealth);
    }
    public void CheckStatus(float health)
    {
        if (GameManagers.instance.GetGameState() != GameManagers.GameState.Playing)
        {
            ForceStopAllCoroutines();
        }
        else
        {
            if (health <= 0)
            {
                plrState = PlayerStates.Dead;
                transform.gameObject.SetActive(false);
                GameManagers.instance.OnPlayerDeath();
                ForceStopAllCoroutines();
            }
            else
            {

            }
        } 
    }
    //End of Check status-------------------------------------

    // Allows outside script to get player states.
    public PlayerStates GetStates()
    {
        return plrState;
    }
    // End of GetStates function----------------



    public event EventHandler<ProduceSoundArgs> ProduceSound;
    public class ProduceSoundArgs : EventArgs { public float soundSize; }
    private void MakeSound(float soundVolume)
    {
        ProduceSound?.Invoke(this, new ProduceSoundArgs { soundSize = soundVolume });
    }



    //Handles Flashlight Input
    private void OnFlashlightInputDetector(object sender, EventArgs e)
    {
        TurnFlashlight();
    }

    public event EventHandler TurnFlashlightEvent;
    private void TurnFlashlight()
    {
        if (GameManagers.instance.GetGameState() != GameManagers.GameState.Playing)
        {
            ForceStopAllCoroutines();
        }
        else
        {
            TurnFlashlightEvent?.Invoke(this, EventArgs.Empty);
        }
    }
    //End of flashlight script----------------------------------------------------



    private void InventorySystem_EquipItemEvent(object sender, InventorySystem.EquipItemEventArgs e)
    {
        equippedItem = e.item.TryGetComponent(out ItemUses itemUses) ? itemUses : null;
    }




    private void OnMouse1PressedReceiver(object sender, OnMouse1PressedArgs e)
    {
        if(GameManagers.instance.GetGameState() != GameManagers.GameState.Playing)
        {
            ForceStopAllCoroutines();
        }
        else
        {
            if(equippedItem != null)
            {
                equippedItem.MainUse(e.isPressed, transform, centerBody);
            }
        }
    }

    public void ResetPlayer()
    {
        healthController.ResetHealth();
        plrStamina = 100;
        plrState = PlayerStates.Idle;
    }


    void Update()
    {
        InteractionDetector();
    }

}
