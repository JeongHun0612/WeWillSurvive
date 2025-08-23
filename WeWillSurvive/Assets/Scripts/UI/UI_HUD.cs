using WeWillSurvive.UI;

namespace WeWillSurvive
{
    public class UI_HUD : UI_Base
    {
        private const int DEFAULT_SORTING_ORDER = 10;

        public override void CanvasInitialize()
        {
            base.CanvasInitialize();

            _canvas.sortingOrder = DEFAULT_SORTING_ORDER;
        }
    }
}
