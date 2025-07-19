using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
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

        private UI_Scene _currentScene;
        private UI_Popup _currentPopup;
        private UI_Overlay _currentOverlay;

        private readonly Stack<UI_Popup> _popupHistory = new Stack<UI_Popup>();
        public int PopupHistoryCount => _popupHistory.Count;

        private readonly Dictionary<Type, UI_Scene> _sceneCache = new Dictionary<Type, UI_Scene>();
        private readonly Dictionary<Type, UI_Popup> _popupCache = new Dictionary<Type, UI_Popup>();
        private readonly Dictionary<Type, UI_Overlay> _overlayCache = new Dictionary<Type, UI_Overlay>();

        private readonly string _path = "Assets/Prefabs/UI/";

        private bool _isInitialized = false;
        public bool IsInitialized => _isInitialized;

        private float _initializationProgress = 0f;
        public float InitializationProgress => _initializationProgress;

        private ResourceManager _resourceManager;
        public ResourceManager ResourceManager => _resourceManager ??= ServiceLocator.Get<ResourceManager>();

        public UI_Black BlackUI { get; private set; }

        public async UniTask InitializeAsync(IProgress<float> progress = null)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("UIManager is already initialized.");
                return;
            }

            _overlayPrefabPaths = LoadPrefabFileNames(_path + "Overlay");
            await InitializeOverlays(_overlayPrefabPaths);
            ShowOverlay<UI_Black>();

            Debug.Log("Starting UI initialization with UniTask...");
            _initializationProgress = 0f;

            _popupPrefabPaths = LoadPrefabFileNames(_path + "Popup");
            _scenePrefabPaths = LoadPrefabFileNames(_path + "Scene");

            int totalItems = _scenePrefabPaths.Length + _popupPrefabPaths.Length;
            int processedItems = 0;

            foreach (string path in _scenePrefabPaths)
            {
                try
                {
                    var asset = await ResourceManager.LoadAssetAsync<GameObject>(path);
                    asset.SetActive(true);

                    UI_Scene prefab = asset.GetComponent<UI_Scene>();
                    if (prefab != null)
                    {
                        UI_Scene instance = Instantiate(prefab, _sceneLayer);
                        Type sceneType = instance.GetType();

                        if (!_sceneCache.ContainsKey(sceneType))
                        {
                            _sceneCache.Add(sceneType, instance);
                            instance.CanvasInitialize();
                            await instance.InitializeAsync();
                            instance.Hide();
                            Debug.Log($"SceneUI loaded and initialized: {sceneType.Name}");
                        }
                        else
                        {
                            Debug.LogWarning($"Duplicate sceneUI type: {sceneType.Name}");
                            Destroy(instance.gameObject);
                        }
                    }
                    else
                    {
                        Debug.LogError($"Failed to load sceneUI prefab at path: {path}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error loading sceneUI prefab at path {path}: {ex.Message}");
                }

                processedItems++;
                _initializationProgress = (float)processedItems / totalItems;
                progress?.Report(_initializationProgress);

                await UniTask.Yield();
            }

            foreach (string path in _popupPrefabPaths)
            {
                try
                {
                    var asset = await ResourceManager.LoadAssetAsync<GameObject>(path);
                    asset.SetActive(true);

                    UI_Popup prefab = asset.GetComponent<UI_Popup>();
                    if (prefab != null)
                    {
                        UI_Popup instance = Instantiate(prefab, _popupLayer);
                        Type popupType = instance.GetType();

                        if (!_popupCache.ContainsKey(popupType))
                        {
                            _popupCache.Add(popupType, instance);
                            instance.CanvasInitialize();
                            await instance.InitializeAsync();
                            instance.Hide();
                            Debug.Log($"PopupUI loaded and initialized: {popupType.Name}");
                        }
                        else
                        {
                            Debug.LogWarning($"Duplicate popupUI type: {popupType.Name}");
                            Destroy(instance.gameObject);
                        }
                    }
                    else
                    {
                        Debug.LogError($"Failed to load popupUI prefab at path: {path}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error loading popupUI prefab at path {path}: {ex.Message}");
                }

                processedItems++;
                _initializationProgress = (float)processedItems / totalItems;
                progress?.Report(_initializationProgress);

                await UniTask.Yield();
            }

            _isInitialized = true;
            _initializationProgress = 1f;
            progress?.Report(1f);

            Debug.Log("UI initialization completed successfully.");
            CloseCurrentOverlay();
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

        private string[] LoadPrefabFileNames(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Debug.LogWarning("폴더를 찾을 수 없습니다: " + folderPath);
                return Array.Empty<string>();
            }

            string[] prefabFiles = Directory.GetFiles(folderPath, "*.prefab");
            string[] result = new string[prefabFiles.Length];

            for (int i = 0; i < prefabFiles.Length; i++)
            {
                result[i] = Path.GetFileNameWithoutExtension(prefabFiles[i]);
            }

            return result;
        }

        private async UniTask InitializeOverlays(string[] paths)
        {
            foreach (string path in paths)
            {
                try
                {
                    var asset = await ResourceManager.LoadAssetAsync<GameObject>(path);
                    UI_Overlay prefab = asset.GetComponent<UI_Overlay>();
                    if (prefab != null)
                    {
                        UI_Overlay instance = Instantiate(prefab, _overlayLayer);
                        Type overlayType = instance.GetType();

                        if (!_overlayCache.ContainsKey(overlayType))
                        {
                            _overlayCache.Add(overlayType, instance);
                            instance.CanvasInitialize();
                            await instance.InitializeAsync();
                            instance.Hide();
                            Debug.Log($"OverlayUI loaded and initialized: {overlayType.Name}");
                        }
                        else
                        {
                            Debug.LogWarning($"Duplicate overlayUI type: {overlayType.Name}");
                            Destroy(instance.gameObject);
                        }
                    }
                    else
                    {
                        Debug.LogError($"Failed to load overlayUI prefab at path: {path}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error loading overlayUI prefab at path {path}: {ex.Message}");
                }

                await UniTask.Yield();
            }
        }
    }
}
