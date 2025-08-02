using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace WeWillSurvive.Core
{
	public enum ESpriteAtlas
    {
		UI_Atlas,
		Item_Atlas,
		Character_Standing_Atlas,
		Character_Seated_Atlas,
	}

    public class SpriteManager : MonoSingleton<SpriteManager>
    {
		[SerializeField] private string[] _spriteAtlasPrefabs;

		private Dictionary<ESpriteAtlas, SpriteAtlas> _spriteAtlasDicts = new();

        private bool _isInitialized = false;
        public bool IsInitialized => _isInitialized;

		private ResourceManager ResourceManager => ServiceLocator.Get<ResourceManager>();

		public async UniTask InitializeAsync()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("SpriteManager is already initialized.");
                return;
            }

            Debug.Log("Starting SpriteManager initialization with UniTask...");

			foreach (string path in _spriteAtlasPrefabs)
			{
				try
				{
					var asset = await ResourceManager.LoadAssetAsync<SpriteAtlas>(path);

					if (asset != null)
                    {
						if (Enum.TryParse<ESpriteAtlas>(asset.name, out var type))
                        {
							if (!_spriteAtlasDicts.ContainsKey(type))
                            {
								_spriteAtlasDicts.Add(type, asset);
								Debug.Log($"spriteAtlas loaded: {asset.name}");
							}
                        }
                        else
                        {
							Debug.LogError($"ESpriteAtlas Parsing Fail: {asset.name}");
                        }
					}
                    else
                    {
						Debug.LogError($"Failed to load spriteAtlas at path: {path}");
					}
				}
				catch (Exception ex)
				{
					Debug.LogError($"Error loading spriteAtlas at path {path}: {ex.Message}");
				}

				await UniTask.Yield();
			}


			_isInitialized = true;
			Debug.Log("Sprite initialization completed successfully.");
		}

		public Sprite GetSprite(ESpriteAtlas type, string name)
		{
			if (!_spriteAtlasDicts.TryGetValue(type, out var atlas))
			{
				Debug.LogError($"[SpriteManager] SpriteAtlas of type {type} not found.");
				return null;
			}

			var sprite = atlas.GetSprite(name);
			if (sprite == null)
			{
				Debug.LogError($"[SpriteManager] Sprite '{name}' not found in atlas '{type}'.");
			}

			return sprite;
		}
	}
}