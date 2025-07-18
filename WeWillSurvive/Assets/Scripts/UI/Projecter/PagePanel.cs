using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WeWillSurvive
{
    public enum EPanelType
    {
        Log,
        Ration,
        Expedition,
        MainEvent
    }

    public abstract class PagePanel : MonoBehaviour
    {
        public EPanelType PanelType { get; protected set; }
        public int PageCount { get; protected set; }
        public int StartPageIndex { get; protected set; }

        public abstract void Initialize();

        public virtual async UniTask RefreshPageAsync(int startPageIndex) 
        {
            StartPageIndex = startPageIndex;
            await UniTask.CompletedTask;
        }

        public virtual void ShowPage(int localIndex) { gameObject.SetActive(true); }
        public virtual void ApplyResult() { }

        public void Hide() => gameObject.SetActive(false);
        public bool HasPage(int localIndex) => localIndex >= StartPageIndex && localIndex < StartPageIndex + PageCount;
    }
}
