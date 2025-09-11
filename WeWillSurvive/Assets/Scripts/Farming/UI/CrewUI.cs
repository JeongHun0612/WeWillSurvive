using System.Collections.Generic;
using UnityEngine;

namespace WeWillSurvive
{
    public class CrewUI : MonoBehaviour
    {
        public static CrewUI Instance { get; private set; }

        [SerializeField] private List<GameObject> crews = new List<GameObject>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void FoundCrew(int who)
        {
            if (who < 0 || who >= crews.Count) return;
            crews[who].SetActive(true);
        }
    }
}
