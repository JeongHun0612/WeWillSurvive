using UnityEngine;
using System.Collections.Generic;

namespace WeWillSurvive
{
    public class ObjSpawner : MonoBehaviour
    {
        public GameObject[] wallPrefabs;
        public GameObject[] crewPrefabs;
        public GameObject waterPrefab;
        public GameObject foodPrefab;
        public GameObject[] itemPrefabs;

        public Vector2 spawnAreaMin;
        public Vector2 spawnAreaMax;

        public int waterCount = 10;
        public int foodCount = 10;
        public int itemCount = 10;

        public Vector2[] crewPositions;

        public float collisionRadius = 0.5f;
        public LayerMask obstacleLayer;
        public Transform mapParent;

        void Start()
        {
            SpawnWalls();
            SpawnCrew();
            SpawnGroupedObjects(waterPrefab, waterCount);
            SpawnGroupedObjects(foodPrefab, foodCount);
            SpawnItems(itemPrefabs, itemCount);
        }

        void SpawnWalls()
        {
            int wallIndex = Random.Range(0, wallPrefabs.Length);
            GameObject wall = Instantiate(wallPrefabs[wallIndex], Vector3.zero, Quaternion.identity, mapParent);
            wall.transform.localPosition = Vector3.zero;
        }

        void SpawnCrew()
        {
            List<Vector2> availablePositions = new List<Vector2>(crewPositions);
            ShuffleList(availablePositions);

            for (int i = 0; i < crewPrefabs.Length; i++)
            {
                Vector2 pos = GetValidPositionCollider(availablePositions[i]);
                GameObject crew = Instantiate(crewPrefabs[i], pos, Quaternion.identity, transform);
            }
        }

        void SpawnGroupedObjects(GameObject prefab, int count)
        {
            int spawned = 0;
            while (spawned < count)
            {
                int groupSize = Random.Range(1, Mathf.Min(4, count - spawned + 1));
                Vector2 startPos = GetRandomPosition();
                for (int i = 0; i < groupSize && spawned < count; i++)
                {
                    Vector2 pos = startPos + Random.insideUnitCircle * collisionRadius;
                    pos = GetValidPositionCollider(pos);
                    Instantiate(prefab, pos, Quaternion.identity, transform);
                    spawned++;
                }
            }
        }

        void SpawnItems(GameObject[] prefabs, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 pos = GetValidPositionCollider(GetRandomPosition());
                GameObject prefab = prefabs[i];
                Instantiate(prefab, pos, Quaternion.identity, transform);
            }
        }

        Vector2 GetRandomPosition()
        {
            float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
            return new Vector2(x, y);
        }

        Vector2 GetValidPositionCollider(Vector2 basePos)
        {
            int attempts = 0;
            while (attempts < 100)
            {
                Collider2D hit = Physics2D.OverlapCircle(basePos, collisionRadius, obstacleLayer);
                if (hit == null)
                    return basePos;

                basePos = GetRandomPosition();
                attempts++;
            }
            return basePos;
        }

        void ShuffleList<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randIndex = Random.Range(i, list.Count);
                T temp = list[i];
                list[i] = list[randIndex];
                list[randIndex] = temp;
            }
        }
    }
}
