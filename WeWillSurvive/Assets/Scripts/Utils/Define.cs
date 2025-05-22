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
        Sick,       // ����
        Panic,      // ����
        Dead,       // ����
        None        // ����
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
