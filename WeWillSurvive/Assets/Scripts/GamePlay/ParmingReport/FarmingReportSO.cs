using UnityEngine;

namespace WeWillSurvive.FarmingReport
{
    [CreateAssetMenu(fileName = "FarmingReportSO", menuName = "Scriptable Objects/FarmingReportSO")]
    public class FarmingReportSO : ScriptableObject
    {
        public EParmingReportType Type;

        // 조건 범위 정의
        public int MinValue;
        public int MaxValue;

        [TextArea(4, 10)]
        public string ReportText;

        public bool IsMatch(int value) 
        { 
            return value >= MinValue && value <= MaxValue; 
        }
    }
}
