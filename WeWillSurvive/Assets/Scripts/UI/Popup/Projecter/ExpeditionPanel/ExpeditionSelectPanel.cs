using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace WeWillSurvive
{
    public class ExpeditionSelectPanel : MonoBehaviour
    {
        [SerializeField] private List<ExpeditionSelectCharacter> _expeditionSelectCharacters;

        private ExpeditionSelectCharacter _selectCharacter;

        public ExpeditionSelectCharacter SelectCharacter => _selectCharacter;

        public async UniTask InitializeAsync()
        {
            foreach (var expeditionSelectCharacter in _expeditionSelectCharacters)
            {
                await expeditionSelectCharacter.InitializeAsync();
                expeditionSelectCharacter.RegisterClickEvent(OnClickSelectCharacter);
            }
        }

        public void UpdateSelectPanel()
        {
            _selectCharacter = null;

            // expeditionSelectCharacter 업데이트
            foreach (var expeditionSelectCharacter in _expeditionSelectCharacters)
            {
                expeditionSelectCharacter.UpdateSelectCharacter();
            }
        }

        public void OnClickSelectCharacter(ExpeditionSelectCharacter selectCharacter)
        {
            if (_selectCharacter != null && _selectCharacter == selectCharacter)
            {
                _selectCharacter.OnSelected(false);
                _selectCharacter = null;
                return;
            }

            if (_selectCharacter != null)
                _selectCharacter.OnSelected(false);

            _selectCharacter = selectCharacter;
            _selectCharacter.OnSelected(true);
        }
    }
}