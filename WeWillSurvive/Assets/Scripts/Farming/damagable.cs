using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace WeWillSurvive
{
    public class damagable : MonoBehaviour
    {
        [SerializeField] private string itemName;
        public float HP = 100f;
        public float hp;
        private float damageAmount = 1f;
        private float damageCooldown = 0f;
        private RectTransform rectT;
        public Material materialAsset;
        private string propertyName = "_BurnAmount";
        public Graphic graphic;

        private bool inBurnoffZone = false;
        private bool canTakeDamage = true;
        private bool canCollect = true;

        private void Start()
        {
            hp = HP;
            rectT = GetComponent<RectTransform>();
            graphic = GetComponent<Graphic>();
            Material instancedMat = new Material(materialAsset);
            graphic.material = instancedMat;
            graphic.materialForRendering.SetFloat("_width", rectT.rect.width * 1.42f);
            graphic.materialForRendering.SetFloat("_height", rectT.rect.height * 1.42f);
            Vector2 objPos = new Vector2(rectT.localPosition.x, rectT.localPosition.y);
            graphic.material.SetVector("_ObjPos", objPos);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("burnoff"))
            {
                inBurnoffZone = true;
            }

            if (canCollect && other.CompareTag("Player"))
            {
                Debug.Log($"item has been collected");
                var player = other.gameObject.GetComponent<PlayerMovement>();
                if (player != null)
                {
                    player.record(itemName);
                }
                Destroy(gameObject);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("burnoff"))
            {
                inBurnoffZone = false;
            }
        }

        private void FixedUpdate()
        {
            //only if using collider as well
            //Vector2 objPos = new Vector2(rectT.localPosition.x, rectT.localPosition.y);
            //graphic.material.SetVector("_ObjPos", objPos);

            if (inBurnoffZone && canTakeDamage && hp > 0)
            {
                TakeDamage(damageAmount);
                if (damageCooldown > 0f) StartCoroutine(DamageCooldownRoutine());
            }
        }

        private void TakeDamage(float amount)
        {
            hp -= amount;
            if (materialAsset != null && graphic != null && graphic.material != null)
            {
                graphic.materialForRendering.SetFloat(propertyName, 1f - (hp / HP));
            }
            Debug.Log($"Damage taken: {amount}, current HP: {hp}");

            if (hp <= 0.525)
            {
                Debug.Log($"{gameObject.name} has disappeared.");
                canCollect = false;
                // blacken and disappear
            }
        }

        private IEnumerator DamageCooldownRoutine()
        {
            canTakeDamage = false;
            yield return new WaitForSeconds(damageCooldown);
            canTakeDamage = true;
        }
    }
}