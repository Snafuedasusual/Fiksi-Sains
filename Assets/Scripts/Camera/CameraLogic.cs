using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    [SerializeField] private Transform plr;
    [SerializeField] private Transform mousePos;
    [SerializeField] private Transform camLookAt;
    private void FollowPlayerPos()
    {

        //transform.position = new Vector3(Mathf.Clamp((plr.position.x + mousePos.position.x)/2f, plr.position.x - 0.5f, plr.position.x + 0.5f), plr.position.y + 13f, Mathf.Clamp(((plr.position.z + mousePos.position.z) - 1.83f)/2f, plr.position.x - 0.5f, plr.position.x + 0.5f));
        
        transform.position = new Vector3(transform.position.x, plr.position.y + 30, (plr.position.z - 0.83F));
        
    }

    private void FollowCameraTarget()
    {
        camLookAt.position = new Vector3(Mathf.Clamp((plr.position.x + mousePos.position.x) / 2f, plr.position.x - 0.5f, plr.position.x + 0.5f), 0, Mathf.Clamp(((plr.position.z + mousePos.position.z) - 1.83f) / 2f, plr.position.x - 0.5f, plr.position.x + 0.5f));
    }

    private void Update()
    {

    }
}
