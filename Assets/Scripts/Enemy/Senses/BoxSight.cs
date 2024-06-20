using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSight : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {

    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit");
    }

}
