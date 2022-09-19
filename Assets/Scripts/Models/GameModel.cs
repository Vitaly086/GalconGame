public class GameModel
{
    public int PlanetCount { get; }

    public GameModel(GameConfig gameConfig)
    {
        PlanetCount = gameConfig.PlanetCount;
    }
}