using UnityEngine;
using WeWillSurvive.Core;

namespace WeWillSurvive.UI
{
    public class UI_Scene : UI_Base
    {
        public override void Initialize()
        {
            base.Initialize();

            // Canvas ¼³Á¤
            Canvas canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            canvas.sortingOrder = 0;
        }
    }
}