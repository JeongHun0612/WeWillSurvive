using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.GameEvent;
using WeWillSurvive.Util;

namespace WeWillSurvive
{
    public class CharacterEventChoiceOption : MonoBehaviour
    {
        [Header("## Components")]
        [SerializeField] private Button _button;
        [SerializeField] private Outline _outline;

        [Header("## ChoiceIcon")]
        [SerializeField] private Image _choiceIcon;

        [Header("## Text Panel")]
        [SerializeField] private TMP_Text _choiceText;
        [SerializeField] private TMP_Text _subText;

        [Header("## Disabled Panel")]
        [SerializeField] private GameObject _disabledPanel;

        public EventChoice EventChoice { get; private set; }

        public void Initialize(Action<CharacterEventChoiceOption> callback)
        {
            if (_button != null)
                _button.onClick.AddListener(() => callback(this));
        }

        public void Disabeld()
        {
            EventChoice = null;
            gameObject.SetActive(false);
        }

        public void UpdateChoiceOption(EventChoice eventChoice)
        {
            EventChoice = eventChoice;

            // ChoiceIcon ����
            var choiceIconData = GameEventUtil.GetChoiceIconData(eventChoice.ChoiceIcon);
            _choiceIcon.sprite = choiceIconData.NormalSprite;

            // ChoiceText ����
            _choiceText.text = eventChoice.ChoiceText;

            // SubText ����
            bool subTextAvailable = eventChoice.RequiredAmount > 0;
            if (subTextAvailable)
            {
                var name = EnumUtil.GetInspectorName(eventChoice.ChoiceIcon);
                var amount = eventChoice.RequiredAmount;
                _subText.text = $"({name} {amount}�� �Ҹ�)";
            }
            _subText.gameObject.SetActive(subTextAvailable);

            // ��ư Ȱ��ȭ ���� ����
            bool isAvailable = GameEventUtil.IsAvailable(eventChoice);
            _button.interactable = isAvailable;
            _disabledPanel.SetActive(!isAvailable);

            // ��ư ���� false�� �ʱ�ȭ
            OnSelected(false);

            gameObject.SetActive(true);
        }

        public void OnSelected(bool isSelected)
        {
            _outline.enabled = isSelected;
        }
    }
}
