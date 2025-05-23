using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using WeWillSurvive.UI;
using static Define;

namespace WeWillSurvive
{
    public class UI_Room : UI_Popup
    {
        // TODO: 사기 + 상태별로 게임 오브젝트 만들어서 배열에 저장
        [SerializeField] GameObject _character;

        UI_Background ui;
        CharacterInfo _characterInfo = null;
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
            if (_characterInfo == null && owner != ECharacter.MaxCount)
            {
                _owner = (int)owner;
                _characterInfo = CharacterManager.Instance.CharacterInfos[_owner];
            }

            if (_characterInfo == null || _character == null)
            {
                Debug.LogError($"[{gameObject.name}] Character 찾을 수 없음");
                return;
            }

            List<ECharacterState> state = _characterInfo.State;
            // 우주 기지 내 존재하지 않거나 죽은 경우
            if (state[0] == ECharacterState.None || state[0] == ECharacterState.Dead)
            {
                // TODO: 빔 프로젝터 비활성화

                // 캐릭터 비활성화
                _character.SetActive(false);
            }
        }
    }
}