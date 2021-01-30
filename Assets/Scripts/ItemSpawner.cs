using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : utility.Singleton<ItemSpawner>
{
    [SerializeField]
    private GameObject[] _itemPrefabs;
    [SerializeField]
    private Vector3 SpawnPosition;
    
    private void SpawnRandomItem()
    {
        int index = Random.Range(0, _itemPrefabs.Length);
        float targetX = Random.Range(-6, 6);
        float targetY = Random.Range(-1.75f, -4);
        Vector3 targetPosition = new Vector3(targetX, targetY, index);

        GameObject obj = Instantiate(_itemPrefabs[index]);
        Debug.Log("d");
        obj.transform.position = transform.position;
        obj.GetComponent<LostItem>().NewPosition = targetPosition;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnRandomItem();
        }
    }
}
