using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WeWillSurvive.Character;
using WeWillSurvive.Core;

namespace WeWillSurvive
{
    public class RationCharacter : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
    {
        [SerializeField] private ECharacter _characterType;

        [Header("## Character Image")]
        [SerializeField] private Image _characterImage;

        [Header("## RationItem (Food, Water, Medicalkit)")]
        [SerializeField] private RationItem _foodItem;
        [SerializeField] private RationItem _waterItem;
        [SerializeField] private RationItem _medicalKitItem;

        [Header("## State Panel")]
        [SerializeField] private StatePanel _statePanel;

        private Sprite _normalSprite;
        private Sprite _notingSprite;

        private CharacterBase _owner;

        private ResourceManager ResourceManager => ServiceLocator.Get<ResourceManager>();
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public async UniTask InitializeAsync()
        {
            _normalSprite = await ResourceManager.LoadAssetAsync<Sprite>($"Assets/Sprites/UI/Character_Icons/{_characterType}_Icon_Normal.png");
            _notingSprite = await ResourceManager.LoadAssetAsync<Sprite>($"Assets/Sprites/UI/Character_Icons/{_characterType}_Icon_Disabled.png");

            _foodItem.Initialize();
            _waterItem.Initialize();
            _medicalKitItem.Initialize();

            _owner = CharacterManager.GetCharacter(_characterType);
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
            if (!_owner.IsInShelter)
            {
                DisabledRationCharacter();
                return;
            }

            _characterImage.sprite = _normalSprite;

            // RationItem 초기화
            _waterItem.Refresh();
            _foodItem.Refresh();
            _medicalKitItem.Refresh();

            bool isInjured = _owner.State.HasState(EState.Injured | EState.Sick);
            _medicalKitItem.gameObject.SetActive(isInjured);


            // StatePanel 초기화
            _statePanel.SetStateText(_owner.GetFormatStateString());
            _statePanel.HidePanel();
        }

        public void ApplyRationItem()
        {
            _foodItem.UsedItem(_owner);
            _waterItem.UsedItem(_owner);
            _medicalKitItem.UsedItem(_owner);
        }

        private void DisabledRationCharacter()
        {
            _characterImage.sprite = _notingSprite;
            _waterItem.gameObject.SetActive(false);
            _foodItem.gameObject.SetActive(false);
            _medicalKitItem.gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_owner.IsInShelter)
                return;

            _statePanel.ShowPanel();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _statePanel.HidePanel();
        }
    }
}
