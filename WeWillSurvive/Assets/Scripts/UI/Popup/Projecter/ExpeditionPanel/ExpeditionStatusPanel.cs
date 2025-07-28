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

        [SerializeField] private Sprite _normalSprite;
        [SerializeField] private Sprite _disabledSprite;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public async UniTask InitializeAsync()
        {


            await UniTask.Yield();
        }


        public void UpdateExpeditionStatusPanel()
        {
            var character = CharacterManager.GetCharacter(_characterType);

            if (character.IsDead || character.IsExploring)
            {
                DisabledStatusPanel();
                return;
            }

            EExpeditionReadyState readyState = GetExpeditionReadyState(character.State);
            string readyMessage = character.Data.ExpeditionMessageData.GetExpeditionReadyMessage(readyState);
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

        private EExpeditionReadyState GetExpeditionReadyState(CharacterState state)
        {
            // (1) 탐사를 정상적으로 나갈 수 있는 경우
            if (state.HasState(EState.Normal | EState.Hungry | EState.Thirsty))
                    return EExpeditionReadyState.Normal;

            // (2) 탐사를 나가기 위험한 상태인 경우
            if (state.HasState(EState.Starve | EState.Dehydrate | EState.Injured | EState.Anxious))
                return EExpeditionReadyState.Warning;

            // (3) 탐사를 나갈 수 없는 경우
            if (state.HasState(EState.Sick | EState.Panic | EState.Mad))
                return EExpeditionReadyState.Impossible;

            return EExpeditionReadyState.Normal;
        }
    }
}
