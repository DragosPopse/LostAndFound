using UnityEngine;

public sealed class Bouncer : MonoBehaviour
{
    [SerializeField] private float _bounceHeight = .5f;
    [SerializeField] private float _bounceMinDuration = .5f, _bounceMaxDuration = 1;
    [SerializeField] private AnimationCurve _curve = null;

    private float _currentDuration = 0;
    private float _currentBounceTime = 0;

    private Vector3 _rootPosition = Vector3.zero;

    private void Awake() => 
        _rootPosition = transform.position;

    private void Update()
    {
        _currentBounceTime += Time.deltaTime;
        if (_currentBounceTime >= _currentDuration)
        {
            _currentBounceTime = 0;

            var random = GameManager.Instance.Random;
            float t = (float) random.NextDouble();
            _currentDuration = Mathf.Lerp(_bounceMinDuration, _bounceMaxDuration, t);
        }

        float lerp = 1f - _currentBounceTime / _currentDuration;
        float eval = _curve.Evaluate(lerp);

        transform.position = _rootPosition + Vector3.up * eval * -_bounceHeight;
    }
}
