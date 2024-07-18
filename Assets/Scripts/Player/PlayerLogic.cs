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
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform mousPosTrans;
    [SerializeField] private float soundBar = 0;

    private bool inShadow = false;

    float playerDirection;
    [SerializeField] private LayerMask enemyLyr;
    private float staminaTime = 0f;

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
    [SerializeField] PlayerStates defaultPlrState;

    private void Start()
    {
        plrInp.OnMoveInput += OnMoveInputDetector;
        plrInp.OnMousePosInput += OnMousePosInputDetector;
        plrInp.OnShiftHold += OnShiftHoldDetector;
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
            MoveSoundChecker();
        }
        else
        {
            plrSprintApplied = 0f;
        }

    }
    //End of Sprint Script-----------------------------------------------------

    private void StaminaBar(bool spaceIsPressed)
    {
        float drainRate = 0.4f;
        if (spaceIsPressed && plrStamina > 0 && playerDirection >= 0.3f)
        {
            if (staminaTime < drainRate)
            {
                staminaTime = staminaTime + Time.deltaTime * 15f;
                
            }
            else
            {
                plrStamina-= 1;
                staminaTime = 0f;

            }
        }
        if(!spaceIsPressed && plrStamina < 100 || spaceIsPressed && plrStamina < 100 && playerDirection < 0.3f)
        {
            if (staminaTime < drainRate)
            {
                staminaTime = staminaTime + Time.deltaTime * 2.5f;

            }
            else
            {
                plrStamina += 1f;
                staminaTime = 0f;
            }
        }
        else
        {

        }
    }


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
        StaminaBar(plrInp.SprintIsPressed());
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
