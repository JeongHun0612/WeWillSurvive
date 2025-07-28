using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using WeWillSurvive.UI;

namespace WeWillSurvive
{
    public enum ERoomType
    {
        CharacterRoom,
        MainRoom,
    }

    public enum ERoom
    {
        Lead,
        Cook,
        Main,
        DrK,
        Bell,

        MaxCount,
    }

    public class UI_Room : UI_Scene
    {
        [SerializeField] private List<RoomBase> _roomBases;

        private Dictionary<ERoom, RoomBase> _roomDicts = new();
        private RoomBase _currentRoom;

        private bool _changingRoom;

        private EventBus EventBus => ServiceLocator.Get<EventBus>();

        public async override UniTask InitializeAsync()
        {
            EventBus.Subscribe<NewDayEvent>(OnNewDay);
            EventBus.Subscribe<MoveRoomEvent>(OnMoveRoom);

            // RoomBase 초기화
            foreach (var roomBase in _roomBases)
            {
                if (!_roomDicts.ContainsKey(roomBase.RoomType))
                {
                    await roomBase.InitializeAsync();
                    roomBase.gameObject.SetActive(false);
                    _roomDicts.Add(roomBase.RoomType, roomBase);
                }
            }

            await UniTask.Yield();
        }

        public override void OnShow()
        {
            _changingRoom = false;

            UIManager.Instance.ShowPopup<UI_HUD>();
        }

        public void MoveRoom(ERoom targetRoom)
        {
            // targetRoom이 범위를 벗어나면
            if (targetRoom < 0 || targetRoom >= ERoom.MaxCount)
                return;

            // 현재 Room이 targetRoom과 동일하다면
            if (_currentRoom != null && _currentRoom.RoomType == targetRoom)
                return;

            // 변경 중이면 취소
            if (_changingRoom)
                return;

            _changingRoom = true;

            // Wipe
            UIManager.Instance.ShowOverlay<UI_Black>().Wipe(
                right: targetRoom < _currentRoom.RoomType,
                coverAction: () => SetRoom(targetRoom),
                finishAction: () => _changingRoom = false);
        }

        private void SetRoom(ERoom targetRoom)
        {
            // 현재 room이 타겟 room과 동일하면 반환
            if (_currentRoom != null && _currentRoom.RoomType == targetRoom)
                return;

            UIManager.Instance.ClosePopups(remain: 1);

            // 현재 room 비할성화
            _currentRoom?.gameObject.SetActive(false);

            // 타켓 room 활성화 및 할당
            if (_roomDicts.TryGetValue(targetRoom, out var room))
            {
                _currentRoom = room;
                room.gameObject.SetActive(true);
                EventBus.Publish(new MoveRoomCompleteEvent { CurrentRoom = _currentRoom.RoomType });
            }
            else
            {
                Debug.LogWarning($"[ChangeRoom] 대상 Room({targetRoom})이 roomDicts에 존재하지 않습니다.");
            }
        }

        private void OnNewDay(NewDayEvent context)
        {
            foreach (var roomBase in _roomDicts.Values)
            {
                roomBase.UpdateRoom();
            }

            SetRoom(ERoom.Main);
        }

        private void OnMoveRoom(MoveRoomEvent context)
        {
            MoveRoom(context.TargetRoom);
        }
    }
}