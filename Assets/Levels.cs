using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum TileTypes
{
    Empty = 0,
    Box = 1,
    BoxOnTarget = 2,
    cell = 3,
    PlayerStart = 4,
    PlayerOnTarget = 5,
    Target = 6,
    Wall = 7
}

public static class Symbols
{
    public static char[] symbols =
    {
        ' ', //empty
        '$', //box
        '*', //box on target 与目标点重合的箱子
        '-', //cell 地板
        '+', //player start 玩家初始位置
        '@', //player on target 与目标点重合的玩家
        '.', //target
        '#'  //wall
     }; 
}

[Serializable]
public class Levels : MonoBehaviour {
    //关卡列表
    public List<Level> levels;
    //预置体列表
    public GameObject PlayerPrefab;
    public GameObject BoxPrefab;
    public GameObject CellPrefab;
    public GameObject TargetPrefab;
    public GameObject WallPrefab;

    //编辑器图案
    public Texture2D playerTexture;
    public Texture2D playerOnTargetTexture;
    public Texture2D boxTexture;
    public Texture2D boxOnTargetTexture;
    public Texture2D cellTexture;
    public Texture2D wallTexture;
    public Texture2D targetTexture;
    public Texture2D emptyTexture;

}
