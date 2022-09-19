using System;
using AssetManagement;
using Models;
using UnityEngine;
using Views;

namespace Presenters
{
    public class ShipPresenter : MonoBehaviour

    {
        public event Action<ShipPresenter> AttackCompleted;
        public int Damage { get; set; }
        public PlanetPresenter ParentPlanet { get; private set; }
        public PlanetPresenter Target { get; private set; }

        [SerializeField] private ShipView _shipView;

        private ShipModel _shipModel;
        private float _releaseTime;


        private void Awake()
        {
            var assetProvider = ServiceLocator.Instance.GetSingle<IAssetProvider>();
            var shipConfig = assetProvider.Load<ShipConfig>(AssetPath.SHIP_CONFIG);
            _shipModel = new ShipModel(shipConfig);
            Damage = _shipModel.Damage;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.TryGetComponent(out PlanetPresenter planet))
            {
                if (planet.Equals(Target))
                {
                    AttackCompleted?.Invoke(this);
                }
            }
        }

        private void OnEnable()
        {
            _releaseTime = 0f;
        }

        private void Update()
        {
            MoveToTarget();
            ReleaseShipWhenFailAttack();
        }

        public void StartShipAttackTarget(PlanetPresenter parentPlanet, PlanetPresenter target)
        {
            Target = target;
            ParentPlanet = parentPlanet;

            UpdateView(parentPlanet);
        }

        private void ReleaseShipWhenFailAttack()
        {
            _releaseTime += Time.deltaTime;
            if (_releaseTime >= 30)
            {
                AttackCompleted?.Invoke(this);
            }
        }

        private void UpdateView(PlanetPresenter planetPresenter)
        {
            _shipView.UpdateColor(planetPresenter.State);
        }

        private void MoveToTarget()
        {
            if (Target != null)
            {
                var targetPosition = Target.transform.position;
                var position = transform.position;
                var rotation = transform.rotation;

                rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y,
                    Mathf.Atan2(targetPosition.y - position.y, targetPosition.x - position.x) * Mathf.Rad2Deg - 90);
                transform.rotation = rotation;


                var nextPosition = Vector3.MoveTowards(position, targetPosition,
                    _shipModel.Speed * Time.deltaTime);
                transform.position = nextPosition;
            }
        }
    }
}