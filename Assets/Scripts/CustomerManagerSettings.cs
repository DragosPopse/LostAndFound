using UnityEngine;

[CreateAssetMenu(menuName = "Customers/Manager Settings")]
public sealed class CustomerManagerSettings : ScriptableObject
{
    public bool debug = false;
    public float minNewCustomerInterval = 5, maxNewCustomerInterval = 10;
}
