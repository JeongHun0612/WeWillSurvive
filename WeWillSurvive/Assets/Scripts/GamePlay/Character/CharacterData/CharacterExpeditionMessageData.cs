using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;

namespace WeWillSurvive
{
    public enum EExpeditionReadyState
    { 
        Normal,
        Warning,
        Impossible
    }

    [CreateAssetMenu(fileName = "CharacterExpeditionMessageData", menuName = "Scriptable Objects/CharacterData/CharacterExpeditionMessageData")]
    public class CharacterExpeditionMessageData : ScriptableObject
    {
        [Header("## 탐사를 정상적으로 나갈 수 있는 경우")]
        [SerializeField] private List<string> _expeditionNormalMessages;

        [Header("## 탐사를 나가기 위험한 상태인 경우")]
        [SerializeField] private List<string> _expeditionWarningeMessages;

        [Header("## 탐사를 나갈 수 없는 경우")]
        [SerializeField] private List<string> _expeditionImpossibleMessages;

        [Header("## 탐사 출발 스크립트")]
        [SerializeField] private List<string> _expeditionStartMessages;

        [Header("## 탐사 사망 스크립트")]
        [SerializeField] private List<string> _expeditionDeadMessages;

        [Header("## 탐사 사망 로그 스크립트")]
        [SerializeField] private List<string> _expeditionDeadLogMessages;

        public string GetExpeditionReadyMessage(CharacterState state)
        {
            // 탐사를 나갈 수 없는 경우
            if (state.IsExpeditionStateImpossible)
                return GetRandomMessage(_expeditionImpossibleMessages);
            // 탐사를 나가기 위험한 상태인 경우
            else if (state.IsExpeditionStateWarning)
                return GetRandomMessage(_expeditionWarningeMessages);

            // 탐사를 정상적으로 나갈 수 있는 경우
            return GetRandomMessage(_expeditionNormalMessages);
        }

        public string GetExpeditionStartMessage()
        {
            return GetRandomMessage(_expeditionStartMessages);
        }

        public string GetExpeditionDeadMessage()
        {
            return GetRandomMessage(_expeditionDeadMessages);
        }

        public string GetExpeditionDeadLogMessage()
        {
            var randomMessage = GetRandomMessage(_expeditionDeadLogMessages);
            var deadMessage = randomMessage.Replace("{}", GameManager.Instance.Day.ToString());
            return deadMessage;
        }

        private string GetRandomMessage(List<string> messages)
        {
            if (messages == null || messages.Count == 0)
                return string.Empty;

            int randomIndex = Random.Range(0, messages.Count);

            return messages[randomIndex];
        }
    }
}
