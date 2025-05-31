using UnityEngine;

namespace WeWillSurvive
{
    public class InstructionUIManager : MonoBehaviour
    {
        public static InstructionUIManager Instance;

        public GameObject itemUI;
        public GameObject crewUI;
        public GameObject putoutUI;

        [SerializeField] float upUI;
        [SerializeField] float frontUI;
        private GameObject currentUI;

        private void Awake()
        {
            Instance = this;
            HideAll();
        }

        public void ShowUI(Transform worldTarget, InteractibleType type)
        {
            HideAll();

            switch (type)
            {
                case InteractibleType.Item:
                    currentUI = itemUI;
                    break;
                case InteractibleType.NPC:
                    currentUI = crewUI;
                    break;
                case InteractibleType.Putout:
                    currentUI = putoutUI;
                    break;
                default:
                    return;
            }

            currentUI.SetActive(true);
            UpdateUIPosition(worldTarget);
        }

        public void HideAll()
        {
            itemUI.SetActive(false);
            crewUI.SetActive(false);
            putoutUI.SetActive(false);
        }

        public void UpdateUIPosition(Transform worldTarget)
        {
            if (currentUI == null || !currentUI.activeSelf) return;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldTarget.position + Vector3.up * upUI + Vector3.forward * frontUI);
            currentUI.transform.position = screenPos;
        }
    }
}

