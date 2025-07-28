using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using WeWillSurvive.UI;

namespace WeWillSurvive
{
    public class UI_HUD : UI_Popup
    {
        [SerializeField] private TMP_Text _dayText;
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;

        private ERoom _currentRoom;

        private EventBus EventBus => ServiceLocator.Get<EventBus>();

        public async override UniTask InitializeAsync()
        {
            EventBus.Subscribe<NewDayEvent>(OnNewDayEvent);
            EventBus.Subscribe<MoveRoomCompleteEvent>(OnMoveRoomCompleteEvent);

            _leftButton.onClick.AddListener(() => MoveRoom(_currentRoom - 1));
            _rightButton.onClick.AddListener(() => MoveRoom(_currentRoom + 1));

            await UniTask.Yield();
        }

        private void MoveRoom(ERoom targetRoom)
        {
            // targetRoom이 범위를 벗어나면
            if (targetRoom < 0 || targetRoom >= ERoom.MaxCount)
                return;

            EventBus.Publish(new MoveRoomEvent { TargetRoom = targetRoom });
        }

        private void UpdateMoveButton()
        {
            // 버튼 조건 체크
            _leftButton.interactable = _currentRoom > 0;
            _rightButton.interactable = _currentRoom < ERoom.MaxCount - 1;
        }

        public void OnClickRoomMonitor()
        {
            UIManager.Instance.ShowPopup<UI_RoomMonitor>();
        }

        private void OnNewDayEvent(NewDayEvent context)
        {
            _currentRoom = ERoom.Main;
            _dayText.text = $"Day {context.CurrentDay}";

            UpdateMoveButton();
        }

        private void OnMoveRoomCompleteEvent(MoveRoomCompleteEvent context)
        {
            _currentRoom = context.CurrentRoom;
            UpdateMoveButton();
        }
    }
}
