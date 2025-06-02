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
        public EMorale Moreale { get; private set; }
        public bool IsExploring { get; private set; }
        public bool IsDead { get; private set; }

        public event Action<EState> OnStateChangedEvent;

        public void Initialize(CharacterData data)
        {
            Data = data;
            State = new CharacterState();
            Status = new CharacterStatus(this);

            Name = data.Name;
            Moreale = EMorale.Normal;
            IsExploring = false;
            IsDead = false;
        }

        public void ResetData()
        {
            State.SetState(EState.Normal);
            Status.ResetStatus();
            Moreale = EMorale.Normal;
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

        public void SetMoreale(EMorale morale)
        {
            Debug.Log($"[{Name}] morale is {morale}");
            Moreale = morale;
        }

        public void OnDead()
        {
            if (IsDead) return;

            Debug.Log($"[{Name}] is Daed!");
            IsDead = true;

            State.SetState(EState.Dead);
            OnStateChangedEvent?.Invoke(State.CurrentState);
        }
    }
}
