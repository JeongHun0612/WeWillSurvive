using UnityEngine;
using System.Collections.Generic;

namespace WeWillSurvive
{
    public class spawn : MonoBehaviour
    {
        [SerializeField] private List<GameObject> Safeprefabs = new List<GameObject>();
        [SerializeField] private List<GameObject> Oneprefabs = new List<GameObject>();
        [SerializeField] private List<int> prefabNum = new List<int>();
        [SerializeField] private List<int> prefabCount = new List<int>();
        [SerializeField] private SlotMaster sharedInventory;
        [SerializeField] private int maxStorage;
        [SerializeField] private int minStorage;

        [SerializeField] private int maxItems = 30;
        private int maxI;
        [SerializeField] private int minItems = 10;
        private int minI;

        private List<Interactible> itemsToAssign = new();
        private List<Interactible> storagesToAssign = new();

        private void Start()
        {
            DistributeItemsAndStorages();
        }

        public void DistributeItemsAndStorages()
        {
            //prefabCount 초기화
            for (int i = 0; i < 12; i++)
            {
                prefabCount[i] = prefabNum[i];
            }
            maxI = maxItems; minI = minItems;

            List<(Vector3 pos, Quaternion rot)> spawnTransforms = new();
            List<Interactible> storagesToAssign = new();

            //자식 오브젝트 순회로 위치 잡기
            foreach (Transform child in transform)
            {
                var entity = child.GetComponent<Interactible>();
                if (entity == null) continue;

                if (entity.interactibleType == InteractibleType.Item)
                {
                    if (entity.interactableValue == 0)
                    {
                        spawnTransforms.Add((entity.transform.position, entity.transform.rotation));
                        Destroy(entity.gameObject);
                    }
                    else
                    {
                        int idx = entity.interactableValue - 1;
                        if (idx >= 0 && idx < 12)
                        {
                            prefabCount[idx]--;
                            if(idx != 0 && idx != 1){minI--;}
                            maxI--;
                        }
                    }
                }
                else if (entity.interactibleType == InteractibleType.Storage)
                {
                    storagesToAssign.Add(entity);
                }
            }
            Shuffle(spawnTransforms);

            //필수품 먼저 넣고 다음 0번 1번 랜덤하게 넣기
            if(spawnTransforms.Count < minI){ Debug.LogWarning("Warning! : not enough random items to spawn all critical items");}
            int maxV = Mathf.Min(maxI, spawnTransforms.Count);
            int spawnCount = Random.Range(minI, maxV);

            int j = 2;
            for(int i=0; i < spawnCount; i++){
                if(minI>0 || j>=12){
                    while(prefabCount[j] <= 0)j++;
                    if(j>=12) continue;
                    prefabCount[j]--;
                    minI--;
                    Instantiate(Oneprefabs[j], spawnTransforms[i].pos, spawnTransforms[i].rot, transform);
                }
                else{
                    int index;
                    bool has0 = prefabCount[0] > 0;
                    bool has1 = prefabCount[1] > 0;

                    if (has0 && !has1) index = 0;
                    else if (!has0 && has1) index = 1;
                    else if (has0 && has1) index = Random.value < 0.5f ? 0 : 1;
                    else break;

                    Instantiate(Oneprefabs[index], spawnTransforms[i].pos, spawnTransforms[i].rot, transform);
                    prefabCount[index]--;
                }
            }

            //남은 것 넣기
            foreach (var store in storagesToAssign)
            {
                if (store.interactableValue == 0 || prefabCount[store.interactableValue-1] <= 0) break;
                store.storageScript.SetStorage(store.interactableValue, Safeprefabs[store.interactableValue-1], prefabCount[store.interactableValue-1]);
            }

            foreach (Transform child in transform)
            {
                var entity = child.GetComponent<Interactible>();
                if (entity == null) continue;
                entity.inventory = sharedInventory;
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


