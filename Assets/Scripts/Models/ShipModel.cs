using Models;

namespace Presenters
{
    public class ShipModel
    {
        public float Speed { get; }
        public int Damage { get; }

        public ShipModel(ShipConfig config)
        {
            Damage = config.Damage;
            Speed = config.Speed;
        }
    }
}