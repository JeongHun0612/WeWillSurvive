using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeWillSurvive.Core;

namespace WeWillSurvive
{
    public enum EBuffEffect
    {
        [InspectorName("����� ��ȭ ����")] BlockHungerWorsen = 0,
        [InspectorName("�񸶸� ��ȭ ����")] BlockThirstWorsen = 1,
        [InspectorName("�λ� ��ȭ ����")] BlockInjuryWorsen = 2,
        [InspectorName("���� ��ȭ ����")] BlockAnxiousWorsen = 3,

        [InspectorName("�ü� �̺�Ʈ �߻� ����")] BlockFacilityEvent = 10,
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

            // ����� ������ ��ȯ �� ����
            var expired = _buffs.Values
                .Where(buff => buff.Duration <= 0)
                .Select(buff => buff.Effect)
                .ToList();

            foreach (var key in expired)
            {
                Debug.Log($"[Buff Delete] '{key}' ������ �Ҹ�Ǿ����ϴ�.");
                _buffs.Remove(key);
            }
        }

        public void AddBuff(Buff newBuff)
        {
            // ������ �߻��ϰ� �ٷ� �Ϸ簡 ������ ������ +1 �� ���
            int targetDuration = newBuff.Duration + 1;
            newBuff.UpdateDruation(targetDuration);


            if (_buffs.TryGetValue(newBuff.Effect, out var buff))
            {
                buff.UpdateDruation(newBuff.Duration);
            }
            else
            {
                _buffs.Add(newBuff.Effect, newBuff);
                Debug.Log($"[Buff Add] '{newBuff.Effect}' {targetDuration - 1}�� �ο�");
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