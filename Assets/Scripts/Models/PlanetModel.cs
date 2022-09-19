using System;
using Random = UnityEngine.Random;

public class PlanetModel
{
    public event Action CountShipChanged;

    public int CountShips
    {
        get => _countShips;
        private set
        {
            _countShips = value;
            CountShipChanged?.Invoke();
        }
    }

    private int _countShips;

    public PlanetModel(PlanetConfig planetConfig)
    {
        CountShips = Random.Range(planetConfig.MinCountShips, planetConfig.MaxCountShips);
    }

    public int TakeHalfShips()
    {
        var count = CountShips /= 2;
        return count;
    }

    public void AddOneShip()
    {
        ++CountShips;
    }

    public void AddCount(int value)
    {
        CountShips += value;
    }

    public void SubtractCount(int value)
    {
        CountShips -= value;
    }
}