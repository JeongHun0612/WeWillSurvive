using System;
using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Core;
using static Define;

namespace WeWillSurvive
{
    /// <summary>
    /// �÷��̾� ���� ����
    /// </summary>
    public class CharacterManager : MonoSceneSingleton<CharacterManager>
    {
        public CharacterInfo[] CharacterInfos;

        protected override void Awake()
        {
            base.Awake();

            // ������ �ʱ�ȭ
            CharacterInfos = new CharacterInfo[(int)ECharacter.MaxCount];
            for (int i = 0; i < CharacterInfos.Length; i++)
                CharacterInfos[i] = new CharacterInfo(Enum.GetName(typeof(ECharacter), i), 100f, 100f, 100f);

            // Test
            //CharacterInfos[(int)ECharacter.Bell].SetState(ECharacterState.None);
        }

        // Day �Ѿ ��, ĳ���� ���� ������Ʈ
        public void UpdateCharacterInfos()
        {
            foreach (CharacterInfo info in CharacterInfos)
            {
                // ���, �񸶸� ������
                info.SubStatus(ECharacterStatus.Hunger, 10f);
                info.SubStatus(ECharacterStatus.Thirst, 10f);

                // �λ�������� �ǰ� ������
                // * �λ�����ڸ��� �� �������� �ι�� �������� ���� ���� �� ����
                if (info.Hurt)
                    info.SubStatus(ECharacterStatus.Health, 10f);

                info.UpdateState();
            }
        }
    }
}
