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

    [SerializeField] private Sprite 
        _defaultSprite = null, 
        _waitingSprite = null, 
        _foundSprite = null,
        _angrySprite = null,
        _gaspSprite = null;

    private SpriteRenderer _renderer = null;

    private CustomerManager.CustomerSpot _spot = null;
    private LostItem _wantedItem = null, _stealItem = null;

    private bool _waiting = false;
    private LostItem _foundItem = null;

    private int _animationLocked = 0;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private IEnumerator OnSpotUpdated()
    {
        _renderer.flipX = GameManager.Instance.Random.Next(0, 2) == 0;

        _renderer.color = Color.white;
        _renderer.sprite = _defaultSprite;

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
        _wantedItemRenderer.color = Color.black;
        _wantedItemRenderer.gameObject.SetActive(true);
    }

    private IEnumerator UpdateCustomer()
    {
        _waiting = true;
        _foundItem = null;

        float min = _settings.minWaitDuration;
        float max = _settings.maxWaitDuration;

        var random = GameManager.Instance.Random;

        // Calculate maximum wait duration.
        float t = (float)random.NextDouble();
        float duration = Mathf.Lerp(min, max, t);
        float remaining = duration;

        // Calculate whether or not the character is trying to steal something.
        bool stealing = _settings.stealChance > random.NextDouble();
        var potentialStealTargets = LostItem.Instances;
        int stealIndex = random.Next(0, potentialStealTargets.Count);
        float stealInterval = _settings.stealInterval;

        // Check if the stealing item is the same as the wanted item.
        if (potentialStealTargets.Count > 0)
            _stealItem = stealing ? potentialStealTargets[stealIndex] : null;
        else
            _stealItem = null;

        while (((remaining -= Time.deltaTime) > 0 || _stealItem) && !_foundItem)
        {
            // Update animation.
            if(_animationLocked == 0)
                _renderer.sprite = _waitingSprite;

            float lerp = 1f - remaining / duration;
            float eval = _settings.angryColorCurve.Evaluate(lerp);

            // Update stealing.
            if (remaining < 0)
            {
                stealInterval -= Time.deltaTime;
                if (stealInterval <= 0)
                    if (_stealItem)
                    {
                        if (_stealItem.IsSelected)
                            stealInterval = _settings.stealInterval;
                        else
                        {
                            _stealItem.LerpFactor = _settings.stealLerp;
                            _stealItem.NewPosition = transform.position;
                        }
                    }
                    else
                        _stealItem = null;
            }

            // Calculate bounce.
            var bounceMaxOffset = new Vector3(0, -_settings.bounceMultiplier, 0);
            float bounceEval = _settings.bounceCurve.Evaluate((duration - remaining) % _settings.bounceDuration);
            var bounceOffset = Vector3.LerpUnclamped(Vector3.zero, bounceMaxOffset, bounceEval);

            transform.position = _spot.transform.position + bounceOffset;

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
        var wanted = CustomerManager.Instance.wantedItems;
        wanted.Remove(_wantedItem);

        _wantedItemRenderer.gameObject.SetActive(false);
        if (!_foundItem)
        {
            _renderer.sprite = _angrySprite;
            yield break;
        }

        _renderer.sprite = _foundSprite;

        _foundItem.enabled = false;
        _foundItem.transform.SetParent(transform);

        float remaining = _settings.itemGrabDuration;

        var trans = _foundItem.transform;
        Vector3 start = trans.localPosition;
        Vector3 end = new Vector3(0, _settings.grabVerticalOffset, 0);

        var startScale = trans.localScale;

        while (remaining > 0)
        {
            remaining -= Time.deltaTime;
            remaining = Mathf.Max(0, remaining);
            
            float lerp = 1f - remaining / _settings.itemGrabDuration;
            float eval = _settings.grabCurve.Evaluate(lerp);

            // Calculate position.
            var pos = Vector3.LerpUnclamped(start, end, eval);
            trans.localPosition = pos;

            // Calculate scale.
            trans.localScale = startScale * _settings.shrinkCurve.Evaluate(lerp);
            
            yield return null;
        }
    }

    private void OnDestroy()
    {
        var manager = CustomerManager.Instance;
        _spot.customer = null;
        manager.OnCustomerDestroyed(prefabIndex);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        LostItem item = collision.gameObject.GetComponent<LostItem>();
        if (!item)
            return;
        _animationLocked++;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!_waiting)
            return;

        LostItem item = collision.gameObject.GetComponent<LostItem>();
        if (!item)
            return;

        var wanted = CustomerManager.Instance.wantedItems;
        if (!wanted.Contains(item))
            return;

        if (item != _wantedItem)
        {
            _renderer.sprite = _angrySprite;
            if(item != _stealItem)
                return;
        }
        else
            _renderer.sprite = _gaspSprite;

        if (item.IsSelected)
            return;

        _foundItem = item;
        wanted.Remove(_wantedItem);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        LostItem item = collision.gameObject.GetComponent<LostItem>();
        if (!item)
            return;

        _animationLocked--;
    }
}
