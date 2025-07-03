using UnityEngine;

namespace WeWillSurvive
{
    public class RationPanel : PagePanel
    {
        public override void Initialize()
        {
            PanelType = EPanelType.Ration;
            PageCount = 1;
        }

        public override void InitializePage(int startPageIndex)
        {
            StartPageIndex = startPageIndex;
        }

        public override void ShowPage(int localIndex)
        {
            base.ShowPage(localIndex);
        }
    }
}
