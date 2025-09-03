using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;

[System.Serializable]
public class GameData
{
    public int level;
    public int gold;
    public int token;
    public Vector3 playerPos;

    public GameData()
    {
        level = 1;
        gold = 100_000;
        token = 0;
        playerPos = new Vector3(1f, 1f, 0f);
    }
}