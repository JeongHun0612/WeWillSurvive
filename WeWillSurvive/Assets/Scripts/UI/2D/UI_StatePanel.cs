using System;
using TMPro;
using WeWillSurvive.UI;
using WeWillSurvive.Character;
using UnityEngine;
using WeWillSurvive.Core;
using System.Collections.Generic;

namespace WeWillSurvive
{
    public class UI_StatePanel : UI_Popup
    {
        [SerializeField] private Transform _state;

        public override void Initialize()
        {
            base.Initialize();
        }

        public void SetPanel(ECharacter owner)
        {
            List<string> descriptions = ServiceLocator.Get<CharacterManager>().GetCharacter(owner)?.State.FormatStateString();

            int idx = 0;
            foreach (string description in descriptions)
                _state.GetChild(idx++).GetComponent<TextMeshProUGUI>().text = description;
        }
    }
}
