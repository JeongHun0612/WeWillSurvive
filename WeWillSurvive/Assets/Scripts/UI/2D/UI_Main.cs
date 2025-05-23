using Cysharp.Threading.Tasks;
using NUnit.Framework;
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
    public class UI_Main : UI_Popup
    {
        [SerializeField] private Button _roomMonitorButton;
        [SerializeField] private Button _nextDayButton;
        [SerializeField] private TextMeshProUGUI _dayText;

        // TODO: ��� + ���º��� �÷��̾� �̹��� �迭�� �־ ����
        // _characterImages[ECharacter.MaxCount][6? 7?]

        UI_Background ui;

        protected override void Init()
        {
            base.Init();

            ui = null;
            if (GameManager.Instance.SceneUI is not UI_Background)
            {
                Debug.LogError("[UI_Main] 2D Scene���� ������ �ʾ���");
                return;
            }
            ui = GameManager.Instance.SceneUI as UI_Background;

            // Room Monitor
            _roomMonitorButton.onClick.AddListener(() =>
                ServiceLocator.Get<ResourceService>().LoadAsset("UI_RoomMonitor").ContinueWith(prefab => Instantiate(prefab)).Forget());

            // Next Day
            _nextDayButton.onClick.AddListener(() => GameManager.Instance.BlackUI.FadeIO(() => NextDay()));

            UpdateUI();
        }

        private void NextDay()
        {
            // �� �������Ϳ��� ���� ����
            // 1. ����� ������
            // Key���� Value��ŭ ��� (��� ����� Ư���ϴ� �������� �ƴϸ� �ƹ��ų� ������ ��)
            Dictionary<ECharacter, float>[] UseItems = new Dictionary<ECharacter, float>[(int)EItem.MaxCount];
            for (int i = 0; i < (int)EItem.MaxCount; i++)
                UseItems[i] = new Dictionary<ECharacter, float>();


            // ���� ������ ���� Ȯ��
            for (int i = 0; i < (int)EItem.MaxCount; i++)
            {
                float useCount = 0;
                foreach (float cnt in UseItems[i].Values) useCount += cnt;
                if (useCount == 0) continue;

                float remainCount = GameManager.Instance.GetItemCount((EItem)i);
                if (remainCount < useCount)
                {
                    Debug.LogError("���� �����ۺ��� ����� �������� ���� - UI ǥ�� ����");
                    return;
                }
            }

            // ������ ���
            string s = "";
            for (int i = 0; i < (int)EItem.MaxCount; i++)
            {
                float useCount = 0;
                foreach (KeyValuePair<ECharacter, float> useItem in UseItems[i])
                {
                    GameManager.Instance.UseItem((EItem)i, useItem.Key, useItem.Value);
                    useCount += useItem.Value;
                }

                // Debug
                if (useCount > 0)
                    s += $"{Enum.GetName(typeof(EItem), i)} {useCount}��, ";
            }
            if (s == "") s = "����";
            Debug.Log($"[Day {GameManager.Instance.Day}] ����� ������: " + s);

            // 2. Ž�� ���� ĳ���� (���� �� ������ ��� ������ ����)
            ECharacter exploreCharacter = ECharacter.MaxCount;
            if (exploreCharacter != ECharacter.MaxCount)
            {
                CharacterManager.Instance.CharacterInfos[(int)exploreCharacter].SetState(ECharacterState.None);
            }

            // 3. �̺�Ʈ?
            // �̺�Ʈ ���� �Լ� ���� ȣ��

            // Day + 1
            GameManager.Instance.Day += 1;

            UpdateUI();
        }

        private void UpdateUI()
        {
            // Popup UI �ʱ�ȭ
            GameManager.Instance.ClosePopupUIs(remain: 1);

            _dayText.text = "Day " + GameManager.Instance.Day;

            // ĳ���� ���� ������Ʈ
            CharacterManager.Instance.UpdateCharacterInfos();

            // ĳ���� �̹��� ������Ʈ
            CharacterInfo[] infos = CharacterManager.Instance.CharacterInfos;
            foreach (CharacterInfo info in infos)
            {
                // ���� ���� �� �������� ������ ĳ���� ��Ȱ��ȭ
                if (info.State[0] == ECharacterState.None)
                {
                    Transform t = transform.Find($"Characters/{info.Name}");
                    if (t != null)
                        t.gameObject.SetActive(false);
                }
                // TODO: ���¿� ���� ��������Ʈ ����
            }

            // TODO: ������ ��ġ
            float cnt = GameManager.Instance.GetItemCount(EItem.Water);
            // �� ���� ��ŭ ��ġ
        }
    }
}