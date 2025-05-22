using UnityEngine;

public static class Define
{
    public enum ECharacter
    {
        Lead,
        Cook,
        DrK,
        Bell,
        MaxCount
    }

    public enum ECharacterStatus
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
