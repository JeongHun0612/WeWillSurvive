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
                Debug.LogError("[UI_Room] 2D Scene���� ������ �ʾ���");
                return;
            }
            ui = GameManager.Instance.SceneUI as UI_Background;
        }

        // �� ����
        public void SetupRoomUI(ECharacter owner = ECharacter.MaxCount)
        {
            if (_player == null && owner != ECharacter.MaxCount)
            {
                _owner = (int)owner;
                _player = CharacterManager.Instance.CharacterInfos[_owner];
            }

            if (_player == null) return;

            ECharacterStatus status = _player.Status;

            // ���� ���� �� �������� �ʴ� ���
            if (status == ECharacterStatus.None)
            {
                // TODO: �� �������� ��Ȱ��ȭ

                // ĳ���� ��Ȱ��ȭ
                gameObject.GetComponentInChildren<ShowStatus>().gameObject.SetActive(false);
            }
        }
    }
}