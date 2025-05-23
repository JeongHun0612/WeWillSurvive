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
        // TODO: ��� + ���º��� ���� ������Ʈ ���� �迭�� ����
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
                Debug.LogError("[UI_Room] 2D Scene���� ������ �ʾ���");
                return;
            }
            ui = GameManager.Instance.SceneUI as UI_Background;
        }

        // �� ����
        public void SetupRoomUI(ECharacter owner = ECharacter.MaxCount)
        {
            if (_characterInfo == null && owner != ECharacter.MaxCount)
            {
                _owner = (int)owner;
                _characterInfo = CharacterManager.Instance.CharacterInfos[_owner];
            }

            if (_characterInfo == null || _character == null)
            {
                Debug.LogError($"[{gameObject.name}] Character ã�� �� ����");
                return;
            }

            List<ECharacterState> state = _characterInfo.State;
            // ���� ���� �� �������� �ʰų� ���� ���
            if (state[0] == ECharacterState.None || state[0] == ECharacterState.Dead)
            {
                // TODO: �� �������� ��Ȱ��ȭ

                // ĳ���� ��Ȱ��ȭ
                _character.SetActive(false);
            }
        }
    }
}