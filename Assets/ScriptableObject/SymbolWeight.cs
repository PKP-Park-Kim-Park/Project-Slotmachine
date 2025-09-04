using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SymbolWeight", menuName = "Scriptable Objects/SymbolWeight")]
public class SymbolWeight : ScriptableObject
{
    [System.Serializable] // 직렬화 가능한 클래스로 유지
    public class WeightedSymbol
    {
        public Symbols symbol;
        [Range(0f, 100f)]
        public float probability;
    }

    [Header("심볼 확률")]
    [SerializeField] private List<WeightedSymbol> weightedSymbols;

    // 외부에서 데이터에 접근 가능
    public List<WeightedSymbol> WeightedSymbols => weightedSymbols;

    private void OnValidate()
    {
        // 인스펙터에서 값이 변경될 때마다 호출
        if (weightedSymbols == null || weightedSymbols.Count == 0)
        {
            SetDefaultProbabilities();
        }
    }

    private void SetDefaultProbabilities()
    {
        weightedSymbols = new List<WeightedSymbol>();
        int symbolCount = System.Enum.GetNames(typeof(Symbols)).Length;
        float equalProbability = 100f / symbolCount;

        for (int i = 1; i <= symbolCount; i++)
        {
            weightedSymbols.Add(new WeightedSymbol { symbol = (Symbols)i, probability = equalProbability });
        }
    }
}
