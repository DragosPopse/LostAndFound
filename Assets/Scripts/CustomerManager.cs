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

    [SerializeField] private CustomerSpot[] _spots;
    [SerializeField] private GameObject[] _customerPrefabs;

    [SerializeField] private CustomerManagerSettings _settings = null;
    
    // Once customer is placed, give an assignment.
    // Customer states: entering, waiting, correct/wrong item, leaving.

    private void Start()
    {
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

    Customer SpawnRandomCustomer()
    {
        int max = _customerPrefabs.Length;
        var random = GameManager.Instance.Random;

        int randomIndex = random.Next(0, max - 1);
        var prefab = _customerPrefabs[randomIndex];
        
        var obj = Instantiate(prefab);
        return obj.GetComponent<Customer>();
    }

    IEnumerator TryAddCustomers()
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
