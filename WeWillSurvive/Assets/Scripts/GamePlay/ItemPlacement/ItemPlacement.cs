using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WeWillSurvive.Item;
using WeWillSurvive.Util;

namespace WeWillSurvive
{
    [RequireComponent(typeof(ShowStatus))]
    public class ItemPlacement : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] protected EItem _itemType;
        [SerializeField] protected List<GameObject> _itemObjects;

        private ShowStatus _showStatus;

        public EItem ItemType => _itemType;
        public float Count { get; protected set; }

        public async virtual UniTask InitializeAsync()
        {
            _showStatus = gameObject.GetComponent<ShowStatus>();
            await UniTask.CompletedTask;
        }

        public virtual void Initialize()
        {
            _showStatus = gameObject.GetComponent<ShowStatus>();
        }

        public virtual void UpdateItemPlacement(float count)
        {
            Count = count;
            ItemObjectAllDeactivate();

            if (_itemObjects == null || _itemObjects.Count == 0)
                return;

            _itemObjects[0].SetActive(count != 0.0f);
        }

        protected void ItemObjectAllDeactivate()
        {
            foreach (var itemObject in _itemObjects)
            {
                itemObject.SetActive(false);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_showStatus == null)
                return;

            if (_itemType == EItem.Food || _itemType == EItem.Water)
                _showStatus.ShowStatusPanel($"{EnumUtil.GetDescription(_itemType)} : {Count}");
            else
                _showStatus.ShowStatusPanel($"{EnumUtil.GetDescription(_itemType)}");
        }
    }
}
