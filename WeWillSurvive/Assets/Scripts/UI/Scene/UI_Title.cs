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
            //Testing farming connection
            //GameManager.Instance.OnStartParming();

            /**/

            GameManager.Instance.OnStartSurvive();

            // Debug ������ �Ҵ�
            var itemDatas = DataManager.LoadDataList<ItemData>();
            foreach (var itemData in itemDatas)
            {
                if (!itemData.IsActive)
                    continue;

                ItemManager.AddItem(itemData.Item, itemData.Count);
            }
            /**/
        }

        public void OnClickGameQuit()
        {
#if UNITY_EDITOR
            // ����Ƽ �����Ϳ��� ���� ���� ���� �÷��� ��带 ����
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // ����� ���ӿ����� ���ø����̼� ����
            Application.Quit();
#endif
        }

        public void OnClickItemSetting()
        {
            UIManager.Instance.ShowPopup<UI_DebugSetting>();
        }
    }
}
