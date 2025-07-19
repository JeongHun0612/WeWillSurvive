using System.Collections.Generic;
using UnityEngine;
using WeWillSurvive.Core;
using WeWillSurvive.Log;

namespace WeWillSurvive.MainEvent
{
    public class MainEventManager : MonoSingleton<MainEventManager>
    {
        public MainEventData DebugEventData;

        public List<MainEventData> DebugMainEventDatas = new();

        private LogManager LogManager => ServiceLocator.Get<LogManager>();

        protected override void Awake()
        {
            base.Awake();

        }

        public void SelectEventChoice(EChoiceType choiceType)
        {
            var eventChoice = DebugEventData.GetEventChoice(choiceType);
            var result = eventChoice.GetRandomResult();

            //result.resultText
        }

        public MainEventData GetRandomMainEventData()
        {
            if (DebugMainEventDatas == null || DebugMainEventDatas.Count == 0)
            {
                return null;
            }

            int randomIndex = Random.Range(0, DebugMainEventDatas.Count);
            return DebugMainEventDatas[randomIndex];
        }
    }
}
