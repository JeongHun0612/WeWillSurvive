using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.UI;

namespace WeWillSurvive
{
    public class RoomBase : MonoBehaviour
    {
        [SerializeField] protected ERoom _roomType;
        [SerializeField] protected Button _background;

        [SerializeField, HideInInspector] private UI_Character _characterUI;
        [SerializeField, HideInInspector] private GameObject _lightOff;

        public ERoom RoomType => _roomType;

        public async virtual UniTask InitializeAsync()
        {
            if (_characterUI != null)
                _characterUI.Initialize();

            _background.onClick.AddListener(() => UIManager.Instance.ClosePopups(remain: 1));

            await UniTask.CompletedTask;
        }

        public virtual void UpdateRoom()
        {
            _lightOff.SetActive(_characterUI.IsNoting);
            _characterUI.UpdateCharacterImage(ERoomType.CharacterRoom);
        }
    }
}