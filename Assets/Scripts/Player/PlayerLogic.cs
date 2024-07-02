using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    [Header("Variables")]
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
    [SerializeField]private Vector3[] plrTrails = new Vector3[10];
    private float staminaTime = 0f;
    private float trailDropTime = 1.5f;


    public void PlayerMovement()
    {
        float plrSpd = plrSpdBase + plrSprintApplied;

        Vector3 plrMoveDir = new Vector3(plrInp.GetMoveDir().x, 0f, plrInp.GetMoveDir().y);

        rb.MovePosition(rb.position + plrMoveDir * plrSpd * Time.deltaTime);


        playerDirection = Vector3.Dot(transform.forward, plrMoveDir);

        //transform.position += transform.forward  * PlrMoveDir.z * Time.deltaTime * plrSpd;
        //transform.position += transform.right * PlrMoveDir.x * Time.deltaTime * plrSpd;
    }


    public void PlayerSprintController(bool spaceIsPressed)
    {

        if (spaceIsPressed && plrStamina > 0 && playerDirection >= 0.3f)
        {
            plrSprintApplied = plrSprintBase;
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
        transform.LookAt(new Vector3(mousPos.x, transform.position.y, mousPos.z));
    }

    public void MakeSound(float soundBarAdder)
    {
        soundBar += soundBarAdder;
    }

    void Update()
    {
        StaminaBar(plrInp.SprintIsPressed());
    }

}
