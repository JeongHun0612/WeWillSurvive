using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Core;

namespace WeWillSurvive.UI
{
    public class UIManager : MonoSceneSingleton<UIManager>
    {
        [SerializeField] private Transform _sceneLayer;
        [SerializeField] private Transform _popupLayer;
        [SerializeField] private Transform _overlayLayer;

        [SerializeField] private string[] _scenePrefabPaths;
        [SerializeField] private string[] _popupPrefabPaths;
        [SerializeField] private string[] _overlayPrefabPaths;
        [SerializeField] private string _loadingPrefabPath;

        private UI_Scene _currentScene;
        private UI_Popup _currentPopup;
        private UI_Overlay _currentOverlay;

        private readonly Stack<UI_Popup> _popupHistory = new Stack<UI_Popup>();
        public int PopupHistoryCount => _popupHistory.Count;

        private readonly Dictionary<Type, UI_Scene> _sceneCache = new Dictionary<Type, UI_Scene>();
        private readonly Dictionary<Type, UI_Popup> _popupCache = new Dictionary<Type, UI_Popup>();
        private readonly Dictionary<Type, UI_Overlay> _overlayCache = new Dictionary<Type, UI_Overlay>();

        private bool _isInitialized = false;
        public bool IsInitialized => _isInitialized;

        private float _initializationProgress = 0f;
        public float InitializationProgress => _initializationProgress;

        private int _processedItems;
        private int _totalItems;

        private ResourceManager _resourceManager;
        public ResourceManager ResourceManager => _resourceManager ??= ServiceLocator.Get<ResourceManager>();

        public UI_Loading LoadingUI { get; private set; }

        public async UniTask InitializeAsync(IProgress<float> progress = null)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("UIManager is already initialized.");
                return;
            }

            // LoadingUI Instantiate
            await LoadingUIInitialize();
            LoadingUI.Show();

            Debug.Log("Starting UI initialization with UniTask...");
            _initializationProgress = 0f;

            _totalItems = _scenePrefabPaths.Length + _popupPrefabPaths.Length;
            _processedItems = 0;

            await InitializeUIElements<UI_Overlay>(_overlayPrefabPaths, _overlayLayer, _overlayCache);
            await InitializeUIElements<UI_Scene>(_scenePrefabPaths, _sceneLayer, _sceneCache);
            await InitializeUIElements<UI_Popup>(_popupPrefabPaths, _popupLayer, _popupCache);

            _isInitialized = true;
            _initializationProgress = 1f;
            progress?.Report(1f);

            Debug.Log("UI initialization completed successfully.");
            LoadingUI.Hide();
        }

        public T ShowPopup<T>(bool remember = true) where T : UI_Popup
        {
            Type popupType = typeof(T);

            if (!_popupCache.TryGetValue(popupType, out UI_Popup popup))
            {
                Debug.LogError($"Popup of type {popupType.Name} not found!");
                return null;
            }

            if (remember && popup.RememberInHistory)
                _popupHistory.Push(popup);

            _currentPopup = popup;
            _currentPopup.Show();

            return popup as T;
        }

        public void CloseCurrentPopup()
        {
            if (_currentPopup != null)
            {
                _currentPopup.Hide();

                if (_currentPopup.RememberInHistory)
                    _popupHistory.Pop();

                if (_popupHistory.Count > 0)
                    _currentPopup = _popupHistory.Peek();
                else
                    _currentPopup = null;
            }
        }

        public void ClosePopups(int remain)
        {
            while (true)
            {
                if (_popupHistory.Count <= remain) break;
                CloseCurrentPopup();
            }
        }

        public void CloseAllPopups()
        {
            ClosePopups(remain: 0);
        }

        public T ShowScene<T>() where T : UI_Scene
        {
            Type sceneType = typeof(T);

            if (!_sceneCache.TryGetValue(sceneType, out UI_Scene scene))
            {
                Debug.LogError($"Scene of type {sceneType.Name} not found!");
                return null;
            }

            if (_currentScene != null)
            {
                if (_currentScene == scene)
                    return scene as T;

                _currentScene.Hide();
            }

            _currentScene = scene;
            _currentScene.Show();

            return scene as T;
        }

        public void CloseCurrentScene()
        {
            if (_currentScene != null)
            {
                _currentScene.Hide();
                _currentScene = null;
            }
        }

        public T ShowOverlay<T>() where T : UI_Overlay
        {
            Type overlayType = typeof(T);

            if (!_overlayCache.TryGetValue(overlayType, out UI_Overlay overlay))
            {
                Debug.LogError($"Overlay of type {overlayType.Name} not found!");
                return null;
            }

            if (_currentOverlay != null)
            {
                if (_currentOverlay == overlay)
                    return overlay as T;

                _currentScene.Hide();
            }

            _currentOverlay = overlay;
            _currentOverlay.Show();

            return overlay as T;
        }

        public void CloseCurrentOverlay()
        {
            if (_currentOverlay != null)
            {
                _currentOverlay.Hide();
                _currentOverlay = null;
            }
        }

        public T GetCurrentPopup<T>() where T : UI_Popup
        {
            if (_currentPopup is not T) return null;
            return _currentPopup as T;
        }

        public T GetCurrentScene<T>() where T : UI_Scene
        {
            return _currentScene as T;
        }

        private async UniTask InitializeUIElements<T>(string[] paths, Transform layer, Dictionary<Type, T> cache, IProgress<float> progress = null) where T : UI_Base
        {
            foreach (string path in paths)
            {
                try
                {
                    var asset = await ResourceManager.LoadAssetAsync<GameObject>(path);
                    T prefab = asset.GetComponent<T>();
                    if (prefab != null)
                    {
                        T instance = Instantiate(prefab, layer);
                        Type uiType = instance.GetType();

                        if (!cache.ContainsKey(uiType))
                        {
                            cache.Add(uiType, instance);
                            instance.CanvasInitialize();
                            await instance.InitializeAsync();
                            instance.Hide();
                            Debug.Log($"{typeof(T).Name} loaded and initialized: {uiType.Name}");
                        }
                        else
                        {
                            Debug.LogWarning($"Duplicate {typeof(T).Name} type: {uiType.Name}");
                            Destroy(instance.gameObject);
                        }
                    }
                    else
                    {
                        Debug.LogError($"Failed to load {typeof(T).Name} prefab at path: {path}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error loading {typeof(T).Name} prefab at path {path}: {ex.Message}");
                }


                _processedItems++;
                float progressValue = (float)_processedItems / _totalItems;
                progress?.Report(progressValue);

                await UniTask.Yield();
            }
        }

        public async UniTask LoadingUIInitialize()
        {
            try
            {
                var asset = await ResourceManager.LoadAssetAsync<GameObject>(_loadingPrefabPath);
                LoadingUI = Instantiate(asset.GetComponent<UI_Loading>(), transform);
                LoadingUI.CanvasInitialize();
                await LoadingUI.InitializeAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading loadingUI prefab: {ex.Message}");
            }
        }
    }
}
