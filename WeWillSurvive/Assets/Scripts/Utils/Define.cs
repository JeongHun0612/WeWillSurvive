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
        Normal,     // ����
        Disease,
        None        // ����, ����
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
