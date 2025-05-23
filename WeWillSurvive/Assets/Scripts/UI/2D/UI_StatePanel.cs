using System;
using TMPro;
using UnityEngine;

namespace WeWillSurvive
{
    public class UI_StatePanel : UI_Popup
    {
        [SerializeField] private Transform _state;
        [SerializeField] private GameObject _stateText;

        protected override void Init()
        {
            base.Init();
        }

        public void SetPanel(Define.ECharacter owner)
        {
            CharacterInfo info = CharacterManager.Instance.CharacterInfos[(int)owner];

            foreach (Define.ECharacterState state in info.State)
            {
                GameObject stateText = Instantiate(_stateText, _state);
                stateText.SetActive(true);
                stateText.GetComponent<TextMeshProUGUI>().text = Enum.GetName(typeof(Define.ECharacterState), state);
            }
        }
    }
}
