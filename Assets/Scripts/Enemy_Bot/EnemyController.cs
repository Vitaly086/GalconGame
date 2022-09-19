using System;
using System.Collections.Generic;
using System.Linq;
using Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy_Bot
{
    public class EnemyController : IDisposable, ITickable

    {
        private const int COOLDOWN_DELAY = 3;

        public event Action<PlanetPresenter, PlanetPresenter> EnemyAndTargetSelected;

        private readonly GamePresenter _gamePresenter;
        private readonly ITickableService _tickableService;
        private IReadOnlyList<PlanetPresenter> _enemyPlanets;
        private IReadOnlyList<PlanetPresenter> _playersPlanets;
        private IReadOnlyList<PlanetPresenter> _availablePlanets;
        private PlanetPresenter _currentPlanet;
        private PlanetPresenter _targetPlanet;
        private float _cooldown;


        public EnemyController(GamePresenter gamePresenter, List<PlanetPresenter> availablePlanets,
            List<PlanetPresenter> enemyPlanets, List<PlanetPresenter> playersPlanets)
        {
            _gamePresenter = gamePresenter;
            _availablePlanets = availablePlanets;
            _enemyPlanets = enemyPlanets;
            _playersPlanets = playersPlanets;

            _gamePresenter.EnemyAttackCompleted += OnEnemyAttackCompleted;
            _gamePresenter.GameFinished += OnGameFinished;
            _tickableService = ServiceLocator.Instance.GetSingle<ITickableService>();
            _tickableService.StartUpdate(this);
        }


        public void Tick()
        {
            ChooseCurrentPlanet();

            if (CanCapture() && IsCooldown() && IsNotEnemyState())
            {
                EnemyAndTargetSelected?.Invoke(_currentPlanet, _targetPlanet);
            }
        }

        private void OnEnemyAttackCompleted()
        {
            _currentPlanet = null;
            _targetPlanet = null;
        }

        private void OnGameFinished()
        {
            _tickableService.StopUpdate(this);
        }

        private bool IsNotEnemyState()
        {
            return _targetPlanet.State != PlanetState.Enemy;
        }

        private bool IsCooldown()
        {
            _cooldown += Time.deltaTime;

            if (_cooldown >= COOLDOWN_DELAY)
            {
                _cooldown = 0;
                return true;
            }

            return false;
        }

        private void ChooseCurrentPlanet()
        {
            if (_enemyPlanets.Count != 0)
            {
                var enemyRandomPlanet = Random.Range(0, _enemyPlanets.Count);
                _currentPlanet = _enemyPlanets[enemyRandomPlanet];
            }
        }

        private bool CanCapture()
        {
            ChooseTargetPlanet();

            if (_targetPlanet.State == PlanetState.Enemy)
            {
                ChooseTargetPlanet();
            }

            return _targetPlanet != null;
        }

        private void ChooseTargetPlanet()
        {
            var commonPlanets = new List<PlanetPresenter>(_availablePlanets.Concat(_playersPlanets));

            var randomPlanet = Random.Range(0, commonPlanets.Count);

            for (var index = 0; index < commonPlanets.Count; index++)
            {
                var planet = commonPlanets[index];

                if (_currentPlanet.GetCount() >= planet.GetCount())
                {
                    _targetPlanet = planet;
                    return;
                }
            }

            _targetPlanet = commonPlanets[randomPlanet];
        }


        public void Dispose()
        {
            _gamePresenter.EnemyAttackCompleted -= OnEnemyAttackCompleted;
            _tickableService.StopUpdate(this);
        }
    }
}