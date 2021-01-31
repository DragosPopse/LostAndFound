using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuFence : utility.Singleton<MainMenuFence>
{
    [SerializeField]
    private AnimationCurve _scaleAnimation;

    [SerializeField]
    private float _scaleAnimationDuration;

    [SerializeField]
    private Vector2 _shakeMagnitude;

    [SerializeField]
    private float _shakeDuration;

    [SerializeField]
    private float _waitDuration;

    private Vector3 _startScale;



    void Start()
    {
        _startScale = transform.localScale;
    }



    public void PlayLiftAnimation()
    {
        StartCoroutine(Co_LiftAnimation());
    }


    public IEnumerator Co_LiftAnimation()
    {
        var startPosition = transform.position;

        var firstPosition = startPosition;
        var secondPosition = startPosition;

        firstPosition.x -= Random.Range(_shakeMagnitude.x, _shakeMagnitude.y);
        secondPosition.x += Random.Range(_shakeMagnitude.x, _shakeMagnitude.y);

        yield return StartCoroutine(Co_GoTowards(firstPosition, _shakeDuration));
        yield return StartCoroutine(Co_GoTowards(secondPosition, _shakeDuration));
        yield return new WaitForSeconds(_waitDuration);
        yield return StartCoroutine(Co_GoTowards(startPosition, _shakeDuration));

        yield return StartCoroutine(Co_ScaleAnimation());
    }

    /*
    private IEnumerator Co_ShakeAnimation()
    {
        float elapsedTime = 0f;
        var startPosition = transform.position;
        while (elapsedTime < _shakeDuration)
        {
            elapsedTime += Time.deltaTime;
            var position = transform.position;
            position.x = startPosition.x + Random.Range(-1f, 1f) * _shakeMagnitude.x;
            position.y = startPosition.y + Random.Range(-1f, 1f) * _shakeMagnitude.y;
            transform.position = position;
            yield return null;
        }

        transform.position = startPosition;
    }*/


    private IEnumerator Co_GoTowards(Vector3 position, float duration)
    {
        float elapsedTime = 0f;
        var startPosition = transform.position;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPosition, position, progress);
            yield return null;
        }
    }


    private IEnumerator Co_ScaleAnimation()
    {
        float elapsedTime = 0f;
        while (elapsedTime < _scaleAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / _scaleAnimationDuration;
            float scaleFactor = _scaleAnimation.Evaluate(progress);
            var scale = transform.localScale;
            scale.y = _startScale.y * scaleFactor;
            transform.localScale = scale;
            yield return null;
        }
    }
}
