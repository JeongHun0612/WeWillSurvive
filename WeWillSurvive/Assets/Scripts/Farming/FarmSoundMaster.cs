using UnityEngine;
using WeWillSurvive.Core;

namespace WeWillSurvive
{

    [System.Serializable]
    public class FarmSlot
    {
        public string StringName;

        public string GetClipName()
        {
            if (!string.IsNullOrEmpty(StringName)) return StringName;
            return null;
        }
    }

    public class FarmSoundMaster : MonoBehaviour
    {
        public static FarmSoundMaster Instance { get; private set; }

        [Header("=== Background BGM ===")]
        public FarmSlot mainBGM;

        [Space(10)]
        [Header("=== Gameplay SFX ===")]
        public FarmSlot chargeSFX;
        public FarmSlot shootSFX;
        public FarmSlot bulletdeleteSFX;
        public FarmSlot wallbreakSFX;
        public FarmSlot itemSFX;
        public FarmSlot crewSFX;
        public FarmSlot emergencyBeepSFX;

        [Space(10)]
        [Header("=== Start SFX ===")]
        public FarmSlot startuiSFX;
        public FarmSlot alarmSFX;
        public FarmSlot alarmFinalSFX;

        [Space(10)]
        [Header("=== End SFX ===")]
        public FarmSlot exitsuccessSFX;
        public FarmSlot shipshootSFX;
        public FarmSlot collapseSFX;
        public FarmSlot hurtTextSFX;



        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public string GetMusic(int num)
        {
            if (num == 0)
            {
                return mainBGM.GetClipName();//
            }
            else
            {
                if (num / 10 == 1)
                {
                    if (num % 10 == 0) { return chargeSFX.GetClipName(); }//
                    if (num % 10 == 1) { return shootSFX.GetClipName(); }//
                    if (num % 10 == 2) { return bulletdeleteSFX.GetClipName(); }//
                    if (num % 10 == 3) { return wallbreakSFX.GetClipName(); }//
                    if (num % 10 == 4) { return itemSFX.GetClipName(); }//
                    if (num % 10 == 5) { return crewSFX.GetClipName(); }//
                    if (num % 10 == 6) { return emergencyBeepSFX.GetClipName(); } //
                }
                else if (num / 10 == 2)
                {
                    if (num % 10 == 1) { return startuiSFX.GetClipName(); } //
                    if (num % 10 == 2) { return alarmSFX.GetClipName(); } //
                    if (num % 10 == 3) { return alarmFinalSFX.GetClipName(); } //
                }
                else
                {
                    if (num % 10 == 1) { return exitsuccessSFX.GetClipName(); }//
                    if (num % 10 == 2) { return shipshootSFX.GetClipName(); }//
                    if (num % 10 == 3) { return collapseSFX.GetClipName(); }//
                    if (num % 10 == 4) { return hurtTextSFX.GetClipName(); }//
                }
            }
            return null;
        }


    }

}
