using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    [SerializeField] PlayerInput plrInp;
    [SerializeField] float plrSpdBase = 5f;
    [SerializeField] float plrSprintBase = 10f;
    [SerializeField] float plrSprintApplied = 0f;
    [SerializeField] float plrStamina = 100f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform mousPosTrans;

    float playerDirection;

    private float staminaTime = 0f;
    private void PlayerMovement()
    {
        float plrSpd = plrSpdBase + plrSprintApplied;

        Vector3 plrMoveDir = new Vector3(plrInp.GetMoveDir().x, 0f, plrInp.GetMoveDir().y);

        rb.MovePosition(rb.position + plrMoveDir * plrSpd * Time.fixedDeltaTime);


        playerDirection = Vector3.Dot(transform.forward, plrMoveDir);

        //transform.position += transform.forward  * PlrMoveDir.z * Time.deltaTime * plrSpd;
        //transform.position += transform.right * PlrMoveDir.x * Time.deltaTime * plrSpd;
    }

    private void PlayerSprintController(bool spaceIsPressed)
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
                Debug.Log(plrStamina);

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
                Debug.Log(plrStamina);
            }
        }
        else
        {

        }
    }
    private void PlayerRotate()
    {
        transform.LookAt(new Vector3(plrInp.GetMousePosition().x, transform.position.y, plrInp.GetMousePosition().z));
    }

    void Update()
    {
        PlayerRotate();
        PlayerSprintController(plrInp.SprintIsPressed());
        StaminaBar(plrInp.SprintIsPressed());
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

}