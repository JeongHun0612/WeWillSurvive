using System;
using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Core;
using static Define;

namespace WeWillSurvive
{
    /// <summary>
    /// 플레이어 정보 관리
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
                CharacterInfos[i] = new CharacterInfo(Enum.GetName(typeof(ECharacter), i), 100f, 100f, 100f);

            // Test
            //CharacterInfos[(int)ECharacter.Bell].SetState(ECharacterState.None);
        }

        // Day 넘어갈 때, 캐릭터 정보 업데이트
        public void UpdateCharacterInfos()
        {
            foreach (CharacterInfo info in CharacterInfos)
            {
                // 허기, 목마름 낮아짐
                info.SubStatus(ECharacterStatus.Hunger, 10f);
                info.SubStatus(ECharacterStatus.Thirst, 10f);

                // 부상당했으면 건강 낮아짐
                // * 부상당하자마자 또 낮아져서 두배로 낮아지는 문제 있을 수 있음
                if (info.Hurt)
                    info.SubStatus(ECharacterStatus.Health, 10f);

                info.UpdateState();
            }
        }
    }
}
