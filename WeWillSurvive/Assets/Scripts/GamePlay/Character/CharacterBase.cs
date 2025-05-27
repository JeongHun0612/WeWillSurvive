using System;
using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Status;
using static Define;

namespace WeWillSurvive.Character
{
    public enum ECharacterType
    {
        Lead,
        Cook,
        DrK,
        Bell
    }

    public class CharacterBase
    {
        // public CharacterData Data { get; private set; }
        public CharacterStatus Status { get; private set; }
        public List<ECharacterState> State { get; private set; }
        public ECharacterMorale MoraleType { get; private set; }
        public string Name { get; private set; }
        public bool IsDead { get; private set; }

        public CharacterBase(ECharacterType type, ECharacterMorale moreale = ECharacterMorale.Normal)
        {
            Status = new CharacterStatus(this);
            Name = Enum.GetName(typeof(ECharacterType), type);
            MoraleType = moreale;
        }

        public virtual void Initialize(CharacterData data, ECharacterMorale moreale = ECharacterMorale.Normal)
        {
            //Data = data;
            Status = new CharacterStatus(this);
            MoraleType = moreale;
        }

        public virtual void OnDayPassed()
        {
            if (IsDead) return;

            State.Clear();

            Status.OnDayPassed();

            if (IsDead)
            {
                AddState(ECharacterState.Dead);
                return;
            }
        }

        public void AddState(ECharacterState state) => State.Add(state);

        public void OnDead()
        {
            if (IsDead) return;

            Debug.Log($"[{Name}] is Dead");
            IsDead = true;
        }
    }
}
