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

        // TODO: 사기 + 상태별로 플레이어 이미지 배열에 넣어서 저장
        // _characterImages[ECharacter.MaxCount][6? 7?]

        UI_Background ui;

        protected override void Init()
        {
            base.Init();

            ui = null;
            if (GameManager.Instance.SceneUI is not UI_Background)
            {
                Debug.LogError("[UI_Main] 2D Scene에서 열리지 않았음");
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
            // 빔 프로젝터에서 받을 정보
            // 1. 사용한 아이템
            // Key에게 Value만큼 사용 (사용 대상을 특정하는 아이템이 아니면 아무거나 넣으면 됨)
            Dictionary<ECharacter, float>[] UseItems = new Dictionary<ECharacter, float>[(int)EItem.MaxCount];
            for (int i = 0; i < (int)EItem.MaxCount; i++)
                UseItems[i] = new Dictionary<ECharacter, float>();


            // 남은 아이템 개수 확인
            for (int i = 0; i < (int)EItem.MaxCount; i++)
            {
                float useCount = 0;
                foreach (float cnt in UseItems[i].Values) useCount += cnt;
                if (useCount == 0) continue;

                float remainCount = GameManager.Instance.GetItemCount((EItem)i);
                if (remainCount < useCount)
                {
                    Debug.LogError("남은 아이템보다 사용한 아이템이 많음 - UI 표기 오류");
                    return;
                }
            }

            // 아이템 사용
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
                    s += $"{Enum.GetName(typeof(EItem), i)} {useCount}개, ";
            }
            if (s == "") s = "없음";
            Debug.Log($"[Day {GameManager.Instance.Day}] 사용한 아이템: " + s);

            // 2. 탐사 보낼 캐릭터 (여러 명 보내는 경우 있으면 수정)
            ECharacter exploreCharacter = ECharacter.MaxCount;
            if (exploreCharacter != ECharacter.MaxCount)
            {
                CharacterManager.Instance.CharacterInfos[(int)exploreCharacter].SetState(ECharacterState.None);
            }

            // 3. 이벤트?
            // 이벤트 별로 함수 만들어서 호출

            // Day + 1
            GameManager.Instance.Day += 1;

            UpdateUI();
        }

        private void UpdateUI()
        {
            // Popup UI 초기화
            GameManager.Instance.ClosePopupUIs(remain: 1);

            _dayText.text = "Day " + GameManager.Instance.Day;

            // 캐릭터 정보 업데이트
            CharacterManager.Instance.UpdateCharacterInfos();

            // 캐릭터 이미지 업데이트
            CharacterInfo[] infos = CharacterManager.Instance.CharacterInfos;
            foreach (CharacterInfo info in infos)
            {
                // 우주 기지 내 존재하지 않으면 캐릭터 비활성화
                if (info.State[0] == ECharacterState.None)
                {
                    Transform t = transform.Find($"Characters/{info.Name}");
                    if (t != null)
                        t.gameObject.SetActive(false);
                }
                // TODO: 상태에 따라 스프라이트 변경
            }

            // TODO: 아이템 배치
            float cnt = GameManager.Instance.GetItemCount(EItem.Water);
            // 물 개수 만큼 배치
        }
    }
}