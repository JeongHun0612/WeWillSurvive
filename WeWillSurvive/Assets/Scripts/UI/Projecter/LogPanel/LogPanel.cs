using UnityEngine;

namespace WeWillSurvive
{
    public class LogPanel : PagePanel
    {
        public override void Initialize()
        {
            PanelType = EPanelType.Log;
        }

        public override void RefreshPage(int startPageIndex)
        {
            StartPageIndex = startPageIndex;

            PageCount = 3;
        }

        public override void ShowPage(int localIndex)
        {
            base.ShowPage(localIndex);
        }
    }
}
