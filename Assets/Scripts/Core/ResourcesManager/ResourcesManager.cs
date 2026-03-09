using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Core.ResourcesManager
{
    public class ResourcesManager : IResourcesManager
    {
        private const string PoolName = "Pool";
        private const string PoolRootName = "PoolRoot";

        private readonly Dictionary<string, Stack<GameObject>> _pool = new();
        private Transform _poolRoot;

        public bool IsInitialized { get; private set; } = false;

        public async UniTask InitializeAsync(CancellationToken _)
        {
            var poolGameObject = new GameObject(PoolName);
            var poolRootGameObject = new GameObject(PoolRootName);
            poolRootGameObject.SetActive(false);

            Transform poolRootTransform = poolRootGameObject.transform;
            poolRootTransform.SetParent(poolGameObject.transform);

            _poolRoot = poolRootTransform;
            Object.DontDestroyOnLoad(poolGameObject);

            await UniTask.CompletedTask;

            IsInitialized = true;
        }

        public void PrepareGameObjects(string resourceKey, int poolSize)
        {
            if (_pool.ContainsKey(resourceKey) == false)
            {
                _pool.Add(resourceKey, new Stack<GameObject>());
            }

            for (int i = 0; i < poolSize; i++)
            {
                GameObject instance = InstantiateGameObject(resourceKey, _poolRoot);
                _pool[resourceKey].Push(instance);
            }
        }

        public async UniTask PrepareGameObjectsAsync(
            string resourceKey,
            int poolSize,
            CancellationToken? cancellationToken = null)
        {
            if (_pool.ContainsKey(resourceKey) == false)
            {
                _pool.Add(resourceKey, new Stack<GameObject>());
            }

            for (int i = 0; i < poolSize; i++)
            {
                GameObject instance = await InstantiateGameObjectAsync(resourceKey, _poolRoot, cancellationToken);
                _pool[resourceKey].Push(instance);
            }
        }

        public GameObject Instantiate(string resourceKey, Transform parent = null)
        {
            GameObject result;

            if (_pool.ContainsKey(resourceKey) == true && _pool[resourceKey].Count > 0)
            {
                result = _pool[resourceKey].Pop();
                result.transform.SetParent(parent);
            }
            else
            {
                result = InstantiateGameObject(resourceKey, parent);
            }

            return result;
        }

        public async UniTask<GameObject> InstantiateAsync(
            string resourceKey,
            Transform parent = null,
            CancellationToken? cancellationToken = null)
        {
            GameObject result;

            if (_pool.ContainsKey(resourceKey) == true && _pool[resourceKey].Count > 0)
            {
                result = _pool[resourceKey].Pop();

                if (parent != null)
                {
                    result.transform.SetParent(parent);
                }
            }
            else
            {
                result = await InstantiateGameObjectAsync(resourceKey, parent, cancellationToken);
            }

            return result;
        }

        public void ReleaseGameObject(string resourceKey, GameObject gameObject)
        {
            if (_pool.TryGetValue(resourceKey, out Stack<GameObject> pool) == true)
            {
                pool.Push(gameObject);
            }
            else
            {
                pool = new Stack<GameObject>();
                pool.Push(gameObject);
                _pool.Add(resourceKey, pool);
            }

            gameObject.transform.SetParent(_poolRoot);
        }

        public T LoadAsset<T>(string resourceKey)
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(resourceKey);
            handle.WaitForCompletion();

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                var errorMessage = handle.OperationException.Message ?? string.Empty;
                throw new AssetLoadingException($"Status: {handle.Status}. ErrorMessage: {errorMessage}");
            }

            if (handle.Result == null)
            {
                throw new AssetLoadingException($"Failed to load asset with key: {resourceKey}");
            }

            return handle.Result;
        }

        public async UniTask<T> LoadAssetAsync<T>(string resourceKey)
        {
            T result = await Addressables.LoadAssetAsync<T>(resourceKey).ToUniTask();

            if (result == null)
            {
                throw new AssetLoadingException($"Failed to load asset with key: {resourceKey}");
            }

            return result;
        }

        private GameObject InstantiateGameObject(string resourceKey, Transform parent = null)
        {
            AsyncOperationHandle<GameObject> handle = parent == null
                ? Addressables.InstantiateAsync(resourceKey)
                : Addressables.InstantiateAsync(resourceKey, parent);

            handle.WaitForCompletion();

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                var errorMessage = handle.OperationException.Message ?? string.Empty;
                throw new AssetLoadingException($"Status: {handle.Status}. ErrorMessage: {errorMessage}");
            }

            if (handle.Result == null)
            {
                throw new AssetLoadingException($"Failed to instantiate GameObject with key: {resourceKey}");
            }

            return handle.Result;
        }

        private async UniTask<GameObject> InstantiateGameObjectAsync(
            string resourceKey,
            Transform parent = null,
            CancellationToken? cancellationToken = null)
        {
            UniTask<GameObject> handle;

            if (cancellationToken != null)
            {
                if (parent == null)
                {
                    handle = Addressables
                        .InstantiateAsync(resourceKey)
                        .ToUniTask(cancellationToken: cancellationToken.Value);
                }
                else
                {
                    handle = Addressables
                        .InstantiateAsync(resourceKey, parent)
                        .ToUniTask(cancellationToken: cancellationToken.Value);
                }
            }
            else
            {
                handle = parent == null
                    ? Addressables.InstantiateAsync(resourceKey).ToUniTask()
                    : Addressables.InstantiateAsync(resourceKey, parent).ToUniTask();
            }

            GameObject result = await handle;

            if (result == null)
            {
                throw new AssetLoadingException($"Failed to instantiate GameObject with key: {resourceKey}");
            }

            return result;
        }
    }
}