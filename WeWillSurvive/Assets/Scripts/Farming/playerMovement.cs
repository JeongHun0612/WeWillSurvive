using UnityEngine;

namespace WeWillSurvive
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        public float maxSpeed = 5f;
        public float acceleration = 10f;
        public float drag = 5f;
        public bool disableRight = false;

        private Rigidbody2D rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            if (disableRight && moveX > 0f)
                moveX = 0f;

            Vector2 inputDir = new Vector2(moveX, moveY).normalized;

            Vector2 targetVelocity = inputDir * maxSpeed;
            Vector2 velocityChange = targetVelocity - rb.linearVelocity;
            velocityChange = Vector2.ClampMagnitude(velocityChange, acceleration * Time.fixedDeltaTime);

            rb.linearVelocity += velocityChange;
            rb.linearVelocity *= 1f / (1f + drag * Time.fixedDeltaTime);
        }
    }
}