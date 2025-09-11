using UnityEngine;

namespace WeWillSurvive
{
    public class BreakingWall : MonoBehaviour
    {

        private bool isBroken = false;

        private void Awake()
        {
        }

        public void TakeDamage(float damage)
        {
            if (isBroken) return;

            BreakWall();
        }

        private void OnHitEffect()
        {
            // TODO: cracks sprite 보여준다거나 하는 효과 넣기
        }

        private void BreakWall()
        {
            isBroken = true;
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
            OnHitEffect();
            Destroy(gameObject);
        }
    }

}
