using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Item;
using WeWillSurvive.UI;
using static Define;

namespace WeWillSurvive
{
    public class UI_StatePanel : UI_Popup
    {
        [SerializeField] private Transform _state;
        [SerializeField] private GameObject _stateContent;

        private const int MAX_STATE = 5;
        private ItemManager _itemManager;
        private CharacterManager _characterManager;

        public async override UniTask InitializeAsync()
        {
            for (int i = 0; i < MAX_STATE; i++)
                Instantiate(_stateContent, _state);

            _itemManager = ServiceLocator.Get<ItemManager>();
            _characterManager = ServiceLocator.Get<CharacterManager>();

            await UniTask.CompletedTask;
        }

        public void SetPanel(EItem item)
        {
            for (int i = 0; i < MAX_STATE; i++)
                _state.GetChild(i).gameObject.SetActive(false);

            switch (item)
            {
                case EItem.Lead:
                case EItem.Cook:
                case EItem.Bell:
                case EItem.DrK:
                    {
                        if (Enum.TryParse(item.ToString(), out ECharacter character))
                        {
                            List<string> descriptions = _characterManager.GetCharacter(character)?.State.FormatStateString();

                            int idx = 0;
                            for (int i = 0; i < MAX_STATE; i++)
                            {
                                SetChildPanel(idx++, i < descriptions.Count ? descriptions[i] : string.Empty);
                            }
                        }
                    }
                    break;
                case EItem.Food:
                case EItem.Water:
                    {
                        float cnt = _itemManager.GetItemCount(item);
                        SetChildPanel(0, $"{item}: {cnt}");
                    }
                    break;
                case EItem.MedicKit:
                case EItem.RepairKit:
                    {
                        int idx = 0;
                        float cnt = _itemManager.GetItemCount(item);
                        if (cnt > 0) SetChildPanel(idx++, $"{item}");

                        cnt = _itemManager.GetItemCount(item + 1);
                        if (cnt > 0) SetChildPanel(idx, $"{item + 1}");
                    }
                    break;
                default:
                    {
                        SetChildPanel(0, $"{item}");
                    }
                    break;
            }
        }

        private void SetChildPanel(int index, string text)
        {
            GameObject go = _state.GetChild(index).gameObject;
            go.SetActive(true);
            go.GetComponent<TextMeshProUGUI>().text = text;
        }
    }
}
