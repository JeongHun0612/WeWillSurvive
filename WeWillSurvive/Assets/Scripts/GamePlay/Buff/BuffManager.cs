using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeWillSurvive.Core;

namespace WeWillSurvive
{
    public enum EBuffEffect
    {
        [InspectorName("배고픔 악화 차단")] BlockHungerWorsen = 0,
        [InspectorName("목마름 악화 차단")] BlockThirstWorsen = 1,
        [InspectorName("부상 악화 차단")] BlockInjuryWorsen = 2,
        [InspectorName("공포 악화 차단")] BlockAnxiousWorsen = 3,

        [InspectorName("조사 이벤트 발생 차단")] BlockExplorationEvent = 10,
        [InspectorName("시설 이벤트 발생 차단")] BlockFacilityEvent = 11,
        [InspectorName("침입 이벤트 발생 차단")] BlockInvasionEvent = 12,
        [InspectorName("교환 이벤트 발생 차단")] BlockTradeEvent = 13,

        None = 1000,
    }

    public class BuffManager : MonoSingleton<BuffManager>
    {
        private Dictionary<EBuffEffect, Buff> _buffs = new();

        public void ResetState()
        {
            _buffs.Clear();
        }

        public void OnNewDay()
        {
            if (_buffs == null || _buffs.Count == 0)
                return;

            foreach (var buff in _buffs.Values)
                buff.OnNewDay();

            // 만료된 버프만 반환 후 제거
            var expired = _buffs.Values
                .Where(buff => buff.Duration <= 0)
                .Select(buff => buff.Effect)
                .ToList();

            foreach (var key in expired)
            {
                Debug.Log($"[Buff Delete] '{key}' 버프가 소멸되었습니다.");
                _buffs.Remove(key);
            }
        }

        public void AddBuff(Buff newBuff)
        {
            // 버프가 발생하고 바로 하루가 지나기 때문에 +1 로 상쇄
            int targetDuration = newBuff.Duration + 1;
            newBuff.UpdateDruation(targetDuration);


            if (_buffs.TryGetValue(newBuff.Effect, out var buff))
            {
                buff.UpdateDruation(newBuff.Duration);
            }
            else
            {
                _buffs.Add(newBuff.Effect, newBuff);
                Debug.Log($"[Buff Add] '{newBuff.Effect}' {targetDuration - 1}일 부여");
            }
        }

        public void AddBuff(EBuffEffect effect, int duration) => AddBuff(new Buff(effect, duration));

        public bool HasBuff(EBuffEffect effect)
        {
            return _buffs.TryGetValue(effect, out var buff) && buff.Duration > 0;
        }

        public IReadOnlyList<Buff> GetActiveBuffs() => _buffs.Values.ToList();
    }

    [System.Serializable]
    public class Buff
    {
        private EBuffEffect _effect;
        private int _duration;

        public EBuffEffect Effect => _effect;
        public int Duration => _duration;

        public Buff(EBuffEffect effect, int duration)
        {
            _effect = effect;
            _duration = Mathf.Max(0, duration);
        }

        public void OnNewDay()
        {
            _duration = Mathf.Max(0, _duration - 1);
        }

        public void UpdateDruation(int newDuration)
        {
            _duration = Mathf.Max(_duration, newDuration);
        }
    }
}