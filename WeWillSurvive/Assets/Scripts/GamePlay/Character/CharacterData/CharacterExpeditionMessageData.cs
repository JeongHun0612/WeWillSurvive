using System.Collections.Generic;
using UnityEngine;

namespace WeWillSurvive
{
    public enum EExpeditionStateType
    { 
        Normal,
        Warning,
        Impossible
    }

    [CreateAssetMenu(fileName = "CharacterExpeditionMessageData", menuName = "Scriptable Objects/CharacterData/CharacterExpeditionMessageData")]
    public class CharacterExpeditionMessageData : ScriptableObject
    {
        [Header("## 탐사를 정상적으로 나갈 수 있는 경우")]
        [SerializeField] private List<string> _normalExpeditionMessages;

        [Header("## 탐사를 나가기 위험한 상태인 경우")]
        [SerializeField] private List<string> _warningExpeditionMessages;

        [Header("## 탐사를 나갈 수 없는 경우")]
        [SerializeField] private List<string> _impossibleExpeditionMessages;

        public string GetExpeditionMessage(EExpeditionStateType type)
        {
            switch (type)
            {
                case EExpeditionStateType.Normal:
                    return GetRandomMessage(_normalExpeditionMessages);
                case EExpeditionStateType.Warning:
                    return GetRandomMessage(_warningExpeditionMessages);
                case EExpeditionStateType.Impossible:
                    return GetRandomMessage(_impossibleExpeditionMessages);
                default:
                    return string.Empty;
            }
        }

        private string GetRandomMessage(List<string> messages)
        {
            int randomIndex = Random.Range(0, messages.Count);

            return messages[randomIndex];
        }
    }
}
