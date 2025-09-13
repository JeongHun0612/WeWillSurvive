using UnityEngine;
using WeWillSurvive.Core;

namespace WeWillSurvive
{
    [RequireComponent(typeof(ParticleSystem))]
    public class EffectDeath : MonoBehaviour
    {
        void Start()
        {
            SoundManager.Instance.PlaySFX(FarmSoundMaster.Instance.GetMusic(12));

            var ps = GetComponent<ParticleSystem>();
            if (!ps.main.loop)
            {
                Destroy(gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
            }
        }
    }
}

