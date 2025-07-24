using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Character;
using WeWillSurvive.Core;

namespace WeWillSurvive
{
    public class RationCharacter : MonoBehaviour
    {
        [SerializeField] private ECharacter _characterType;

        [Header("## Character Image")]
        [SerializeField] private Image _characterImage;

        [Header("## RationItem (Food, Water, Medicalkit)")]
        [SerializeField] private RationItem _foodItem;
        [SerializeField] private RationItem _waterItem;
        [SerializeField] private RationItem _medicalKitItem;

        private Sprite _normalSprite;
        private Sprite _notingSprite;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Initialize()
        {
            _normalSprite = SpriteManager.Instance.GetSprite(ESpriteAtlas.UI_Atlas, $"{_characterType}_Icon_Normal");
            _notingSprite = SpriteManager.Instance.GetSprite(ESpriteAtlas.UI_Atlas, $"{_characterType}_Icon_Disabled");

            _foodItem.Initialize();
            _waterItem.Initialize();
            _medicalKitItem.Initialize();
        }

        public void RegisterEvent(RationPanel rationPanel)
        {
            _foodItem.ItemSelectedEvent -= rationPanel.UpdateFoodItemCount;
            _foodItem.ItemSelectedEvent += rationPanel.UpdateFoodItemCount;

            _waterItem.ItemSelectedEvent -= rationPanel.UpdateWaterItemCount;
            _waterItem.ItemSelectedEvent += rationPanel.UpdateWaterItemCount;
        }

        public void Refresh()
        {
            var character = CharacterManager.GetCharacter(_characterType);

            if (character.IsDead || character.IsExploring)
            {
                DisabledRationCharacter();
                return;
            }

            _characterImage.sprite = _normalSprite;

            _waterItem.Refresh();
            _foodItem.Refresh();
            _medicalKitItem.Refresh();

            bool isInjured = character.State.HasState(EState.Injured | EState.Sick);
            _medicalKitItem.gameObject.SetActive(isInjured);
        }

        public void ApplyRationItem()
        {
            CharacterBase target = CharacterManager.GetCharacter(_characterType);
            _foodItem.UsedItem(target);
            _waterItem.UsedItem(target);
            _medicalKitItem.UsedItem(target);
        }

        private void DisabledRationCharacter()
        {
            _characterImage.sprite = _notingSprite;
            _waterItem.gameObject.SetActive(false);
            _foodItem.gameObject.SetActive(false);
            _medicalKitItem.gameObject.SetActive(false);
        }
    }
}
