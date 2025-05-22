using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using static Define;

namespace WeWillSurvive
{
    public class UI_Room : UI_Popup
    {
        [SerializeField] TextMeshProUGUI _ownerText;

        UI_Background ui;
        CharacterInfo _player = null;
        int _owner = -1;

        protected override void Init()
        {
            base.Init();

            ui = null;
            if (GameManager.Instance.SceneUI is not UI_Background)
            {
                Debug.LogError("[UI_Room] 2D Scene에서 열리지 않았음");
                return;
            }
            ui = GameManager.Instance.SceneUI as UI_Background;
        }

        // 방 세팅
        public void SetupRoomUI(ECharacter owner = ECharacter.MaxCount)
        {
            if (_player == null && owner != ECharacter.MaxCount)
            {
                _owner = (int)owner;
                _player = CharacterManager.Instance.CharacterInfos[_owner];
            }

            if (_player == null) return;

            // Test
            _ownerText.text = Enum.GetName(typeof(ECharacter), owner);

            ECharacterStatus status = _player.Status;

            // 우주 기지 내 존재하지 않으면 방 불 꺼짐
            if (status == ECharacterStatus.None)
                ui.LightOff();

            // 정상 상태 아니면 빔 프로젝터 비활성화
            if (status != ECharacterStatus.Normal)
            {
                // TODO: 빔 프로젝터 비활성화
            }
        }
    }
}