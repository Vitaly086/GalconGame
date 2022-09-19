using Models;
using UnityEngine;

namespace Views
{
    public class ShipView : MonoBehaviour

    {
        [field: SerializeField] public SpriteRenderer Sprite { get; private set; }
        
        [SerializeField] private Color _playerColor;
        [SerializeField] private Color _enemyColor;


        public void UpdateColor(PlanetState parentPlanet)
        {
            switch (parentPlanet)
            {
                case PlanetState.Enemy:
                    Sprite.color = _enemyColor;
                    break;

                case PlanetState.Player:
                    Sprite.color = _playerColor;
                    break;
            }
        }
    }
}