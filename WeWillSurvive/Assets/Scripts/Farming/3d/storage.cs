using UnityEngine;
using System.Collections.Generic;

namespace WeWillSurvive
{
    public class storage : MonoBehaviour
    {
        [Header("배치 설정")]
        [SerializeField] private int rowLength = 3;
        [SerializeField] private float colSpacing = 2.0f;
        [SerializeField] private float rowSpacing = 2.0f;
        [SerializeField] private float yPosition = -0.35f;
        [SerializeField] private float offset = 0.4f;

        [Header("아이템 설정")]
        [SerializeField] private GameObject itemPrefab;
        private List<GameObject> items = new List<GameObject>();
        [SerializeField] private int itemID = 0;
        private int amount = 0;

        public void SetStorage(int setID, GameObject item, int setAmount)
        {
            itemID = setID;
            itemPrefab = item;
            increase(setAmount);
        }

        public void increase(int added = 1)
        {
            for (int i = 0; i < added; i++)
            {
                Vector3 localPos = GetLocalPosition(amount);
                GameObject newItem = Instantiate(itemPrefab, transform);
                newItem.transform.localPosition = localPos;
                items.Add(newItem);
                amount++;
            }
        }

        public void decrease(int subtracted = 1)
        {
            for (int i = 0; i < subtracted; i++)
            {
                if (amount <= 0) return;

                amount--;
                GameObject toRemove = items[items.Count - 1];
                items.RemoveAt(items.Count - 1);
                Destroy(toRemove);
            }
        }

        private Vector3 GetLocalPosition(int index)
        {
            int xIndex = index % rowLength;
            int zIndex = index / rowLength;

            return new Vector3(
                xIndex * colSpacing / 10 - offset,
                yPosition,
                zIndex * rowSpacing / 10 - offset
            );
        }

        public int GetItemID()
        {
            return itemID;
        }
        public int GetStorageAmount()
        {
            return amount;
        }
    }
}
