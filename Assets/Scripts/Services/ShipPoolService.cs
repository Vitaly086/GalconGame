using System;
using AssetManagement;
using Models;
using Presenters;
using UnityEngine;

namespace DefaultNamespace.Services
{
    public class ShipPoolService : MonoBehaviour, IShipPoolService
    {
        [SerializeField] private Transform _root ;
        [SerializeField] private int _poolCount = 20;

        private MonoBehaviourPool<ShipPresenter> _pool;

        public void Initialize(AssetProvider assetProvider)
        {
            var shipConfig = assetProvider.Load<ShipConfig>(AssetPath.SHIP_CONFIG);

            _pool = new MonoBehaviourPool<ShipPresenter>(shipConfig.ShipPrefab, _root, _poolCount);
        }

        public MonoBehaviourPool<ShipPresenter> GetPool()
        {
            return _pool;
        }
    }
}