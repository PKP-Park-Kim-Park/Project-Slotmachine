using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Symbols 열거형과 실제 Sprite를 매핑하고 관리하는 싱글톤 클래스
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

    private Dictionary<Symbols, Sprite> _symbolSpriteMap;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _symbolSpriteMap = symbolSpritePairs.ToDictionary(pair => pair.symbol, pair => pair.sprite);
    }

    /// Symbols 열거형에 해당하는 Sprite를 반환
    public Sprite GetSprite(Symbols symbol)
    {
        if (_symbolSpriteMap.TryGetValue(symbol, out Sprite sprite))
        {
            return sprite;
        }
        Debug.LogWarning($"SymbolManager에서 '{symbol}'에 해당하는 스프라이트를 찾을 수 없습니다.");
        return null;
    }

    /// 관리되는 모든 심볼 스프라이트 목록을 반환
    public List<Sprite> GetAllSprites()
    {
        return _symbolSpriteMap.Values.ToList();
    }
}