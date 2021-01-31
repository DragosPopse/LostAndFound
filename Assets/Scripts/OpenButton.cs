using UnityEngine;
using UnityEngine.Events;

public sealed class OpenButton : MonoBehaviour
{
    [SerializeField] private UnityEvent _mouseDownEvent = null;

    private void OnMouseDown()
    {
        if (!enabled)
            return;

        _mouseDownEvent.Invoke();

        GetComponent<LostItem>().enabled = true;
        GetComponent<LostItemSorter>().enabled = true;
        enabled = false;
    }
}
