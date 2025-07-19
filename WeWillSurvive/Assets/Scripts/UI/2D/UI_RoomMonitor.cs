using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.UI;
using static Define;

namespace WeWillSurvive
{
    public class UI_RoomMonitor : UI_Popup
    {
        [SerializeField] Button _leadButton;
        [SerializeField] Button _cookButton;
        [SerializeField] Button _bellButton;
        [SerializeField] Button _drKButton;

        private UI_Background _ui;
        private Button[] _buttons = new Button[4];

        private EventBus EventBus => ServiceLocator.Get<EventBus>();

        public async override UniTask InitializeAsync()
        {
            _buttons[(int)ECharacter.Lead] = _leadButton;
            _buttons[(int)ECharacter.Cook] = _cookButton;
            _buttons[(int)ECharacter.Bell] = _bellButton;
            _buttons[(int)ECharacter.DrK] = _drKButton;

            foreach (CharacterBase character in ServiceLocator.Get<CharacterManager>().GetAllCharacters())
            {
                string roomName = Enum.GetName(typeof(ECharacter), character.Data.Type);
                Enum.TryParse(roomName, out ERoom room);
                _buttons[(int)character.Data.Type].onClick.AddListener(() =>
                    EventBus.Publish(new MoveRoomEvent() { TargetRoom = room })
                );
            }

            await UniTask.CompletedTask;
        }

        public override void OnShow()
        {
            base.OnShow();

            // 우주 기지 내 존재하지 않거나 죽으면 방 불 꺼짐 + 클릭 못함
            foreach (CharacterBase character in ServiceLocator.Get<CharacterManager>().GetAllCharacters())
            {
                if (character.State.HasState(EState.Exploring) || character.State.HasState(EState.Dead))
                {
                    _buttons[(int)character.Data.Type].GetComponent<Image>().color = new Color32(100, 100, 100, 255);
                    _buttons[(int)character.Data.Type].interactable = false;
                }
                else
                {
                    _buttons[(int)character.Data.Type].interactable = true;
                }
            }
        }
    }
}