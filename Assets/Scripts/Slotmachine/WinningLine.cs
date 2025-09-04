using System.Collections.Generic;
using UnityEngine;

public class WinningLine
{
    public List<Vector2Int> Coordinates;
    public Patterns Pattern { get; }
    public float Odds { get; }

    public WinningLine(List<Vector2Int> coord, Patterns pattern, float odds)
    {
        Coordinates = coord;
        Pattern = pattern;
        Odds = odds;
    }
}
