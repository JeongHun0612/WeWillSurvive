
namespace WeWillSurvive
{
    public struct EndDayEvent
    {

    }

    public struct NewDayEvent
    {
        public int CurrentDay;
    }

    public struct MoveRoomEvent
    {
        public ERoom TargetRoom;
    }

    public struct MoveRoomCompleteEvent
    {
        public ERoom CurrentRoom;
    }
}
