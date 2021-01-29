using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using utility;

public sealed class CustomerManager : Singleton<CustomerManager>
{
    [Serializable]
    public sealed class CustomerSpot
    {
        public Transform transform = null;
        [NonSerialized] public Customer customer = null;
    }

    [NonSerialized] public List<LostItem> wantedItems = new List<LostItem>(); 

    [SerializeField] private CustomerSpot[] _spots;
    [SerializeField] private GameObject[] _customerPrefabs;

    [SerializeField] private CustomerManagerSettings _settings = null;
    
    // Once customer is placed, give an assignment.
    // Customer states: entering, waiting, correct/wrong item, leaving.

    private readonly List<int> _availableCustomerTypes = new List<int>();

    public void OnCustomerDestroyed(int index) => 
        _availableCustomerTypes.Add(index);

    protected override void Awake()
    {
        base.Awake();

        // Add all the available indexes.
        for (int i = 0; i < _customerPrefabs.Length; i++) 
            _availableCustomerTypes.Add(i);
    }

    private void Start()
    {
        if(_settings.playOnStart)
            Restart();
    }

    #region Externally called
    public void Stop()
    {
        StopAllCoroutines();
    }

    public void Restart()
    {
        Stop();
        StartCoroutine(TryAddCustomers());
    }
    #endregion

    private int GetFreeSpotCount
    {
        get
        {
            // Calculate number of free spots.
            int freeSpotCount = _spots.Length;
            foreach (var spot in _spots)
                if (spot.customer)
                    freeSpotCount--;
            return freeSpotCount;
        }
    }

    private CustomerSpot GetRandomSpot
    {
        get
        {
            // Get available spots.
            var availableSpots = new List<CustomerSpot>();
            foreach (var spot in _spots)
                if (!spot.customer)
                    availableSpots.Add(spot);

            // If no available spots are available.
            if (availableSpots.Count == 0)
                return null;

            // Pick random available spot.
            int max = availableSpots.Count;
            var random = GameManager.Instance.Random;

            int randomIndex = random.Next(0, max - 1);
            return availableSpots[randomIndex];
        }
    }

    private Customer SpawnRandomCustomer()
    {
        int max = _availableCustomerTypes.Count;
        if (max == 0)
            return null;

        var random = GameManager.Instance.Random;

        // Get a random available prefab index.
        int randomIndex = random.Next(0, max - 1);
        int prefabIndex = _availableCustomerTypes[randomIndex];
        var prefab = _customerPrefabs[prefabIndex];

        // Make index unavailable.
        // This makes sure that you don't have the same models in the scene.
        _availableCustomerTypes.RemoveAt(randomIndex);
        
        // Spawn object at a random position.
        var obj = Instantiate(prefab);
        var customer = obj.GetComponent<Customer>();

        // Assign prefab index.
        customer.prefabIndex = prefabIndex;
        return customer;
    }

    private IEnumerator TryAddCustomers()
    {
        float min = _settings.minNewCustomerInterval;
        float max = _settings.maxNewCustomerInterval;

        var random = GameManager.Instance.Random;

        // Try to add new customers after intervals.
        while (true)
        {
            // If there is no spot available, wait before starting the interval.
            while (GetFreeSpotCount == 0)
                yield return null;

            // Calculate random interval.
            float t = (float)random.NextDouble();
            float duration = Mathf.Lerp(min, max, t);

            // Wait for the randomized amount of time.
            yield return new WaitForSeconds(duration);

            // Spawn new customer and assign a corresponding spot.
            var spot = GetRandomSpot;
            var customer = SpawnRandomCustomer();

            spot.customer = customer;
            customer.Spot = spot;

            if (_settings.debug) 
                print("A new customer has been spawned.");
        }
    }
}
