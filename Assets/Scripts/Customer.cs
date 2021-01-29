using System;
using System.Collections;
using UnityEngine;
using utility;

public sealed class Customer : Multiton<Customer>
{
    public CustomerManager.CustomerSpot Spot
    {
        set
        {
            _spot = value;
            OnSpotUpdated();
        }
    }

    [NonSerialized] public int prefabIndex = 0;

    [SerializeField] private CustomerSettings _settings;
    private CustomerManager.CustomerSpot _spot = null;
    private Coroutine _spotUpdatedCoroutine = null;

    private void OnSpotUpdated()
    {
        if(_spotUpdatedCoroutine != null)
            StopCoroutine(_spotUpdatedCoroutine);
        _spotUpdatedCoroutine = StartCoroutine(_OnSpotUpdated());

        IEnumerator _OnSpotUpdated()
        {
            yield break;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
    }
}
