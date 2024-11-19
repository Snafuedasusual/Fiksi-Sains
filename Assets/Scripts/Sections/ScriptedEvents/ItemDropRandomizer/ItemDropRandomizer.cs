using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropRandomizer : MonoBehaviour
{
    [SerializeField] GameObject[] itemDrops;

    private void OnEnable()
    {
        var randomDrop = Random.Range(0, itemDrops.Length);
        var selectedDrop = Instantiate(itemDrops[randomDrop], transform);
        selectedDrop.transform.eulerAngles = new Vector3(selectedDrop.transform.eulerAngles.x, Random.Range(0, 361), selectedDrop.transform.eulerAngles.z);
        selectedDrop.transform.position = transform.position;
    }

    private void OnDisable()
    {
        if (transform.childCount <= 0) return;
        for(int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
