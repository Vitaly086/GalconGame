using Presenters;
using UnityEngine;

namespace Models
{
    [CreateAssetMenu(fileName = "ShipConfig", menuName = "ScriptableObjects/ShipConfig")]
    public class ShipConfig : ScriptableObject
    {
        [field: SerializeField] public ShipPresenter ShipPrefab { get; private set; }

        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
    }
}