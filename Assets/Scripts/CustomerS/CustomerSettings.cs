using UnityEngine;

[CreateAssetMenu(menuName = "Customers/Customer Settings")]
public sealed class CustomerSettings : ScriptableObject
{
    // Movement / Spawning.
    public float verticalSpawnOffset = -1;
    public AnimationCurve moveCurve = null;
    public float moveMinDuration = .5f, moveMaxDuration = 1;

    // Item grabbing.
    public float itemGrabDuration = 1;
    public AnimationCurve grabCurve = null;
    public AnimationCurve shrinkCurve = null;
    public float grabVerticalOffset = 1;

    // Waiting / patience.
    public float minWaitDuration = 5, maxWaitDuration = 10;
    public Color angryColor = Color.red;
    public AnimationCurve angryColorCurve = null;

    // Stealing.
    [Range(0, 1)] public float stealChance = .1f;
    public float stealInterval = 2;
}
