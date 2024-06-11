using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFIxedPos : MonoBehaviour
{
    [SerializeField] Transform plr;

    private void FollowPlr()
    {
        transform.position = new Vector3(plr.position.x, plr.position.y + 13, (plr.position.z - 1.83F));
    }

    private void Update()
    {
        FollowPlr();
    }
}
