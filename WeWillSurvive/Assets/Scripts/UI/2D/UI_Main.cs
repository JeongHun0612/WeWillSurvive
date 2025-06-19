using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeWillSurvive.Character;
using WeWillSurvive.Core;
using WeWillSurvive.UI;
using WeWillSurvive.Item;
using System.Linq;

namespace WeWillSurvive
{
    public class UI_Main : UI_Popup
    {
        [SerializeField] private Button _roomMonitorButton;
        [SerializeField] private Button _nextDayButton;
        [SerializeField] private TextMeshProUGUI _dayText;

        [Header("Items")]
        [SerializeField] private Transform _foods;
        [SerializeField] private Transform _waters;
        [SerializeField] private Transform _boardGame;
        [SerializeField] private Transform _gun;

        [Header("Debugs")]
        [SerializeField] private Button _getFoodButton;
        [SerializeField] private Button _useFoodButton;
        [SerializeField] private Button _getSpecialFoodButton;
        [SerializeField] private Button _getWaterButton;
        [SerializeField] private Button _useWaterButton;

        private UI_Background ui;
        private ItemManager _itemManager;

        private Sprite[] _foodSprites;
        private Sprite[] _waterSprites;

        public override void Initialize()
        {
            base.Initialize();

            ui = UIManager.Instance.GetCurrentScene<UI_Background>();
            if (ui == null)
            {
                Debug.LogError($"[{name}] 2D Scene에서 열리지 않았음");
                return;
            }

            // Room Monitor
            _roomMonitorButton.onClick.AddListener(() => UIManager.Instance.ShowPopup<UI_RoomMonitor>());

            // Next Day
            _nextDayButton.onClick.AddListener(() => UIManager.Instance.BlackUI.FadeIO(() =>
            {
                GameManager.Instance.NewDay();
                UpdateUI();
            }));

            _itemManager = ServiceLocator.Get<ItemManager>();
            InitializeDebugButtons();
            UpdateUI();
        }

        public override async UniTask InitializeAsync()
        {
            ResourceManager resource = ServiceLocator.Get<ResourceManager>();

            _foodSprites = new Sprite[2];
            _foodSprites[0] = await resource.LoadAssetAsync<Sprite>("food1");
            _foodSprites[1] = await resource.LoadAssetAsync<Sprite>("food2");

            _waterSprites = new Sprite[3];
            _waterSprites[0] = await resource.LoadAssetAsync<Sprite>("water1");
            _waterSprites[1] = await resource.LoadAssetAsync<Sprite>("water2");
            _waterSprites[2] = await resource.LoadAssetAsync<Sprite>("water3");

        }

        public override void OnShow()
        {
            base.OnShow();

            UpdateUI();
        }

        private void InitializeDebugButtons()
        {
            _getFoodButton.onClick.AddListener(() =>
            {
                _itemManager.AddItem(EItem.Food);
                UpdateUI();
            });
            _useFoodButton.onClick.AddListener(() =>
            {
                _itemManager.UsedItem(ServiceLocator.Get<CharacterManager>().Characters[ECharacter.Bell], EItem.Food, 0.25f);
                UpdateUI();
            });
            _getSpecialFoodButton.onClick.AddListener(() =>
            {
                _itemManager.AddItem(EItem.SpecialFood);
                UpdateUI();
            });
            _getWaterButton.onClick.AddListener(() =>
            {
                _itemManager.AddItem(EItem.Water);
                UpdateUI();
            });
            _useWaterButton.onClick.AddListener(() =>
            {
                _itemManager.UsedItem(ServiceLocator.Get<CharacterManager>().Characters[ECharacter.Bell], EItem.Water, 0.25f);
                UpdateUI();
            });
        }

        private void UpdateUI()
        {
            // Popup UI 초기화
            UIManager.Instance.ClosePopups(remain: 1);

            _dayText.text = "Day " + GameManager.Instance.Day;

            CharacterManager characterManager = ServiceLocator.Get<CharacterManager>();

            // 캐릭터 이미지 업데이트
            foreach (CharacterBase character in characterManager.GetAllCharacters())
            {
                Transform t = transform.Find($"Characters/{character.Name}");
                if (t == null || !t.gameObject.activeSelf) continue;

                // 우주 기지 내 존재하지 않으면 캐릭터 비활성화
                if (character.State.HasState(EState.Exploring))
                    t.gameObject.SetActive(false);
                else
                    t.GetChild(0).GetComponent<Image>().sprite = character.MainSprite;
            }

            // TODO: 아이템 배치
            UpdateFoodUI();
            UpdateWaterUI();

            _boardGame.gameObject.SetActive(_itemManager.GetItemCount(EItem.BoardGame) > 0);
            _gun.gameObject.SetActive(_itemManager.GetItemCount(EItem.LaserGun) > 0);
        }

        private void UpdateFoodUI()
        {
            // 초기화
            for (int i = 0; i < _foods.childCount; i++)
            {
                GameObject go = _foods.GetChild(i).gameObject;
                Image image = go.GetComponent<Image>();
                if (image.sprite == _foodSprites[1]) image.sprite = _foodSprites[0];
                go.SetActive(false);
            }

            GameObject foodBox = _foods.GetChild(6).gameObject;
            float foodCount = _itemManager.GetItemCount(EItem.Food);

            bool hasSpecialFood = _itemManager.GetItemCount(EItem.SpecialFood) > 0;
            _foods.GetChild(10).gameObject.SetActive(hasSpecialFood);

            if (hasSpecialFood)
            {
                if (foodCount > 3)
                    foodBox.SetActive(true);
                else
                    ActivateFoodRange(2, 0, foodCount);
            }
            else
            {
                if (foodCount > 9)
                {
                    foodBox.SetActive(true);
                    ActivateFoodRange(_foods.childCount - 2, 7, foodCount);
                }
                else
                {
                    ActivateFoodRange(_foods.childCount - 2, 0, foodCount);
                }
            }
        }

        private void UpdateWaterUI()
        {
            // 초기화
            for (int i = 1; i < _waters.childCount; i++)
            {
                GameObject go = _waters.GetChild(i).gameObject;
                Image image = go.GetComponent<Image>();
                image.sprite = _waterSprites[0];
                go.SetActive(false);
            }

            GameObject waterTank = _waters.GetChild(0).gameObject;
            float waterCount = _itemManager.GetItemCount(EItem.Water);

            waterTank.SetActive(waterCount > 6);
            if (waterCount > 6)
            {
                ActivateWaterRange(_waters.childCount - 1, 4, waterCount);
            }
            else
            {
                ActivateWaterRange(_waters.childCount - 1, 1, waterCount);
            }
        }

        private void ActivateFoodRange(int start, int end, float count)
        {
            if (count != Math.Floor(count))
                _foods.GetChild(start).GetComponent<Image>().sprite = _foodSprites[1];

            for (int i = start; i >= end; i--)
            {
                if (i == 6 || i == 10) continue;
                _foods.GetChild(i).gameObject.SetActive((count--) > 0);
            }
        }

        private void ActivateWaterRange(int start, int end, float count)
        {
            if (count != Math.Floor(count))
            {
                float f = count % 1;
                Image image = _waters.GetChild(start).GetComponent<Image>();

                if (f < 0.3f) image.sprite = _waterSprites[2];
                else image.sprite = _waterSprites[1];
            }

            for (int i = start; i >= end; i--)
            {
                _waters.GetChild(i).gameObject.SetActive((count--) > 0);
            }
        }
    }
}