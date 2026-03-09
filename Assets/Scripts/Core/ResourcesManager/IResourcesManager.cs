using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.ResourcesManager
{
    public interface IResourcesManager
    {
        void PrepareGameObjects(string resourceKey, int poolSize);
        UniTask PrepareGameObjectsAsync(string resourceKey, int poolSize, CancellationToken? cancellationToken = null);

        GameObject Instantiate(string resourceKey, Transform parent = null);

        UniTask<GameObject> InstantiateAsync(
            string resourceKey,
            Transform parent = null,
            CancellationToken? cancellationToken = null);

        void ReleaseGameObject(string resourceKey, GameObject gameObject);
        T LoadAsset<T>(string resourceKey);
        UniTask<T> LoadAssetAsync<T>(string resourceKey);
    }
}