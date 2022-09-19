namespace Enemy_Bot
{
    public interface ITickableService : IService
    {
        void StartUpdate<T>(T tickable) where T : ITickable;
        void StopUpdate<T>(T tickable) where T : ITickable;
    }

}