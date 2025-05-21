using UnityEngine;

public static class Define
{
    public enum EPlayer
    {
        Lead,
        Cook,
        DrK,
        Bell,
        MaxCount
    }

    public enum EPlayerStatus
    {
        Normal,     // 정상
        Disease,
        None        // 나감, 죽음
    }

    public enum ERoom
    {
        Lead,
        Cook,
        Main,
        DrK,
        Bell,
        MaxCount
    }

    public enum EItem
    {
        Water,
        Food,
        MaxCount
    }
}
