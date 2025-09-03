using UnityEngine;

namespace WeWillSurvive
{
    public class FloatingEffect : MonoBehaviour
    {
        [Header("Movement Range")]
        [SerializeField] private float amplitude = 0.1f;
        [SerializeField] private float frequency = 1f;
        [SerializeField] private float horizontalFactor = 0.5f;

        [Header("Random Rotation (Z)")]
        [SerializeField] private float rotationMin = -10f;
        [SerializeField] private float rotationMax = 20f;
        [SerializeField] private float rotationStep = 10f;
        [SerializeField] private bool fixedR = false;
        [SerializeField] private float fixedRotation = 0f;

        private Vector3 basePosition;
        private float randomOffset;

        void Start()
        {
            basePosition = transform.localPosition;
            randomOffset = Random.Range(0f, 100f);

            if (!fixedR && rotationStep > 0f)
            {
                int steps = Mathf.FloorToInt((rotationMax - rotationMin) / rotationStep) + 1;
                int randomStep = Random.Range(0, steps);
                float chosenRotation = rotationMin + randomStep * rotationStep;
                transform.localRotation = Quaternion.Euler(0f, 0f, chosenRotation);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0f, 0f, fixedRotation);
            }
        }

        void Update()
        {
            float time = Time.time * frequency + randomOffset;
            float y = Mathf.Sin(time) * amplitude;
            float x = Mathf.Cos(time * 0.8f) * amplitude * horizontalFactor;
            transform.localPosition = basePosition + new Vector3(x, y, 0f);
        }
    }
}
