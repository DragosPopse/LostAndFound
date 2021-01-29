using UnityEngine;

[CreateAssetMenu(menuName = "Customers/Customer Settings")]
public sealed class CustomerSettings : ScriptableObject
{
    public float verticalSpawnOffset = -1;
    public AnimationCurve spawnCurve = null;
    public float spawnMinDuration = 1, spawnMaxDuration = 2;
}
