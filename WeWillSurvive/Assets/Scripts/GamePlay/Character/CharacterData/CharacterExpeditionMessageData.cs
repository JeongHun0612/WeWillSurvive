using System.Collections.Generic;
using UnityEngine;

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

        [Header("## 탐사를 나간 후")]
        [SerializeField] private List<string> _expeditionStartMessages;

        public string GetExpeditionReadyMessage(EExpeditionReadyState type)
        {
            switch (type)
            {
                case EExpeditionReadyState.Normal:
                    return GetRandomMessage(_expeditionNormalMessages);
                case EExpeditionReadyState.Warning:
                    return GetRandomMessage(_expeditionWarningeMessages);
                case EExpeditionReadyState.Impossible:
                    return GetRandomMessage(_expeditionImpossibleMessages);
                default:
                    return string.Empty;
            }
        }

        public string GetExpeditionStartMessage()
        {
            return GetRandomMessage(_expeditionStartMessages);
        }

        private string GetRandomMessage(List<string> messages)
        {
            int randomIndex = Random.Range(0, messages.Count);

            return messages[randomIndex];
        }
    }
}
