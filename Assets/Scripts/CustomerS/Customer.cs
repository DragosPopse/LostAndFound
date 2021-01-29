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
            StartCoroutine(Move());
        }
    }

    [NonSerialized] public int prefabIndex = 0;

    [SerializeField] private CustomerSettings _settings;
    private CustomerManager.CustomerSpot _spot = null;

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

    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
    }
}
