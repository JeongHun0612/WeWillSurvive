using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System.Linq;

namespace WeWillSurvive
{
    public class SlotMaster : MonoBehaviour
    {
        [System.Serializable]
        public class ItemDefinition
        {
            public string itemName;
            public Sprite sprite;
            public int amountLimit = 1;
        }

        [System.Serializable]
        public class CrewDefinition
        {
            public string crewName;
            public Sprite sprite;
            public int crewSlotSize = 1;
        }

        [System.Serializable]
        private class Slot
        {
            public GameObject slotObject;
            [HideInInspector] public Image image;
            [HideInInspector] public TextMeshProUGUI text;
            [HideInInspector] public object data; // ItemDefinition or CrewDefinition
            [HideInInspector] public int amount;
        }

        [Header("아이템 정의")]
        [SerializeField] private List<ItemDefinition> itemDefinitions;

        [Header("크루 정의")]
        [SerializeField] private List<CrewDefinition> crewDefinitions;

        [Header("슬롯 UI 오브젝트들")]
        [SerializeField] private List<GameObject> slotObjects;
        private List<Slot> slots = new List<Slot>();

        [Header("슬롯이 찼을시 경고ui")]
        [SerializeField] private GameObject warningUI;
        [SerializeField] private GameObject interactionUI;
        private Coroutine warningCoroutine;

        private void Start()
        {
            foreach (var obj in slotObjects)
            {
                var slot = new Slot
                {
                    slotObject = obj,
                    image = obj.transform.Find("Image").GetComponent<Image>(),
                    text = obj.GetComponentInChildren<TextMeshProUGUI>(),
                    data = null,
                    amount = 0
                };
                slot.image.sprite = null;
                slot.text.text = "";
                slots.Add(slot);
            }
        }

        public bool AddItem(int itemId)
        {
            if (itemId < 0 || itemId >= itemDefinitions.Count)
            {
                Debug.LogWarning($"Item ID {itemId} is out of range.");
                return false;
            }

            var def = itemDefinitions[itemId];

            // 기존 슬롯에 추가
            foreach (var slot in slots)
            {
                if (slot.data == def && slot.amount < def.amountLimit)
                {
                    slot.amount++;
                    slot.text.text = slot.amount.ToString();
                    return true;
                }
            }

            // 빈 슬롯에 새로 할당
            foreach (var slot in slots)
            {
                if (slot.data == null)
                {
                    slot.data = def;
                    slot.amount = 1;
                    slot.image.sprite = def.sprite;
                    slot.text.text = "1";
                    return true;
                }
            }

            // 슬롯에 들어갈 곳이 없음
            ShowWarningForSeconds();
            return false;
        }

        public bool AddCrew(int crewId)
        {
            if (crewId < 0 || crewId >= crewDefinitions.Count)
            {
                Debug.LogWarning($"Crew ID {crewId} is out of range.");
                return false;
            }

            var def = crewDefinitions[crewId];
            int size = def.crewSlotSize;

            for (int i = 0; i <= slots.Count - size; i++)
            {
                bool canFit = true;
                for (int j = 0; j < size; j++)
                {
                    if (slots[i + j].data != null)
                    {
                        canFit = false;
                        break;
                    }
                }

                if (canFit)
                {
                    for (int j = 0; j < size; j++)
                    {
                        slots[i + j].data = def;
                        slots[i + j].amount = 1;
                        slots[i + j].image.sprite = def.sprite;
                        slots[i + j].text.text = "";
                    }
                    return true;
                }
            }

            ShowWarningForSeconds();
            return false;
        }

        public int GetAvailableAddAmount(int itemId)
        {
            if (itemId < 0 || itemId >= itemDefinitions.Count)
            {
                Debug.LogWarning($"Item ID {itemId} is out of range.");
                return 0;
            }

            var def = itemDefinitions[itemId];
            int maxAmount = def.amountLimit;

            foreach (var slot in slots)
            {
                if (slot.data == def && slot.amount < maxAmount)
                {
                    return maxAmount - slot.amount;
                }
            }

            foreach (var slot in slots)
            {
                if (slot.data == null)
                {
                    return maxAmount;
                }
            }
            ShowWarningForSeconds();
            return 0;
        }

        public List<int> returnItem()
        {
            int[] itemCounts = new int[itemDefinitions.Count];

            foreach (var slot in slots)
            {
                if (slot.data is ItemDefinition def)
                {
                    int index = itemDefinitions.IndexOf(def);
                    if (index >= 0)
                        itemCounts[index] += slot.amount;
                }
            }

            return itemCounts.ToList();
        }

        public List<int> returnCrew()
        {
            int[] crewReturned = new int[crewDefinitions.Count];

            foreach (var slot in slots)
            {
                if (slot.data is CrewDefinition def)
                {
                    int index = crewDefinitions.IndexOf(def);
                    if (index >= 0)
                        crewReturned[index] = 1;
                }
            }

            return crewReturned.ToList();
        }


        public void flushAll()
        {
            foreach (var slot in slots)
            {
                slot.data = null;
                slot.amount = 0;
                slot.image.sprite = null;
                slot.text.text = "";
            }
        }

        private void ShowWarningForSeconds(float duration = 1.5f)
        {
            if (warningUI == null) return;

            if (warningCoroutine != null)
                StopCoroutine(warningCoroutine);

            warningCoroutine = StartCoroutine(ShowWarningCoroutine(duration));
        }

        private IEnumerator ShowWarningCoroutine(float duration)
        {
            warningUI.SetActive(true);
            interactionUI.SetActive(false);
            yield return new WaitForSeconds(duration);
            warningUI.SetActive(false);
            interactionUI.SetActive(true);
        }

    }
}
