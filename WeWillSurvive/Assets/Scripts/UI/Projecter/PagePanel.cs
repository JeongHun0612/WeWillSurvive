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

    public class PagePanel : MonoBehaviour
    {
        public EPanelType PanelType { get; protected set; }
        public int PageCount { get; protected set; }
        public int StartPageIndex { get; protected set; }

        public virtual void Initialize() { }
        public virtual void RefreshPage(int startPageIndex) { StartPageIndex = startPageIndex; }
        public virtual void ShowPage(int localIndex) { gameObject.SetActive(true); }
        public virtual void ApplyResult() { }

        public void Hide() => gameObject.SetActive(false);
        public bool HasPage(int localIndex) => localIndex >= StartPageIndex && localIndex < StartPageIndex + PageCount;
    }
}
