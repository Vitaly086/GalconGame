using System;
using System.Collections;
using Models;
using Presenters;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlanetPresenter : MonoBehaviour
{
    private const float ONE_SECOND = 1f;

    public event Action<PlanetPresenter> PlanetCaptured;

    private event Action StateChanged;

    public PlanetState State
    {
        get => _state;
        private set
        {
            _state = value;
            StateChanged?.Invoke();
        }
    }

    public float Radius { get; private set; }


    [SerializeField] private PlanetView _planetView;

    private PlanetState _state;
    private PlanetModel _planetModel;
    private int _addShipPerSeconds;
    private IEnumerator _increaseCoroutine;
    private ShipSpawnerHandler _shipSpawnerHandler;

    public void Initialize(PlanetConfig planetConfig)
    {
        SetDefaultState();
        _addShipPerSeconds = planetConfig.AddShipPerSeconds;
        RandomScale();

        _planetModel = new PlanetModel(planetConfig);
        _planetModel.CountShipChanged += OnCountChanged;
        StateChanged += RefreshPresenter;

        _shipSpawnerHandler = new ShipSpawnerHandler(this);

        _planetView.Text.text = _planetModel.CountShips.ToString();
    }

    public int GetCount()
    {
        return _planetModel.CountShips;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent(out ShipPresenter ship))
        {
            if (Equals(ship.Target.gameObject, gameObject))
            {
                UpdateCount(ship);
            }
        }
    }

    public void ChangeState(PlanetState planetState)
    {
        StopAllCoroutines();
        State = planetState;
        PlanetCaptured?.Invoke(this);
    }

    public void TryCapture(PlanetPresenter target)
    {
        var halfShips = _planetModel.TakeHalfShips();
        _shipSpawnerHandler.StartCapture(halfShips, target);
    }

    public void IsSelect(bool isSelected)
    {
        _planetView.ChangeSelectedPlanetColor(isSelected);
    }

    private void RefreshPresenter()
    {
        _planetView.UpdateColor(State);

        _increaseCoroutine = IncreaseCountShip();

        if (State != PlanetState.Neutral)
        {
            StartCoroutine(_increaseCoroutine);
        }
    }

    private void RandomScale()
    {
        var scaleSize = Random.Range(_planetView.MinScale, _planetView.MaxScale);
        transform.localScale = new Vector2(scaleSize, scaleSize);
        var bounds = _planetView.Sprite.bounds;
        Radius = Math.Max(bounds.extents.x, bounds.extents.y);
    }

    private IEnumerator IncreaseCountShip()
    {
        var time = 0f;
        var delayAddShip = ONE_SECOND / _addShipPerSeconds;

        while (State != PlanetState.Neutral)
        {
            time += Time.deltaTime;

            if (time >= delayAddShip)
            {
                _planetModel.AddOneShip();
                time = 0;
            }

            yield return null;
        }
    }

    private void UpdateCount(ShipPresenter ship)
    {
        if (ship.ParentPlanet.State == State)
        {
            _planetModel.AddCount(ship.Damage);
        }
        else
        {
            _planetModel.SubtractCount(ship.Damage);
        }

        if (HasPlanetCaptured(ship))
        {
            ChangeState(ship.ParentPlanet.State);
        }
    }

    private bool HasPlanetCaptured(ShipPresenter ship) =>
        _planetModel.CountShips <= 0 && ship.ParentPlanet.State != State;

    private void OnCountChanged() => _planetView.Text.text = _planetModel.CountShips.ToString();

    private void SetDefaultState() => State = PlanetState.Neutral;

    private void OnDestroy()
    {
        _planetModel.CountShipChanged -= OnCountChanged;
        StateChanged -= RefreshPresenter;
    }
}