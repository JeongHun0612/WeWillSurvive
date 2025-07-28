using UnityEngine;
using WeWillSurvive.Core;

namespace WeWillSurvive.Expedition
{
    public enum EExpeditionState
    {
        Normal,
        Ready,
        Expedition
    }

    public class ExpeditionManager : MonoSingleton<ExpeditionManager>
    {
        private bool _isReady;
        private EExpeditionState _currentState;


        public bool IsReady => _isReady;
        public EExpeditionState CurrentState { get; set; }

        protected override void Awake()
        {
            _currentState = EExpeditionState.Normal;
        }
    }
}
