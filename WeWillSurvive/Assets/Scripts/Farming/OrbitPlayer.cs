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

        public float pushThreshold = 3f;

        private Rigidbody2D rb;
        private Animator animator;
        private Image circleRenderer;
        private Bullet currentBullet;
        public GameObject bulletPrefab;

        public Color circleColor = Color.white;
        public Color endColor = Color.red;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
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

            if (!isCharging && Input.GetMouseButtonDown(0))
            {
                isCharging = true;
                chargeTime = 0f;
                targetRotationSpeed = 0f;

                GameObject bulletObj = Instantiate(bulletPrefab, circle.position, Quaternion.identity, circle);
                currentBullet = bulletObj.GetComponent<Bullet>();
            }
            else if (currentBullet != null && isCharging && Input.GetMouseButton(0))
            {
                chargeTime += Time.deltaTime;
                float clampedRatio = Mathf.Clamp01(chargeTime / maxChargeTime);
                circle.localScale = Vector3.one * (1f + (clampedRatio / 100f));
                circleRenderer.color = Color.Lerp(circleColor, endColor, clampedRatio);

                float clampedTime = Mathf.Min(chargeTime, maxChargeTime);
                currentBullet.Swell(clampedTime / maxChargeTime);
            }
            else if (currentBullet != null && isCharging && Input.GetMouseButtonUp(0))
            {
                isCharging = false;
                targetRotationSpeed = rotationSpeed;

                float clampedTime = Mathf.Min(chargeTime, maxChargeTime);
                float chargeRatio = clampedTime / maxChargeTime;

                Vector2 pushDir = (transform.position - circle.position).normalized;
                rb.AddForce(pushDir * (chargeRatio * maxForce), ForceMode2D.Impulse);

                circle.localScale = Vector3.one;
                circleRenderer.color = circleColor;
                chargeTime = 0f;

                currentBullet.gameObject.transform.SetParent(transform.parent, worldPositionStays: true);
                currentBullet.Shoot(-pushDir);
                currentBullet = null;
            }

            animator.SetBool("isPush", (rb.linearVelocity.magnitude > pushThreshold));
            if (rb.linearVelocity.x < -0.01f) { transform.localRotation = Quaternion.Euler(0f, 180f, 0f); }
            else if (rb.linearVelocity.x > 0.01f) { transform.localRotation = Quaternion.Euler(0f, 0f, 0f); }

        }
    }
}
