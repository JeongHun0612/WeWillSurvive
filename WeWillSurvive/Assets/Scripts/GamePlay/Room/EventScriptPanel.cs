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
        [Header("�̺�Ʈ ��ũ��Ʈ �г�")]
        [SerializeField] private RectTransform _panel;

        [Header("�̺�Ʈ ���� �ؽ�Ʈ")]
        [SerializeField] private TMP_Text _eventText;

        [Header("�̺�Ʈ Ȯ�� �ؽ�Ʈ")]
        [SerializeField] private TMP_Text _totalRateText;
        [SerializeField] private TMP_Text _descRateText;

        [Header("�̺�Ʈ ������ �ɼ�")]
        [SerializeField] private List<CharacterEventChoiceOption> _choiceOptions = new();

        [Header("Tweening �ɼ�")]
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

            // �̺�Ʈ �ؽ�Ʈ ������Ʈ
            _eventText.text = dailyCharacterEvent.DailyEventData.GetRandomDescription();

            // ���� Ȯ�� �ؽ�Ʈ ������Ʈ
            var totalRate = owner.EventBaseRate + owner.EventStateModifier + owner.EventSelectionModifier;
            _totalRateText.text = $"���� Ȯ�� : {totalRate}%";
            _descRateText.text = $"�⺻ Ȯ��({owner.EventBaseRate})% + ĳ���� ����({owner.EventStateModifier}%) + �����̺�Ʈ({owner.EventSelectionModifier}%)";

            // ChoiceOption ������Ʈ
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
