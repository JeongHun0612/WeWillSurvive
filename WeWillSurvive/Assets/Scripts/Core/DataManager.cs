using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace WeWillSurvive.Core
{
    public class DataManager : IService
    {
        public string BasePath
        {
            get
            {
#if UNITY_EDITOR
                string path = Path.Combine(Application.dataPath, "Datas/JsonData");
#else
                string path = Path.Combine(Application.persistentDataPath, "JsonData");
#endif

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        }

        public async UniTask InitializeAsync()
        {
            await UniTask.CompletedTask;
        }

        public List<T> LoadDataList<T>(string path = "", string fileName = "")
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = typeof(T).Name;
            }

            path = string.IsNullOrWhiteSpace(path) ? BasePath : path;
            string filePath = Path.Combine(path, $"{fileName}.json");

            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"File not found - {filePath}");
                return new List<T>();
            }

            try
            {
                string json = File.ReadAllText(filePath);
                SerializableList<T> wrapper = JsonUtility.FromJson<SerializableList<T>>(json);
                return wrapper.items ?? new List<T>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load data from {filePath}: {ex.Message}");
                return new List<T>();
            }
        }

        public void SaveDataList<T>(List<T> dataLsit, string path = "", string fileName = "", bool prettyPrint = true)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = typeof(T).Name;
            }

            path = string.IsNullOrWhiteSpace(path) ? BasePath : path;
            string filePath = Path.Combine(path, $"{fileName}.json");

            try
            {
                string json = JsonUtility.ToJson(new SerializableList<T>(dataLsit), prettyPrint);
                File.WriteAllText(filePath, json);
                Debug.Log($"Save Data - {filePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save file: {filePath}\n{ex.Message}");
            }
        }

        public T LoadData<T>(string path = "", string fileName = "")
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = typeof(T).Name;
            }

            path = string.IsNullOrWhiteSpace(path) ? BasePath : path;
            string filePath = Path.Combine(path, $"{fileName}.json");

            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"File not found - {filePath}");
                return default;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load data from {filePath}: {ex.Message}");
                return default;
            }
        }

        public void SaveData<T>(T data, string path = "", string fileName = "", bool prettyPrint = true)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = typeof(T).Name;
            }

            path = string.IsNullOrWhiteSpace(path) ? BasePath : path;
            string filePath = Path.Combine(path, $"{fileName}.json");

            try
            {
                string json = JsonUtility.ToJson(data, prettyPrint);
                File.WriteAllText(filePath, json);
                Debug.Log($"Save Data : {filePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save file: {filePath}\n{ex.Message}");
            }
        }
    }

    [System.Serializable]
    public class SerializableList<T>
    {
        public List<T> items;

        public SerializableList(List<T> items)
        {
            this.items = items;
        }
    }
}
