using UnityEngine;

namespace WeWillSurvive
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovementFlat : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float boosterMultiplier = 2f;
        public float maxSpeed = 10f;
        public float mouseSensitivity = 2f;

        private Rigidbody rb;
        private float rotationX = 0f;
        private float rotationY = 0f;

        // 입력 저장용
        private Vector2 moveInput;
        private bool isBoosting = false;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;

            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            // 마우스 회전 처리
            RotateView();

            // 키보드 입력 처리
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            moveInput = new Vector2(h, v);
            isBoosting = Input.GetKey(KeyCode.LeftShift);
        }

        void FixedUpdate()
        {
            Move();
            
            Vector3 horizontalVelocity = rb.linearVelocity;

            if (horizontalVelocity.magnitude > maxSpeed)
            {
                Vector3 clamped = horizontalVelocity.normalized * maxSpeed;
                rb.linearVelocity = new Vector3(clamped.x, rb.linearVelocity.y, clamped.z);
            }
        }

        void Move()
        {
            float speed = isBoosting ? moveSpeed * boosterMultiplier : moveSpeed;

            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            Vector3 moveDir = (forward * moveInput.y + right * moveInput.x).normalized;

            if (moveDir.sqrMagnitude > 0.01f)
            {
                rb.AddForce(moveDir * speed, ForceMode.Acceleration);
            }
        }

        void RotateView()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            rotationY += mouseX;
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);
            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        }
    }
}