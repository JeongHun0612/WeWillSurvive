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
        [SerializeField] GameObject[] _characters;

        UI_Background ui;
        CharacterInfo _characterInfo = null;
        int _owner = -1;

        public override void Initialize()
        {
            base.Initialize();

            ui =  UIManager.Instance.GetCurrentScene<UI_Background>();
            if (ui == null)
            {
                Debug.LogError($"[{name}] 2D Scene���� ������ �ʾ���");
                return;
            }
        }

        // �� ����
        public void SetupRoomUI(ECharacter owner = ECharacter.MaxCount)
        {
            foreach (var c in _characters)
                c.SetActive(false);

            if (_characterInfo == null && owner != ECharacter.MaxCount)
            {
                _owner = (int)owner;
                _characterInfo = CharacterManager.Instance.CharacterInfos[_owner];
            }

            GameObject character = _characters[(int)owner];
            character.SetActive(true);

            if (_characterInfo == null || character == null)
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
                character.SetActive(false);
            }
        }
    }
}