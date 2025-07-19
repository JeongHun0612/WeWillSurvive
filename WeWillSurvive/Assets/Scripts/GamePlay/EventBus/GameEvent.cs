using UnityEngine;
using static Define;

namespace WeWillSurvive
{
    public struct NewDayEvent
    {
        public int CurrentDay;
    }

    public struct MoveRoomEvent
    {
        public ERoom TargetRoom;
    }
}
