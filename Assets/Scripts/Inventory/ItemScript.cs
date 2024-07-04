using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemScript : MonoBehaviour, F_Interaction
{
    [SerializeField] GameObject inventoryPrefab;
    [SerializeField] private string itemName;
    [SerializeField] private int siblingOrder;
    [SerializeField] private bool isFirearm;
    
    public float ammo;


    public void OnInteract(Transform plr)
    {
        for(int i = 0;  i < plr.childCount; i++)
        {
            if (plr.GetChild(i).CompareTag("Inventory"))
            {
                if(plr.GetChild(i).childCount > 0)
                {
                    for(int j = 0; j < plr.GetChild(i).childCount; j++)
                    {
                        if(plr.GetChild(i).GetChild(j).TryGetComponent<ItemUses>(out ItemUses item))
                        {
                            if(item.GetName() == itemName)
                            {
                                Debug.Log("Its the same!");
                            }
                            else
                            {
                                Transform itemAdded = Instantiate(inventoryPrefab.transform, plr.GetChild(i).transform);
                                itemAdded.SetSiblingIndex(siblingOrder - 1);
                                Destroy(transform.gameObject);
                                break;
                            }
                        }
                        else
                        {

                        }
                    }
                }
                else
                {
                    Instantiate(inventoryPrefab.transform, plr.GetChild(i).transform);
                    Destroy(transform.gameObject);
                    break;
                }
            }
            else
            {
                //Do Nothing.
            }
        }
    }
}
