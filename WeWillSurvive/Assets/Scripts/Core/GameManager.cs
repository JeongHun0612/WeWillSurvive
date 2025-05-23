using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WeWillSurvive.UI;
using static Define;

namespace WeWillSurvive.Core
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public int Day;

        private async void Start()
        {
            ServiceLocator.AutoRegisterServices();

            await UIManager.Instance.InitializeAsync();

            if (SceneManager.GetActiveScene().name == "2D")
            {
                ServiceLocator.Get<ResourceService>().LoadAsset("UI_Background").ContinueWith(prefab => Instantiate(prefab)).Forget();
                //UIManager.Instance.ShowScene<UI_Background>();
            }

            // Temp
            Day = 1;
        }

        #region UI
        public UI_Black BlackUI = null;
        public UI_Scene SceneUI;
        public Stack<UI_Popup> PopUIStack = new Stack<UI_Popup>();

        // Popup UI �ʱ�ȭ
        public void CloseAllPopupUI()
        {
            ClosePopupUIs(remain: 0);
        }

        /// <param name="remain"> ���� �ʰ� ���ܵ� Popup UI ���� </param>
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

        // ���� ���� �ִ� Popup UI �ݱ�
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
        /// � �������� �������� �󸶳� ����� ������
        /// ��� ����� ������ �ִ� ���
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
        /// ��ο��� ����ϰų� �̺�Ʈ�� ����ϴ� ���
        /// </summary>
        public void UseItem(EItem type, float count)
        {
            float remain = _itemCount[(int)type];
            if (remain < count) return;

            _itemCount[(int)type] = remain - count;

            // TODO: ��ο��� ����ϴ� ���
            switch (type)
            {
                case EItem.SpecialFood:
                    break;
                case EItem.SuperMedicKit:
                    break;
            }

            // �̺�Ʈ���� ����ϴ� ��� - �̺�Ʈ �Լ����� ó��
        }

        // �Ĺ� �ý��ۿ��� �ⱸ�� �־��� �� / �̺�Ʈ�� ������ ����� �� 
        public void GetItem(EItem type, float count)
        {
            _itemCount[(int)type] += count;
        }

        #endregion
    }
}
