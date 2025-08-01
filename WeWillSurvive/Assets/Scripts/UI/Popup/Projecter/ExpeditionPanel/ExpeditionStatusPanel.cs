using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Character;
using WeWillSurvive.Core;

namespace WeWillSurvive
{
    public class ExpeditionStatusPanel : MonoBehaviour
    {
        [SerializeField] private ECharacter _characterType;
        [SerializeField] private Image _characterIcon;
        [SerializeField] private TMP_Text _readyText;

        private Sprite _normalSprite;
        private Sprite _disabledSprite;

        private CharacterBase _owner;

        private ResourceManager ResourceManager => ServiceLocator.Get<ResourceManager>();

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public async UniTask InitializeAsync()
        {
            _normalSprite = await ResourceManager.LoadAssetAsync<Sprite>($"Assets/Sprites/UI/Character_Icons/{_characterType}_Icon_Normal.png");
            _disabledSprite = await ResourceManager.LoadAssetAsync<Sprite>($"Assets/Sprites/UI/Character_Icons/{_characterType}_Icon_Disabled.png");

            _owner = CharacterManager.GetCharacter(_characterType);
        }


        public void UpdateExpeditionStatusPanel()
        {
            if (_owner.IsDead || _owner.IsExploring)
            {
                DisabledStatusPanel();
                return;
            }

            string readyMessage = _owner.Data.ExpeditionMessageData.GetExpeditionReadyMessage(_owner.State);
            EnabledStatusPanel(readyMessage);
        }

        private void DisabledStatusPanel()
        {
            _characterIcon.sprite = _disabledSprite;
            _readyText.text = "...";
        }

        private void EnabledStatusPanel(string readyMessage)
        {
            _characterIcon.sprite = _normalSprite;
            _readyText.text = readyMessage;
        }
    }
}
