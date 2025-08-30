using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace WeWillSurvive
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Dictionary<string, int> collectedItems = new Dictionary<string, int>();
        public float maxSpeed = 5f;
        public float acceleration = 10f;
        public float drag = 5f;
        public bool disableRight = false;
        [Range(0f, 1f)]
        public float rightStrength = 1f;
        private bool inBurnoffZone = false;
        private bool canTakeDamage = true;

        public int HP = 3;
        private float damageCooldown = 2f;
        private int hp;

        private Rigidbody2D rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            hp = HP;
        }

        void FixedUpdate()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");

            if (disableRight && moveX > 0f)
            {
                if (disableRight) moveX = 0f;
                else moveX *= rightStrength;
            }

            Vector2 inputDir = new Vector2(moveX, moveY).normalized;

            Vector2 targetVelocity = inputDir * maxSpeed;
            Vector2 velocityChange = targetVelocity - rb.linearVelocity;
            velocityChange = Vector2.ClampMagnitude(velocityChange, acceleration * Time.fixedDeltaTime);

            rb.linearVelocity += velocityChange;
            rb.linearVelocity *= 1f / (1f + drag * Time.fixedDeltaTime);


            if (inBurnoffZone && canTakeDamage && hp > 0)
            {
                TakeDamage();
                if (damageCooldown > 0f && canTakeDamage) StartCoroutine(DamageCooldownRoutine());
            }
        }

        private void TakeDamage()
        {
            hp--;

            if (hp <= 0)
            {
                Debug.Log($"Player has died. Start end sequence.");
                // game end seqence
            }
        }

        private IEnumerator DamageCooldownRoutine()
        {
            canTakeDamage = false;
            yield return new WaitForSeconds(damageCooldown);
            canTakeDamage = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("burnoff2"))
            {
                inBurnoffZone = true;
            }

            if (other.CompareTag("roomExit"))
            {
                Debug.Log($"Player has ended the room. Start room end seqence.");
            }

        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("burnoff2"))
            {
                inBurnoffZone = false;
            }
        }

        public void record(string itemName)
        {
            if (collectedItems.ContainsKey(itemName))
            {
                collectedItems[itemName]++; // 이미 있으면 개수 증가
            }
            else
            {
                collectedItems[itemName] = 1; // 처음이면 1로 설정
            }

            Debug.Log($"Collected '{itemName}' x{collectedItems[itemName]} (Total unique items: {collectedItems.Count})");
        }
    }
}