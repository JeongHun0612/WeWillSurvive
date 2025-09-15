using UnityEngine;
using System.Collections;

namespace WeWillSurvive
{
    public class SlideIn : MonoBehaviour
    {
        [Header("Slide Settings")]
        [SerializeField] private Vector3 targetPosition; // 목표 위치 (로컬 기준)
        [SerializeField] private Vector3 originalPosition;
        [SerializeField] private bool useLocalPosition = true; // 로컬/월드 선택

        private RectTransform rectTransform;
        private Coroutine slideRoutine;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            originalPosition = useLocalPosition ? rectTransform.anchoredPosition : rectTransform.position;
        }

        public void Slide(float moveTime, bool slidein = true)
        {
            if (slideRoutine != null)
                StopCoroutine(slideRoutine);

            slideRoutine = StartCoroutine(SlideCoroutine(moveTime, slidein));
        }

        private IEnumerator SlideCoroutine(float moveTime, bool slidein)
        {
            Vector3 start = originalPosition;
            Vector3 end = targetPosition;

            float elapsed = 0f;
            while (elapsed < moveTime)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / moveTime);

                Vector3 newPos = slidein ? Vector3.Lerp(start, end, t) : Vector3.Lerp(end, start, t);
                if (useLocalPosition)
                    rectTransform.anchoredPosition = newPos;
                else
                    rectTransform.position = newPos;

                yield return null;
            }

            if (useLocalPosition)
                rectTransform.anchoredPosition = slidein ? end : start;
            else
                rectTransform.position = slidein ? end : start;

            slideRoutine = null;
        }
    }
}

