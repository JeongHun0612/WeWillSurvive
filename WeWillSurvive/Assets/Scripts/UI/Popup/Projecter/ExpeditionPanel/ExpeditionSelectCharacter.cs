using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Character;
using WeWillSurvive.Core;

namespace WeWillSurvive
{
    public class ExpeditionSelectCharacter : MonoBehaviour
    {
        [SerializeField] private ECharacter _characterType;
        [SerializeField] private GameObject _deadImage;

        private Image _image;
        private Button _button;

        private Sprite _selectSprite;
        private Sprite _disabledSprite;

        private CharacterBase _owner;

        public ECharacter CharacterType => _characterType;
        public CharacterBase Owner => _owner;


        private ResourceManager ResourceManager => ServiceLocator.Get<ResourceManager>();
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();


        public async UniTask InitializeAsync()
        {
            _image = gameObject.GetComponent<Image>();
            _button = gameObject.GetComponent<Button>();

            _selectSprite = await ResourceManager.LoadAssetAsync<Sprite>($"Assets/Sprites/UI/Character_Icons/{_characterType}_Icon_Normal.png");
            _disabledSprite = await ResourceManager.LoadAssetAsync<Sprite>($"Assets/Sprites/UI/Character_Icons/{_characterType}_Icon_Disabled.png");

            _owner = CharacterManager.GetCharacter(_characterType);
        }

        public void RegisterClickEvent(Action<ExpeditionSelectCharacter> callback)
        {
            _button.onClick.AddListener(() => callback?.Invoke(this));
        }

        public void UpdateSelectCharacter()
        {
            _image.sprite = _disabledSprite;

            if (_owner.IsDead || _owner.IsExploring || _owner.State.IsExpeditionStateImpossible)
            {
                _button.interactable = false;
                _deadImage.SetActive(true);
            }
            else
            {
                _button.interactable = true;
                _deadImage.SetActive(false);
            }
        }

        public void OnSelected(bool isSelect)
        {
            if (isSelect)
            {
                _image.sprite = _selectSprite;
            }
            else
            {
                _image.sprite = _disabledSprite;
            }
        }
    }
}