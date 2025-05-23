using System.Collections.Generic;
using UnityEngine;
using static Define;

namespace WeWillSurvive
{
    public class CharacterInfo
    {
        public List<ECharacterState> State { get; private set; }
        public string Name { get; private set; }
        public float Hunger => _status[(int)ECharacterStatus.Hunger];
        public float Thirst => _status[(int)ECharacterStatus.Thirst];
        public float Health => _status[(int)ECharacterStatus.Health];
        public bool Hurt => _status[(int)ECharacterStatus.Health] < _maxStatus[(int)ECharacterStatus.Health];

        float _maxHunger => _maxStatus[(int)ECharacterStatus.Hunger];
        float _maxThirst => _maxStatus[(int)ECharacterStatus.Thirst];
        float _maxHealth => _maxStatus[(int)ECharacterStatus.Health];

        private float[] _status;
        private float[] _maxStatus;

        public CharacterInfo(string name, float maxHunger, float maxThirst, float maxHealth)
        {
            State = new List<ECharacterState> { ECharacterState.Normal };
            Name = name;

            _status = new float[(int)ECharacterStatus.MaxCount];
            _maxStatus = new float[(int)ECharacterStatus.MaxCount];

            _maxStatus[(int)ECharacterStatus.Hunger] = maxHunger;
            _maxStatus[(int)ECharacterStatus.Thirst] = maxThirst;
            _maxStatus[(int)ECharacterStatus.Health] = maxHealth;

            for (int i = 0; i < _status.Length; i++)
                _status[i] = _maxStatus[i];
        }

        #region State
        public void SetState(ECharacterState state)
        {
            State.Clear();
            State.Add(state);
        }
        public void AddState(ECharacterState state) => State.Add(state);

        public void UpdateState()
        {
            // ���ּ��� ���� ��� : ���� ��, ���� State �����ؾ���
            // �̹� ���� ���     : Status 0 ���Ϸ� �������� �ڵ����� ����
            if (State[0] == ECharacterState.None || State[0] == ECharacterState.Dead)
                return;

            // �ʱ�ȭ
            State.Clear();

            // 1. ���� (��ħ -> ����)
            if (Health < _maxHealth)
            {
                // TODO: ���� ���� ���ϱ�
                if (Health < _maxHealth * 0.5f)
                    AddState(ECharacterState.Sick);
                else
                    AddState(ECharacterState.Hurt);
            }

            // 2. ���� (���� -> ���а�)
            if (Thirst < _maxThirst * 0.7f)
            {
                // TODO: ���а� ���� ���ϱ�
                if (Thirst < _maxThirst * 0.3f)
                    AddState(ECharacterState.Dehydrate);
                else
                    AddState(ECharacterState.Thirsty);
            }

            // 3. ��� (����� -> �������)
            if (Hunger < _maxHunger * 0.7f)
            {
                // TODO: ������� ���� ���ϱ�
                if (Hunger < _maxHunger * 0.3f)
                    AddState(ECharacterState.Starve);
                else
                    AddState(ECharacterState.Hungry);
            }

            // 4. �Ҿ��� (���� ���� �ü��� ���� �߻�)
            if (false)
                AddState(ECharacterState.Anxiety);

            // 5. ���� (ȥ�� ���� ��� - �� ���� ��쵵?)
            bool alone = true;
            foreach (CharacterInfo info in CharacterManager.Instance.CharacterInfos)
                if (info.Name != Name && (info.State[0] != ECharacterState.None || info.State[0] != ECharacterState.Dead))
                    alone = false;
            if (alone)
                AddState(ECharacterState.Panic);

            // 5. ���� (�ƹ��͵� ���� ��)
            if (State.Count == 0)
                AddState(ECharacterState.Normal);
        }
        #endregion

        #region Status
        public void SetStatus(ECharacterStatus status, float value)
        {
            if (value <= 0)
                SetState(ECharacterState.Dead);

            _status[(int)status] = Mathf.Clamp(value, 0f, _maxStatus[(int)status]);
        }
        public void AddStatus(ECharacterStatus status, float value) => SetStatus(status, _status[(int)status] + value);
        public void SubStatus(ECharacterStatus status, float value) => SetStatus(status, _status[(int)status] - value);
        #endregion
    }
}
