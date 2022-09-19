using System;
using Models;
using UnityEngine;
using UnityEngine.EventSystems;


public class PlayerInputHandler : MonoBehaviour, IPointerClickHandler
{
    public event Action<PlanetPresenter, PlanetPresenter> PlayerAndTargetSelected;

    [SerializeField] private GamePresenter _gamePresenter;
    [SerializeField] private Camera _camera;

    private readonly RaycastHit2D[] _rayHit = new RaycastHit2D[1];
    private PlanetPresenter _currentPlayerPlanet;
    private PlanetPresenter _previousPlayerPlanet;
    private PlanetPresenter _target;

    private void Start()
    {
        _gamePresenter.PlayerAttackCompleted += ClearSelectedPlanets;
        _gamePresenter.GameStarted += ClearSelectedPlanets;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var point = _camera.ScreenToWorldPoint(Input.mousePosition);

        Physics2D.RaycastNonAlloc(point, Vector3.forward, _rayHit);

        if (_rayHit[0].collider.TryGetComponent(out PlanetPresenter planet))
        {
            if (planet.State == PlanetState.Player)
            {
                _previousPlayerPlanet?.IsSelect(false);

                _currentPlayerPlanet = planet;
                _previousPlayerPlanet = _currentPlayerPlanet;

                _target = null;
                _currentPlayerPlanet.IsSelect(true);
            }
            else
            {
                _target = planet;
            }

            if (_currentPlayerPlanet != null && _target != null)
            {
                PlayerAndTargetSelected?.Invoke(arg1: _currentPlayerPlanet, arg2: _target);
            }
        }
    }

    private void ClearSelectedPlanets()
    {
        _currentPlayerPlanet?.IsSelect(false);
        _currentPlayerPlanet = null;
        _previousPlayerPlanet = null;
        _target = null;
    }

    private void OnDestroy()
    {
        _gamePresenter.PlayerAttackCompleted -= ClearSelectedPlanets;
        _gamePresenter.GameStarted -= ClearSelectedPlanets;
    }
}