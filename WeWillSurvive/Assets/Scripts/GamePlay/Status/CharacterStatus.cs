using System.Collections.Generic;
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

            AddStatus(new HungerStatus(_owner));
            AddStatus(new ThirstStatus(_owner));
        }

        public void OnNewDay()
        {
            foreach (var status in _statuses.Values)
            {
                status.OnNewDay(_owner);

                 //Debug.Log($"{_owner.Name} - {status.StatusType} : {status.CurrentValue}");

                if (_owner.IsDead) break;
            }
        }

        public void AddStatus(IStatus status)
        {
            if (!_statuses.ContainsKey(status.StatusType))
            {
                _statuses.Add(status.StatusType, status);
            }
        }

        public void RemoveStatus(EStatusType type)
        {
            if (_statuses.ContainsKey(type))
            {
                _statuses.Remove(type);
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
    }
}
