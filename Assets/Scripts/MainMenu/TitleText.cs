using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleText : utility.Singleton<TitleText>
{
    [SerializeField]
    private AnimationCurve _animation;

    [SerializeField]
    private float _duration;

    private Vector3 _startScale;


    private void Start()
    {
        _startScale = transform.localScale;
    }

    public void PlayAnimation()
    {
        StartCoroutine(Co_Animation());
    }


    private IEnumerator Co_Animation()
    {
        float elapsedTime = 0f;
        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / _duration;
            float factor = _animation.Evaluate(progress);
            transform.localScale = _startScale * factor;
            yield return null;
        }
    }
}
