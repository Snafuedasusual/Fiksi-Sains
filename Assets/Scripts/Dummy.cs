using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [SerializeField] float timer = 0f;
    float maxTimer = 8f;
    [SerializeField] bool front = true;
    // Update is called once per frame
    void Update()
    {
        if(timer < maxTimer)
        {
            timer += Time.deltaTime;
            Debug.Log(timer);
            if(front == true)
            {
                transform.position += transform.forward * 3.5f * Time.deltaTime;
            }
            else
            {
                transform.position += -transform.forward * 3.5f * Time.deltaTime;
            }
        }
        else
        {
            front = !front;
            timer = 0f;
        }
    }
}
