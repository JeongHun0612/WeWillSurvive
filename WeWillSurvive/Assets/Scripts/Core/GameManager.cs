using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WeWillSurvive.Character;
using WeWillSurvive.Status;
using WeWillSurvive.UI;
using WeWillSurvive.Item;

namespace WeWillSurvive.Core
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public int Day;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

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
            Dictionary<EItem, Dictionary<ECharacter, float>> UsedItems = new Dictionary<EItem, Dictionary<ECharacter, float>>();

            // 아이템 사용
            string s = "";
            foreach (KeyValuePair<EItem, Dictionary<ECharacter, float>> usedItem in UsedItems)
            {
                float usedCount = 0;
                foreach (KeyValuePair<ECharacter, float> usedCharacter in usedItem.Value)
                {
                    CharacterBase character = CharacterManager.GetCharacter(usedCharacter.Key);
                    ItemManager.UsedItem(character, usedItem.Key, usedCharacter.Value);
                    usedCount += usedCharacter.Value;
                }

                // Debug
                if (usedCount > 0)
                    s += $"{Enum.GetName(typeof(EItem), usedItem.Key)} {usedCount}개, ";
            }
            if (s == "") s = "없음";

            // 2. 탐사 보낼 캐릭터 (여러 명 보내는 경우 있으면 수정)
            // 캐릭터 정보 업데이트
            CharacterManager.UpdateCharacterStatus();

            // Temp
            string explorerName = "없음";
            if (Day == 2)
            {
                ECharacter exploreCharacter = ECharacter.Lead;
                CharacterManager.GetCharacter(exploreCharacter).State.AddState(EState.Exploring);
                explorerName = Enum.GetName(typeof(ECharacter), exploreCharacter);
            }

            // 3. 이벤트?
            // 이벤트 별로 함수 만들어서 호출

            // Day + 1
            Debug.Log($"[Day {Day}]\n사용한 아이템: {s} / 나간 사람: {explorerName}");
            Day += 1;
        }
    }
}
