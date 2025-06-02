using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace WeWillSurvive.Core
{
    public class EventBus : IService
    {
        private Dictionary<Type, List<Delegate>> _eventDictionary = new Dictionary<Type, List<Delegate>>();

        public UniTask InitializeAsync()
        {
            return UniTask.CompletedTask;
        }

        public void Subscribe<T>(Action<T> callback) where T : struct
        {
            Type eventType = typeof(T);

            if (!_eventDictionary.ContainsKey(eventType))
            {
                _eventDictionary[eventType] = new List<Delegate>();
            }

            _eventDictionary[eventType].Add(callback);
        }

        public void Unsubscribe<T>(Action<T> callback) where T : struct
        {
            Type eventType = typeof(T);

            if (_eventDictionary.ContainsKey(eventType))
            {
                _eventDictionary[eventType].Remove(callback);
            }
        }

        public void Publish<T>(T eventData) where T : struct
        {
            Type eventType = typeof(T);

            if (_eventDictionary.TryGetValue(eventType, out var dict))
            {
                if (dict == null || dict.Count == 0)
                    return;

                foreach (var callback in dict)
                {
                    ((Action<T>)callback)?.Invoke(eventData);
                }
            }
        }
    }
}
