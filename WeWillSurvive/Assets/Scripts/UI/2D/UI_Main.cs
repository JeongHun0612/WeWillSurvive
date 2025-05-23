using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using static Define;

namespace WeWillSurvive
{
    public class UI_Main : UI_Popup
    {
        [SerializeField] private Button _roomMonitorButton;
        [SerializeField] private Button _nextDayButton;
        [SerializeField] private TextMeshProUGUI _dayText;

        // TODO: ��� + ���º��� �÷��̾� �̹��� �迭�� �־ ����
        // _characterImages[ECharacter.MaxCount][6? 7?]

        UI_Background ui;

        protected override void Init()
        {
            base.Init();

            ui = null;
            if (GameManager.Instance.SceneUI is not UI_Background)
            {
                Debug.LogError("[UI_Main] 2D Scene���� ������ �ʾ���");
                return;
            }
            ui = GameManager.Instance.SceneUI as UI_Background;

            // Room Monitor
            _roomMonitorButton.onClick.AddListener(() =>
                ServiceLocator.Get<ResourceService>().LoadAsset("UI_RoomMonitor").ContinueWith(prefab => Instantiate(prefab)).Forget());

            // Next Day
            _nextDayButton.onClick.AddListener(() => GameManager.Instance.BlackUI.FadeIO(() => NextDay()));

            UpdateUI();
        }

        private void NextDay()
        {
            // Day + 1
            GameManager.Instance.Day += 1;

            UpdateUI();
        }

        private void UpdateUI()
        {
            // Popup UI �ʱ�ȭ
            GameManager.Instance.ClosePopupUIs(remain: 1);

            _dayText.text = "Day " + GameManager.Instance.Day;

            // ĳ���� ���� ������Ʈ
            CharacterManager.Instance.UpdateCharacterInfos();

            // ĳ���� �̹��� ������Ʈ
            CharacterInfo[] infos = CharacterManager.Instance.CharacterInfos;
            foreach (CharacterInfo info in infos)
            {
                // ���� ���� �� �������� ������ ĳ���� ��Ȱ��ȭ
                if (info.State[0] == ECharacterState.None)
                {
                    Transform t = transform.Find($"Characters/{info.Name}");
                    if (t != null)
                        t.gameObject.SetActive(false);
                }
                // TODO: ���¿� ���� ��������Ʈ ����
            }

            // TODO: ������ ��ġ
            float cnt = GameManager.Instance.GetItemCount(EItem.Water);
            // �� ���� ��ŭ ��ġ
        }
    }
}