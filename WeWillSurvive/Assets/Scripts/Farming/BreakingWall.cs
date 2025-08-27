using UnityEngine;

namespace WeWillSurvive
{
    public class BreakingWall : MonoBehaviour
    {
        [SerializeField] private float maxHp = 10f;
        private float currentHp;

        private bool isBroken = false;

        private void Awake()
        {
            currentHp = maxHp;
        }

        public void TakeDamage(float damage)
        {
            if (isBroken) return;

            currentHp -= damage;
            Debug.Log($"Wall took {damage}, hp left: {currentHp}");

            // 피격 효과 (예: 색상 변환, 흔들림)
            OnHitEffect();

            if (currentHp <= 0f)
            {
                BreakWall();
            }
        }

        private void OnHitEffect()
        {
            // TODO: sprite 색상 조금 어둡게 한다거나,
            // cracks sprite 보여준다거나 하는 효과 넣기
        }

        private void BreakWall()
        {
            isBroken = true;

            // 충돌 막기 해제
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            // 시각적 효과 (벽 파편, 애니메이션 등)
            Destroy(gameObject);
            // 애니메이션 끝난 뒤 Destroy(this.gameObject);

            Debug.Log("Wall is broken!");
        }
    }

}
