using System;
using System.Collections.Generic;
using DefaultNamespace.Services;
using Enemy_Bot;
using Models;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class GamePresenter : MonoBehaviour
{
    public event Action GameFinished;
    public event Action PlayerAttackCompleted;
    public event Action EnemyAttackCompleted;
    public event Action GameStarted;

    [SerializeField] private Camera _camera;
    [SerializeField] private GameView _gameView;
    [SerializeField] private PlayerInputHandler _playerInput;

    private EnemyController _enemyController;
    private readonly List<PlanetPresenter> _playersPlanets = new List<PlanetPresenter>();
    private readonly List<PlanetPresenter> _enemyPlanets = new List<PlanetPresenter>();
    private List<PlanetPresenter> _availablePlanets = new List<PlanetPresenter>();
    private GameModel _gameModel;
    private PlanetSpawnerHandler _planetSpawner;
    private bool _isPlayerAndEnemyCreated;


    public void Initialize(GameModel gameModel)
    {
        _gameView.GameStarted += OnStartGame;
        _gameModel = gameModel;
        _playerInput.PlayerAndTargetSelected += OnPlayerAndTargetSelected;


        Assert.IsTrue(_gameModel.PlanetCount > 2, "Please, change planet count");
    }

    private void OnStartGame()
    {
        _isPlayerAndEnemyCreated = false;
        ClearGameBoard();

        SpawnPlanets();

        foreach (var availablePlanet in _availablePlanets)
        {
            availablePlanet.PlanetCaptured += OnPlanetCaptured;


            availablePlanet.gameObject.SetActive(true);
        }

        ChooseStartPlayerPlanet();
        CreateEnemy();
        _isPlayerAndEnemyCreated = true;
    }

    private void ChooseStartPlayerPlanet()
    {
        var playerPlanet = Random.Range(0, _availablePlanets.Count);
        _availablePlanets[playerPlanet].ChangeState(PlanetState.Player);
    }

    private void CreateEnemy()
    {
        var enemyPlanet = Random.Range(0, _availablePlanets.Count);
        _availablePlanets[enemyPlanet].ChangeState(PlanetState.Enemy);

        _enemyController = new EnemyController(this, _availablePlanets, _enemyPlanets, _playersPlanets);
        _enemyController.EnemyAndTargetSelected += OnEnemyAndTargetSelected;
    }

    private void OnPlanetCaptured(PlanetPresenter capturedPlanet)
    {
        _availablePlanets.Remove(capturedPlanet);
        _enemyPlanets.Remove(capturedPlanet);
        _playersPlanets.Remove(capturedPlanet);

        switch (capturedPlanet.State)
        {
            case PlanetState.Player:
                _playersPlanets.Add(capturedPlanet);
                break;
            case PlanetState.Enemy:
                _enemyPlanets.Add(capturedPlanet);
                break;
        }

        if (IsGameFinished())
        {
            ServiceLocator.Instance.GetSingle<IShipPoolService>().GetPool().ReleaseAll();
            transform.position = new Vector3(transform.position.x, transform.position.y,
                _gameView.transform.position.z - transform.position.z);
            GameFinished?.Invoke();
        }
    }

    private void SpawnPlanets()
    {
        _planetSpawner = new PlanetSpawnerHandler(_gameModel, _camera, this);
        _availablePlanets = new List<PlanetPresenter>(_planetSpawner.GetPlanets());
    }

    private void OnPlayerAndTargetSelected(PlanetPresenter playerPlanet, PlanetPresenter targetPlanet)
    {
        playerPlanet.TryCapture(targetPlanet);
        PlayerAttackCompleted?.Invoke();
    }

    private void OnEnemyAndTargetSelected(PlanetPresenter enemyPlanet, PlanetPresenter targetPlanet)
    {
        enemyPlanet.TryCapture(targetPlanet);
        EnemyAttackCompleted?.Invoke();
    }

    private void ClearGameBoard()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y,
            _gameView.transform.position.z);


        foreach (var playerPlanet in _playersPlanets)
        {
            playerPlanet.StopAllCoroutines();
            Destroy(playerPlanet.gameObject);
        }

        _playersPlanets.Clear();

        foreach (var enemyPlanet in _enemyPlanets)
        {
            enemyPlanet.StopAllCoroutines();
            Destroy(enemyPlanet.gameObject);
        }

        _enemyPlanets.Clear();

        foreach (var availablePlanet in _availablePlanets)
        {
            Destroy(availablePlanet.gameObject);
        }

        _planetSpawner?.Dispose();
        GameStarted?.Invoke();
    }

    private bool IsGameFinished()
    {
        if (_enemyPlanets.Count == 0 && _isPlayerAndEnemyCreated)
        {
            _gameView.ShowWinHUD();

            return true;
        }

        if (_playersPlanets.Count == 0 && _isPlayerAndEnemyCreated)
        {
            _gameView.ShowLoseHUD();

            return true;
        }

        return false;
    }

    private void OnDestroy()
    {
        _playerInput.PlayerAndTargetSelected -= OnPlayerAndTargetSelected;
        _enemyController.EnemyAndTargetSelected -= OnEnemyAndTargetSelected;


        foreach (var planet in _playersPlanets)
        {
            planet.PlanetCaptured -= OnPlanetCaptured;
        }

        foreach (var planet in _enemyPlanets)
        {
            planet.PlanetCaptured -= OnPlanetCaptured;
        }

        _enemyController.Dispose();
    }
}