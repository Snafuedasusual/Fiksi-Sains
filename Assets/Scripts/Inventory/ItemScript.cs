using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerLogic>(out PlayerLogic playerLogic))
        {
            Debug.Log("PLAYER!");
        }
        else
        {
            //do nothing
        }
    }
}
