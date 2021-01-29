using UnityEngine;
using utility;
using Random = System.Random;

public sealed class GameManager : Singleton<GameManager>
{
    public Random Random => 
        _random;

    [SerializeField, Tooltip("Leave empty to generate random seed")] 
    private string _seed = "";

    private Random _random = null;

    protected override void Awake()
    {
        base.Awake();
        var chosenSeed = _seed == "" ? 
            System.DateTime.Now.GetHashCode() : _seed.GetHashCode();
        _random = new Random(chosenSeed);
    }
}
