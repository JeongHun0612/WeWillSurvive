using UnityEngine;
using UnityEngine.UI;

namespace WeWillSurvive
{
    public class OrbitPlayer : MonoBehaviour
    {
        public Transform circle;

        public float radius = 2f;
        public float rotationSpeed = 180f;
        public float stopSmoothness = 6f;
        public float startSmoothness = 3f;

        public float maxChargeTime = 2f;
        public float maxForce = 10f;

        private float currentRotationSpeed;
        private float targetRotationSpeed;
        private bool isCharging = false;
        private float chargeTime = 0f;

        private Rigidbody2D rb;
        private Image circleRenderer;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            circleRenderer = circle.GetComponent<Image>();

            circle.localPosition = new Vector3(radius, 0f, 0f);
            currentRotationSpeed = rotationSpeed;
            targetRotationSpeed = rotationSpeed;
        }

        void Update()
        {
            if (targetRotationSpeed < currentRotationSpeed)
                currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, targetRotationSpeed, Time.deltaTime * stopSmoothness);
            else
                currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, targetRotationSpeed, Time.deltaTime * startSmoothness);

            circle.RotateAround(transform.position, Vector3.forward, currentRotationSpeed * Time.deltaTime);

            if (Input.GetMouseButtonDown(0))
            {
                isCharging = true;
                chargeTime = 0f;
                targetRotationSpeed = 0f;
            }

            if (isCharging && Input.GetMouseButton(0))
            {
                chargeTime += Time.deltaTime;
                float clampedRatio = Mathf.Clamp01(chargeTime / maxChargeTime);
                circle.localScale = Vector3.one * (1f + (clampedRatio / 100f));
                circleRenderer.color = Color.Lerp(Color.white, Color.red, clampedRatio);
            }

            if (isCharging && Input.GetMouseButtonUp(0))
            {
                isCharging = false;
                targetRotationSpeed = rotationSpeed;

                float clampedTime = Mathf.Min(chargeTime, maxChargeTime);
                float chargeRatio = clampedTime / maxChargeTime;

                Vector2 pushDir = (transform.position - circle.position).normalized;
                rb.AddForce(pushDir * (chargeRatio * maxForce), ForceMode2D.Impulse);

                circle.localScale = Vector3.one;
                circleRenderer.color = Color.white;
                chargeTime = 0f;
            }
        }
    }
}
