
using WeWillSurvive.Item;

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

    public struct ItemSelectedEvent
    {
        public EItem Item;
        public bool IsSelected;
    }

    public struct ChoiceOptionSelectedEvent
    {
        public EItem Item;
        public bool IsSelected;
        public float RequiredAmount;
    }
    public struct RationItemSelectedEvent
    {
        public EItem Item;
        public bool IsSelected;
        public float RemainCount;
    }
}
