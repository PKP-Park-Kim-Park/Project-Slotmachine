using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Symbols 열거형과 실제 Sprite를 매핑하고 관리하는 싱글톤 클래스입니다.
/// </summary>
public class SymbolManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static SymbolManager Instance { get; private set; }

    [System.Serializable]
    public class SymbolSpritePair
    {
        public Symbols symbol;
        public Sprite sprite;
    }

    [Tooltip("인스펙터에서 심볼과 스프라이트를 연결합니다.")]
    [SerializeField] private List<SymbolSpritePair> symbolSpritePairs;

    // 빠른 조회를 위한 딕셔너리
    private Dictionary<Symbols, Sprite> _symbolSpriteMap;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 리스트를 딕셔너리로 변환하여 조회 성능을 높입니다.
        _symbolSpriteMap = symbolSpritePairs.ToDictionary(pair => pair.symbol, pair => pair.sprite);
    }

    /// <summary>
    /// Symbols 열거형에 해당하는 Sprite를 반환합니다.
    /// </summary>
    public Sprite GetSprite(Symbols symbol)
    {
        if (_symbolSpriteMap.TryGetValue(symbol, out Sprite sprite))
        {
            return sprite;
        }
        Debug.LogWarning($"SymbolManager에서 '{symbol}'에 해당하는 스프라이트를 찾을 수 없습니다.");
        return null;
    }

    /// <summary>
    /// 관리되는 모든 심볼 스프라이트 목록을 반환합니다.
    /// </summary>
    /// <returns>모든 스프라이트의 리스트</returns>
    public List<Sprite> GetAllSprites()
    {
        return _symbolSpriteMap.Values.ToList();
    }
}
