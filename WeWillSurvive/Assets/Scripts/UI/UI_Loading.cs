using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.UI;

namespace WeWillSurvive
{
    public class UI_Loading : UI_Overlay
    {
        [SerializeField] private Image _background;

        [SerializeField] private Sprite[] _loadingSprites;

        public override void OnShow()
        {
            //int maxSpriteLen = _loadingSprites.Length;
            //int randomBackgroundIndex = UnityEngine.Random.Range(0, maxSpriteLen);

            //_background.sprite = _loadingSprites[randomBackgroundIndex];
            //_background.gameObject.SetActive(true);
        }
    }
}
