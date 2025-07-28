using System;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Character;
using WeWillSurvive.Core;

namespace WeWillSurvive
{
    [RequireComponent(typeof(Button))]
    public class RoomMonitorButton : MonoBehaviour
    {
        [SerializeField] private ECharacter _characterType;
        [SerializeField] private ERoom _roomType;

        private Button _button;
        private CharacterBase _owner;

        private CharacterManager CharacterManager => ServiceLocator.Get<CharacterManager>();

        public void Initialize(Action<ERoom> callback)
        {
            _button = gameObject.GetComponent<Button>();
            _button.onClick.AddListener(() => callback?.Invoke(_roomType));

            _owner = CharacterManager.GetCharacter(_characterType);
        }

        public void UpdateButtonState()
        {
            if (_owner.IsDead || _owner.IsExploring)
            {
                //_button.image.color = new Color32(100, 100, 100, 255);
                _button.interactable = false;
            }
            else
            {
                //_button.image.color = Color.white;
                _button.interactable = true;
            }
        }
    }
}
