using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;
using WeWillSurvive.Core;
using static Define;

namespace WeWillSurvive
{
    public class PlayerInfo
    {
        public EPlayerStatus Status { get; private set; }
        public float Hunger { get; private set; }
        public float Thirst { get; private set; }

        public PlayerInfo()
        {
            Status = EPlayerStatus.Normal;
            Hunger = 100f;
            Thirst = 100f;
        }

        public void SetStatus(EPlayerStatus status) => Status = status;
        public void SetHunger(float value) => Hunger = Mathf.Clamp(value, 0f, 100f);
        public void SetThirst(float value) => Thirst = Mathf.Clamp(value, 0f, 100f);
    }

    /// <summary>
    /// 플레이어 상태 관리
    /// </summary>
    public class PlayerManager : MonoSceneSingleton<PlayerManager>
    {
        public PlayerInfo[] PlayerInfos;

        protected override void Awake()
        {
            base.Awake();

            // 데이터 초기화
            PlayerInfos = new PlayerInfo[(int)EPlayer.MaxCount];
            for (int i = 0; i < PlayerInfos.Length; i++)
                PlayerInfos[i] = new PlayerInfo();
        }

        public void SetPlayerStatus(EPlayer player, EPlayerStatus status)
        {
            PlayerInfos[(int)player].SetStatus(status);
        }
        public EPlayerStatus GetPlayerStatus(EPlayer player)
        {
            return PlayerInfos[(int)player].Status;
        }

        public void SetPlayerHunger(EPlayer player, float hunger)
        {
            PlayerInfos[(int)player].SetHunger(hunger);
        }
        public float GetPlayerHunger(EPlayer player)
        {
            return PlayerInfos[(int)player].Hunger;
        }

        public void SetPlayerThirst(EPlayer player, float thirsty)
        {
            PlayerInfos[(int)player].SetThirst(thirsty);
        }
        public float GetPlayerThirst(EPlayer player)
        {
            return PlayerInfos[(int)player].Thirst;
        }
    }
}
