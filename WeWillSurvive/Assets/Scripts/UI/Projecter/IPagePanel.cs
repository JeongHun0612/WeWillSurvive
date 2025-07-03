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

    public interface IPagePanel
    {
        public EPanelType Type { get; }
        public int PageCount { get; }

        public void Initialize();
        public void InitializePage();

        public void Hide();
        public void ShowPage(int localPageIndex);
    }

    public struct PageRange
    {
        public IPagePanel Panel;
        public int StartPageIndex;

        public PageRange(IPagePanel panel, int startPageIndex)
        {
            Panel = panel;
            StartPageIndex = startPageIndex;
        }
    }
}
