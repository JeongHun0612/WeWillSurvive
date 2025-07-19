using UnityEngine;

namespace WeWillSurvive.UI
{
    public class UI_Overlay : UI_Base
    {
        private const int DEFAULT_SORTING_ORDER = 50;

        public override void CanvasInitialize()
        {
            base.CanvasInitialize();

            _canvas.sortingOrder = DEFAULT_SORTING_ORDER;
        }
    }
}
