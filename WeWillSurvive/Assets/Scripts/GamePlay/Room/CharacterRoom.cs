using Cysharp.Threading.Tasks;
using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.GameEvent;

namespace WeWillSurvive.Room
{
    public class CharacterRoom : RoomBase
    {
        [SerializeField] private GameObject _lightOff;
        [SerializeField] private UI_Character _characterUI;
        [SerializeField] private EventScriptPanel _eventScriptPanel;

        private bool _hasCharacterEvent;

        private EventBus EventBus => ServiceLocator.Get<EventBus>();

        public async override UniTask InitializeAsync()
        {
            await base.InitializeAsync();

            if (_characterUI != null)
                _characterUI.Initialize();

            if (_eventScriptPanel != null)
                _eventScriptPanel.Initialize();

            EventBus.Subscribe<EndDayEvent>(OnEndDayEvent);

            await UniTask.CompletedTask;
        }

        public override void OnShowRoom()
        {
            base.OnShowRoom();

            if (_eventScriptPanel != null)
                _eventScriptPanel.ShowPanel();
        }

        public override void UpdateRoom()
        {
            _hasCharacterEvent = false;

            _lightOff.SetActive(_characterUI.IsNoting);
            _characterUI.UpdateCharacterImage(ERoomType.CharacterRoom);

            var dailyCharacterEvent = GameEventManager.Instance.DailyCharacterEvent;
            if (dailyCharacterEvent != null && dailyCharacterEvent.Character == _characterUI.CharacterType)
            {
                _eventScriptPanel.UpdateEventScriptPanel(dailyCharacterEvent, _characterUI.Owner);
                _hasCharacterEvent = true;
            }
        }

        private void OnEndDayEvent(EndDayEvent context)
        {
            if (!_hasCharacterEvent)
                return;

            var dailyCharacterEvent = GameEventManager.Instance.DailyCharacterEvent;
            var choiceOption = _eventScriptPanel.SelectedChoiceOption;

            // ������ �ɼǿ� ���� �̺�Ʈ ���̽� �Ҵ�
            var eventChoice = (choiceOption == null) ? dailyCharacterEvent.DailyEventData.GetEventChoice(EChoiceIcon.Noting) : choiceOption.EventChoice;

            // �̺�Ʈ ��� �ӽ� ����
            GameEventManager.Instance.SelectedCharacterEventChoice(eventChoice);
        }
    }
}
