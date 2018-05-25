using UnityEngine;
using System.Collections;

//单个关卡 参数
public class Level : ScriptableObject{
    //地图长宽
    [SerializeField]
    int sizeX;
    [SerializeField]
    int sizeY;
    public int SizeX{
        get {
            return sizeX;
        }
        set {
            if (value < 4)
                sizeX = 4;
            else
                sizeX = value;
        }
    }
    public int SizeY{
        get{
            return sizeY;
        }
        set{
            if (value < 4)
                sizeY = 4;
            else
                sizeY = value;
        }
    }

    // 地图布局定义 用字符数组表示
    [SerializeField]
    public string[] levelDef;
    [SerializeField]
    public bool foldout;
}
