using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace WeWillSurvive
{
    public class damagable : MonoBehaviour
    {
        [SerializeField] private string itemName;
        private bool canCollect = true;

        private void Start()
        {
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
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
        }

        private void FixedUpdate()
        {
        }
    }
}