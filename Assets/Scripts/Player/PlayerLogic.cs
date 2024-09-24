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
    [SerializeField] private Rigidbody rb;
    [SerializeField] CapsuleCollider bodyCollider;
    [SerializeField] GameObject visual;

    [Header("Variables")]
    [SerializeField] float plrSpdBase = 5f;
    [SerializeField] float plrSprintBase = 10f;
    [SerializeField] float plrSprintApplied = 0f;
    [SerializeField] float plrStamina = 100f;
    [SerializeField] float centerBody = 1.1f;
    [SerializeField] Transform targetInteract;
    [SerializeField] private Transform mousPosTrans;
    [SerializeField] private ItemUses equippedItem;

    [Header("LayerMasks")]
    [SerializeField] private LayerMask interactableObjs;
    [SerializeField] private LayerMask enemyLyr;

    [Header("Animation")]
    [SerializeField] RuntimeAnimatorController currentController;

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
        Dead,
    }

    public enum PlrAnimations
    {
        MOVEMENT_WALK,
        MOVEMENT_RUN,
        ATTACK1,
        NONE
    }

    public event EventHandler<PlayThisMovementAnimArgs> PlayThisMovementAnim;
    public class PlayThisMovementAnimArgs : EventArgs { public RuntimeAnimatorController controller; public PlrAnimations playThisAnim; public float xAxis; public float yAxis; }

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
        plrInp.OnMouse1Pressed -= OnMouse1PressedReceiver;
        inventorySystem.EquipItemEvent -= InventorySystem_EquipItemEvent;
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
    [SerializeField] private Vector2 plrDirection;
    private void OnMoveInputDetector(object sender, SendMoveInputArgs e)
    {
        PlayerMovement(e.plrDir);
    }
    private void PlayerMovement(Vector2 dir)
    {
        var plrSpd = plrSpdBase + plrSprintApplied;

        var plrMoveDir = new Vector3(dir.x, 0f, dir.y).normalized;

        plrDirection = new Vector2(plrMoveDir.x, plrMoveDir.z);

        var walkSound = 0.3f;
        var sprintSound = 10f;
        if (GameManagers.instance.GetGameState() != GameManagers.GameState.Playing)
        {
            StopAllCoroutines();
        }
        else
        {
            if (plrState == PlayerStates.InteractingToggle || plrState == PlayerStates.Hiding || plrState == PlayerStates.InteractingHold || plrState == PlayerStates.InVent || plrState == PlayerStates.Dead || plrState == PlayerStates.Null)
            {
                plrDirection = Vector3.zero;
                PlayMovementAnim();
            }
            else
            {
                rb.MovePosition(rb.position + plrMoveDir * plrSpd * Time.deltaTime);

                var localMoveDir = (transform.forward * plrMoveDir.z) + (transform.right * plrMoveDir.x);

                //rb.MovePosition(rb.position + transform.forward * plrMoveDir.z * plrSpd * Time.deltaTime);
                //rb.MovePosition(rb.position + transform.right * plrMoveDir.x * plrSpd * Time.deltaTime);

                playerDirection = Vector3.Dot(transform.forward, plrMoveDir);
                //playerDirection = Vector3.Dot(transform.forward.normalized, localMoveDir.normalized);

                

                if (plrMoveDir != Vector3.zero && plrSprintApplied == 0)
                {
                    plrState = PlayerStates.Walking;
                    MakeSound(walkSound);
                    PlayMovementAnim();
                    return;
                }
                else if(plrMoveDir == Vector3.zero && plrSprintApplied == 0)
                {
                    MakeSound(0f);
                    plrState = PlayerStates.Idle;
                    PlayMovementAnim();
                }
                else if(plrMoveDir != Vector3.zero && plrSprintApplied > 0)
                {
                    MakeSound(sprintSound);
                    plrState = PlayerStates.Sprinting;
                    PlayMovementAnim();
                }

            }
            
        }
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
                if(spaceIsPressed == false)
                {
                    if(plrStamina >= 100) { plrStamina = 100; StopCoroutine(StaminaRefillController()); IsStaminaRefillRunning = null; return; }
                    RefillStamina();
                    StopDrainStamina();
                }
                else
                {
                    if (plrStamina <= 0) { plrSprintApplied = 0f; StopDrainStamina(); return; };
                    if (playerDirection <= 0) { plrSprintApplied = 0f; StopDrainStamina(); RefillStamina(); return; };
                    if (plrStamina > 0 && playerDirection >= 0.3f && plrState == PlayerStates.Walking) 
                    { 
                        plrSprintApplied = plrSprintBase; 
                        plrState = PlayerStates.Sprinting;
                        DrainStamina();
                        StopRefillStamina();
                    }
                }
            }
        } 
    }
    private void OnStartSprint(bool activated)
    {
        if (activated == true && plrStamina > 0 && playerDirection >= 0.3f && plrState == PlayerStates.Walking)
        {
            plrSprintApplied = plrSprintBase;
            plrState = PlayerStates.Sprinting;
            DrainStamina();
            return;
        }
        else if (activated == true && plrStamina <= 0)
        {
            plrSprintApplied = 0f;
            DrainStamina();
            return;

        }
        else if (playerDirection <= 0)
        {
            plrSprintApplied = 0f;
            RefillStamina();
            return;
        }
        else if(activated == false)
        {
            plrSprintApplied = 0f;
            StopCoroutine(StaminaDrainer());
            IsStaminaDrainerRunning = null;
            return;
        }
        else
        {
            plrSprintApplied = 0f;
        }
    }
    private void OnStopSprint(bool activated)
    {
        if(activated == false)
        {
            if(plrStamina >= 100) { plrStamina = 100; StopCoroutine(StaminaRefillController()); IsStaminaRefillRunning = null; return; }
            RefillStamina();
        }
        else if(activated == true) { StopCoroutine(StaminaRefillController()); IsStaminaRefillRunning = null; return; }
        else
        {

        }
    }
    //End of Sprint Script-----------------------------------------------------



    void PlayMovementAnim()
    {
        if(plrState == PlayerStates.Idle || plrState == PlayerStates.Walking)
        {
            PlayThisMovementAnim?.Invoke(this, new PlayThisMovementAnimArgs { playThisAnim = PlrAnimations.MOVEMENT_WALK, xAxis = plrDirection.x, yAxis = plrDirection.y, controller = currentController });
            return;
        }
        else if(plrState == PlayerStates.Sprinting)
        {
            PlayThisMovementAnim?.Invoke(this, new PlayThisMovementAnimArgs { playThisAnim = PlrAnimations.MOVEMENT_RUN, xAxis = plrDirection.x, yAxis = plrDirection.y, controller = currentController });
            return;
        }
        else
        {
            PlayThisMovementAnim?.Invoke(this, new PlayThisMovementAnimArgs { playThisAnim = PlrAnimations.MOVEMENT_WALK, xAxis = plrDirection.x, yAxis = plrDirection.y, controller = currentController });
            return;
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
    private void StopDrainStamina()
    {
        if (GameManagers.instance.GetGameState() != GameManagers.GameState.Playing)
        {
            ForceStopAllCoroutines();
            IsStaminaDrainerRunning = null;
        }
        else
        {
            StopCoroutine(StaminaDrainer());
            IsStaminaDrainerRunning = null;
            plrSprintApplied = 0;
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
                        staminaTime += Time.deltaTime * 7f;
                        yield return null;
                    }
                    if (plrStamina <= 0)
                    {
                        StopCoroutine(StaminaDrainer());
                        IsStaminaDrainerRunning = null;
                        yield break;
                    }
                    else if (plrStamina > 0 && playerDirection >= 0.3f)
                    {
                        plrStamina--;

                    }
                    else if(playerDirection <= 0)
                    {
                        StopCoroutine(StaminaDrainer());
                        IsStaminaDrainerRunning = null;
                        yield break;
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
    private void StopRefillStamina()
    {
        if (GameManagers.instance.GetGameState() != GameManagers.GameState.Playing)
        {
            ForceStopAllCoroutines();
            IsStaminaRefillRunning = null;
        }
        else
        {
            StopCoroutine(StaminaRefillController());
            IsStaminaRefillRunning = null;
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
                        staminaTime += Time.deltaTime * 6.5f;
                        yield return null;
                    }
                    if (plrStamina == 100)
                    {
                        StopCoroutine(StaminaRefillController());
                        IsStaminaRefillRunning = null;
                        yield break;
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
            if(plrState == PlayerStates.Null)
            {

            }
            else
            {
                mousePosition.position = new Vector3(Mathf.Clamp(mousPos.x, transform.position.x - 20f, transform.position.x + 20f), 0, Mathf.Clamp(mousPos.z, transform.position.z - 10f, transform.position.z + 10f));
                PlayerRotate(mousePosition.position);
            }
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
            if (plrState == PlayerStates.InteractingToggle || plrState == PlayerStates.Hiding || plrState == PlayerStates.InteractingHold || plrState == PlayerStates.Dead || plrState == PlayerStates.Null || plrState == PlayerStates.InVent)
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
    public class InteractNotifArgs : EventArgs { public Transform target; public string notif; }
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
                if (plrState == PlayerStates.Null) return;
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
        var playerWidth = 0.175f;
        var playerArmLength = 1.75f;
        if (GameManagers.instance.GetGameState() != GameManagers.GameState.Playing)
        {
            ForceStopAllCoroutines();
        }
        else
        {
            if (RotaryHeart.Lib.PhysicsExtension.Physics.CapsuleCast(transform.position, highestPoint, playerWidth, transform.forward, out RaycastHit hit, playerArmLength, interactableObjs, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Editor))
            {
                var interact = hit.transform.TryGetComponent(out IInteraction interaction) ? interaction : null;
                if (interact == null) return;
                targetInteract = hit.transform;
                InteractNotif?.Invoke(this, new InteractNotifArgs { target = targetInteract, notif = interact.UpdateNotif() });


            }
            else
            {
                targetInteract = null;
                InteractNotif?.Invoke(this, new InteractNotifArgs { target = null, notif = string.Empty });
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
        currentController = equippedItem.GetController();
    }


    public void NullifyState()
    {
        plrState = PlayerStates.Null;
        plrDirection = Vector3.zero;
        PlayMovementAnim();
    }

    public void UnNullifyState()
    {
        plrState = PlayerStates.Idle;
    }

    public void HidePlayer()
    {
        rb.useGravity = false;
        bodyCollider.enabled = false;
        visual.SetActive(false);
    }

    public void UnHidePlayer()
    {
        bodyCollider.enabled = true;
        visual.SetActive(true);
        rb.useGravity = true;
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
                if(plrState == PlayerStates.Idle || plrState == PlayerStates.Walking) { equippedItem.MainUse(e.isPressed, transform, centerBody);}
            }
        }
    }

    public event EventHandler<PlayThisAttackAnimArgs> PlayThisAttackAnim;
    public class PlayThisAttackAnimArgs : EventArgs { public RuntimeAnimatorController controller; public PlrAnimations anim;}
    public void Mouse1PlayAnim()
    {
        PlayThisAttackAnim?.Invoke(this, new PlayThisAttackAnimArgs { controller = currentController, anim = PlrAnimations.ATTACK1});
    }

    public void ResetPlayer()
    {
        healthController.ResetHealth();
        plrStamina = 100;
        plrState = PlayerStates.Idle;
        UnHidePlayer();
    }


    void Update()
    {
        InteractionDetector();
    }

}
