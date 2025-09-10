using System;
public class Money
{
    public event Action OnMoneyChanged;
    public int _token { get; private set; }
    public int _gold { get; private set; }
    public int _startGold;
    private const int CONVERT_SIZE = 100_000;

    public Money(int startGold = 0, int startToken = 0)
    {
        _gold = startGold;
        _startGold = startGold;
        _token = startToken;
    }

    public void AddGold(int amount)
    {
        _gold += amount;
        OnMoneyChanged?.Invoke();
    }

    public bool SpendGold(int amount)
    {
        if (amount > 0 && _gold >= amount)
        {
            _gold -= amount;
            OnMoneyChanged?.Invoke();
            return true;
        }
        return false; // 골드 부족
    }

    public void AddToken(int amount)
    {
        _token += amount;
        OnMoneyChanged?.Invoke();
    }

    public bool SpendToken(int amount)
    {
        if (amount > 0 && _token >= amount)
        {
            _token -= amount;
            OnMoneyChanged?.Invoke();
            return true;
        }

        return false; // 토큰 부족
    }

    public void ConvertToken()
    {
        if (_gold < 0)
        {
            _gold = 0;
        }
        _token += _gold / CONVERT_SIZE;
        _gold = _startGold;
        OnMoneyChanged?.Invoke();
    }
}
