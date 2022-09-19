using System;
using System.Collections;
using DefaultNamespace.Services;
using Presenters;

public class ShipSpawnerHandler
{
    private readonly PlanetPresenter _parentPlanet;
    private readonly MonoBehaviourPool<ShipPresenter> _shipPool;
    
    
    public ShipSpawnerHandler(PlanetPresenter parentPlanet)
    {
        _parentPlanet = parentPlanet;
        _shipPool = ServiceLocator.Instance.GetSingle<IShipPoolService>().GetPool();
    }
    
    public void StartCapture(int halfShips, PlanetPresenter target)
    {
        _parentPlanet.StartCoroutine(SpawnShip(halfShips, target));
    }

    private IEnumerator SpawnShip(int halfShips, PlanetPresenter target)
    {
        for (int i = 0; i < halfShips; i++)
        {
            var ship = _shipPool.Take();
            ship.transform.position = _parentPlanet.transform.position;
            ship.AttackCompleted += OnAttackCompleted;

            ship.StartShipAttackTarget(_parentPlanet, target);
            yield return null;
        }
    }

    private void OnAttackCompleted(ShipPresenter ship)
    {
        ship.AttackCompleted -= OnAttackCompleted;

        _shipPool.Release(ship);
    }
    
}