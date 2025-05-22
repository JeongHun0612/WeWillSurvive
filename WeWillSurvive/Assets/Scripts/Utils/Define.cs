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
        Normal,     // Á¤»ó
        Sick,       // ¾ÆÇÄ
        Panic,      // °øÆ÷
        Dead,       // Á×À½
        None        // ³ª°¨
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
