using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkPanel : utility.Singleton<BlinkPanel>
{
    private Image _image;

    private Color _color;

    private void Start()
    {
        _image = GetComponent<Image>();
        _color = _image.color;
    }


    public IEnumerator LerpAlpha(AnimationCurve curve, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            float alpha = curve.Evaluate(progress);
            _image.color = new Color(_color.r, _color.g, _color.b, alpha / 255f);

            yield return null;
        }
    }
}
