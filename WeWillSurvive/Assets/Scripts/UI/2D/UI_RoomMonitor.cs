using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using WeWillSurvive.UI;
using static Define;

namespace WeWillSurvive
{
    public class UI_RoomMonitor : UI_Popup
    {
        [SerializeField] Button _leadButton;
        [SerializeField] Button _cookButton;
        [SerializeField] Button _bellButton;
        [SerializeField] Button _drKButton;

        UI_Background ui;

        protected override void Init()
        {
            base.Init();

            ui = null;
            if (GameManager.Instance.SceneUI is not UI_Background)
            {
                Debug.LogError("[UI_RoomMonitor] 2D Scene���� ������ �ʾ���");
                return;
            }
            ui = GameManager.Instance.SceneUI as UI_Background;

            // ���� ���� �� �������� �ʰų� ������ �� �� ���� + Ŭ�� ����
            CharacterInfo[] infos = CharacterManager.Instance.CharacterInfos;
            if (infos[(int)ECharacter.Lead].State[0] == ECharacterState.None || infos[(int)ECharacter.Lead].State[0] == ECharacterState.Dead)
                _leadButton.GetComponent<Image>().color = new Color32(100, 100, 100, 255);
            else
                _leadButton.onClick.AddListener(() => ui.ChangeBackground(ERoom.Lead));

            if (infos[(int)ECharacter.Cook].State[0] == ECharacterState.None || infos[(int)ECharacter.Cook].State[0] == ECharacterState.Dead)
                _cookButton.GetComponent<Image>().color = new Color32(100, 100, 100, 255);
            else
                _cookButton.onClick.AddListener(() => ui.ChangeBackground(ERoom.Cook));

            if (infos[(int)ECharacter.DrK].State[0] == ECharacterState.None || infos[(int)ECharacter.DrK].State[0] == ECharacterState.Dead)
                _drKButton.GetComponent<Image>().color = new Color32(100, 100, 100, 255);
            else
                _drKButton.onClick.AddListener(() => ui.ChangeBackground(ERoom.DrK));

            if (infos[(int)ECharacter.Bell].State[0] == ECharacterState.None || infos[(int)ECharacter.Bell].State[0] == ECharacterState.Dead)
                _bellButton.GetComponent<Image>().color = new Color32(100, 100, 100, 255);
            else
                _bellButton.onClick.AddListener(() => ui.ChangeBackground(ERoom.Bell));
        }
    }
}