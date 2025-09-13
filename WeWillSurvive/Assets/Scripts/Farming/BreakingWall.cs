using UnityEngine;
using WeWillSurvive.Core;

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
            string soundName = FarmSoundMaster.Instance.GetMusic(13);
            SoundManager.Instance.PlaySFX(soundName);
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
