using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerLogic : MonoBehaviour, IInflictDamage, IMakeSound
{
    [Header("Variables")]
    [SerializeField] float plrHealth = 100;
    [SerializeField] PlayerInput plrInp;
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
        Hiding
    }

    public PlayerStates plrState;
    [SerializeField] PlayerStates defaultPlrState;

    public Vector3 PlayerMovement()
    {
        float plrSpd = plrSpdBase + plrSprintApplied;

        Vector3 plrMoveDir = new Vector3(plrInp.GetMoveDir().x, 0f, plrInp.GetMoveDir().y);

        if(plrState == PlayerStates.InteractingToggle || plrState == PlayerStates.Hiding)
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
        return plrMoveDir;

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

    public void PlayerRotate(Vector3 mousPos)
    {
        if(plrState == PlayerStates.InteractingToggle || plrState == PlayerStates.Hiding)
        {

        }
        else
        {
            transform.LookAt(new Vector3(mousPos.x, transform.position.y, mousPos.z));
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

    float IE_knockTime;
    IEnumerator HitIsCoolingDown;
    private IEnumerator HitCooldown()
    {
        float draintime = 0.2f;
        while(IE_knockTime < draintime)
        {
            IE_knockTime += Time.deltaTime;
            yield return 0;
        }
        rb.isKinematic = false;
        rb.isKinematic = true;
        IE_knockTime = 0f;
        HitIsCoolingDown = null;
    }

    public void KnockBack(Transform sender, float knockBackPwr)
    {
        if (HitIsCoolingDown == null && plrState == PlayerStates.Idle)
        {
            HitIsCoolingDown = HitCooldown();
            Vector3 direction = (transform.position - sender.position).normalized;
            rb.AddForce(direction * knockBackPwr, ForceMode.Impulse);
            StartCoroutine(HitIsCoolingDown);
        }
    }

    void Update()
    {
        StaminaBar(plrInp.SprintIsPressed());
    }

}
