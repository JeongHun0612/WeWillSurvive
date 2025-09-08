using UnityEngine;

namespace WeWillSurvive
{
    public class Bullet : MonoBehaviour
    {
        public float bulletSpeed = 0.5f;
        public float liveTime = 5f;
        private float liveTimer = 0f;
        public float maxSize = 1f;
        public float minSize = 0.1f;
        public float maxDamage = 10f;
        private float swellAmount = 0f;
        private Vector3 originalScale;
        private Rigidbody2D rb;
        public GameObject hitEffectPrefab;

        [SerializeField] private bool isCharging = true;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            originalScale = transform.localScale;
        }

        void Update()
        {
            if (!isCharging)
            {
                liveTimer += Time.deltaTime;
                if (liveTimer >= liveTime)
                {
                    Die();
                }
            }
        }

        public void Swell(float amount)
        {
            swellAmount = amount;
            float size = Mathf.Lerp(minSize, maxSize, swellAmount);
            transform.localScale = originalScale * size;
        }

        public void Shoot(Vector2 dir)
        {
            isCharging = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.AddForce(dir * bulletSpeed, ForceMode2D.Impulse);
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (!isCharging)
            {
                if (col.CompareTag("BreakingWall"))
                {
                    float damage = swellAmount * maxDamage + 0.5f;
                    col.GetComponent<BreakingWall>()?.TakeDamage(damage);
                    Die();
                }

                if (col.CompareTag("NormalWall"))
                {
                    Die();
                }
            }
        }

        void OnTriggerStay2D(Collider2D col)
        {
            if (!isCharging)
            {
                if (col.CompareTag("BreakingWall"))
                {
                    float damage = swellAmount * maxDamage + 0.5f;
                    col.GetComponent<BreakingWall>()?.TakeDamage(damage);
                    Die();
                }

                if (col.CompareTag("NormalWall"))
                {
                    Die();
                }
            }
        }

        void Die()
        {
            if (hitEffectPrefab != null)
            {
                GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

                float bulletScale = transform.localScale.x;

                ParticleSystem ps = effect.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    var emission = ps.emission;
                    var burst = emission.GetBurst(0);

                    int minCount = 10;
                    int maxCount = 30;
                    int newCount = Mathf.RoundToInt(Mathf.Lerp(minCount, maxCount, bulletScale / maxSize));

                    burst.count = newCount;
                    emission.SetBurst(0, burst);
                }
            }
            Destroy(gameObject);
        }
    }
}
