using System;
using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.Expedition;
using WeWillSurvive.Item;
using WeWillSurvive.Log;
using WeWillSurvive.Status;

namespace WeWillSurvive.Character
{
    public enum EMorale
    {
        VeryLow,
        Low,
        Normal,
        High,
        VeryHigh
    }

    public class CharacterBase
    {
        private int _explorationDayCounter = 0;                         // 탐사 카운트
        private int _maxExplorationDays = 0;                            // 총 탐사 일자

        public CharacterData Data { get; private set; }                 // 캐릭터 데이터
        public CharacterState State { get; private set; }               // 캐릭터 상태 관리 클래스
        public CharacterStatus Status { get; private set; }             // 캐릭터 스테이터스 관리 클래스

        public string Name => Data.Name;
        public ECharacter Type => Data.Type;

        public EMorale Morale { get; private set; }                     // 캐릭터의 사기
        public bool IsExploring { get; private set; }                   // 캐릭터가 탐사를 나갔는지
        public bool IsDead { get; private set; }                        // 캐릭터가 죽었는지

        // 캐릭터 이벤트 성공 확률 관련 변수
        public float EventBaseRate { get; private set; }                // 기본 이벤트 성공 퍼센트
        public float EventStateModifier { get; set; }                   // 상태에 따라 변동되는 성공 퍼센트
        public float EventSelectionModifier { get; set; }               // 리드의 선택에 의해 변동되는 성공 퍼센트

        // 계산된 캐릭터 이벤트 성공 확률 반환
        public float EventSuccessRate
        {
            get
            {
                float calculatedRate = EventBaseRate + EventStateModifier + EventSelectionModifier;
                return Mathf.Clamp(calculatedRate, 0f, 100f);
            }
        }

        public int TotalExploringCount { get; private set; }            // 캐릭터 총 탐사 카운트
        public int TotalCharacterEventCount { get; private set; }       // 캐릭터 이벤트 총 발생 카운트


        public bool IsInShelter => !IsDead && !IsExploring;
        public Sprite MainSprite => Data.SpriteData.GetSeatedSprite(State, Morale);
        public Sprite RoomSprite => Data.SpriteData.GetStandingSprite(State, Morale);


        private LogManager LogManager => ServiceLocator.Get<LogManager>();
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public void Initialize(CharacterData data)
        {
            Data = data;
            State = new CharacterState();
            Status = new CharacterStatus(this);

            EventBaseRate = data.BaseEventRate;

            ResetData();
        }

        public void ResetData()
        {
            IsExploring = false;
            IsDead = false;

            State.SetState(EState.Normal);
            Status.ResetStatus();
            Morale = EMorale.Normal;

            EventStateModifier = Data.NormalStateModifier;
            EventSelectionModifier = 0f;

            _explorationDayCounter = 0;
            _maxExplorationDays = 0;
            TotalExploringCount = 0;
            TotalCharacterEventCount = 0;
        }

        public void OnNewDay()
        {
            if (IsDead) return;

            if (IsExploring)
            {
                _explorationDayCounter++;
                Debug.Log($"[{Name}] 탐사 - [{_explorationDayCounter}/{_maxExplorationDays}]일차");

                // 탐사 완료
                if (_explorationDayCounter >= _maxExplorationDays)
                {
                    OnExpeditionComplete();
                }
            }
            else
            {
                Status.OnNewDay();
            }
        }

        public void SetMorale(EMorale morale)
        {
            Debug.Log($"[{Name}] morale is {morale}");
            Morale = morale;
        }

        public string GetFormatStateString()
        {
            string styledName = $"<color=#000000><size=125%>{Name}</size></color>";
            string stateText = State.FormatStateString();

            return $"{styledName}\n{stateText}";
        }

        public void OnDead()
        {
            if (IsDead) return;

            Debug.Log($"[{Name}] is Dead!");
            IsDead = true;

            Status.ResetStatus();
            State.SetState(EState.Dead);

            LogManager.ClearCharacterStatusLog(Data.Type);

            // 파밍 단계에서 사망은 로그 출력 X
            if (GameManager.Instance.Day > 1)
            {
                // Dead Log 출력
                string stateMessage = Data.StateMessageData.GetStateActiveMessage(EState.Dead);
                LogManager.AddCharacterStatusLog(Data.Type, stateMessage);
            }
        }

        public void OnExploring()
        {
            if (IsExploring) return;

            IsExploring = true;
            TotalExploringCount++;

            // 탐사 기간 할당
            _maxExplorationDays = ExpeditionManager.Instance.GetRandomExpeditionDay();
            _explorationDayCounter = 0;

            // 탐사 출발 메시지 로그 전달
            string expeditionStartMessage = Data.ExpeditionMessageData.GetExpeditionStartMessage();
            LogManager.AddExpeditionResultLog(expeditionStartMessage);
        }

        private void OnExpeditionComplete()
        {
            // 탐사 후 상태 적용
            Status.ApplyExpeditionResults();

            ExpeditionManager.Instance.UpdateExpeditionState(EExpeditionState.Normal);
            IsExploring = false;

            if (IsDead)
            {
                Debug.Log($"[{Name}] 탐사 중 사망");
                var deadMessage = Data.ExpeditionMessageData.GetExpeditionDeadMessage();
                LogManager.AddExpeditionResultLog(deadMessage);
                return;
            }

            // 탐사 장소 랜덤 할당
            var expeditionData = ExpeditionManager.Instance.GetRandomExpeditionData();

            foreach (var rewardData in expeditionData.RewardDatas)
            {
                var rewardItem = rewardData.RewardItem;
                EItem item = rewardItem.Item;

                // 물과 식량을 제외한 아이템은 이미 가지고 있을 시 파밍 X
                if (item != EItem.Food && item != EItem.Water && ItemManager.HasItem(item))
                    continue;

                if (item != EItem.None)
                {
                    // 탐사 보상 아이템 추가
                    int amount = rewardItem.GetRandomAmount();
                    ItemManager.AddItem(item, amount);
                    LogManager.AddResultItemData(new ResultItemData(item, amount));
                }

                // 탐사 결과 로그
                LogManager.AddExpeditionResultLog(rewardData.ExploringMessage);
            }
        }
    }
}
