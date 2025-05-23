using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Define;

namespace WeWillSurvive.Core
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public int Day;

        private void Start()
        {
            ServiceLocator.AutoRegisterServices();

            ServiceLocator.Get<ResourceService>().LoadAsset("UI_Black").ContinueWith(prefab => { BlackUI = Instantiate(prefab).GetComponent<UI_Black>(); }).Forget();

            // Temp
            Day = 1;

            if (SceneManager.GetActiveScene().name == "2D")
                ServiceLocator.Get<ResourceService>().LoadAsset("UI_Background").ContinueWith(prefab => Instantiate(prefab)).Forget();
        }

        #region UI
        public UI_Black BlackUI = null;
        public UI_Scene SceneUI;
        public Stack<UI_Popup> PopUIStack = new Stack<UI_Popup>();

        // Popup UI 초기화
        public void CloseAllPopupUI()
        {
            ClosePopupUIs(remain: 0);
        }

        /// <param name="remain"> 끄지 않고 남겨둘 Popup UI 개수 </param>
        public void ClosePopupUIs(int remain)
        {
            while (true)
            {
                if (PopUIStack.Count <= remain) break;
                UI_Popup ui = PopUIStack.Pop();
                if (ui != null)
                    Destroy(ui.gameObject);
            }
        }

        // 제일 위에 있는 Popup UI 닫기
        public void ClosePopupUI()
        {
            if (PopUIStack.Count == 0) return;
            UI_Popup ui = PopUIStack.Pop();
            if (ui != null)
                Destroy(ui.gameObject);
        }
        #endregion

        #region Item
        float[] _itemCount = new float[(int)EItem.MaxCount];

        public float GetItemCount(EItem type)
        {
            return _itemCount[(int)type];
        }

        /// <summary>
        /// 어떤 아이템을 누구에게 얼마나 사용할 것인지
        /// 사용 대상이 정해져 있는 경우
        /// </summary>
        public void UseItem(EItem type, ECharacter target, float count)
        {
            if (type == EItem.MaxCount || target == ECharacter.MaxCount)
                return;

            float remain = _itemCount[(int)type];
            if (remain < count) return;

            CharacterInfo character = CharacterManager.Instance.CharacterInfos[(int)target];

            switch (type)
            {
                case EItem.Food:
                    character.AddStatus(ECharacterStatus.Hunger, 30f);
                    break;
                case EItem.Water:
                    character.AddStatus(ECharacterStatus.Thirst, 30f);
                    break;
                case EItem.MedicKit:
                    character.SetStatus(ECharacterStatus.Health, character.MaxHealth);
                    break;
                default:
                    return;
            }

            _itemCount[(int)type] = remain - count;
        }

        /// <summary>
        /// 모두에게 사용하거나 이벤트로 사용하는 경우
        /// </summary>
        public void UseItem(EItem type, float count)
        {
            float remain = _itemCount[(int)type];
            if (remain < count) return;

            _itemCount[(int)type] = remain - count;

            // TODO: 모두에게 사용하는 경우
            switch (type)
            {
                case EItem.SpecialFood:
                    break;
                case EItem.SuperMedicKit:
                    break;
            }

            // 이벤트에서 사용하는 경우 - 이벤트 함수에서 처리
        }

        // 파밍 시스템에서 출구에 넣었을 때 / 이벤트로 아이템 얻었을 때 
        public void GetItem(EItem type, float count)
        {
            _itemCount[(int)type] += count;
        }

        #endregion
    }
}
