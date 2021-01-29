using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : utility.Singleton<ItemSpawner>
{
    [SerializeField]
    private GameObject[] _itemPrefabs;
   
    
    private void SpawnRandomItem()
    {
        int index = Random.Range(0, _itemPrefabs.Length - 1);
        GameObject obj = Instantiate(_itemPrefabs[index]);
        obj.transform.position = transform.position;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnRandomItem();
        }
    }
}
