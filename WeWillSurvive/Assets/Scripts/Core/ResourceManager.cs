using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace WeWillSurvive.Core
{
    public class ResourceManager : IService
    {
        private Dictionary<string, Object> _loadedAssets = new Dictionary<string, Object>();

        public UniTask InitializeAsync()
        {
            return UniTask.CompletedTask;
        }

        public async UniTask<T> LoadAssetAsync<T>(string key = default) where T : Object
        {
            if (string.IsNullOrEmpty(key))
            {
                key = typeof(T).Name;
            }

            if (_loadedAssets.TryGetValue(key, out var chaced) && chaced is T typed)
            {
                Debug.Log($"이미 로드된 리소스: {key}");
                return typed;
            }

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _loadedAssets[key] = handle.Result;
                Debug.Log($"로드 성공: {key}");
                return handle.Result;
            }
            else
            {
                Debug.LogError($"로드 실패: {key}");
            }

            return null;
        }

        public void UnloadAsset(string key)
        {
            if (_loadedAssets.ContainsKey(key))
            {
                Addressables.Release(_loadedAssets[key]);
                _loadedAssets.Remove(key);
                Debug.Log($"언로드 완료: {key}");
            }
        }

        public async UniTask<GameObject> CreateAsset(string key)
        {
            var handle = await Addressables.InstantiateAsync(key);
            return handle;
        }
    }
}
