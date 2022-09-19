using System.Collections.Generic;
using UnityEngine;

namespace Enemy_Bot
{
    public class TickableService : MonoBehaviour, ITickableService
    {
        private readonly List<ITickable> _updateList = new List<ITickable>();

        public void StartUpdate<T>(T tickable) where T : ITickable
        {
            _updateList.Add(tickable);
        }

        public void StopUpdate<T>(T tickable) where T : ITickable
        {
            if (_updateList.Contains(tickable))
            {
                _updateList.Remove(tickable);
            }
        }

        private void Update()
        {
            for (var i = 0; i < _updateList.Count; i++)
            {
                _updateList[i].Tick();
            }
        }
    }
}