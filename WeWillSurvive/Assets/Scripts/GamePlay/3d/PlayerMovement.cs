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

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;

            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            RotateView();
            Move();
            Vector3 horizontalVelocity = rb.linearVelocity;

            if (horizontalVelocity.magnitude > maxSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
                rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
            }
        }

        void Move()
        {
            float speed = Input.GetKey(KeyCode.LeftShift) ? moveSpeed * boosterMultiplier : moveSpeed;

            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
            forward.Normalize();
            right.Normalize();

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 moveDir = (forward * v + right * h).normalized;

            // 방향 입력이 있을 때만 힘을 가함
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
