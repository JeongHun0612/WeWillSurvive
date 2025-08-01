using UnityEngine;
using WeWillSurvive.Character;
using WeWillSurvive.Core;

namespace WeWillSurvive.Expedition
{
    public enum EExpeditionState
    {
        Normal,
        Ready,
        Exploring
    }

    public class ExpeditionManager : MonoSingleton<ExpeditionManager>
    {
        private const int MIN_EXPEDITION_DAY = 2;
        private const int MAX_EXPEDITION_DAY = 3;

        private EExpeditionState _currentState;

        public EExpeditionState CurrentState => _currentState;


        protected override void Awake()
        {
            _currentState = EExpeditionState.Normal;

        }

        public void ReadyExpedition()
        {
            _currentState = EExpeditionState.Ready;
        }

        public void StartExpedition(CharacterBase target)
        {
            _currentState = EExpeditionState.Exploring;
            target.OnExploring();
        }

        public void CompleteExpedition()
        {
            _currentState = EExpeditionState.Normal;
        }

        public int GetRandomExpeditionDay()
        {
            return Random.Range(MIN_EXPEDITION_DAY, MAX_EXPEDITION_DAY + 1);
        }
    }
}
