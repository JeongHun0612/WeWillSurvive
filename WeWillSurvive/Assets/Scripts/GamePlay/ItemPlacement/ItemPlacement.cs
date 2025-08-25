using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WeWillSurvive.Core;
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

        public EItem ItemType { get => _itemType; protected set => _itemType = value; }
        public float Count { get; protected set; }

        protected ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public async virtual UniTask InitializeAsync()
        {
            _showStatus = gameObject.GetComponent<ShowStatus>();
            await UniTask.CompletedTask;
        }

        public virtual void UpdateItemPlacement()
        {
            if (_itemObjects == null || _itemObjects.Count == 0)
                return;

            Count = ItemManager.GetItemCount(_itemType);
            ItemObjectAllDeactivate();

            ItemObjectActivate(Count);
        }

        protected virtual void ItemObjectActivate(float count)
        {
            _itemObjects[0].SetActive(count != 0f);
        }

        protected virtual string BuildStatusText() => EnumUtil.GetInspectorName(_itemType);

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

            _showStatus.ShowStatusPanel(BuildStatusText());
        }
    }
}
