using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.UI;
using WeWillSurvive.Item;
using static Define;

namespace WeWillSurvive
{
    public class UI_Main : UI_Popup
    {
        [SerializeField] private Button _roomMonitorButton;
        [SerializeField] private Button _nextDayButton;
        [SerializeField] private TextMeshProUGUI _dayText;

        [Header ("Items")]
        [SerializeField] private Transform _foods;
        [SerializeField] private Transform _waters;

        UI_Background ui;

        public override void Initialize()
        {
            base.Initialize();

            ui = UIManager.Instance.GetCurrentScene<UI_Background>();
            if (ui == null)
            {
                Debug.LogError($"[{name}] 2D Scene에서 열리지 않았음");
                return;
            }

            // Room Monitor
            _roomMonitorButton.onClick.AddListener(() => UIManager.Instance.ShowPopup<UI_RoomMonitor>());

            // Next Day
            _nextDayButton.onClick.AddListener(() => UIManager.Instance.BlackUI.FadeIO(() =>
            {
                GameManager.Instance.NewDay();
                UpdateUI();
            }));

            UpdateUI();
        }

        public override void OnShow()
        {
            base.OnShow();

            UpdateUI();
        }

        private void UpdateUI()
        {
            // Popup UI 초기화
            UIManager.Instance.ClosePopups(remain: 1);

            _dayText.text = "Day " + GameManager.Instance.Day;

            CharacterManager characterManager = ServiceLocator.Get<CharacterManager>();

            // 캐릭터 이미지 업데이트
            foreach (CharacterBase character in characterManager.GetAllCharacters())
            {
                Transform t = transform.Find($"Characters/{character.Name}");
                if (t == null || !t.gameObject.activeSelf) continue;

                // 우주 기지 내 존재하지 않으면 캐릭터 비활성화
                if (character.State.HasState(EState.Exploring))
                    t.gameObject.SetActive(false);
                else
                    t.GetChild(0).GetComponent<Image>().sprite = character.MainSprite;
            }

            // TODO: 아이템 배치
            float cnt;

            cnt = MathF.Ceiling(ServiceLocator.Get<ItemManager>().GetItemCount(EItem.Water));
            for (int i = _waters.childCount - 1; i >= 0; i--)
                _waters.GetChild(i).gameObject.SetActive((cnt--) > 0);

            cnt = MathF.Ceiling(ServiceLocator.Get<ItemManager>().GetItemCount(EItem.Food));
            for (int i = _foods.childCount - 1; i >= 0; i--)
                _foods.GetChild(i).gameObject.SetActive((cnt--) > 0);
        }
    }
}