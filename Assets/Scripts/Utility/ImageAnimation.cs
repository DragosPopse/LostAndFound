using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private float speed = 1;

    private Image _image = null;
    private float _current = 0;

    private void Awake() => 
        _image = GetComponent<Image>();

    private void Update()
    {
        _current += Time.deltaTime * speed;
        _current %= _sprites.Length;

        int index = Mathf.FloorToInt(_current);
        _image.sprite = _sprites[index];
    }
}
