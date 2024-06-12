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

    Vector3 mousPosSimple;

    private float staminaTime = 0f;
    private void PlayerMovement()
    {
        float plrSpd = plrSpdBase + plrSprintApplied;

        Vector3 plrMoveDir = new Vector3(plrInp.GetMoveDir().x, 0f, plrInp.GetMoveDir().y);

        Vector3 plrMoveDirTwo = new Vector3(plrInp.GetMoveDir().x, 0f, plrInp.GetMoveDir().y) * 2;

        rb.MovePosition(rb.position + plrMoveDir * plrSpd * Time.fixedDeltaTime);

        Debug.Log(Vector3.Angle(rb.position + (plrMoveDir * 10), transform.forward));

        //transform.position += transform.forward  * PlrMoveDir.z * Time.deltaTime * plrSpd;
        //transform.position += transform.right * PlrMoveDir.x * Time.deltaTime * plrSpd;
    }

    private void PlayerSprintController(bool spaceIsPressed)
    {

        if (spaceIsPressed && plrStamina > 0)
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
        if (spaceIsPressed && plrStamina > 0)
        {
            if (staminaTime < drainRate)
            {
                staminaTime = staminaTime + Time.deltaTime;
                
            }
            else
            {
                plrStamina-= 10;
                staminaTime = 0f;
            }
        }
        if(!spaceIsPressed && plrStamina < 100)
        {
            if (staminaTime < drainRate)
            {
                staminaTime = staminaTime + Time.deltaTime;

            }
            else
            {
                plrStamina += 2.5f;
                staminaTime = 0f;
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
        PlayerSprintController(plrInp.GetSpacePressed());
        StaminaBar(plrInp.GetSpacePressed());

        mousPosSimple = new Vector3(mousPosTrans.position.x, transform.position.y, mousPosTrans.position.z);
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

}
