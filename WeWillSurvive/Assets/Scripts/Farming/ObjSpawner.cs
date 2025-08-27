using UnityEngine;
using System.Collections.Generic;

namespace WeWillSurvive
{

    public class ObjSpawner : MonoBehaviour
    {
        public GameObject[] crewPrefabs;
        public GameObject waterPrefab;
        public GameObject foodPrefab;
        public GameObject[] otherPrefabs;

        public Transform[] corners;
        public int waterCount = 10;
        public int foodCount = 10;
        public int otherCount = 10;
        public float minDistance = 1.5f;
        public Vector2 spawnAreaMin;
        public Vector2 spawnAreaMax;

        private List<Vector2> spawnPositions = new List<Vector2>();

        void Start()
        {
            List<int> cornerIndices = new List<int> { 0, 1, 2, 3 };
            int emptyCorner = Random.Range(0, cornerIndices.Count);
            cornerIndices.RemoveAt(emptyCorner);

            for (int i = 0; i < crewPrefabs.Length && i < cornerIndices.Count; i++)
            {
                Vector2 pos = corners[cornerIndices[i]].position;
                Instantiate(crewPrefabs[i], pos, Quaternion.identity);
                spawnPositions.Add(pos);
            }

            for (int i = 0; i < 30; i++)
            {
                Vector2 candidate = GetRandomPosition();
                if (IsFarEnough(candidate, spawnPositions))
                    spawnPositions.Add(candidate);
                else
                    i--;
            }

            SpawnCluster(waterPrefab, waterCount);
            SpawnCluster(foodPrefab, foodCount);
            SpawnOthers(otherPrefabs, otherCount);
        }

        Vector2 GetRandomPosition()
        {
            float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
            return new Vector2(x, y);
        }

        bool IsFarEnough(Vector2 candidate, List<Vector2> existing)
        {
            foreach (var pos in existing)
            {
                if (Vector2.Distance(candidate, pos) < minDistance)
                    return false;
            }
            return true;
        }

        void SpawnCluster(GameObject prefab, int count)
        {
            int spawned = 0;
            while (spawned < count && spawnPositions.Count > 0)
            {
                int index = ChooseClusterIndex(prefab);
                Vector2 pos = spawnPositions[index];
                Instantiate(prefab, pos, Quaternion.identity);
                spawnPositions.RemoveAt(index);
                spawned++;
            }
        }

        void SpawnOthers(GameObject[] prefabs, int count)
        {
            int spawned = 0;
            while (spawned < count && spawnPositions.Count > 0)
            {
                int index = Random.Range(0, spawnPositions.Count);
                Vector2 pos = spawnPositions[index];
                GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
                Instantiate(prefab, pos, Quaternion.identity);
                spawnPositions.RemoveAt(index);
                spawned++;
            }
        }

        int ChooseClusterIndex(GameObject prefab)
        {
            if (spawnPositions.Count == 0) return 0;

            int bestIndex = Random.Range(0, spawnPositions.Count);
            float bestScore = float.MaxValue;

            foreach (var pos in spawnPositions)
            {
                float score = 0;
                GameObject[] existing = GameObject.FindGameObjectsWithTag(prefab.tag);
                foreach (var obj in existing)
                {
                    score += Vector2.Distance(pos, obj.transform.position);
                }
                if (score < bestScore)
                {
                    bestScore = score;
                    bestIndex = spawnPositions.IndexOf(pos);
                }
            }
            return bestIndex;
        }
    }


}
