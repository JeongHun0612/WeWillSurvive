using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.Item;
using WeWillSurvive.UI;

namespace WeWillSurvive
{
    public class UI_Title : UI_Scene
    {
        private DataManager DataManager => ServiceLocator.Get<DataManager>();
        private ItemManager ItemManager => ServiceLocator.Get<ItemManager>();

        public void OnClickGameStart()
        {
            GameManager.Instance.OnStartSurvive();

            // Debug 데이터 할당
            var itemDatas = DataManager.LoadDataList<ItemData>();
            foreach (var itemData in itemDatas)
            {
                if (!itemData.IsActive)
                    continue;

                ItemManager.AddItem(itemData.Item, itemData.Count);
            }
        }

        public void OnClickGameQuit()
        {
#if UNITY_EDITOR
            // 유니티 에디터에서 실행 중일 때는 플레이 모드를 종료
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // 빌드된 게임에서는 어플리케이션 종료
            Application.Quit();
#endif
        }

        public void OnClickItemSetting()
        {
            UIManager.Instance.ShowPopup<UI_DebugSetting>();
        }
    }
}
