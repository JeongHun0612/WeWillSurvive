using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.UI;
using WeWillSurvive.Character;
using static Define;

namespace WeWillSurvive
{
    public class ShowStatus : MonoBehaviour
    {
        [SerializeField] private ECharacter _characterType;
        [SerializeField] private Vector2 _statusPanelPosition = Vector2.zero;

        private void Awake()
        {
            // 이름 제대로 설정돼있으면 Parse해서 사용
            if (!Enum.TryParse(gameObject.name, true, out _characterType))
            {
                Debug.LogError($"[{gameObject.name}] 잘못된 이름");
                return;
            }

            // 클릭 가능 범위 Image와 맞추기
            GetComponent<RectTransform>().sizeDelta = transform.Find("CharacterImage").GetComponent<RectTransform>().sizeDelta;
        }

        public void ShowStatusPanel()
        {
            UIManager.Instance.ClosePopups(remain: 1);
            UI_StatePanel ui = UIManager.Instance.ShowPopup<UI_StatePanel>();
            ui.transform.GetChild(0).localPosition = _statusPanelPosition;
            ui.SetPanel(_characterType);
        }
    }
}
