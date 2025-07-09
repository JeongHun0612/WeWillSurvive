using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace WeWillSurvive.Character
{
    [Flags]
    public enum EState
    {
        [Description("정상")]
        Normal = 0,
        [Description("허기짐")]
        Hungry = 1 << 0,
        [Description("영양 결핍 (굶음)")]
        Starve = 1 << 1,
        [Description("갈증")]
        Thirsty = 1 << 2,
        [Description("수분 고갈 (탈수)")]
        Dehydrate = 1 << 3,
        [Description("다침")]
        Injured = 1 << 4,
        [Description("병듦")]
        Sick = 1 << 5,
        [Description("불안함")]
        Anxious = 1 << 6,
        [Description("공포")]
        Panic = 1 << 7,
        [Description("미침")]
        Mad = 1 << 8,
        [Description("탐사")]
        Exploring = 1 << 9,
        [Description("사망")]
        Dead = 1 << 10,
    }

    public class CharacterState
    {
        public EState CurrentState { get; private set; }

        public void SetState(EState state)
        {
            if (CurrentState == EState.Dead) return;

            CurrentState = state;
        }

        public void AddState(EState status)
        {
            if (CurrentState == EState.Dead) return;

            CurrentState |= status;
        }

        public void RemoveState(EState status)
        {
            CurrentState &= ~status;
        }

        public bool HasState(EState status)
        {
            return CurrentState.HasFlag(status);
        }

        public List<string> FormatStateString()
        {
            if (CurrentState == EState.Normal)
                return new List<string>() { GetStateText(CurrentState) };

            var descriptions = new List<string>();

            foreach (EState state in Enum.GetValues(typeof(EState)))
            {
                if (state == EState.Normal || !HasState(state)) continue;

                descriptions.Add(GetStateText(state));
            }

            return descriptions;
        }

        private string GetStateText(EState state)
        {
            return state switch
            {
                EState.Normal => "Normal",
                EState.Hungry => "Hungry",
                EState.Starve => "Starve",
                EState.Thirsty => "Thirsty",
                EState.Dehydrate => "Dehydrate",
                EState.Injured => "Injured",
                EState.Sick => "Sick",
                EState.Anxious => "Anxious",
                EState.Panic => "Panic",
                EState.Mad => "Mad",
                EState.Exploring => "Exploring",
                EState.Dead => "Dead",
                _ => string.Empty
            };
        }
    }
}
