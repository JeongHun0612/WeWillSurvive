using UnityEngine;

namespace WeWillSurvive
{
    public class ExpeditionPanel : PagePanel
    {
        public override void Initialize()
        {
            PanelType = EPanelType.Expedition;
        }

        public override void InitializePage(int startPageIndex)
        {
            StartPageIndex = startPageIndex;

            // TODO 플레이어 중 누군가가 탐험을 나가있는 경우

            bool isExpedition = false;

            if (isExpedition)
                PageCount = 0;
            else
                PageCount = 1;
        }

        public override void ShowPage(int localIndex)
        {
            base.ShowPage(localIndex);
        }
    }
}
