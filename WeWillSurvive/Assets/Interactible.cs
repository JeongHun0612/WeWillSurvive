using UnityEngine;

namespace WeWillSurvive
{
    public enum InteractibleType
    {
        None,
        Item,
        NPC
    }

    public class Interactible : MonoBehaviour
    {
        [SerializeField] private SlotMaster inventory;
        public InteractibleType interactibleType = InteractibleType.None;

        // UI 표시용 텍스트
        private string itemPrompt = "E to Pick up";
        private string npcPrompt = "E to Save Crewmate";
        private string interactionPrompt;
        public int interactableValue;

        private void start()
        {
            if (interactibleType == InteractibleType.Item) interactionPrompt = itemPrompt;
            if (interactibleType == InteractibleType.NPC) interactionPrompt = npcPrompt;
        }

        // 선택되었을 때, UI 표시를 위한 콜백
        public virtual void ShowInteractionUI()
        {
            // UI 매니저에 메시지 전달 등 구현
            Debug.Log($"{gameObject.name} - {interactionPrompt}");
        }

        public virtual void HideInteractionUI()
        {
            // UI 매니저에 메시지 숨김 처리 등 구현
            Debug.Log($"{gameObject.name} interaction UI hidden");
        }

        // 상호작용 함수 - E 눌렀을 때 실행될 동작
        public virtual void Interact()
        {
            if (interactibleType == InteractibleType.Item)
            {
                Debug.Log($"Item{interactableValue} 획득");
                if (inventory.AddItem(interactableValue))
                {
                    Destroy(gameObject);
                }
            }

            if (interactibleType == InteractibleType.NPC)
            {
                Debug.Log($"Crew{interactableValue} 구출");
                if (inventory.AddCrew(interactableValue))
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}

