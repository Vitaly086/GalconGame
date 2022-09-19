using UnityEngine;

[CreateAssetMenu(fileName = "PlanetConfig", menuName = "ScriptableObjects/PlanetConfig")]

public class PlanetConfig : ScriptableObject
{
    [field: SerializeField] public int AddShipPerSeconds { get; private set; }
    [field: SerializeField] public PlanetPresenter PlanetPrefab { get; private set; }
    [field: SerializeField] public int MaxCountShips { get; private set; }
    [field: SerializeField] public int MinCountShips { get; private set; }
}