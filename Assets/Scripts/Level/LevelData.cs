using UnityEngine;
using System;

public class LevelData
{
    public event Action OnLevelChanged;
    public int _level { get; private set; }
    public int _minGold { get; private set; }
    public int _maxGold { get; private set; }
    public int _unitGold { get; private set; }
    private const int CONVERT_SIZE = 10_000;
    private const int MAX_SIZE = 5;

    public LevelData(int level)
    {
        SetLevel(level);
    }

    public void SetLevel(int level)
    {
        if (_level == level && _level != 0) return;

        _level = level;
        _minGold = level * CONVERT_SIZE;
        _maxGold = level * MAX_SIZE * CONVERT_SIZE;
        _unitGold = _minGold * level;

        // 레벨 변경
        Debug.Log($"레벨을 {level}(으)로 올립니다.");
        OnLevelChanged?.Invoke();
    }
}
