using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Core;
using static Define;

namespace WeWillSurvive
{
    public struct ClickEvent { }

    public class UI_Background : UI_Scene
    {
        [Header("UI")]
        [SerializeField] Image _backgroundImage;
        [SerializeField] Button _leftButton;
        [SerializeField] Button _rightButton;
        [SerializeField] Sprite[] _backgroundSprites;

        GameObject _wipe = null;

        [Header("����")]
        [SerializeField] float _changeDuration = 0.5f;

        // �ӽ�
        const float SCREEN_WIDTH = 1920f;

        int _currentRoomIdx;
        bool _changingBackground;

        protected override void Init()
        {
            base.Init();
            SetBackground(ERoom.Main);

            ServiceLocator.Get<ResourceService>().LoadAsset("UI_Wipe").ContinueWith(prefab =>
            {
                GameObject go = Instantiate(prefab);
                _wipe = go.transform.GetChild(0).gameObject;
            }).Forget();

            _leftButton.onClick.AddListener(() => ChangeBackground((ERoom)(_currentRoomIdx - 1)));
            _rightButton.onClick.AddListener(() => ChangeBackground((ERoom)(_currentRoomIdx + 1)));
        }

        void SetBackground(ERoom roomName)
        {
            // ��� �̹��� ����
            _currentRoomIdx = (int)roomName;
            _backgroundImage.sprite = _backgroundSprites[_currentRoomIdx];

            // ��ư ���� üũ
            _leftButton.interactable = _currentRoomIdx > 0;
            _rightButton.interactable = _currentRoomIdx < (int)ERoom.MaxCount - 1;

            // Popup UI �ʱ�ȭ
            GameManager.Instance.CloseAllPopupUI();

            // ��濡 �´� Popup UI
            string popUIName = string.Empty;
            switch (roomName)
            {
                case ERoom.Main:
                    popUIName = "UI_Main";
                    break;
            }
            if (!string.IsNullOrEmpty(popUIName))
                ServiceLocator.Get<ResourceService>().LoadAsset(popUIName).ContinueWith(prefab => Instantiate(prefab)).Forget();
        }

        public void ChangeBackground(ERoom roomName)
        {
            // ���� ���̸� ���
            if (_changingBackground)
                return;

            int sign = (int)roomName < _currentRoomIdx ? 1 : -1;
            _wipe.transform.localPosition = new Vector3(-(SCREEN_WIDTH + 50f) * sign, 0f, 0f);
            _changingBackground = true;

            // Wipe Animation
            _wipe.transform.DOKill();
            _wipe.transform.DOLocalMoveX(0f, _changeDuration)
                .OnComplete(() =>
                {
                    SetBackground(roomName);
                    _wipe.transform.DOLocalMoveX((SCREEN_WIDTH + 50f) * sign, _changeDuration)
                        .OnComplete(() => _changingBackground = false);
                });
        }
    }
}
