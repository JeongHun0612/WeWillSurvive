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
        [SerializeField] private RationItem _medicKitItem;

        [Header("## State Panel")]
        [SerializeField] private StatePanel _statePanel;

        private Sprite _normalSprite;
        private Sprite _notingSprite;

        private CharacterBase _owner;

        public RationItem FoodItem => _foodItem;
        public RationItem WaterItem => _waterItem;
        public RationItem MedicKitItem => _medicKitItem;

        private ResourceManager ResourceManager => ServiceLocator.Get<ResourceManager>();
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public async UniTask InitializeAsync()
        {
            _normalSprite = await ResourceManager.LoadAssetAsync<Sprite>($"Assets/Sprites/UI/Character_Icons/{_characterType}_Icon_Normal.png");
            _notingSprite = await ResourceManager.LoadAssetAsync<Sprite>($"Assets/Sprites/UI/Character_Icons/{_characterType}_Icon_Disabled.png");

            _foodItem.Initialize();
            _waterItem.Initialize();
            _medicKitItem.Initialize();

            _owner = CharacterManager.GetCharacter(_characterType);
        }

        public void RegisterEvent(RationPanel rationPanel)
        {
            _foodItem.ItemSelectedEvent -= rationPanel.OnClickFoodItem;
            _foodItem.ItemSelectedEvent += rationPanel.OnClickFoodItem;

            _waterItem.ItemSelectedEvent -= rationPanel.OnClickWaterItem;
            _waterItem.ItemSelectedEvent += rationPanel.OnClickWaterItem;

            _medicKitItem.ItemSelectedEvent -= rationPanel.OnClickMedicKitItem;
            _medicKitItem.ItemSelectedEvent += rationPanel.OnClickMedicKitItem;
        }

        public void Refresh()
        {
            // RationItem 셋팅
            _waterItem.Refresh();
            _foodItem.Refresh();
            _medicKitItem.Refresh();

            // 캐릭터 이미지 셋팅
            _characterImage.sprite = (_owner.IsInShelter) ? _normalSprite : _notingSprite;

            // RationItem 셋팅
            _waterItem.gameObject.SetActive(_owner.IsInShelter);
            _foodItem.gameObject.SetActive(_owner.IsInShelter);

            bool isInjured = _owner.State.HasState(EState.Injured | EState.Sick);
            _medicKitItem.gameObject.SetActive(_owner.IsInShelter && isInjured);

            // StatePanel 셋팅
            _statePanel.SetStateText(_owner.GetFormatStateString());
            _statePanel.HidePanel();
        }

        public void ApplyAllRationItem()
        {
            ApplyFoodItem();
            ApplyWaterItem();
            ApplyMedicKitItem();
        }

        public void ApplyFoodItem() => _foodItem.UsedItem(_owner);
        public void ApplyWaterItem() => _waterItem.UsedItem(_owner);
        public void ApplyMedicKitItem() => _medicKitItem.UsedItem(_owner);

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