using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotate : MonoBehaviour
{
    [SerializeField] Transform lookAtObj;

    private void Update()
    {
        if (lookAtObj == null) return;

        //float angle = Vector3.Angle(new Vector3(lookAtObj.position.x, 0, lookAtObj.position.z) - new Vector3(transform.position.x, 0, transform.position.z), new Vector3(transform.position.x, 0 ,transform.position.z) + Vector3.forward);

        //float angle = Vector3.SignedAngle(new Vector3(lookAtObj.position.x, 0, lookAtObj.position.z) - new Vector3(transform.position.x, 0, transform.position.z), new Vector3(transform.position.x, 0, transform.position.z) + Vector3.forward, transform.up);
        
        var dir = lookAtObj.position - transform.position;

        float angle = (Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg - 90) % 360;
        Quaternion angleAxis = Quaternion.AngleAxis(-angle, Vector3.up);

        Debug.Log(angle);

        //transform.eulerAngles = new Vector3(transform.rotation.x, angle, transform.rotation.z);
        transform.rotation = angleAxis;
    }
}
