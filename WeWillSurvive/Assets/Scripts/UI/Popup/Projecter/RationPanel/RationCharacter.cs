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

        public void RationItemsRegisterEvent(RationPanel rationPanel)
        {
            _foodItem.RegisterEvent(rationPanel.OnClickFoodItem);
            _waterItem.RegisterEvent(rationPanel.OnClickWaterItem);
            _medicKitItem.RegisterEvent(rationPanel.OnClickMedicKitItem);
        }

        public void Refresh()
        {
            bool isInsShelter = _owner.IsInShelter;

            if (isInsShelter)
            {
                _waterItem.Refresh();
                _foodItem.Refresh();
                _medicKitItem.Refresh();

                _statePanel.SetStateText(_owner.GetFormatStateString());
            }

            _waterItem.gameObject.SetActive(isInsShelter);
            _foodItem.gameObject.SetActive(isInsShelter);

            // MedicKit은 부상 상태에서만 활성화
            bool isInjured = _owner.State.HasState(EState.Injured | EState.Sick);
            _medicKitItem.gameObject.SetActive(isInsShelter && isInjured);

            // 캐릭터 이미지 셋팅
            _characterImage.sprite = (isInsShelter) ? _normalSprite : _notingSprite;

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

            SoundManager.Instance.PlaySFX(ESFX.SFX_Click_1);

            _statePanel.ShowPanel();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _statePanel.HidePanel();
        }
    }
}