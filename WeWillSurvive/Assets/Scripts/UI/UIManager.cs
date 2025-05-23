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

        [SerializeField] private string[] _scenePrefabPaths;
        [SerializeField] private string[] _popupPrefabPaths;

		private UI_Scene _currentScene;
        private UI_Popup _currentPopup;

        private readonly Stack<UI_Popup> _popupHistory = new Stack<UI_Popup>();
        private readonly Dictionary<Type, UI_Scene> _sceneCache = new Dictionary<Type, UI_Scene>();
        private readonly Dictionary<Type, UI_Popup> _popupCache = new Dictionary<Type, UI_Popup>();

        private bool _isInitialized = false;
        public bool IsInitialized => _isInitialized;

        private float _initializationProgress = 0f;
        public float InitializationProgress => _initializationProgress;

		private ResourceService _resourceSerivce;
		public ResourceService ResourceService => _resourceSerivce ??= ServiceLocator.Get<ResourceService>();


		public async UniTask InitializeAsync(IProgress<float> progress = null)
		{
			if (_isInitialized)
			{
				Debug.LogWarning("UIManager is already initialized.");
				return;
			}

			Debug.Log("Starting UI initialization with UniTask...");
			_initializationProgress = 0f;

			int totalItems = _scenePrefabPaths.Length + _popupPrefabPaths.Length;
			int processedItems = 0;

			foreach (string path in _scenePrefabPaths)
			{
				try
				{
					var asset = await ResourceService.LoadAsset(path);
					asset.SetActive(false);

					UI_Scene prefab = asset.GetComponent<UI_Scene>();
					if (prefab != null)
					{
						UI_Scene instance = Instantiate(prefab, _sceneLayer);
						Type sceneType = instance.GetType();

						if (!_sceneCache.ContainsKey(sceneType))
						{
							_sceneCache.Add(sceneType, instance);
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
					var asset = await ResourceService.LoadAsset(path);
					asset.SetActive(false);

					UI_Popup prefab = asset.GetComponent<UI_Popup>();
					if (prefab != null)
					{
						UI_Popup instance = Instantiate(prefab, _popupLayer);
						Type popupType = instance.GetType();

						if (!_popupCache.ContainsKey(popupType))
						{
							_popupCache.Add(popupType, instance);
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
		}

		public T ShowPopup<T>(bool remember = true) where T : UI_Popup
		{
			Type popupType = typeof(T);

			if (!_popupCache.TryGetValue(popupType, out UI_Popup popup))
			{
				Debug.LogError($"Popup of type {popupType.Name} not found!");
				return null;
			}

			if (_currentPopup != null)
			{
				if (_currentPopup == popup)
					return popup as T;

				if (remember && _currentPopup.RememberInHistory)
				{
					_popupHistory.Push(_currentPopup);
				}

				_currentPopup.Hide();
			}

			_currentPopup = popup;
			_currentPopup.Show();

			return popup as T;
		}

		public void CloseCurrentPopup()
		{
			if (_currentPopup != null)
			{
				_currentPopup.Hide();
				_currentPopup = null;
			}

			if (_popupHistory.Count > 0)
			{
				_currentPopup = _popupHistory.Pop();
				_currentPopup.Show();
			}
		}

		public void CloseAllPopups()
		{
			if (_currentPopup != null)
			{
				_currentPopup.Hide();
				_currentPopup = null;
			}

			_popupHistory.Clear();
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

		public void HideCurrentScene()
		{
			if (_currentScene != null)
			{
				_currentScene.Hide();
				_currentScene = null;
			}
		}

		public T GetCurrentPopup<T>() where T : UI_Popup
		{
			return _currentPopup as T;
		}

		public T GetCurrentScene<T>() where T : UI_Scene
		{
			return _currentScene as T;
		}
	}
}
