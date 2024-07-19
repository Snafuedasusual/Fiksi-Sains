using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static PlayerInput;

public class PlayerLogic : MonoBehaviour, IInflictDamage, IMakeSound
{
    [Header("Components")]
    [SerializeField] PlayerInput plrInp;
    [SerializeField] InventorySystem inventorySystem;
    [SerializeField] Handle handle;

    [Header("Variables")]
    [SerializeField] float plrHealth = 100;
    [SerializeField] float plrSpdBase = 5f;
    [SerializeField] float plrSprintBase = 10f;
    [SerializeField] float plrSprintApplied = 0f;
    [SerializeField] float plrStamina = 100f;
    [SerializeField] Transform targetInteract;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform mousPosTrans;
    [SerializeField] private float soundBar = 0;

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
    }
    public PlayerStates plrState;

    private void Start()
    {
        plrInp.OnMoveInput += OnMoveInputDetector;
        plrInp.OnMousePosInput += OnMousePosInputDetector;
        plrInp.OnShiftHold += OnShiftHoldDetector;
        plrInp.OnInteractInput += OnInteractInputDetector;
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

        if (plrState == PlayerStates.InteractingToggle || plrState == PlayerStates.Hiding || plrState == PlayerStates.InteractingHold)
        {

        }
        else
        {
            rb.MovePosition(rb.position + plrMoveDir * plrSpd * Time.deltaTime);


            playerDirection = Vector3.Dot(transform.forward, plrMoveDir);



            if (plrMoveDir != Vector3.zero)
            {
                plrState = PlayerStates.Walking;
                MoveSoundChecker();
            }
            else
            {
                plrState = PlayerStates.Idle;
                soundBar = 0;
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

        if (spaceIsPressed && plrStamina > 0 && playerDirection >= 0.3f)
        {
            plrSprintApplied = plrSprintBase;
            plrState = PlayerStates.Sprinting;
            DrainStamina();
            MoveSoundChecker();
        }
        else
        {
            plrSprintApplied = 0f;
            RefillStamina();

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
        var staminaRate = 0.4f;
        if (IsStaminaDrainerRunning != null)
        {

        }
        else
        {
            if (plrState == PlayerStates.Sprinting)
            {
                IsStaminaDrainerRunning = StaminaDrainer();
                while (plrState == PlayerStates.Sprinting)
                {
                    staminaTime = 0f;
                    while (staminaTime < staminaRate)
                    {
                        staminaTime += Time.deltaTime * 20;
                        yield return null;
                    }
                    if (plrStamina <= 0)
                    {

                    }
                    else if (plrStamina > 0)
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
        var staminaRate = 0.4f;
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
                        staminaRate += Time.deltaTime * 2.5f;
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
        if(plrState == PlayerStates.InteractingToggle || plrState == PlayerStates.Hiding || plrState == PlayerStates.InteractingHold)
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

    public void SoundProducer(float soundAdder)
    {
        soundBar = soundAdder;
        Collider[] listColliders = Physics.OverlapSphere(transform.position, 100f, enemyLyr);
        foreach(var collider in listColliders)
        {
            if(collider.TryGetComponent<IMakeSound>(out IMakeSound sound))
            {
                sound.SoundReceiver(new Vector3(transform.position.x, transform.position.y, transform.position.z), soundBar);
            }
        }
    }

    public void MoveSoundChecker()
    {
        if (plrState == PlayerStates.Walking)
        {
            SoundProducer(0.3f);
        }
        if (plrState == PlayerStates.Sprinting)
        {
            SoundProducer(15f);
        }
        if(plrState == PlayerStates.Idle)
        {
            SoundProducer(0f);
        }
    }

    public void DealDamage(float dmgVal, Transform dmgSender, float knckBckPwr)
    {
        plrHealth -= dmgVal;

        if(plrHealth < 1)
        {
            transform.gameObject.SetActive(false);
        }
        else
        {
            KnockBack(dmgSender, knckBckPwr);
        }
    }

    
    IEnumerator HitIsCoolingDown;
    private IEnumerator HitCooldown()
    {
        var IE_knockTime = 0f;
        var draintime = 0.2f;
        while(IE_knockTime < draintime)
        {
            IE_knockTime += Time.deltaTime;
            yield return 0;
        }
        rb.isKinematic = true;
        rb.isKinematic = false;
        IE_knockTime = 0f;
        HitIsCoolingDown = null;
    }
    public void KnockBack(Transform sender, float knockBackPwr)
    {
        if (HitIsCoolingDown == null && plrState == PlayerStates.Idle)
        {
            HitIsCoolingDown = HitCooldown();
            var direction = (transform.position - sender.position).normalized;
            rb.AddForce(direction * knockBackPwr, ForceMode.Impulse);
            StartCoroutine(HitIsCoolingDown);
        }
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
