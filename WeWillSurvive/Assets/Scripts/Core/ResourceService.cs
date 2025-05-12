using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace WeWillSurvive.Core
{
    public class ResourceService : IService
    {
        private Dictionary<string, GameObject> _loadedAssets = new Dictionary<string, GameObject>();

        public void Initialize()
        {
        }

        public async UniTask<GameObject> LoadAsset(string key)
        {
            if (_loadedAssets.ContainsKey(key))
            {
                Debug.Log($"�̹� �ε�� ���ҽ�: {key}");
                return _loadedAssets[key];
            }

            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(key);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _loadedAssets[key] = handle.Result;
                Debug.Log($"�ε� ����: {key}");
                return handle.Result;
            }
            else
            {
                Debug.LogError($"�ε� ����: {key}");
            }

            return null;
        }

        public void UnloadAsset(string key)
        {
            if (_loadedAssets.ContainsKey(key))
            {
                Addressables.Release(_loadedAssets[key]);
                _loadedAssets.Remove(key);
                Debug.Log($"��ε� �Ϸ�: {key}");
            }
        }

        public async UniTask<GameObject> CreateAsset(string key)
        {
            var handle = await Addressables.InstantiateAsync(key);
            return handle;
        }
    }
}
