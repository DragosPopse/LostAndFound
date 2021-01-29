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
            StartCoroutine(OnSpotUpdated());
        }
    }
    
    [NonSerialized] public int prefabIndex = 0;

    [SerializeField] private CustomerSettings _settings = null;
    [SerializeField] private SpriteRenderer _wantedItemRenderer = null;

    private CustomerManager.CustomerSpot _spot = null;
    private LostItem _wantedItem = null;

    private bool _waiting = false;

    private IEnumerator OnSpotUpdated()
    {
        yield return StartCoroutine(Move());
        yield return StartCoroutine(AskForMissingItem());
        yield return StartCoroutine(UpdateCustomer());
        yield return StartCoroutine(OnMissingItemReceived());
        yield return StartCoroutine(Move(true));
        Destroy(gameObject);
    }

    private IEnumerator Move(bool reversed = false)
    {
        var offset = new Vector3(0, _settings.verticalSpawnOffset, 0);
        Vector3 start = _spot.transform.position + offset;
        Vector3 stop = _spot.transform.position;

        if (reversed)
        {
            var a = start;
            start = stop;
            stop = a;
        }

        float min = _settings.moveMinDuration;
        float max = _settings.moveMaxDuration;

        var random = GameManager.Instance.Random;

        float t = (float)random.NextDouble();
        float duration = Mathf.Lerp(min, max, t);

        // Move customer towards the target spot.
        float remaining = duration;
        while (remaining > 0)
        {
            remaining -= Time.deltaTime;
            remaining = Mathf.Max(0, remaining);

            float lerp = 1f - remaining / duration;
            float eval = _settings.moveCurve.Evaluate(lerp);

            var clamped = Vector3.LerpUnclamped(start, stop, eval);

            transform.position = clamped;
            yield return null;
        }
    }

    private IEnumerator AskForMissingItem()
    {
        var items = LostItem.Instances;
        var wantedItems = CustomerManager.Instance.wantedItems;

        int max = items.Count;
        if (max - wantedItems.Count == 0)
            yield break;

        LostItem item = null;

        do
        {
            // Pick random available spot.
            var random = GameManager.Instance.Random;
            int randomIndex = random.Next(0, max);
            item = items[randomIndex];
        } while (wantedItems.Contains(item));

        wantedItems.Add(item);
        _wantedItem = item;

        // ReSharper disable once LocalVariableHidesMember
        var renderer = item.GetComponent<SpriteRenderer>();
        _wantedItemRenderer.sprite = renderer.sprite;
        _wantedItemRenderer.color = renderer.color;
        _wantedItemRenderer.gameObject.SetActive(true);
    }

    private IEnumerator UpdateCustomer()
    {
        _waiting = true;
        while (_waiting)
            yield return null;
    }

    private IEnumerator OnMissingItemReceived()
    {
        var wantedItems = CustomerManager.Instance.wantedItems;
        wantedItems.Remove(_wantedItem);
        _wantedItemRenderer.gameObject.SetActive(false);
        yield break;
    }

    private void OnDestroy()
    {
        var manager = CustomerManager.Instance;
        manager.OnCustomerDestroyed(prefabIndex);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_waiting)
            return;

        LostItem item = collision.gameObject.GetComponent<LostItem>();
        if (item == _wantedItem)
            _waiting = false;
        print("Found wanted item!");
    }
}
