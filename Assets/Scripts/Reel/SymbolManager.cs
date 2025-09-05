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
        public Sprite[] sprites; // 애니메이션을 위해 Sprite 배열로 변경
    }

    [Tooltip("인스펙터에서 심볼과 스프라이트를 연결합니다.")]
    [SerializeField] private List<SymbolSpritePair> symbolSpritePairs;

    private Dictionary<Symbols, Sprite[]> _symbolSpriteMap;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Dictionary 초기화 로직을 Sprite 배열에 맞게 수정
        _symbolSpriteMap = new Dictionary<Symbols, Sprite[]>();
        foreach (var pair in symbolSpritePairs)
        {
            if (pair.sprites != null && pair.sprites.Length > 0)
            {
                _symbolSpriteMap[pair.symbol] = pair.sprites;
            }
            else
            {
                Debug.LogWarning($"Symbol '{pair.symbol}'에 할당된 스프라이트가 없습니다.");
            }
        }
    }

    /// Symbols 열거형에 해당하는 Sprite를 반환
    public Sprite[] GetSprites(Symbols symbol)
    {
        if (_symbolSpriteMap.TryGetValue(symbol, out Sprite[] sprite))
        {
            return sprite;
        }
        Debug.LogWarning($"SymbolManager에서 '{symbol}'에 해당하는 스프라이트를 찾을 수 없습니다.");
        return null;
    }

    /// Symbols 열거형에 해당하는 첫 번째 Sprite(기본 이미지)를 반환
    public Sprite GetStaticSprite(Symbols symbol)
    {
        if (_symbolSpriteMap.TryGetValue(symbol, out Sprite[] sprites) && sprites.Length > 0)
        {
            return sprites[0];
        }
        Debug.LogWarning($"SymbolManager에서 '{symbol}'에 해당하는 스프라이트를 찾을 수 없습니다.");
        return null;
    }

    /// 관리되는 모든 심볼 스프라이트 목록을 반환
    public List<Sprite> GetAllSprites()
    {
        // 각 심볼의 첫 번째 스프라이트만 블러 효과를 위해 반환
        return _symbolSpriteMap.Values.Select(sprites => sprites.Length > 0 ? sprites[0] : null).Where(s => s != null).ToList();
    }
}