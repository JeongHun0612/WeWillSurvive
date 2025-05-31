using UnityEngine;

namespace WeWillSurvive
{
    public enum InteractibleType
    {
        None,
        Item,
        NPC,
        Putout
    }

    public class Interactible : MonoBehaviour
    {
        [SerializeField] private SlotMaster inventory;
        public InteractibleType interactibleType;
        public int interactableValue;

        public virtual void ShowInteractionUI()
        {
            Debug.Log($"{gameObject.name} is selected");
            InstructionUIManager.Instance.ShowUI(transform, interactibleType);
        }

        public virtual void HideInteractionUI()
        {
            Debug.Log($"{gameObject.name} interaction UI hidden");
            InstructionUIManager.Instance.HideAll();
        }
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

