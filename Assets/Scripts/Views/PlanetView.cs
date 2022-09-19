using Models;
using TMPro;
using UnityEngine;

public class PlanetView : MonoBehaviour
{
    [field: SerializeField] public SpriteRenderer Sprite { get; private set; }
    [field: SerializeField] public float MaxScale { get; private set; }
    [field: SerializeField] public float MinScale { get; private set; }
    [field: SerializeField] public TextMeshProUGUI Text { get; private set; }

    [SerializeField] private Color _playerColor;
    [SerializeField] private Color _playerSelectedColor;
    [SerializeField] private Color _enemyColor;
    [SerializeField] private Color _neutralColor;


    public void UpdateColor(PlanetState state)
    {
        switch (state)
        {
            case PlanetState.Player:
                Sprite.color = _playerColor;
                break;

            case PlanetState.Enemy:
                Sprite.color = _enemyColor;
                break;

            case PlanetState.Neutral:
                Sprite.color = _neutralColor;
                break;
        }
    }

    public void ChangeSelectedPlanetColor(bool isSelected)
    {
        Sprite.color = isSelected ? _playerSelectedColor : _playerColor;
    }
}