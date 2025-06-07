using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.UI;

namespace WeWillSurvive
{
    public class UI_Room : UI_Popup
    {
        // TODO: 사기 + 상태별로 게임 오브젝트 만들어서 배열에 저장
        [SerializeField] GameObject[] _characters;

        UI_Background _ui;

        public override void Initialize()
        {
            base.Initialize();

            _ui =  UIManager.Instance.GetCurrentScene<UI_Background>();
            if (_ui == null)
            {
                Debug.LogError($"[{name}] 2D Scene에서 열리지 않았음");
                return;
            }
        }        

        // 방 세팅
        public void SetupRoomUI(ECharacter owner)
        {
            foreach (var c in _characters)
                c.SetActive(false);

            CharacterBase characterInfo = ServiceLocator.Get<CharacterManager>().GetCharacter(owner);

            GameObject character = _characters[(int)owner];
            character.SetActive(true);

            if (characterInfo == null || character == null)
            {
                Debug.LogError($"[{gameObject.name}] Character 찾을 수 없음");
                return;
            }

            // 우주 기지 내 존재하지 않거나 죽은 경우
            if (characterInfo.State.HasState(EState.Exploring) || characterInfo.State.HasState(EState.Dead))
            {
                // TODO: 빔 프로젝터 비활성화

                // 캐릭터 비활성화
                character.SetActive(false);
            }
        }
    }
}