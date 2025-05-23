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
        [SerializeField] private ECharacter _characterType = ECharacter.MaxCount;
        [SerializeField] private Vector2 _statusPanelPosition = Vector2.zero;

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
            UIManager.Instance.ClosePopups(remain: 1);
            UI_StatePanel ui = UIManager.Instance.ShowPopup<UI_StatePanel>();
            ui.transform.GetChild(0).localPosition = _statusPanelPosition;
            ui.SetPanel(_characterType);
        }
    }
}
