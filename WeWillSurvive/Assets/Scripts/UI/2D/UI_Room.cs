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

            ECharacterStatus status = _player.Status;

            // 우주 기지 내 존재하지 않는 경우
            if (status == ECharacterStatus.None)
            {
                // TODO: 빔 프로젝터 비활성화

                // 캐릭터 비활성화
                gameObject.GetComponentInChildren<ShowStatus>().gameObject.SetActive(false);
            }
        }
    }
}