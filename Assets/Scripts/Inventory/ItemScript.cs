using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    [SerializeField] GameObject inventoryPrefab;
    [SerializeField] private string itemName;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerLogic>(out PlayerLogic plrLogic))
        {
            //Debug.Log(collision.transform.name);
            for (int i = 0; i < collision.transform.childCount; i++)
            {
                if (collision.transform.GetChild(i).CompareTag("Inventory"))
                {
                    if (collision.transform.GetChild(i).childCount > 0)
                    {
                        for (int j = 0; j < collision.transform.GetChild(i).childCount; j++)
                        {
                            if (collision.transform.GetChild(i).GetChild(j).TryGetComponent<ItemUses>(out ItemUses item))
                            {
                                if (item.GetName() == itemName)
                                {
                                    Debug.Log("Same Name!");
                                }
                                else
                                {
                                    Instantiate(inventoryPrefab.transform, collision.transform.GetChild(i).transform);
                                    Destroy(transform.gameObject);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        Instantiate(inventoryPrefab.transform, collision.transform.GetChild(i).transform);
                        Destroy(transform.gameObject);
                        break;
                    }
                }
                else
                {
                    //Do Nothing
                }
            }
        }
        else
        {
            //Do nothing
        }
    }
}
