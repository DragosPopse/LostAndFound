using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuFence : utility.Singleton<MainMenuFence>
{
    [SerializeField]
    private AnimationCurve _scaleAnimation;

    [SerializeField]
    private float _scaleAnimationDuration;

    private Vector3 _startScale;

    void Start()
    {
        _startScale = transform.localScale;
    }


    public void PlayScaleAnimation()
    {
        StartCoroutine(Co_ScaleAnimation());
    }


    private IEnumerator Co_ScaleAnimation()
    {
        yield break;
    }
}
