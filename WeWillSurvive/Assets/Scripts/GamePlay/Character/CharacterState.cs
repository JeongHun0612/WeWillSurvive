using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using WeWillSurvive.Util;

namespace WeWillSurvive.Character
{
    [Flags]
    public enum EState
    {
        [InspectorName("정상")]
        Normal = 0,

        [InspectorName("허기짐")]
        Hungry = 1 << 0,

        [InspectorName("영양 결핍")]
        Starve = 1 << 1,

        [InspectorName("갈증")]
        Thirsty = 1 << 2,

        [InspectorName("수분 고갈")]
        Dehydrate = 1 << 3,

        [InspectorName("다침")]
        Injured = 1 << 4,

        [InspectorName("병듦")]
        Sick = 1 << 5,

        [InspectorName("불안함")]
        Anxious = 1 << 6,

        [InspectorName("공포")]
        Panic = 1 << 7,

        [InspectorName("미침")]
        Mad = 1 << 8,

        [InspectorName("탐사")]
        Exploring = 1 << 9,

        [InspectorName("사망")]
        Dead = 1 << 10,
    }

    public class CharacterState
    {
        public EState CurrentState { get; private set; }

        public bool IsHurt => HasState(EState.Injured | EState.Sick);
        public bool IsAnxious => HasState(EState.Anxious | EState.Panic);
        public bool IsMad => HasState(EState.Mad);

        public bool IsExpeditionStateNormal => HasState(EState.Normal) || HasState(EState.Hungry | EState.Thirsty);
        public bool IsExpeditionStateWarning => HasState(EState.Starve | EState.Dehydrate | EState.Injured | EState.Anxious | EState.Panic);
        public bool IsExpeditionStateImpossible => HasState(EState.Sick | EState.Mad);

        public void SetState(EState state)
        {
            CurrentState = state;
        }

        public void AddState(EState state)
        {
            if (CurrentState == EState.Dead) return;

            CurrentState |= state;
        }

        public void RemoveState(EState state)
        {
            CurrentState &= ~state;
        }

        public bool HasState(EState state)
        {
            if (state == EState.Normal)
                return CurrentState == EState.Normal;

            return (CurrentState & state) != 0;
        }

        public string FormatStateString()
        {
            if (CurrentState == EState.Normal)
                return EnumUtil.GetInspectorName(EState.Normal);

            var descriptions = new List<string>();

            foreach (EState state in Enum.GetValues(typeof(EState)))
            {
                if (state == EState.Normal) continue;

                if (CurrentState.HasFlag(state))
                {
                    descriptions.Add(EnumUtil.GetInspectorName(state));
                }
            }

            return string.Join("\n", descriptions);
        }
    }
}
