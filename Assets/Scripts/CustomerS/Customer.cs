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
            // Spawn at target position.
            var offset = new Vector3(0, _settings.verticalSpawnOffset, 0);
            transform.position = _spot.transform.position + offset;

            float min = _settings.spawnMinDuration;
            float max = _settings.spawnMaxDuration;
            
            var random = GameManager.Instance.Random;

            float t = (float) random.NextDouble();
            float duration = Mathf.Lerp(min, max, t);

            Vector3 start = transform.position;
            Vector3 stop = _spot.transform.position;

            // Move customer towards the target spot.
            float remaining = duration;
            while (remaining > 0)
            {
                remaining -= Time.deltaTime;
                remaining = Mathf.Max(0, remaining);

                float lerp = 1f - remaining / duration;
                float eval = _settings.spawnCurve.Evaluate(lerp);

                var clamped = Vector3.LerpUnclamped(start, stop, eval);
                transform.position = clamped;
                yield return null;
            }

            yield break;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
    }
}
