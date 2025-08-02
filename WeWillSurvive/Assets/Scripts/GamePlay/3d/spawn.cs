using UnityEngine;
using System.Collections.Generic;

namespace WeWillSurvive
{
    public class spawn : MonoBehaviour
    {
        [SerializeField] private List<GameObject> prefabs = new List<GameObject>();
        [SerializeField] private SlotMaster sharedInventory;
        [SerializeField] private int maxStorage = 9;
        [SerializeField] private int minStorage = 5;

        [SerializeField] private int totalFood;
        private int maxFood;
        [SerializeField] private int totalWater;
        private int maxWater;

        private List<Interactible> itemsToAssign = new();
        private List<Interactible> storagesToAssign = new();

        private void Start()
        {
            DistributeItemsAndStorages();
        }

        public void DistributeItemsAndStorages()
        {
            maxFood = totalFood;
            maxWater = totalWater;

            // 1. 자식 오브젝트 순회
            foreach (Transform child in transform)
            {
                var entity = child.GetComponent<Interactible>();
                if (entity == null) continue;

                entity.inventory = sharedInventory;

                if (entity.interactibleType == InteractibleType.Item)
                {
                    if (entity.interactableValue == 0)
                    {
                        itemsToAssign.Add(entity);
                    }
                    else
                    {
                        if (entity.interactableValue == 1) maxFood--;
                        if (entity.interactableValue == 2) maxWater--;
                    }
                }
                else
                {
                    if (entity.interactibleType == InteractibleType.Storage) storagesToAssign.Add(entity);
                }
            }

            // 2. 랜덤으로 아이템 부여
            Shuffle(itemsToAssign);
            foreach (var item in itemsToAssign)
            {
                if (maxFood > 0 && maxWater > 0)
                {
                    if (Random.value < 0.5f)
                    {
                        item.interactableValue = 1; // food
                        maxFood--;
                    }
                    else
                    {
                        item.interactableValue = 2; // water
                        maxWater--;
                    }
                }
                else if (maxFood > 0)
                {
                    item.interactableValue = 1;
                    maxFood--;
                }
                else if (maxWater > 0)
                {
                    item.interactableValue = 2;
                    maxWater--;
                }
                else break; // 수량 모두 소진됨
            }

            // 3. storage에게 랜덤 아이템 수량 배정
            Shuffle(storagesToAssign);
            foreach (var storage in storagesToAssign)
            {
                if (maxFood <= 0 && maxWater <= 0) break;

                int amount = Random.Range(minStorage, maxStorage + 1);

                if (storage.interactableValue == 1)
                {
                    // food 배정
                    amount = Mathf.Min(amount, maxFood);
                    storage.storageScript.SetStorage(1, prefabs[0], amount);
                    maxFood -= amount;
                }
                if (storage.interactableValue == 2)
                {
                    // water 배정
                    amount = Mathf.Min(amount, maxWater);
                    storage.storageScript.SetStorage(2, prefabs[1], amount);
                    maxWater -= amount;
                }
            }
        }

        private void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rand = Random.Range(i, list.Count);
                (list[i], list[rand]) = (list[rand], list[i]);
            }
        }
    }
}


