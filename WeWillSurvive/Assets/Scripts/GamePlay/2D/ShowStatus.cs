using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.UI;
using static Define;

namespace WeWillSurvive
{
    public class ShowStatus : MonoBehaviour
    {
        [SerializeField] ECharacter _characterType = ECharacter.MaxCount;
        [SerializeField] Vector2 _statusPanelPosition = Vector2.zero;

        private void Awake()
        {
            // �̸� ����� ������������ Parse�ؼ� ���
            ECharacter type;
            if (Enum.TryParse(gameObject.name, true, out type))
                _characterType = type;

            // �̸� + enum ��� �߸��Ǿ������� ����
            if (_characterType == ECharacter.MaxCount)
            {
                Debug.LogError($"[{gameObject.name}] �߸��� �̸�");
                return;
            }

            // Ŭ�� ���� ���� Image�� ���߱�
            GetComponent<RectTransform>().sizeDelta = transform.Find("CharacterImage").GetComponent<RectTransform>().sizeDelta;
        }

        public void ShowStatusPanel()
        {
            ServiceLocator.Get<ResourceService>().LoadAsset("UI_StatusPanel").ContinueWith(prefab =>
            {
                GameManager.Instance.ClosePopupUIs(remain: 1);
                GameObject go = Instantiate(prefab);
                go.transform.GetChild(0).localPosition = _statusPanelPosition;
                go.AddComponent<UI_Popup>();
            }).Forget();
        }
    }
}
