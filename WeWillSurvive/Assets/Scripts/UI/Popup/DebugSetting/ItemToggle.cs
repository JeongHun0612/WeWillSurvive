using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Item;

namespace WeWillSurvive
{
    public class ItemToggle : MonoBehaviour
    {
        [SerializeField] private Toggle _toggle;

        [Header("## Count")]
        [SerializeField] private GameObject _countPanel;
        [SerializeField] private TMP_Text _countText;

        public EItem Item;
        [HideInInspector] public bool IsActive;
        [HideInInspector] public float Count = 1f;

        public void Initialize(ItemData itemData)
        {
            _toggle.onValueChanged.AddListener(OnToggleValueChanged);

            _toggle.isOn = false;
            _countPanel.SetActive(false);

            if (Count <= 0f)
                Count = 1f;

            if (Item == EItem.Lead)
            {
                _toggle.isOn = true;
                _toggle.enabled = false;
                return;
            }

            if (itemData == null)
            {
                IsActive = false;
                _toggle.isOn = IsActive;
            }
            else
            {
                IsActive = itemData.IsActive;
                _toggle.isOn = IsActive;
                Count = itemData.Count;
            }

            _countText.text = $"{Count:F2}";
        }

        public void OnToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                _countPanel.SetActive(Item == EItem.Food || Item == EItem.Water);
            }
            else
            {
                _countPanel.SetActive(false);
            }

            IsActive = isOn;
        }

        public void OnClickUP()
        {
            Count = Mathf.Min(Count + 0.25f, 10f);
            _countText.text = $"{Count:F2}";
        }

        public void OnClickDown()
        {
            Count = Mathf.Max(Count - 0.25f, 0f);
            _countText.text = $"{Count:F2}";
        }
    }
}
