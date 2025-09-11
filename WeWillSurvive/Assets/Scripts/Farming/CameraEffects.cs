using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

namespace WeWillSurvive
{
    public class CameraEffects : MonoBehaviour
    {
        public static CameraEffects Instance { get; private set; }

        [Header("Cinemachine Camera Reference")]
        [SerializeField] private CinemachineCamera vcam;

        [Header("Nudge Settings")]
        [SerializeField] private float nudgeDistance = 1000f;

        [Header("Zoom + Shake Settings")]
        [SerializeField] private float zoomTargetFOV = 70f;
        [SerializeField] private float maxShake = 2f;

        [SerializeField] private CinemachineBasicMultiChannelPerlin noise;
        private CinemachinePositionComposer composer;
        private Vector3 defaultTargetOffset;
        private float defaultFOV;

        private void Awake()
        {
            // Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (vcam == null)
            {
                Debug.LogError("CameraEffects: No CinemachineCamera assigned!");
                enabled = false;
                return;
            }

            composer = vcam.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer;
            if (composer == null)
            {
                Debug.LogError("CameraEffects: No CinemachinePositionComposer found!");
                enabled = false;
                return;
            }

            defaultTargetOffset = composer.TargetOffset;
            defaultFOV = vcam.Lens.FieldOfView;

            if (noise != null)
            {
                noise.AmplitudeGain = 0f;
                noise.FrequencyGain = 1f;
            }
            else
            {
                Debug.LogError("CameraEffects: No Perline noise found!");
            }
        }

        #region Nudge Effect
        public void NudgeLeft(float nudgeDuration, float stayDuration)
        {
            StopAllCoroutines();
            StartCoroutine(NudgeCoroutine(nudgeDistance, nudgeDuration, stayDuration));
        }

        private IEnumerator NudgeCoroutine(float distance, float duration, float stay)
        {
            Vector3 target = defaultTargetOffset + Vector3.left * distance;
            float t = 0f;

            // move left
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                composer.TargetOffset = Vector3.Lerp(defaultTargetOffset, target, t);
                yield return null;
            }

            yield return new WaitForSeconds(stay);

            // move back
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                composer.TargetOffset = Vector3.Lerp(target, defaultTargetOffset, t);
                yield return null;
            }

            composer.TargetOffset = defaultTargetOffset;
        }
        #endregion

        #region Zoom + Shake Effect
        public void ZoomOutShake(float zoomDuration)
        {
            StopAllCoroutines();
            StartCoroutine(ZoomOutShakeCoroutine(zoomTargetFOV, zoomDuration, maxShake));
        }

        private IEnumerator ZoomOutShakeCoroutine(float targetFOV, float duration, float maxShake)
        {
            float startFOV = vcam.Lens.FieldOfView;
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                vcam.Lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, t);

                if (noise != null)
                    noise.AmplitudeGain = Mathf.Lerp(0f, maxShake, t);

                yield return null;
            }
        }

        public void ResetCamera()
        {
            composer.TargetOffset = defaultTargetOffset;
            vcam.Lens.FieldOfView = defaultFOV;

            if (noise != null)
                noise.AmplitudeGain = 0f;
        }
        #endregion
    }
}
