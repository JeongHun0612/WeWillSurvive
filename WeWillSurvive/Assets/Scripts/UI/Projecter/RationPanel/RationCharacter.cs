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
        [SerializeField] private Sprite _normalSprite;
        [SerializeField] private Sprite _notingSprite;

        [Header("## RationItem (Food, Water, Medicalkit)")]
        [SerializeField] private RationItem _foodItem;
        [SerializeField] private RationItem _waterItem;
        [SerializeField] private RationItem _medicalKitItem;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Initialize(RationPanel rationPanel)
        {
            _foodItem.Initialize();
            _waterItem.Initialize();
            _medicalKitItem.Initialize();

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
                _characterImage.sprite = _notingSprite;
                _characterImage.color = new Color(1f, 1f, 1f, 0.3f);
            }
            else
            {
                _characterImage.sprite = _normalSprite;
                _characterImage.color = Color.white;

                _waterItem.Refresh();
                _foodItem.Refresh();
                _medicalKitItem.Refresh();

                bool isInjured = character.State.HasState(EState.Injured | EState.Sick);
                _medicalKitItem.gameObject.SetActive(isInjured);
            }
        }

        public void ApplyRationItem()
        {
            CharacterBase target = CharacterManager.GetCharacter(_characterType);
            _foodItem.UsedItem(target);
            _waterItem.UsedItem(target);
            _medicalKitItem.UsedItem(target);
        }
    }
}
