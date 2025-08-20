using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.Item;
using WeWillSurvive.UI;

namespace WeWillSurvive.Room
{
    public class MainRoom : RoomBase
    {
        [Header("## Characters")]
        [SerializeField] private List<UI_Character> _characterUIs;

        [Header("Item Placements")]
        [SerializeField] private List<ItemPlacement> _itemPlacements;

        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public async override UniTask InitializeAsync()
        {
            await base.InitializeAsync();

            foreach (var characterUI in _characterUIs)
            {
                characterUI.Initialize();
            }

            foreach (var itemPlacement in _itemPlacements)
            {
                await itemPlacement.InitializeAsync();
            }

            await UniTask.CompletedTask;
        }

        public override void UpdateRoom()
        {
            // ĳ���� ���� ������Ʈ
            foreach (var characterUI in _characterUIs)
            {
                characterUI.UpdateCharacterImage(ERoomType.MainRoom);
            }

            // ������ ������Ʈ
            foreach (var itemPlacement in _itemPlacements)
            {
                float itemCount = ItemManager.GetItemCount(itemPlacement.ItemType);
                itemPlacement.UpdateItemPlacement(itemCount);
            }
        }

        public void OnClickProjecter()
        {
            UIManager.Instance.ClosePopups(remain: 1);
            UIManager.Instance.ShowPopup<UI_Projecter>();
        }
    }
}