using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction_PlayerInput : MonoBehaviour
{
    private bool e_debounce = false;
    private GameObject target = null;
    [SerializeField] LayerMask interactable;

    private void FPressed()
    {
        if (!e_debounce && Input.GetKeyDown(KeyCode.F) && target != null)
        {
            if(target.TryGetComponent<F_Interaction>(out F_Interaction interact))
            {
                interact.OnInteract(transform);
            }
            else
            {

            }
        }
        else
        {

        }
    }

    private void InteractionDetector()
    {
        float capsuleSize = 0.25f;
        RaycastHit hit;
        int layerNumItem = 7;
        int layerNumObj = 8;

        int layerMaskItem = 7 << layerNumItem;
        int layerMaskObj = 8 << layerNumObj;


        if (RotaryHeart.Lib.PhysicsExtension.Physics.CapsuleCast(transform.position + Vector3.down * 2f, transform.position + Vector3.up * 2.5f, capsuleSize, transform.forward, out hit, 2f, interactable, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.None))
        {
            if(hit.transform.gameObject.TryGetComponent<F_Interaction>(out F_Interaction fInteract0) && target == null)
            {
                target = hit.transform.gameObject;
            }
            if(hit.transform.gameObject.TryGetComponent<F_Interaction>(out F_Interaction fInteract1) && target != null)
            {
                target = CompareDistance(target, hit.transform.gameObject, transform.gameObject);
            }
            else
            {

            }
        }
        else
        {
            target = null;
        }
    }

    GameObject obj1;
    GameObject obj2;

    private GameObject CompareDistance(GameObject obj1, GameObject obj2, GameObject plr)
    {
        this.obj1 = obj1;
        this.obj2 = obj2;
        float distanceObj1 = Vector3.Distance(obj1.transform.position, plr.transform.position);
        float distanceObj2 = Vector3.Distance(obj2.transform.position, plr.transform.position);
        GameObject selectedItem;
        if(distanceObj1 < distanceObj2)
        {
            selectedItem = obj1;
        }
        else
        {
            selectedItem = obj2;
        }
        return selectedItem;
    }


    private void Update()
    {
        FPressed();
    }



    private void FixedUpdate()
    {
        InteractionDetector();
    }
}
