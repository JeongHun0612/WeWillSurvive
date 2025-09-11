using System.Collections.Generic;
using UnityEngine;

namespace WeWillSurvive.FarmingReport
{
    [CreateAssetMenu(fileName = "FarmingReportData", menuName = "Scriptable Objects/FarmingReportData")]
    public class FarmingReportData : ScriptableObject
    {
        [SerializeField] private EParmingReportType _type;
        [SerializeField] private List<FarmingReport> _farmingReports = new();

        public EParmingReportType Type => _type;

        public string GetFarmingReportText(int value)
        {
            foreach (var farmingReport in _farmingReports)
            {
                if (value >= farmingReport.Value)
                    return farmingReport.ReportText;
            }

            return _farmingReports[_farmingReports.Count - 1].ReportText;
        }
    }

    [System.Serializable]
    public class FarmingReport
    {
        [SerializeField]
        [TextArea(4, 10)] 
        private string _reportText;

        [SerializeField]
        [Tooltip("해당 값 이상일 시")]
        private float _value;

        public string ReportText => _reportText;
        public float Value => _value;
    }
}
