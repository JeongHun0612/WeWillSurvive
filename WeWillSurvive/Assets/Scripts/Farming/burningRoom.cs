using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace WeWillSurvive
{
    public class burningRoom : MonoBehaviour
    {
        [Range(0f, 1f)]
        public float BurnAmount = 0f;
        public float eachBurn = 0.0001f;
        public bool isStart = false;

        public Material materialAsset;
        private string propertyName = "_BurnAmount";

        public List<Graphic> targetGraphics;

        public Transform targetObject;
        public float moveFactor = 30f;
        public float offset = 103f;
        public float warningShot = 1f;

        public void CallStart()
        {
            StartCoroutine(StartWarningRoutine());
        }

        private IEnumerator StartWarningRoutine()
        {
            //make sound
            yield return new WaitForSeconds(warningShot);
            //make sound
            yield return new WaitForSeconds(warningShot);
            //make last sound
            isStart = true;
        }

        void FixedUpdate()
        {
            if (materialAsset != null && targetGraphics != null)
            {
                foreach (var graphic in targetGraphics)
                {
                    if (graphic != null && graphic.material != null)
                    {
                        graphic.materialForRendering.SetFloat(propertyName, BurnAmount);
                    }
                }
            }

            if (targetObject != null)
            {
                Vector3 pos = targetObject.position;
                pos.x = BurnAmount * moveFactor - offset;
                targetObject.position = pos;
            }

            if (isStart) BurnAmount += eachBurn;
        }
    }
}