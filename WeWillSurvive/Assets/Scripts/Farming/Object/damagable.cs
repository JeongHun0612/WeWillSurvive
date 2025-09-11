using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using WeWillSurvive.Core;
using WeWillSurvive.Item;

namespace WeWillSurvive
{
    public class damagable : MonoBehaviour
    {
        [SerializeField] private EItem item;
        private bool canCollect = true;

        private void Start()
        {
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (canCollect && other.CompareTag("Player"))
            {
                Debug.Log($"item has been collected");
                var itemManager = ServiceLocator.Get<ItemManager>();
                if (itemManager != null)
                {
                    itemManager.AddItem(item);
                    if (item == EItem.Bell) { CrewUI.Instance.FoundCrew(0); }
                    else if (item == EItem.Cook) { CrewUI.Instance.FoundCrew(1); }
                    else if (item == EItem.DrK) { CrewUI.Instance.FoundCrew(2); }
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