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

    public void InitializeVent(Transform vent, Transform plr)
    {
        if(plrLogic != null)
        {
            plrLogic.plrState = PlayerLogic.PlayerStates.Idle;
            plrLogic = null;
            currentIndex = 0;
            plr.localScale = new Vector3(1f, 1f, 1f);
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
                    this.plr = plr;
                    plr.localScale = new Vector3(0.005f, 0.005f, 0.005f);
                    plrLogic.plrState = PlayerStates.InteractingHold;
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
        if(plrLogic.PlayerMovement().x > 0)
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
            if(vent_dbounce == true)
            {
                vent_dbounce = true;
            }
        }
        else if(plrLogic.PlayerMovement().x < 0)
        {
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
            if (vent_dbounce == true)
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
