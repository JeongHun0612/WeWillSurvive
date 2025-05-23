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
        Hunger,
        Thirst,
        Health,
        MaxCount
    }

    public enum ECharacterState
    {
        Normal,     // ����
        Hungry,     // �����
        Starve,     // ���� ���� (����)
        Thirsty,    // ����
        Dehydrate,  // ���� �� (Ż��)
        Hurt,       // ��ħ
        Sick,       // ����
        Anxiety,    // �Ҿ���
        Panic,      // ����
        Dead,       // ���
        None        // ����
    }

    public enum ECharacterMorale
    {
        VeryLow,     // �ſ� ����
        Low,
        Normal,
        High,
        VeryHigh     // �ſ� ����
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
        Food,
        SpecialFood,    // Ư����
        Water,
        MedicKit,
        SuperMedicKit,  // �Ƿ� ���� ŰƮ
        NiceSpacesuit,  // ��� ���ֺ�
        Radio,          // ���� ��� ���
        LaserGun,       // ��
        BoardGame,
        MaxCount
    }
}
