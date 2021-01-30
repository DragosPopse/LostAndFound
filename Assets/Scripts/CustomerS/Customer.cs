using System;
using System.Collections;
using UnityEngine;
using utility;

[RequireComponent(typeof(SpriteRenderer))]
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

    private SpriteRenderer _renderer = null;

    private CustomerManager.CustomerSpot _spot = null;
    private LostItem _wantedItem = null;

    private bool _waiting = false;
    private bool _foundItem = false;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private IEnumerator OnSpotUpdated()
    {
        _renderer.color = Color.white;
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

        while (items.Count - wantedItems.Count == 0)
            yield return null;

        int max = items.Count;
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
        _foundItem = false;

        float min = _settings.minWaitDuration;
        float max = _settings.maxWaitDuration;

        var random = GameManager.Instance.Random;

        float t = (float)random.NextDouble();
        float duration = Mathf.Lerp(min, max, t);
        float remaining = duration;

        while ((remaining -= Time.deltaTime) > 0 && !_foundItem)
        {
            float lerp = 1f - remaining / duration;
            float eval = _settings.angryColorCurve.Evaluate(lerp);

            // Update color based on how long the customer is waiting.
            // The customer gets redder (angry) the longer it waits.
            var color = Color.LerpUnclamped(Color.white, _settings.angryColor, eval);
            _renderer.color = color;
            yield return null;
        }

        _waiting = false;
    }

    private IEnumerator OnMissingItemReceived()
    {
        var wantedItems = CustomerManager.Instance.wantedItems;
        wantedItems.Remove(_wantedItem);
        _wantedItemRenderer.gameObject.SetActive(false);
        if (!_foundItem)
            yield break;

        _wantedItem.enabled = false;
        _wantedItem.transform.SetParent(transform);

        float remaining = _settings.itemGrabDuration;

        var trans = _wantedItem.transform;
        Vector3 start = trans.localPosition;
        Vector3 end = Vector3.zero;

        while (remaining > 0)
        {
            remaining -= Time.deltaTime;
            remaining = Mathf.Max(0, remaining);
            
            float lerp = 1f - remaining / _settings.itemGrabDuration;
            float eval = _settings.grabCurve.Evaluate(lerp);

            var pos = Vector3.LerpUnclamped(start, end, eval);
            trans.localPosition = pos;

            trans.localScale = Vector3.one * _settings.shrinkCurve.Evaluate(lerp);
            yield return null;
        }
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!_waiting)
            return;

        LostItem item = collision.gameObject.GetComponent<LostItem>();
        if (!item.IsSelected && item == _wantedItem) 
            _foundItem = true;
    }
}
