using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.GameEvent;

namespace WeWillSurvive.Room
{
    public class EventScriptPanel : MonoBehaviour
    {
        [Header("이벤트 스크립트 패널")]
        [SerializeField] private RectTransform _panel;

        [Header("이벤트 내용 텍스트")]
        [SerializeField] private TMP_Text _eventText;

        [Header("이벤트 확률 텍스트")]
        [SerializeField] private GameObject _eventRatePanel;
        [SerializeField] private TMP_Text _totalRateText;
        [SerializeField] private TMP_Text _descRateText;

        [Header("이벤트 선택지 옵션")]
        [SerializeField] private List<CharacterEventChoiceOption> _choiceOptions = new();

        [Header("Tweening 옵션")]
        [SerializeField] private float _duration = 0.8f;
        [SerializeField] private Ease _easeType = Ease.InOutExpo;


        private Vector2 _panelStartPosition;
        private float _panelTargetY;

        private Tween _tween;

        private CharacterEventChoiceOption _selectedChoiceOption;
        public CharacterEventChoiceOption SelectedChoiceOption => _selectedChoiceOption;

        public void Initialize()
        {
            foreach (var choiceOption in _choiceOptions)
            {
                choiceOption.Initialize(OnClickEventChoiceOption);
            }

            _panelStartPosition = _panel.anchoredPosition;
            _panelStartPosition.y = -(_panel.rect.height);
            _panelTargetY = 30f;
        }

        public void ShowPanel()
        {
            _panel.anchoredPosition = _panelStartPosition;

            _tween?.Kill();
            _tween = _panel.DOAnchorPosY(_panelTargetY, _duration)
                .SetEase(_easeType);
        }

        public void UpdateEventScriptPanel(DailyCharacterEvent dailyCharacterEvent, CharacterBase owner)
        {
            _selectedChoiceOption = null;

            // 이벤트 텍스트 업데이트
            _eventText.text = dailyCharacterEvent.DailyEventData.GetRandomDescription();

            // 성공 확률 텍스트 업데이트
            if (owner.Type != ECharacter.Lead)
            {
                var totalRate = owner.EventBaseRate + owner.EventStateModifier + owner.EventSelectionModifier;
                _totalRateText.text = $"성공 확률 : {totalRate}%";
                _descRateText.text = $"기본 확률({owner.EventBaseRate})% + 캐릭터 상태({owner.EventStateModifier}%) + 리드이벤트({owner.EventSelectionModifier}%)";
            }
            else
            {
                _eventRatePanel.SetActive(false);
            }

            // ChoiceOption 업데이트
            UpdateChoiceOptions(dailyCharacterEvent.DailyEventData);
        }

        private void UpdateChoiceOptions(MainEventData mainEventData)
        {
            foreach (var choiceOption in _choiceOptions)
                choiceOption.Disabeld();

            int index = 0;
            foreach (var choice in mainEventData.Choices)
            {
                if (choice.ChoiceIcon == EChoiceIcon.Noting)
                    continue;

                _choiceOptions[index].UpdateChoiceOption(choice);
                index++;
            }
        }

        public void OnClickEventChoiceOption(CharacterEventChoiceOption choiceOption)
        {
            if (_selectedChoiceOption != null && _selectedChoiceOption == choiceOption)
            {
                _selectedChoiceOption.OnSelected(false);
                _selectedChoiceOption = null;
            }
            else
            {
                if (_selectedChoiceOption != null)
                    _selectedChoiceOption.OnSelected(false);

                _selectedChoiceOption = choiceOption;
                _selectedChoiceOption.OnSelected(true);
            }
        }
    }
}
