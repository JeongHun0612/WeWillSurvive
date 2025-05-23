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
            // 우주선에 없는 경우 : 나갈 때, 직접 State 수정해야함
            // 이미 죽은 경우     : Status 0 이하로 떨어지면 자동으로 죽음
            if (State[0] == ECharacterState.None || State[0] == ECharacterState.Dead)
                return;

            // 초기화
            State.Clear();

            // 1. 아픔 (다침 -> 병듦)
            if (Health < _maxHealth)
            {
                // TODO: 병듦 조건 정하기
                if (Health < _maxHealth * 0.5f)
                    AddState(ECharacterState.Sick);
                else
                    AddState(ECharacterState.Hurt);
            }

            // 2. 갈증 (갈증 -> 수분고갈)
            if (Thirst < _maxThirst * 0.7f)
            {
                // TODO: 수분고갈 조건 정하기
                if (Thirst < _maxThirst * 0.3f)
                    AddState(ECharacterState.Dehydrate);
                else
                    AddState(ECharacterState.Thirsty);
            }

            // 3. 허기 (허기짐 -> 영양결핍)
            if (Hunger < _maxHunger * 0.7f)
            {
                // TODO: 영양결핍 조건 정하기
                if (Hunger < _maxHunger * 0.3f)
                    AddState(ECharacterState.Starve);
                else
                    AddState(ECharacterState.Hungry);
            }

            // 4. 불안함 (우주 기지 시설에 문제 발생)
            if (false)
                AddState(ECharacterState.Anxiety);

            // 5. 공포 (혼자 남은 경우 - 다 나간 경우도?)
            bool alone = true;
            foreach (CharacterInfo info in CharacterManager.Instance.CharacterInfos)
                if (info.Name != Name && (info.State[0] != ECharacterState.None || info.State[0] != ECharacterState.Dead))
                    alone = false;
            if (alone)
                AddState(ECharacterState.Panic);

            // 5. 정상 (아무것도 없을 때)
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
