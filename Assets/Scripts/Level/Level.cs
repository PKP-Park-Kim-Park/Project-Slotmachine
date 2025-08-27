using UnityEngine;

public class Level
{
    public int _level { get; private set; }
    public int _minGold { get; private set; }
    public int _maxGold { get; private set; }
    public int _unitGold { get; private set; }
    private const int CONVERT_SIZE = 10_000;
    private const int MAX_SIZE = 5;

    public Level(int level)
    {
        _level = level;
        _minGold = level * CONVERT_SIZE;
        _maxGold = level * MAX_SIZE * CONVERT_SIZE;
        _unitGold = _minGold * level;
    }
}
