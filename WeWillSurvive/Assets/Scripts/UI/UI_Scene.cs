using UnityEngine;
using WeWillSurvive.Core;

namespace WeWillSurvive.UI
{
    public class UI_Scene : UI_Base
    {
        public override void CanvasInitialize()
        {
            base.CanvasInitialize();

            _canvas.sortingOrder = 0;
        }
    }
}