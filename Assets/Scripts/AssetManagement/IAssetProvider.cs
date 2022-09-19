using UnityEngine;

namespace AssetManagement
{
    public interface IAssetProvider : IService
    {
        T Load<T>(string path) where T : Object;
    }
}