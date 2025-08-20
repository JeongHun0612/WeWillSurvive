using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.UI;

namespace WeWillSurvive.Room
{
    public abstract class RoomBase : MonoBehaviour
    {
        [SerializeField] protected ERoom _roomType;
        [SerializeField] protected Button _background;

        public ERoom RoomType => _roomType;

        public abstract void UpdateRoom();

        public async virtual UniTask InitializeAsync()
        {
            _background.onClick.AddListener(() => UIManager.Instance.ClosePopups(remain: 1));

            await UniTask.CompletedTask;
        }

        public virtual void OnShowRoom()
        {
            gameObject.SetActive(true);
        }
    }
}