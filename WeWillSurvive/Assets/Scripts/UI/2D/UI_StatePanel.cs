using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.Item;
using WeWillSurvive.UI;
using static Define;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace WeWillSurvive
{
    public class UI_StatePanel : UI_Popup
    {
        [SerializeField] private Transform _state;
        [SerializeField] private GameObject _stateContent;

        private const int MAX_STATE = 5;

        public override void Initialize()
        {
            base.Initialize();
            for (int i = 0; i < MAX_STATE; i++)
                Instantiate(_stateContent, _state);
        }

        public void SetPanel(EItem item)
        {
            if (!_initialized)
                Initialize();

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
                            List<string> descriptions = ServiceLocator.Get<CharacterManager>().GetCharacter(character)?.State.FormatStateString();

                            int idx = 0;
                            for (int i = 0; i < MAX_STATE; i++)
                            {
                                GameObject go = _state.GetChild(idx++).gameObject;
                                go.SetActive(true);
                                if (i < descriptions.Count)
                                    go.GetComponent<TextMeshProUGUI>().text = descriptions[i];
                                else
                                    go.GetComponent<TextMeshProUGUI>().text = string.Empty;
                            }
                        }
                    }
                    break;
                case EItem.Food:
                case EItem.Water:
                    {
                        float cnt = ServiceLocator.Get<ItemManager>().GetItemCount(item);
                        GameObject go = _state.GetChild(0).gameObject;
                        go.SetActive(true);
                        go.GetComponent<TextMeshProUGUI>().text = $"{item.ToString()}: {cnt}";
                    }
                    break;
                default:
                    {
                        float cnt = ServiceLocator.Get<ItemManager>().GetItemCount(item);
                        GameObject go = _state.GetChild(0).gameObject;
                        go.SetActive(true);
                        go.GetComponent<TextMeshProUGUI>().text = $"{item.ToString()}";
                    }
                    break;
            }
        }
    }
}
