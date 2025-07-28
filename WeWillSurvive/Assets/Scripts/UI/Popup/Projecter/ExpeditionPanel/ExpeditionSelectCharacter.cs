using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Character;
using WeWillSurvive.Core;

namespace WeWillSurvive
{
    public class ExpeditionSelectCharacter : MonoBehaviour
    {
        [SerializeField] private ECharacter _characterType;

        [SerializeField] private Sprite _normalSprite;
        [SerializeField] private Sprite _disabledSprite;

        private Image _image;
        private Button _button;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Initialize()
        {
            _image = gameObject.GetComponent<Image>();
            _button = gameObject.GetComponent<Button>();
        }

        public void UpdateSelectCharacter()
        {
            var character = CharacterManager.GetCharacter(_characterType);

            if (character.IsDead || character.IsExploring)
            {
                _image.sprite = _disabledSprite;
                _button.interactable = false;
            }
            else
            {
                _image.sprite = _normalSprite;
                _button.interactable = true;
            }
        }
    }
}