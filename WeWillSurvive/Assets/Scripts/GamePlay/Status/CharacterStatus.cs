using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Character;

namespace WeWillSurvive.Status
{
    public class CharacterStatus
    {
        private readonly CharacterBase _owner;
        private Dictionary<EStatusType, IStatus> _statuses = new();

        public CharacterStatus(CharacterBase owner)
        {
            _owner = owner;

            ResetStatus();
        }

        public void ResetStatus()
        {
            _statuses.Clear();

            AddStatus(EStatusType.Hunger);
            AddStatus(EStatusType.Thirst);

            // Debug
            //AddStatus(new InjuryStatus(_owner));
            //AddStatus(new AnxiousStatus(_owner));
        }

        public void OnNewDay()
        {
            foreach (var status in _statuses.Values)
            {
                status.OnNewDay();

                if (_owner.IsDead) break;
            }
        }

        public void ApplyExpeditionResults()
        {
            foreach (var status in _statuses.Values)
            {
                status.OnExpeditionResult();

                if (_owner.IsDead) break;
            }
        }

        public void AddStatus(EStatusType type)
        {
            if (!HasStatus(type))
            {
                switch (type)
                {
                    case EStatusType.Hunger:
                        _statuses.Add(type, new HungerStatus(_owner));
                        break;
                    case EStatusType.Thirst:
                        _statuses.Add(type, new ThirstStatus(_owner));
                        break;
                    case EStatusType.Injury:
                        _statuses.Add(type, new InjuryStatus(_owner));
                        break;
                    case EStatusType.Anxious:
                        _statuses.Add(type, new AnxiousStatus(_owner));
                        break;
                    default:
                        Debug.LogWarning($"StatusType에는 {type}이 존재하지 않습니다.");
                        break;
                }
            }
            else
            {
                Debug.LogWarning($"이미 [{type}]이 존재합니다.");
            }
        }

        public void RemoveStatus(EStatusType type)
        {
            if (HasStatus(type))
            {
                _statuses.Remove(type);
                Debug.Log($"[{type}] 상태 제거");
            }
        }

        public void WorsenStatus(EStatusType type, int step = 1)
        {
            var status = GetStatus<IStatus>(type);

            if (status == null)
            {
                AddStatus(type);
            }
            else
            {
                status.WorsenStatus(step);
            }
        }

        public void RecoveryStatus(EStatusType type, int step = 1)
        {
            var status = GetStatus<IStatus>(type);

            if (status != null)
            {
                status.RecoveryStatus(step);
            }
        }

        public T GetStatus<T>(EStatusType type) where T : class, IStatus
        {
            if (_statuses.TryGetValue(type, out var status))
            {
                return (T)status;
            }

            return null;
        }

        public bool HasStatus(EStatusType type)
        {
            return _statuses.ContainsKey(type);
        }
    }
}
