using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.Log;

namespace WeWillSurvive.MainEvent
{
    public class MainEventManager : MonoSingleton<MainEventManager>
    {
        public MainEventData DebugEventData;

        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        protected override void Awake()
        {
            base.Awake();

        }

        public void Test(EChoiceType choiceType)
        {

        }

        public void SelectEventChoice(EChoiceType choiceType)
        {
            var eventChoice = DebugEventData.GetEventChoice(choiceType);
            var result = eventChoice.GetRandomResult();

            //result.resultText
        }
    }
}
