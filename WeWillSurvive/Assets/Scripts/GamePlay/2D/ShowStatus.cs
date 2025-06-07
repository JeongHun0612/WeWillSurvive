using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.UI;
using WeWillSurvive.Character;
using static Define;
using UnityEngine.UI;
using WeWillSurvive.Item;

namespace WeWillSurvive
{
    public class ShowStatus : MonoBehaviour
    {
        [SerializeField] private EItem _itemType;
        [SerializeField] private Vector2 _statusPanelPosition = Vector2.zero;

        private void Awake()
        {
            if ((int)_itemType >= 100)
            {
                // 클릭 가능 범위 Image와 맞추기
                Image image = transform.Find("CharacterImage").GetComponent<Image>();
                image.raycastPadding = image.sprite.border;
            }
        }

        public void ShowStatusPanel()
        {
            UIManager.Instance.ClosePopups(remain: 1);
            UI_StatePanel ui = UIManager.Instance.ShowPopup<UI_StatePanel>();
            ui.transform.GetChild(0).localPosition = _statusPanelPosition;
            ui.SetPanel(_itemType);
        }
    }
}
