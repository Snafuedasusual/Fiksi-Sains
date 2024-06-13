using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCast : MonoBehaviour
{
    void FixedUpdate()
    {
        TestCasting();
    }

    private void TestCasting()
    {
        RaycastHit hit;
        bool canMove = Physics.Raycast(transform.position, transform.forward, out hit, 5f);
        Debug.Log(canMove);
    }
}
