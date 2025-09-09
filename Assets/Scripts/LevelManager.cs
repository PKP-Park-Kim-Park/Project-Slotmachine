using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public event Action<int> OnLevelUp;

    private Money _money;
    private LevelData _levelData;

    public void Initialize(Money money, LevelData levelData)
    {
        _money = money;
        _levelData = levelData;

        if (_money != null)
        {
            _money.OnMoneyChanged += CheckLevelUp;
        }
    }

    private void OnDestroy()
    {
        if (_money != null)
        {
            _money.OnMoneyChanged -= CheckLevelUp;
        }
    }
    // 레벨 오르는 조건
    private void CheckLevelUp()
    {
        if (_money == null || _levelData == null) return;

        // 소지금이 현재 레벨의 최대치(_maxGold)를 초과하면 레벨업
        if (_money._gold > _levelData._maxGold * 5)
        {
            int newLevel = _levelData._level + 1;
            _levelData.SetLevel(newLevel);
            OnLevelUp?.Invoke(newLevel);
        }
    }
}
