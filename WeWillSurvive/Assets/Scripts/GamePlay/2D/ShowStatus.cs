using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.UI;
using static Define;

namespace WeWillSurvive
{
    public class ShowStatus : MonoBehaviour
    {
        [SerializeField] ECharacter _characterType = ECharacter.MaxCount;
        [SerializeField] Vector2 _statusPanelPosition = Vector2.zero;

        private void Awake()
        {
            // 이름 제대로 설정돼있으면 Parse해서 사용
            ECharacter type;
            if (Enum.TryParse(gameObject.name, true, out type))
                _characterType = type;

            // 이름 + enum 모두 잘못되어있으면 오류
            if (_characterType == ECharacter.MaxCount)
            {
                Debug.LogError($"[{gameObject.name}] 잘못된 이름");
                return;
            }

            // 클릭 가능 범위 Image와 맞추기
            GetComponent<RectTransform>().sizeDelta = transform.Find("CharacterImage").GetComponent<RectTransform>().sizeDelta;
        }

        public void ShowStatusPanel()
        {
            ServiceLocator.Get<ResourceService>().LoadAsset("UI_StatusPanel").ContinueWith(prefab =>
            {
                GameManager.Instance.ClosePopupUIs(remain: 1);
                GameObject go = Instantiate(prefab);
                go.transform.GetChild(0).localPosition = _statusPanelPosition;
                go.AddComponent<UI_Popup>();
            }).Forget();
        }
    }
}
