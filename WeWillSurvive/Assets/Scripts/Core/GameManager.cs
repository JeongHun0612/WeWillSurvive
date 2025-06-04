using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WeWillSurvive.Character;
using WeWillSurvive.Status;
using WeWillSurvive.UI;
using static Define;

namespace WeWillSurvive.Core
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public int Day;

        private async void Start()
        {
            await ServiceLocator.AutoRegisterServices();
            await UIManager.Instance.InitializeAsync();

            if (SceneManager.GetActiveScene().name == "2D")
            {
                UIManager.Instance.ShowScene<UI_Background>();
            }

            // Temp
            Day = 1;
        }

        public void NewDay()
        {
            // 빔 프로젝터에서 받을 정보
            // 1. 사용한 아이템
            // Key에게 Value만큼 사용 (사용 대상을 특정하는 아이템이 아니면 아무거나 넣으면 됨)
            Dictionary<ECharacter, float>[] UseItems = new Dictionary<ECharacter, float>[(int)EItem.MaxCount];
            for (int i = 0; i < (int)EItem.MaxCount; i++)
                UseItems[i] = new Dictionary<ECharacter, float>();


            // 남은 아이템 개수 확인
            for (int i = 0; i < (int)EItem.MaxCount; i++)
            {
                float useCount = 0;
                foreach (float cnt in UseItems[i].Values) useCount += cnt;
                if (useCount == 0) continue;

                float remainCount = GetItemCount((EItem)i);
                if (remainCount < useCount)
                {
                    Debug.LogError("남은 아이템보다 사용한 아이템이 많음 - UI 표기 오류");
                    return;
                }
            }

            // 아이템 사용
            string s = "";
            for (int i = 0; i < (int)EItem.MaxCount; i++)
            {
                float useCount = 0;
                foreach (KeyValuePair<ECharacter, float> useItem in UseItems[i])
                {
                    UseItem((EItem)i, useItem.Key, useItem.Value);
                    useCount += useItem.Value;
                }

                // Debug
                if (useCount > 0)
                    s += $"{Enum.GetName(typeof(EItem), i)} {useCount}개, ";
            }
            if (s == "") s = "없음";

            // 2. 탐사 보낼 캐릭터 (여러 명 보내는 경우 있으면 수정)
            // 캐릭터 정보 업데이트
            ServiceLocator.Get<CharacterManager>().UpdateCharacterStatus();

            ECharacter exploreCharacter = ECharacter.Lead;
            ServiceLocator.Get<CharacterManager>().GetCharacter(exploreCharacter).State.AddState(EState.Exploring);

            // 3. 이벤트?
            // 이벤트 별로 함수 만들어서 호출

            // Day + 1
            Debug.Log($"[Day {Day}]\n사용한 아이템: {s} / 나간 사람: {Enum.GetName(typeof(ECharacter), exploreCharacter)}");
            Day += 1;
        }

        #region Item
        float[] _itemCount = new float[(int)EItem.MaxCount];

        public float GetItemCount(EItem type)
        {
            return _itemCount[(int)type];
        }

        public void UseItem(EItem type, ECharacter target, float count)
        {
            if (type == EItem.MaxCount)
                return;

            float remain = _itemCount[(int)type];
            if (remain < count) return;

            CharacterStatus characterStatus = ServiceLocator.Get<CharacterManager>().GetCharacter(target)?.Status;
            if (characterStatus == null)
                return;

            switch (type)
            {
                case EItem.Food:
                    characterStatus.GetStatus<HungerStatus>(EStatusType.Hunger).ApplyRecovery(30f);
                    break;
                case EItem.Water:
                    characterStatus.GetStatus<ThirstStatus>(EStatusType.Thirst).ApplyRecovery(30f);
                    break;
                case EItem.MedicKit:
                    characterStatus.GetStatus<InjuryStatus>(EStatusType.Injury).ApplyRecovery(float.MaxValue);
                    break;
                default:
                    return;
            }

            _itemCount[(int)type] = remain - count;
        }

        public void UseItem(EItem type, float count)
        {
            float remain = _itemCount[(int)type];
            if (remain < count) return;

            _itemCount[(int)type] = remain - count;

            switch (type)
            {
                case EItem.SpecialFood:
                    break;
                case EItem.SuperMedicKit:
                    break;
            }
        }

        public void GetItem(EItem type, float count)
        {
            _itemCount[(int)type] += count;
        }
        #endregion
    }
}
