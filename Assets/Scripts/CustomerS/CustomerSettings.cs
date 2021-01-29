using UnityEngine;

[CreateAssetMenu(menuName = "Customers/Customer Settings")]
public sealed class CustomerSettings : ScriptableObject
{
    public float verticalSpawnOffset = -1;
    public AnimationCurve moveCurve = null;
    public float moveMinDuration = .5f, moveMaxDuration = 1;

    public float itemGrabDuration = 1;
    public AnimationCurve grabCurve = null;
    public AnimationCurve shrinkCurve = null;
}
