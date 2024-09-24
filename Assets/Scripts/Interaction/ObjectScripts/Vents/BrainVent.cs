using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerLogic;
using UnityEngine.Playables;

public class BrainVent : MonoBehaviour
{
    [SerializeField] Transform[] vents;
    [SerializeField] Transform[] ventSpots;
    [SerializeField] Transform plr;
    [SerializeField] PlayerLogic plrLogic;
    [SerializeField] int currentIndex;
    [SerializeField] Vector2 vect2;

    private PlayerInput playerInput;
    public void InitializeVent(Transform vent, Transform plr)
    {
        if(plrLogic != null)
        {
            plrLogic.plrState = PlayerLogic.PlayerStates.Idle;
            plrLogic.UnHidePlayer();
            plr.transform.position += transform.up + Vector3.up * 0.25f;
            if (Physics.Raycast(plr.transform.position, -Vector3.up, out RaycastHit hit)) plr.transform.position = hit.point;
            plrLogic = null;
            currentIndex = 0;
            this.plr = null;
            
        }
        else
        {
            for (int i = 0; i < ventSpots.Length; i++)
            {
                if (ventSpots[i] == vent)
                {
                    currentIndex = i;
                    plrLogic = plr.GetComponent<PlayerLogic>();
                    playerInput = plr.GetComponent<PlayerInput>();
                    this.plr = plr;
                    plrLogic.HidePlayer();
                    plrLogic.plrState = PlayerStates.InVent;
                    StartCoroutine(CheckInputs());
                }
            }
        }
        
    }

    private IEnumerator CheckInputs()
    {
        while (plrLogic != null)
        {
            if(plrLogic == null)
            {
                break;
            }
            else
            {
                ChangeVents();
            }
            yield return 0;
        }
    }

    bool vent_dbounce = false;
    private void ChangeVents()
    {
        vect2 = playerInput.GetInputDir();
        //Debug.Log(vent_dbounce);
        //Debug.Log(plrLogic.GetPlayerMovement());
        if (playerInput.GetInputDir().x > 0)
        {
            if (vent_dbounce == false)
            {
                vent_dbounce = true;
                if (currentIndex < ventSpots.Length - 1)
                {
                    currentIndex++; 
                }
                else
                {
                    currentIndex = 0;
                }

                plr.position = ventSpots[currentIndex].position;
                plr.LookAt(new Vector3(vents[currentIndex].position.x, plr.position.y, vents[currentIndex].position.z));
            }
            else if(vent_dbounce == true)
            {
                vent_dbounce = true;
            }
        }
        else if(plrLogic.GetPlayerMovement().x < 0)
        {
            Debug.Log("Play!");
            if (vent_dbounce == false)
            {
                vent_dbounce = true;
                if(currentIndex > 0)
                {
                    currentIndex--;
                }
                else
                {
                    currentIndex = ventSpots.Length - 1;
                }
                
                plr.position = ventSpots[currentIndex].position;
                plr.LookAt(new Vector3(vents[currentIndex].position.x, plr.position.y, vents[currentIndex].position.z));
            }
            else if (vent_dbounce == true)
            {
                vent_dbounce = true;
            }
        }
        else
        {
            vent_dbounce = false;
        }
    }
}
