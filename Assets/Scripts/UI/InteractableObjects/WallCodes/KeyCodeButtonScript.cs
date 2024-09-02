using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyCodeButtonScript : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if(transform.parent.TryGetComponent(out KeyCodeScript keyCodeScr))
        {
            keyCodeScr.Input(transform.gameObject);
        }
    }
}
