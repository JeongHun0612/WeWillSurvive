using UnityEngine;
using UnityEngine.UI;

namespace WeWillSurvive
{

    public class TriggerHoldUI : MonoBehaviour
    {
        [Header("UI Settings")]
        [SerializeField] private GameObject holdUI;       // The whole UI object (setActive on/off)
        [SerializeField] private Image progressRing;      // The fill ring (Image -> Filled -> Radial360)
        [SerializeField] private float holdTime = 1f;     // Time required to fill

        [Header("Trigger Settings")]
        [SerializeField] private string triggerTag = "HoldZone"; // Tag of trigger object
        private KeyCode holdKey = KeyCode.Mouse1;
        private float holdTimer = 0f;
        private bool inTrigger = false;

        private void Start()
        {
            if (holdUI != null)
                holdUI.SetActive(false);

            if (progressRing != null)
                progressRing.fillAmount = 1f;
        }

        private void Update()
        {
            if (!inTrigger) return; // only works inside trigger

            if (Input.GetKey(holdKey))
            {
                holdTimer += Time.deltaTime;
                float progress = Mathf.Clamp01(holdTimer / holdTime);

                if (progressRing != null)
                {

                    if (progress >= 1f)
                    {
                        Debug.Log("Hold complete! Action triggered.");
                        HideUI();
                        FarmingSet.Instance.EndSeq(true);
                    }
                    else
                    {
                        progressRing.fillAmount = (1f - progress);
                    }
                }
            }
            else if (Input.GetKeyUp(holdKey))
            {
                ResetHold();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(triggerTag))
            {
                inTrigger = true;
                holdTimer = 0f;
                if (progressRing != null)
                    progressRing.fillAmount = 0f;

                if (holdUI != null)
                    holdUI.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(triggerTag))
            {
                inTrigger = false;
                HideUI();
            }
        }

        public void HideUI()
        {
            holdTimer = 0f;

            if (progressRing != null)
                progressRing.fillAmount = 1f;

            if (holdUI != null)
                holdUI.SetActive(false);
        }

        private void ResetHold()
        {
            holdTimer = 0f;
            if (progressRing != null)
                progressRing.fillAmount = 1f;
            // keep UI on, or HideUI() depending on design
        }
    }

}
