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

        /// <summary>
        /// 특정 라벨을 가진 모든 Addressable 리소스를 로드
        /// </summary>
        public async UniTask<List<T>> LoadAssetsByLabelAsync<T>(string label) where T : Object
        {
            List<T> loadedList = new List<T>();

            var handle = Addressables.LoadAssetsAsync<T>(label, null);  // 콜백 없이 로드
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var asset in handle.Result)
                {
                    string key = asset.name;
                    if (!_loadedAssets.ContainsKey(key))
                    {
                        _loadedAssets[key] = asset;
                    }

                    loadedList.Add(asset);
                }

                Debug.Log($"label : {label} 리소스 {loadedList.Count}개 로드 성공");
            }
            else
            {
                Debug.LogError($"{label} 라벨 리소스 로드 실패");
            }

            return loadedList;
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
