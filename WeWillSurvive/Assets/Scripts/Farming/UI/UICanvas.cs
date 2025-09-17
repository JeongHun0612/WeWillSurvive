using UnityEngine;
using System.Collections;
using WeWillSurvive.Core;
using WeWillSurvive.UI;

namespace WeWillSurvive
{
    public class UICanvas : MonoBehaviour
    {
        [SerializeField] private GameObject mapUI;
        [SerializeField] private GameObject timerUI;
        [SerializeField] private GameObject crewUI;
        [SerializeField] private GameObject startUI;
        void Awake()
        {
            mapUI.SetActive(true);
            timerUI.SetActive(true);
            crewUI.SetActive(true);
            startUI.SetActive(true);
        }
        public void StartSequence(float bias, float startui, float countdown, float slide)
        {
            StartCoroutine(SSequence(bias, startui, countdown, slide));
        }

        public void EndSequence(float bias, float endslide)
        {
            StartCoroutine(ESequence(bias, endslide));
        }

        private IEnumerator SSequence(float bias, float startui, float countdown, float slide)
        {
            yield return new WaitForSeconds(bias);
            startUI.GetComponent<StartUI>().StartSequence(startui);
            yield return new WaitForSeconds(startui + slide);
            timerUI.GetComponent<SevenSegTimer>().StartSequence(countdown);
            timerUI.GetComponent<SlideIn>().Slide(countdown * 0.5f);
            mapUI.GetComponent<SlideIn>().Slide(countdown * 0.5f);
            crewUI.GetComponent<SlideIn>().Slide(countdown * 0.5f);
        }

        private IEnumerator ESequence(float bias, float endslide)
        {
            yield return new WaitForSeconds(bias);
            timerUI.GetComponent<SevenSegTimer>().Halt();
            timerUI.GetComponent<SlideIn>().Slide(endslide, false);
            mapUI.GetComponent<SlideIn>().Slide(endslide, false);
            crewUI.GetComponent<SlideIn>().Slide(endslide, false);
        }

        public void OnClickSetting()
        {
            SoundManager.Instance.PlaySFX(ESFX.SFX_Click_2);

            UIManager.Instance.CloseAllPopups();
            UIManager.Instance.ShowPopup<UI_InGameSetting>();

            Time.timeScale = 0f;
        }
    }
}
