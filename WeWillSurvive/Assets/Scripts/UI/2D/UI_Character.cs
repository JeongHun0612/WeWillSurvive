using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WeWillSurvive.Character;
using WeWillSurvive.Core;

namespace WeWillSurvive
{
    public enum ECharacterUIType
    { 
        Main,
        Room,
    }

    [RequireComponent(typeof(ShowStatus))]
    public class UI_Character : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private ECharacter _characterType;
        [SerializeField] private ECharacterUIType _characterUIType;
        [SerializeField] private Image _characterImage;

        private ShowStatus _showStatus;
        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public ECharacter CharacterType => _characterType;
        public CharacterBase Owner { get; private set; }

        public void Initialize()
        {
            _showStatus = gameObject.GetComponent<ShowStatus>();

            // 클릭 가능 범위 Image와 맞추기
            _characterImage.raycastPadding = _characterImage.sprite.border;

            if (Owner == null)
                Owner = CharacterManager.GetCharacter(_characterType);
        }

        public void UpdateCharacterImage()
        {
            if (Owner.IsExploring || Owner.IsDead)
            {
                gameObject.SetActive(false);
            }
            else
            {
                if (_characterUIType == ECharacterUIType.Main)
                    _characterImage.sprite = Owner.MainSprite;
                else if (_characterUIType == ECharacterUIType.Room)
                    _characterImage.sprite = Owner.RoomSprite;

                gameObject.SetActive(true);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_showStatus == null)
                return;

            string stateText = Owner.State.FormatStateString();
            _showStatus.ShowStatusPanel(stateText);
        }
    }
}
