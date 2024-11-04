using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class KeyCodeScript : MonoBehaviour, IInitializeScript
{

    [SerializeField] private int thisKeyCodeValue;
    [SerializeField] private EventCommsUI eventComms;

    [SerializeField] private bool canInput = false;

    public void InitializeScript()
    {
        eventComms.EnableCodeInputEvent += EnableCodeInputEventReceiver;
        eventComms.DisableCodeInputEvent += DisableCodeInputEventReceiver;
    }


    public void DeInitializeScript()
    {
        eventComms.EnableCodeInputEvent -= EnableCodeInputEventReceiver;
        eventComms.DisableCodeInputEvent -= DisableCodeInputEventReceiver;
    }

    private void OnEnable()
    {
        InitializeScript();
    }
    private void Start()
    {
        InitializeScript();
    }
    private void OnDisable()
    {
        DeInitializeScript();
    }
    private void OnDestroy()
    {
        DeInitializeScript();
    }


    private void EnableCodeInputEventReceiver(object sender, System.EventArgs e)
    {
        canInput = true;
    }


    private void DisableCodeInputEventReceiver(object sender, System.EventArgs e)
    {
        canInput = false;
    }


    public void Input(GameObject sender)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if(sender.transform == transform.GetChild(i) && canInput == true)
            {
                eventComms.CodeInput(thisKeyCodeValue);
                eventComms.PlayAudioButtonPress();
            }
        }
    }

}
