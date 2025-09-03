using UnityEngine;

namespace WeWillSurvive
{
    [RequireComponent(typeof(ParticleSystem))]
    public class EffectDeath : MonoBehaviour
    {
        void Start()
        {
            var ps = GetComponent<ParticleSystem>();
            if (!ps.main.loop)
            {
                Destroy(gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
            }
        }
    }
}

