using System;
using TMPro;
using WeWillSurvive.UI;
using UnityEngine;

namespace WeWillSurvive
{
    public class UI_StatePanel : UI_Popup
    {
        [SerializeField] private Transform _state;

        public override void Initialize()
        {
            base.Initialize();
        }

        public void SetPanel(Define.ECharacter owner)
        {
            CharacterInfo info = CharacterManager.Instance.CharacterInfos[(int)owner];

            int idx = 0;
            foreach (Define.ECharacterState state in info.State)
            {
                _state.GetChild(idx++).GetComponent<TextMeshProUGUI>().text
                    = Enum.GetName(typeof(Define.ECharacterState), state);
            }
        }
    }
}
