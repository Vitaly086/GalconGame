using System;
using System.Collections.Generic;
using AssetManagement;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class PlanetSpawnerHandler : IDisposable
{

    private readonly GameModel _gameModel;
    private readonly Camera _camera;
    private readonly GamePresenter _gamePresenter;
    private readonly List<PlanetPresenter> _planets = new List<PlanetPresenter>();
    
    private readonly Vector2 _maxScreenSize;
    private readonly Vector2 _minScreenSize;


    public PlanetSpawnerHandler(GameModel gameModel, Camera camera, GamePresenter gamePresenter)
    {
        _gameModel = gameModel;
        _camera = camera;
        _gamePresenter = gamePresenter;
        
        _maxScreenSize = _camera.ViewportToWorldPoint(new Vector2(0.9f, 0.9f));
        _minScreenSize = _camera.ViewportToWorldPoint(new Vector2(0.1f, 0.1f));  
        
        SpawnPlanets();
    }

    public IReadOnlyList<PlanetPresenter> GetPlanets()
    {
        return _planets;
    }
    
     private void SpawnPlanets()
    {
        var assetProvider = ServiceLocator.Instance.GetSingle<IAssetProvider>();
        var planetConfig = assetProvider.Load<PlanetConfig>(AssetPath.PLANET_CONFIG);

        for (int i = 0; i < _gameModel.PlanetCount; i++)
        {
            var currentPlanet = Object.Instantiate(planetConfig.PlanetPrefab, _gamePresenter.transform);
            currentPlanet.Initialize(planetConfig);
            currentPlanet.name = "Planet_" + i;

            currentPlanet.transform.localPosition = GetAvailablePosition(currentPlanet);

            currentPlanet.gameObject.SetActive(false);


            _planets.Add(currentPlanet);
        }
    }
     

     private Vector2 GetAvailablePosition(PlanetPresenter currentPlanet)
    {
        if (IsFirstPlanet())
        {
            return GetRandomPosition();
        }

        var position = Vector2.zero;
        var canSpawnInPosition = false;

        var tick = 0;

        while (canSpawnInPosition == false) //todo  
        {
            tick++;
            if (tick == 1000000)
            {
                throw new Exception("choose fewer planets");
            }

            position = GetRandomPosition();

            canSpawnInPosition = CanSpawn(currentPlanet, position);
        }

        return position;
    }

    private bool CanSpawn(PlanetPresenter currentPlanet, Vector2 position)
    {
        foreach (var neighbour in _planets)
        {
            var distance = Vector2.Distance(position, neighbour.transform.position);

            var sumRadius = currentPlanet.Radius + neighbour.Radius;

            distance -= sumRadius;

            if (distance < sumRadius)
            {
                return false;
            }
        }

        return true;
    }

    private Vector2 GetRandomPosition()
    {
                  
        var x = Random.Range(_minScreenSize.x, _maxScreenSize.x);
        var y = Random.Range(_minScreenSize.y, _maxScreenSize.y);

        return new Vector2(x, y);
    }

    private bool IsFirstPlanet()
    {
        return _planets.Count == 0;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}