using UnityEngine;

namespace WeWillSurvive
{
    public class FloatingEffect : MonoBehaviour
    {
        [Header("Movement Range")]
        [SerializeField] private float amplitude = 0.1f; // 최대 이동 범위
        [SerializeField] private float frequency = 1f;   // 진동 속도
        [SerializeField] private float horizontalFactor = 0.5f; // x 움직임 비율

        private Vector3 basePosition;
        private float randomOffset;

        void Start()
        {
            basePosition = transform.localPosition;
            randomOffset = Random.Range(0f, 100f); // 개별 오브젝트마다 위상 차
        }

        void Update()
        {
            float time = Time.time * frequency + randomOffset;

            // sin 파형을 이용한 상하 이동
            float y = Mathf.Sin(time) * amplitude;

            // cos 파형을 이용해 살짝 좌우도 움직이게
            float x = Mathf.Cos(time * 0.8f) * amplitude * horizontalFactor;

            transform.localPosition = basePosition + new Vector3(x, y, 0f);
        }
    }

}
