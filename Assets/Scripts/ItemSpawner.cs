using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : utility.Singleton<ItemSpawner>
{
    [SerializeField]
    private GameObject[] _itemPrefabs;
    [SerializeField]
    private Vector3 SpawnPosition;


    private List<GameObject> _itemPool = new List<GameObject>();
    private List<GameObject> _usedItems = new List<GameObject>();


    private void Start()
    {
        foreach (var obj in _itemPrefabs)
        {
            _itemPool.Add(obj);
        }
    }


    private void SpawnRandomItem()
    {
        if (_itemPool.Count == 0)
        {
            return;
        }
        int index = Random.Range(0, _itemPool.Count);
        float targetX = Random.Range(-6, 6);
        float targetY = Random.Range(-1.75f, -4);
        Vector3 targetPosition = new Vector3(targetX, targetY, index);

        GameObject obj = Instantiate(_itemPool[index]);
        _usedItems.Add(_itemPool[index]);
        _itemPool.Remove(_itemPool[index]);

        obj.transform.position = transform.position;
        obj.GetComponent<LostItem>().NewPosition = targetPosition;
    }


    public void PoolItem(LostItem item)
    {
        var sprite = item.GetComponent<SpriteRenderer>().sprite;
        for (int i = 0; i < _usedItems.Count; i++)
        {
            if (_usedItems[i].GetComponent<SpriteRenderer>().sprite == sprite)
            {
                _itemPool.Add(_usedItems[i]);
                _usedItems.Remove(_usedItems[i]);
                break;
            }    
        }
    }


    private void Update()
    {
        if (GameManager.Instance.GameState == GameManager.State.Game)
        {
            SpawnRandomItem();
        }
    }
}
