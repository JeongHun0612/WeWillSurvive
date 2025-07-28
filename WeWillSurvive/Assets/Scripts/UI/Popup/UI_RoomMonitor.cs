using Cysharp.Threading.Tasks;
using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.UI;

namespace WeWillSurvive
{
    public class UI_RoomMonitor : UI_Popup
    {
        [SerializeField] private RoomMonitorButton[] _roomMonitorButtons;

        private EventBus EventBus => ServiceLocator.Get<EventBus>();

        public async override UniTask InitializeAsync()
        {
            foreach (var roomMonitorButton in _roomMonitorButtons)
            {
                roomMonitorButton.Initialize(OnClickRoomMonitor);
            }

            EventBus.Subscribe<NewDayEvent>(OnNewDayEvent);

            await UniTask.Yield();
        }

        public void OnClickRoomMonitor(ERoom targetRoom)
        {
            EventBus.Publish(new MoveRoomEvent() { TargetRoom = targetRoom });
            ClosePopupUI();
        }

        private void OnNewDayEvent(NewDayEvent context)
        {
            foreach (var roomMonitorButton in _roomMonitorButtons)
            {
                roomMonitorButton.UpdateButtonState();
            }
        }
    }
}