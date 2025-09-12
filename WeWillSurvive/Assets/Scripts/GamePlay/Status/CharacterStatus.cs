using System.Collections.Generic;
using System.Linq;
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

            AddStatus<HungerStatus>();
            AddStatus<ThirstStatus>();
            AddStatus<InjuryStatus>();
            AddStatus<AnxiousStatus>();

#if UNITY_EDITOR
            if (_owner.Data.StatusDebugData != null)
            {
                int hungerLevel = (int)_owner.Data.StatusDebugData.Hunger;
                if (hungerLevel > 0)
                    GetStatus<HungerStatus>().WorsenStatus(hungerLevel);

                int thirstLevel = (int)_owner.Data.StatusDebugData.Thirst;
                if (thirstLevel > 0)
                    GetStatus<ThirstStatus>().WorsenStatus(thirstLevel);

                int injuryLevel = (int)_owner.Data.StatusDebugData.Injury;
                if (injuryLevel > 0)
                    GetStatus<InjuryStatus>().WorsenStatus(injuryLevel);

                int anxietyLevel = (int)_owner.Data.StatusDebugData.Anxiety;
                if (anxietyLevel > 0)
                    GetStatus<AnxiousStatus>().WorsenStatus(anxietyLevel);
            }
#endif
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

        public void AddStatus<T>() where T : IStatus
        {
            IStatus newStatus = (T)System.Activator.CreateInstance(typeof(T), _owner);
            EStatusType type = newStatus.StatusType;

            if (!HasStatus(type))
            {
                _statuses.Add(type, newStatus);
            }
            else
            {
                // 기존 Status를 리셋
                var status = GetStatus<IStatus>(type);
                if (status != null)
                    status.ResetStatus();
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

        public T GetStatus<T>(EStatusType type) where T : class, IStatus
        {
            if (_statuses.TryGetValue(type, out var status))
            {
                return (T)status;
            }

            Debug.LogWarning($"[{type}]의 Status가 존재하지 않습니다.");
            return null;
        }

        public T GetStatus<T>() where T : class, IStatus
        {
            return _statuses.Values.FirstOrDefault(status => status is T) as T;
        }

        public bool HasStatus(EStatusType type)
        {
            return _statuses.ContainsKey(type);
        }
    }
}