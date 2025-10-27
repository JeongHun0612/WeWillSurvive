using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using WeWillSurvive.UI;

namespace WeWillSurvive.Room
{
    public class MainRoom : RoomBase
    {
        [Header("## Characters")]
        [SerializeField] private List<UI_Character> _characterUIs;

        [Header("Item Placements")]
        [SerializeField] private List<ItemPlacement> _itemPlacements;

        [SerializeField] private Image _projecter;

        private Sprite[] _projecterSprites;

        private ResourceManager ResourceManager => ServiceLocator.Get<ResourceManager>();

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

            _projecterSprites = new Sprite[2];
            _projecterSprites[0] = await ResourceManager.LoadAssetAsync<Sprite>("Assets/Sprites/Background/Projecter.png");
            _projecterSprites[1] = await ResourceManager.LoadAssetAsync<Sprite>("Assets/Sprites/Background/Projecter_update.png");

            _projecter.sprite = _projecterSprites[1];

            await UniTask.CompletedTask;
        }

        public override void UpdateRoom()
        {
            // 캐릭터 상태 업데이트
            foreach (var characterUI in _characterUIs)
            {
                characterUI.UpdateCharacterImage(ERoomType.MainRoom);
            }

            // 아이템 업데이트
            foreach (var itemPlacement in _itemPlacements)
            {
                itemPlacement.UpdateItemPlacement();
            }
        }

        public void OnClickProjecter()
        {
            if (_projecter.sprite == _projecterSprites[1])
                _projecter.sprite = _projecterSprites[0];

            SoundManager.Instance.PlaySFX(ESFX.SFX_Click_2);

            UIManager.Instance.CloseAllPopups();
            UIManager.Instance.ShowPopup<UI_Projecter>();
        }
    }
}