using System;
using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Expedition;
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
        private int _explorationDayCounter = 0;
        private int _maxExplorationDays = 0;

        public CharacterData Data { get; private set; }
        public CharacterState State { get; private set; }
        public CharacterStatus Status { get; private set; }
        public string Name { get; private set; }
        public EMorale Morale { get; private set; }
        public int TotalExploringCount { get; private set; }
        public bool IsExploring { get; private set; }
        public bool IsDead { get; private set; }

        public Sprite MainSprite => Data.SpriteData.GetSeatedSprite(State, Morale);
        public Sprite RoomSprite => Data.SpriteData.GetStandingSprite(State, Morale);

        public void Initialize(CharacterData data)
        {
            Data = data;
            State = new CharacterState();
            Status = new CharacterStatus(this);

            Name = data.Name;
            Morale = EMorale.Normal;
            IsExploring = false;
            IsDead = false;
        }

        public void ResetData()
        {
            State.SetState(EState.Normal);
            Status.ResetStatus();
            Morale = EMorale.Normal;
            IsExploring = false;
            IsDead = false;
        }

        public void OnNewDay()
        {
            if (IsDead) return;

            if (IsExploring)
            {
                _explorationDayCounter++;
                Debug.Log($"[{Name}] 탐사 - [{_explorationDayCounter}/{_maxExplorationDays}]일차");

                if (_explorationDayCounter >= _maxExplorationDays)
                {
                    State.SetState(EState.Normal);
                    OnExpeditionComplete();
                }
            }
            else
            {
                State.SetState(EState.Normal);
                Status.OnNewDay();
            }
        }

        public void SetMorale(EMorale morale)
        {
            Debug.Log($"[{Name}] morale is {morale}");
            Morale = morale;
        }

        public void OnDead()
        {
            if (IsDead) return;

            Debug.Log($"[{Name}] is Dead!");
            IsDead = true;

            State.SetState(EState.Dead);
        }

        public void OnExploring()
        {
            if (IsExploring) return;

            IsExploring = true;
            TotalExploringCount++;

            _maxExplorationDays = ExpeditionManager.Instance.GetRandomExpeditionDay();
            _explorationDayCounter = 0;
        }

        private void OnExpeditionComplete()
        {
            // TOOD 탐사 완료 (상태 변화 및 결과 반영 (로그, 아이템))
            Status.ApplyExpeditionResults();

            ExpeditionManager.Instance.CompleteExpedition();
            IsExploring = false;
        }
    }
}
