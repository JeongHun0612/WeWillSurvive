using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;
using WeWillSurvive.Core;
using static Define;

namespace WeWillSurvive
{
    public class CharacterInfo
    {
        public string Name { get; private set; }
        public ECharacterStatus Status { get; private set; }
        public float Hunger { get; private set; }
        public float Thirst { get; private set; }

        public CharacterInfo(string name)
        {
            Status = ECharacterStatus.Normal;
            Hunger = 100f;
            Thirst = 100f;
            Name = name;
        }

        public void SetStatus(ECharacterStatus status) => Status = status;
        public void SetHunger(float value) => Hunger = Mathf.Clamp(value, 0f, 100f);
        public void SetThirst(float value) => Thirst = Mathf.Clamp(value, 0f, 100f);
    }

    /// <summary>
    /// 플레이어 상태 관리
    /// </summary>
    public class CharacterManager : MonoSceneSingleton<CharacterManager>
    {
        public CharacterInfo[] CharacterInfos;

        protected override void Awake()
        {
            base.Awake();

            // 데이터 초기화
            CharacterInfos = new CharacterInfo[(int)ECharacter.MaxCount];
            for (int i = 0; i < CharacterInfos.Length; i++)
                CharacterInfos[i] = new CharacterInfo(Enum.GetName(typeof(ECharacter), i));

            // Test
            //CharacterInfos[(int)ECharacter.Bell].SetStatus(ECharacterStatus.None);
        }

        public void SetCharacterStatus(ECharacter player, ECharacterStatus status)
        {
            CharacterInfos[(int)player].SetStatus(status);
        }
        public ECharacterStatus GetCharacterStatus(ECharacter player)
        {
            return CharacterInfos[(int)player].Status;
        }

        public void SetCharacterHunger(ECharacter player, float hunger)
        {
            CharacterInfos[(int)player].SetHunger(hunger);
        }
        public float GetCharacterHunger(ECharacter player)
        {
            return CharacterInfos[(int)player].Hunger;
        }

        public void SetCharacterThirst(ECharacter player, float thirsty)
        {
            CharacterInfos[(int)player].SetThirst(thirsty);
        }
        public float GetCharacterThirst(ECharacter player)
        {
            return CharacterInfos[(int)player].Thirst;
        }
    }
}
