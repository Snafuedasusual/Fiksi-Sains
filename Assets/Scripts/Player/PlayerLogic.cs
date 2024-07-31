using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static PlayerInput;

public class PlayerLogic : MonoBehaviour, IInflictDamage
{
    [Header("Script References")]
    [SerializeField] EntityHealthController healthController;
    [SerializeField] PlayerInput plrInp;
    
    [Header("Components")]
    [SerializeField] InventorySystem inventorySystem;
    [SerializeField] Handle handle;

    [Header("Variables")]
    [SerializeField] float plrSpdBase = 5f;
    [SerializeField] float plrSprintBase = 10f;
    [SerializeField] float plrSprintApplied = 0f;
    [SerializeField] float plrStamina = 100f;
    [SerializeField] Transform targetInteract;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform mousPosTrans;

    [Header("LayerMasks")]
    [SerializeField]
    private LayerMask interactableObjs;
    [SerializeField] private LayerMask enemyLyr;


    float playerDirection;


    [Header("PlayerStates")]
    [SerializeField] PlayerStates defaultPlrState;
    public enum PlayerStates
    {
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
    public PlayerStates plrState;

    private void Start()
    {
        plrInp.OnMoveInput += OnMoveInputDetector;
        plrInp.OnMousePosInput += OnMousePosInputDetector;
        plrInp.OnShiftHold += OnShiftHoldDetector;
        plrInp.OnInteractInput += OnInteractInputDetector;
        healthController.SendDmgToLogic += SendDmgToLogicReceiver;
        plrInp.OnFlashlightInput += OnFlashlightInputDetector;
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

        var walkSound = 0.3f;

        if (plrState == PlayerStates.InteractingToggle || plrState == PlayerStates.Hiding || plrState == PlayerStates.InteractingHold || plrState == PlayerStates.InVent || plrState == PlayerStates.Dead)
        {

        }
        else
        {
            rb.MovePosition(rb.position + plrMoveDir * plrSpd * Time.deltaTime);


            playerDirection = Vector3.Dot(transform.forward, plrMoveDir);



            if (plrMoveDir != Vector3.zero)
            {
                plrState = PlayerStates.Walking;
                MakeSound(walkSound);
            }
            else
            {
                MakeSound(0f);
                plrState = PlayerStates.Idle;
            }

            //transform.position += transform.forward  * PlrMoveDir.z * Time.deltaTime * plrSpd;
            //transform.position += transform.right * PlrMoveDir.x * Time.deltaTime * plrSpd;

        }
        plrDirection = plrMoveDir;
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
        if(plrState == PlayerStates.Dead || plrState == PlayerStates.InVent || plrState == PlayerStates.Hiding)
        {

        }
        else
        {
            if (spaceIsPressed && plrStamina > 0 && playerDirection >= 0.3f)
            {
                plrSprintApplied = plrSprintBase;
                plrState = PlayerStates.Sprinting;
                MakeSound(sprintSound);
                DrainStamina();
            }
            else if (spaceIsPressed && plrStamina <= 0 && playerDirection >= 0.3f)
            {
                plrSprintApplied = 0f;
                MakeSound(15f);
                plrState = PlayerStates.Sprinting;
            }
            else
            {
                plrSprintApplied = 0f;
                RefillStamina();

            }
        }
    }
    //End of Sprint Script-----------------------------------------------------



    //Handles Stamina Bar Drain and Regen.
    private void DrainStamina()
    {
        if(IsStaminaDrainerRunning == null)
        {
            StartCoroutine(StaminaDrainer());
        }
        else
        {

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
                        staminaTime += Time.deltaTime * 7.5f;
                        yield return null;
                    }
                    if (plrStamina <= 0)
                    {

                    }
                    else if (plrStamina > 0 && playerDirection >= 0.3f)
                    {
                        plrStamina--;

                    }

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
        if(IsStaminaRefillRunning == null)
        {
            StartCoroutine(StaminaRefillController());
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
                        staminaTime += Time.deltaTime * 2.5f;
                        yield return null;
                    }
                    if (plrStamina == 100)
                    {

                    }
                    else if (plrStamina < 100)
                    {
                        plrStamina++;
                    }
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
        PlayerRotate(e.mousPos);
    }
    public void PlayerRotate(Vector3 mousPos)
    {
        if(plrState == PlayerStates.InteractingToggle || plrState == PlayerStates.Hiding || plrState == PlayerStates.InteractingHold || plrState == PlayerStates.Dead)
        {

        }
        else
        {
            transform.LookAt(new Vector3(mousPos.x, transform.position.y, mousPos.z));
        }
    }
    //End Of Player Look At Script----------------------------------------------------



    //Handles Interactions when Input detected.
    private void OnInteractInputDetector(object sender, EventArgs e)
    {
        PlayerInteracts();
    }
    private void PlayerInteracts()
    {
        if(targetInteract == null)
        {

        }
        else
        {
            if(targetInteract.TryGetComponent(out IInteraction interact))
            {
                interact.OnInteract(transform);
            }
        }
    }
    private void InteractionDetector()
    {
        var highestPoint = transform.position + Vector3.up * 3f;
        var playerWidth = 0.35f;
        var playerArmLength = 1.75f;
        if (RotaryHeart.Lib.PhysicsExtension.Physics.CapsuleCast(transform.position, highestPoint, playerWidth, transform.forward, out RaycastHit hit, playerArmLength, interactableObjs, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.None))
        {
            targetInteract = hit.transform;
        }
        else
        {
            targetInteract = null;
        }
    }
    // End of Interaction script--------------------------------------




    private void SendDmgToLogicReceiver(object sender, EntityHealthController.SendDmgToLogicArgs e)
    {
        CheckStatus(e.currentHealth);
    }

    public void CheckStatus(float health)
    {
        if(health <= 0)
        {
            plrState = PlayerStates.Dead;
            transform.gameObject.SetActive(false);
        }
        else
        {

        }
    }

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


    private void OnFlashlightInputDetector(object sender, EventArgs e)
    {
        TurnFlashlight();
    }

    public event EventHandler TurnFlashlightEvent;
    void TurnFlashlight()
    {
        TurnFlashlightEvent?.Invoke(this, EventArgs.Empty);
    }



    void Update()
    {
        InteractionDetector();
    }

    private void OnDisable()
    {
        plrInp.OnMoveInput -= OnMoveInputDetector;
        plrInp.OnMousePosInput -= OnMousePosInputDetector;
    }

    private void OnDestroy()
    {
        plrInp.OnMoveInput -= OnMoveInputDetector;
        plrInp.OnMousePosInput -= OnMousePosInputDetector;
    }

    private void OnEnable()
    {
        plrInp.OnMoveInput += OnMoveInputDetector;
        plrInp.OnMousePosInput += OnMousePosInputDetector;
    }
}
