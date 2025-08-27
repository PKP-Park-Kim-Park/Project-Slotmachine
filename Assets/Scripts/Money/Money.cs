public class Money
{
    public int _token { get; private set; }
    public int _gold { get; private set; }
    private const int CONVERT_SIZE = 100_000;

    public Money(int startGold = 0, int startToken = 0)
    {
        _gold = startGold;
        _token = startToken;
    }

    public void AddGold(int amount)
    {
        _gold += amount;
    }

    public bool SpendGold(int amount)
    {
        if (amount > 0 && _gold >= amount)
        {
            _gold -= amount;
            return true;
        }
        return false; // 골드 부족
    }

    public void AddToken(int amount)
    {
        _token += amount;
    }

    public bool SpendToken(int amount)
    {
        if (amount > 0 && _token >= amount)
        {
            _token -= amount;
            return true;
        }

        return false; // 토큰 부족
    }

    public void ConvertToken()
    {
        _token += _gold / CONVERT_SIZE;

        _gold = 0;
    }
}
