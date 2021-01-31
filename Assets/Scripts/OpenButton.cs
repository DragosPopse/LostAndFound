using UnityEngine;
using UnityEngine.Events;

[RequireComponent(
    typeof(LostItem), 
    typeof(LostItemSorter), 
    typeof(LostItemSorter))]
[RequireComponent(typeof(SpriteRenderer))]
public sealed class OpenButton : MonoBehaviour
{
    [SerializeField] private UnityEvent _mouseDownEvent = null;
    [SerializeField] private Sprite _upSprite = null, _downSprite = null;

    private SpriteRenderer _renderer = null;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        _renderer.sprite = _downSprite;

        if (!enabled)
            return;

        _mouseDownEvent.Invoke();

        GetComponent<LostItem>().enabled = true;
        GetComponent<LostItemSorter>().enabled = true;
        enabled = false;
    }

    private void OnMouseUp()
    {
        _renderer.sprite = _upSprite;
    }
}
