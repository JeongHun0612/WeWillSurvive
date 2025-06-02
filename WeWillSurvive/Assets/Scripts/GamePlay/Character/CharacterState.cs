using System;
using System.Collections.Generic;
using UnityEngine;

namespace WeWillSurvive.Character
{
    [Flags]
    public enum EState
    {
        Normal = 0,             // 정상
        Hungry = 1 << 0,        // 허기짐
        Starve = 1 << 1,        // 영양 결핍 (굶음)
        Thirsty = 1 << 2,       // 갈증
        Dehydrate = 1 << 3,     // 수분 고갈 (탈수)
        Injured = 1 << 4,       // 다침
        Sick = 1 << 5,          // 병듦
        Anxious = 1 << 6,       // 불안함
        Panic = 1 << 7,         // 공포
        Mad = 1 << 8,           // 미침
        Exploring = 1 << 9,     // 나감
        Dead = 1 << 10,         // 사망
    }

    public class CharacterState : MonoBehaviour
    {
        public EState CurrentState { get; private set; }

        public void SetState(EState state)
        {
            CurrentState = state;
        }

        public void AddState(EState status)
        {
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

        public string FormatStateString()
        {
            if (CurrentState == EState.Normal)
                return GetStateText(CurrentState);

            var descriptions = new List<string>();

            foreach (EState state in Enum.GetValues(typeof(EState)))
            {
                if (state == EState.Normal || !HasState(state)) continue;

                descriptions.Add(GetStateText(state));
            }

            return string.Join("\n", descriptions);
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
