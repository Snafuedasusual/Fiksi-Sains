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

    public bool isSeen = false;
    public bool inShadow = false;

    float playerDirection;
    [SerializeField]private Vector3[] plrTrails = new Vector3[5];
    private float staminaTime = 0f;
    private float trailDropTime = 1.5f;

    int indexAdd = 0;
    int indexRemove = 0;


    public void PlayerMovement()
    {
        float plrSpd = plrSpdBase + plrSprintApplied;

        Vector3 plrMoveDir = new Vector3(plrInp.GetMoveDir().x, 0f, plrInp.GetMoveDir().y);

        rb.MovePosition(rb.position + plrMoveDir * plrSpd * Time.deltaTime);


        playerDirection = Vector3.Dot(transform.forward, plrMoveDir);


        if(plrMoveDir != Vector3.zero && isSeen)
        {
            float dropRate = 1.5f;
            if (trailDropTime < dropRate)
            {
                trailDropTime = trailDropTime + Time.deltaTime;
            }
            else
            {
                if (indexAdd < plrTrails.Length)
                {
                    plrTrails[indexAdd] = transform.position;
                    indexAdd++;
                    Debug.Log(indexAdd);
                }
                else
                {
                    indexAdd = 0;
                    plrTrails[indexAdd] = transform.position;
                    Debug.Log(indexAdd);
                }
                trailDropTime = 0f;
            }
        }

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


    void Update()
    {
        StaminaBar(plrInp.SprintIsPressed());
    }

}
