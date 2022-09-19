using Presenters;

namespace DefaultNamespace.Services
{
    public interface IShipPoolService: IService
    {
        MonoBehaviourPool<ShipPresenter> GetPool();
    }
}