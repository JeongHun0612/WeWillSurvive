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
        Normal,     // 정상
        Hungry,     // 허기짐
        Starve,     // 영양 결핍 (굶음)
        Thirsty,    // 갈증
        Dehydrate,  // 수분 고갈 (탈수)
        Hurt,       // 다침
        Sick,       // 병듦
        Anxiety,    // 불안함
        Panic,      // 공포
        Dead,       // 사망
        None        // 나감
    }

    public enum ECharacterMorale
    {
        VeryLow,     // 매우 낮음
        Low,
        Normal,
        High,
        VeryHigh     // 매우 높음
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
        SpecialFood,    // 특별실
        Water,
        MedicKit,
        SuperMedicKit,  // 의료 만능 키트
        NiceSpacesuit,  // 고급 우주복
        Radio,          // 예비 통신 장비
        LaserGun,       // 총
        BoardGame,
        MaxCount
    }
}
