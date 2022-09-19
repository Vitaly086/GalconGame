using AssetManagement;
using DefaultNamespace.Services;
using Enemy_Bot;
using UnityEngine;


public class GameStarter : MonoBehaviour
{
    [SerializeField] private GamePresenter _gamePresenter;
    [SerializeField] private GameConfig _gameConfig;
    [SerializeField] private ShipPoolService _shipPoolService;
    [SerializeField] private TickableService _tickableService;

    private void Awake()
    {
        RegisterServices();

        var gameModel = new GameModel(_gameConfig);
        _gamePresenter.Initialize(gameModel);
    }

    private void RegisterServices()
    {
        var serviceLocator = new ServiceLocator();

        var assetProvide = new AssetProvider();
        serviceLocator.RegisterSingle<IAssetProvider>(assetProvide);
        
        _shipPoolService.Initialize(assetProvide);
        serviceLocator.RegisterSingle<IShipPoolService>(_shipPoolService);
        
        serviceLocator.RegisterSingle<ITickableService>(_tickableService);
    }
}