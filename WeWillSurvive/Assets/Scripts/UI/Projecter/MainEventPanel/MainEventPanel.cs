using UnityEngine;

namespace WeWillSurvive
{
    public class MainEventPanel : PagePanel
    {
        public override void Initialize()
        {
            PanelType = EPanelType.MainEvent;
        }

        public override void RefreshPage(int startPageIndex)
        {
            StartPageIndex = startPageIndex;

            PageCount = 1;
        }

        public override void ShowPage(int localIndex)
        {
            base.ShowPage(localIndex);
        }
    }
}
