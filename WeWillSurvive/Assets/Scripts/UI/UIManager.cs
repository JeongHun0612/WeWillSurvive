using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Core;

namespace WeWillSurvive.UI
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField] private Transform _sceneLayer;
        [SerializeField] private Transform _hudLayer;
        [SerializeField] private Transform _popupLayer;
        [SerializeField] private Transform _overlayLayer;

        [SerializeField] private string[] _scenePrefabPaths;
        [SerializeField] private string[] _hudPrefabPaths;
        [SerializeField] private string[] _popupPrefabPaths;
        [SerializeField] private string[] _overlayPrefabPaths;

        [SerializeField] private UI_Loading _loadingUI;

        private UI_Scene _currentScene;
        private UI_HUD _currentHUD;
        private UI_Popup _currentPopup;
        private UI_Overlay _currentOverlay;

        private readonly Stack<UI_Popup> _popupHistory = new Stack<UI_Popup>();
        public int PopupHistoryCount => _popupHistory.Count;

        private readonly Dictionary<Type, UI_Scene> _sceneCache = new Dictionary<Type, UI_Scene>();
        private readonly Dictionary<Type, UI_HUD> _hudCache = new Dictionary<Type, UI_HUD>();
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

        public UI_Loading LoadingUI => _loadingUI;

        protected override void Awake()
        {
            base.Awake();

            LoadingUIInitialize();
        }

        public async UniTask InitializeAsync(IProgress<float> progress = null)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("UIManager is already initialized.");
                return;
            }

            _resourceManager = ServiceLocator.Get<ResourceManager>();

            Debug.Log("Starting UI initialization with UniTask...");
            _initializationProgress = 0f;

            _totalItems = _scenePrefabPaths.Length + _popupPrefabPaths.Length;
            _processedItems = 0;

            await InitializeUIElements<UI_Scene>(_scenePrefabPaths, _sceneLayer, _sceneCache);
            await InitializeUIElements<UI_HUD>(_hudPrefabPaths, _hudLayer, _hudCache);
            await InitializeUIElements<UI_Popup>(_popupPrefabPaths, _popupLayer, _popupCache);
            await InitializeUIElements<UI_Overlay>(_overlayPrefabPaths, _overlayLayer, _overlayCache);

            _isInitialized = true;
            _initializationProgress = 1f;
            progress?.Report(1f);

            Debug.Log("UI initialization completed successfully.");
        }

        #region Scene, HUD, Overlay
        public T ShowScene<T>() where T : UI_Scene
            => ShowCommon<T, UI_Scene>(_sceneCache, ref _currentScene);
        public T ShowHUD<T>() where T : UI_HUD
            => ShowCommon<T, UI_HUD>(_hudCache, ref _currentHUD);
        public T ShowOverlay<T>() where T : UI_Overlay
            => ShowCommon<T, UI_Overlay>(_overlayCache, ref _currentOverlay);

        public void CloseCurrentScene() => CloseCommon(ref _currentScene);
        public void CloseCurrentHUD() => CloseCommon(ref _currentHUD);
        public void CloseCurrentOverlay() => CloseCommon(ref _currentOverlay);
        #endregion


        #region Popup
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
        #endregion

        public void CloseAllUIs()
        {
            CloseCurrentScene();
            CloseCurrentHUD();
            CloseAllPopups();
        }

        public T GetCurrentScene<T>() where T : UI_Scene => _currentScene as T;
        public T GetCurrentHUD<T>() where T : UI_HUD => _currentHUD as T;
        public T GetCurrentPopup<T>() where T : UI_Popup => _currentPopup as T;
        public T GetCurrentOverlay<T>() where T : UI_Overlay => _currentOverlay as T;

        private T ShowCommon<T, TBase>(Dictionary<Type, TBase> cache, ref TBase current)
            where T : TBase
            where TBase : UI_Base
        {
            var type = typeof(T);

            if (!cache.TryGetValue(type, out var target) || target == null)
            {
                Debug.LogError($"UI of type {type.Name} not found!");
                return null;
            }

            if (current != null)
            {
                if (ReferenceEquals(current, target))
                    return target as T;

                current.Hide();
            }

            current = target;
            current.Show();
            return (T)target;
        }

        private void CloseCommon<TBase>(ref TBase current) where TBase : UI_Base
        {
            if (current == null)
                return;

            current.Hide();
            current = null;
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

        public void LoadingUIInitialize()
        {
            if (_loadingUI == null)
            {
                Debug.LogError("UI_Loading is null!");
                return;
            }

            _loadingUI.CanvasInitialize();
            _loadingUI.Initialize();
            _loadingUI.Hide();

            Debug.Log($"UI_Loading loaded and initialized");
        }
    }
}