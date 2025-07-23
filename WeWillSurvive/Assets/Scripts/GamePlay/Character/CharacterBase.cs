using System;
using System.Collections.Generic;
using UnityEngine;
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
        public CharacterData Data { get; private set; }
        public CharacterState State { get; private set; }
        public CharacterStatus Status { get; private set; }
        public string Name { get; private set; }
        public EMorale Morale { get; private set; }
        public bool IsExploring { get; private set; }
        public bool IsDead { get; private set; }
        public Sprite MainSprite => Data.SpriteData.GetSeatedSprite(State, Morale);
        public Sprite RoomSprite => Data.SpriteData.GetStandingSprite(State, Morale);

        public event Action<EState> OnStateChangedEvent;

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

            State.SetState(EState.Normal);
            Status.OnNewDay();

            if (!IsExploring)
            {
                OnStateChangedEvent?.Invoke(State.CurrentState);
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
            OnStateChangedEvent?.Invoke(State.CurrentState);
        }
    }
}
