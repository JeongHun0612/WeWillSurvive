using UnityEngine;

namespace WeWillSurvive
{
    public enum InteractibleType
    {
        None,
        Item,
        Storage,
        NPC,
        Putout
    }

    public class Interactible : MonoBehaviour
    {
        public SlotMaster inventory;
        public storage storageScript;
        public collectionResult resultAdd;
        public InteractibleType interactibleType;
        public int interactableValue;

        public virtual void ShowInteractionUI()
        {
            InstructionUIManager.Instance.ShowUI(transform, interactibleType);
        }

        public virtual void HideInteractionUI()
        {
            InstructionUIManager.Instance.HideAll();
        }
        public virtual void Interact()
        {
            if (interactibleType == InteractibleType.Item)
            {
                if (inventory.AddItem(interactableValue))
                {
                    Destroy(gameObject);
                }
            }

            if (interactibleType == InteractibleType.NPC)
            {
                if (inventory.AddCrew(interactableValue))
                {
                    Destroy(gameObject);
                }
            }

            if (interactibleType == InteractibleType.Storage)
            {
                int itemID = storageScript.GetItemID();
                int repeatGet = storageScript.GetStorageAmount();
                int possibleAdd = inventory.GetAvailableAddAmount(itemID);
                if (repeatGet > possibleAdd) repeatGet = possibleAdd;

                storageScript.decrease(repeatGet);
                for (int i = 0; i < repeatGet; i++) inventory.AddItem(itemID);
            }

            if (interactibleType == InteractibleType.Putout)
            {
                resultAdd.Add(inventory.returnItem(), inventory.returnCrew());
                inventory.flushAll();
            }
        }

        public void Escape()
        {
            resultAdd.Add(inventory.returnItem(), inventory.returnCrew());
            inventory.flushAll();
            resultAdd.EndGame();
        }
    }
}

