using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEditor;

[Serializable]
[CustomEditor(typeof(Levels))]
public class LevelEditor : Editor {
    Levels targetLevels;
    GUISkin skin;
    //界面是否折叠
    bool foldTex = false;
    bool foldPrefab = false;
    bool foldLevels = false;
    // 默认单元 cell(地板)
    TileTypes tileTypes = TileTypes.Cell;

    //用一组button表示地图单元
    void SetupSkin()
    {
        if (skin == null)
            skin = (GUISkin)ScriptableObject.CreateInstance(typeof(GUISkin));
        //border 边界 padding margin 偏移
        skin.button.border = new RectOffset(0, 0, 0, 0);
        skin.button.padding = new RectOffset(0, 0, 0, 0);
        skin.button.margin = new RectOffset(0, 0, 0, 0);
        //单元长宽
        skin.button.fixedHeight = 30;
        skin.button.fixedWidth = 30;
        skin.button.stretchHeight = true;
        skin.button.stretchWidth = true;
    }

    public override void OnInspectorGUI()
    {
        targetLevels = (Levels)target;
        List<Level> levels = targetLevels.levels;
        SetupSkin();
        //垂直组布局
        EditorGUILayout.BeginVertical();
        //编辑器图标
        foldTex = EditorGUILayout.Foldout(foldTex, "editor textures");
        if (foldTex){
            targetLevels.playerTexture = EditorGUILayout.ObjectField("Player Texture", targetLevels.playerTexture, typeof(Texture2D), false) as Texture2D;
            targetLevels.playerOnTargetTexture = EditorGUILayout.ObjectField("Player On Target Texture", targetLevels.playerOnTargetTexture, typeof(Texture2D), false) as Texture2D;
            targetLevels.boxTexture = EditorGUILayout.ObjectField("Box Texture", targetLevels.boxTexture, typeof(Texture2D), false) as Texture2D;
            targetLevels.boxOnTargetTexture = EditorGUILayout.ObjectField("Box On Target Texture", targetLevels.boxOnTargetTexture, typeof(Texture2D), false) as Texture2D;
            targetLevels.wallTexture = EditorGUILayout.ObjectField("Wall Texture", targetLevels.wallTexture, typeof(Texture2D), false) as Texture2D;
            targetLevels.targetTexture = EditorGUILayout.ObjectField("Target Texture", targetLevels.targetTexture, typeof(Texture2D), false) as Texture2D;
            targetLevels.emptyTexture = EditorGUILayout.ObjectField("empty Texture", targetLevels.emptyTexture, typeof(Texture2D), false) as Texture2D;
            targetLevels.cellTexture = EditorGUILayout.ObjectField("Cell Texture", targetLevels.cellTexture, typeof(Texture2D), false) as Texture2D;
        }
        //地图预置体
        foldPrefab = EditorGUILayout.Foldout(foldPrefab, "Prefabs");
        if (foldPrefab){
            targetLevels.PlayerPrefab = EditorGUILayout.ObjectField("Player Prefab", targetLevels.PlayerPrefab, typeof(GameObject), false) as GameObject;
            targetLevels.BoxPrefab = EditorGUILayout.ObjectField("Box Prefab", targetLevels.BoxPrefab, typeof(GameObject), false) as GameObject;
            targetLevels.TargetPrefab = EditorGUILayout.ObjectField("Target Prefab", targetLevels.TargetPrefab, typeof(GameObject), false) as GameObject;
            targetLevels.WallPrefab = EditorGUILayout.ObjectField("Wall Prefab", targetLevels.WallPrefab, typeof(GameObject), false) as GameObject;
            targetLevels.CellPrefab = EditorGUILayout.ObjectField("Cell Prefab", targetLevels.CellPrefab, typeof(GameObject), false) as GameObject;
        }
        //EditorGUILayout.Separator();
        foldLevels = EditorGUILayout.Foldout(foldLevels, "Stage List");
        if (foldLevels){
            //i:关卡号 lvl即LVL
            int i = 1;
            foreach (Level lvl in levels){
                EditorGUI.indentLevel++;
                lvl.foldout = EditorGUILayout.Foldout(lvl.foldout, "Level " + i);
                if (lvl.foldout){
                    EditorGUILayout.Separator();
                    EditorGUILayout.BeginHorizontal();
                    // 移除关卡 按钮
                    bool removeButton = GUILayout.Button("Remove", GUILayout.Width(80.0f));
                    if (removeButton){
                        levels.Remove(lvl);
                        EditorGUILayout.EndHorizontal();
                        break;
                    }
                    // 加载关卡 按钮
                    bool loadButton = GUILayout.Button("Load Stage", GUILayout.Width(80.0f));
                    if (loadButton){
                        ClearStage();
                        GameObject temp;
                        for (int _y = 0; _y < lvl.SizeY; _y++) {
                            for (int _x = 0; _x < lvl.SizeX; _x++) {
                                //当前单元
                                char s = lvl.levelDef[_x * lvl.SizeY + _y][0];
                                switch (SymbolToState(s)) { 
                                    case (int)TileTypes.Box:
                                        temp = Instantiate(targetLevels.CellPrefab, _y, 0, _x);
                                        temp.transform.parent = GameObject.Find("Grounds").transform;
                                        temp = Instantiate(targetLevels.BoxPrefab, _y, 0.5f, _x);
                                        temp.transform.parent = GameObject.Find("Boxes").transform;
                                        break;
                                    case (int)TileTypes.BoxOnTarget:
                                        temp = Instantiate(targetLevels.TargetPrefab, _y, 0, _x);
                                        temp.transform.parent = GameObject.Find("Targets").transform;
                                        break;
                                    case (int)TileTypes.Cell:
                                        temp = Instantiate(targetLevels.CellPrefab, _y, 0, _x);
                                        temp.transform.parent = GameObject.Find("Grounds").transform;
                                        break;
                                    case (int)TileTypes.PlayerStart:
                                        temp = Instantiate(targetLevels.CellPrefab, _y, 0, _x);
                                        temp.transform.parent = GameObject.Find("Grounds").transform;
                                        temp = Instantiate(targetLevels.PlayerPrefab, _y, 0.8f, _x);
                                        break;
                                    case (int)TileTypes.PlayerOnTarget:
                                        temp = Instantiate(targetLevels.TargetPrefab, _y, 0, _x);
                                        temp.transform.parent = GameObject.Find("Targets").transform;
                                        temp = Instantiate(targetLevels.PlayerPrefab, _y, 0.8f, _x);
                                        break;
                                    case (int)TileTypes.Wall:
                                        temp = Instantiate(targetLevels.WallPrefab, _y, 0, _x);
                                        temp.transform.parent = GameObject.Find("Walls").transform;
                                        break;
                                    case (int)TileTypes.Target:
                                        temp = Instantiate(targetLevels.TargetPrefab, _y, 0, _x);
                                        temp.transform.parent = GameObject.Find("Targets").transform;
                                        break;
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    // 地图长宽 输入框
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Size (X * Y):", GUILayout.Width(90.0f));
                    lvl.SizeX = EditorGUILayout.IntField(lvl.SizeX, GUILayout.Width(48.0f));
                    lvl.SizeY = EditorGUILayout.IntField(lvl.SizeY, GUILayout.Width(48.0f));
                    EditorGUILayout.EndHorizontal();

                    // 使用一个字符数组存放地图布局 size = sizeX*sizeY
                    if (lvl.levelDef == null)
                        lvl.levelDef = new string[0];
                    Array.Resize(ref lvl.levelDef, lvl.SizeX * lvl.SizeY);
                    // 显示字符数组
                    EditorGUILayout.LabelField("Level Definition:");
                    for (int m = 0; m < lvl.SizeY; m++) {
                        EditorGUILayout.BeginHorizontal();
                        for (int n = 0; n < lvl.SizeX; n++) {
                            EditorGUILayout.LabelField(lvl.levelDef[n * lvl.SizeY + m],
                                GUILayout.Width(30.0f));
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.Separator();
                    // 单元格类型 枚举列表
                    EditorGUILayout.BeginHorizontal();
                    tileTypes = (TileTypes)EditorGUILayout.EnumPopup("Unit Type", tileTypes);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Separator();
                    //地图单元编辑
                    for (int _y = 0; _y < lvl.SizeY; _y++){
                        EditorGUILayout.BeginHorizontal();
                        for (int _x = 0; _x < lvl.SizeX; _x++){
                            int index = _x * lvl.SizeY + _y;
                            // 初始化 默认全部单元为empty,再尝试读取各单元的定义
                            char chr = ' ';
                            try{
                                chr = lvl.levelDef[index][0];
                            }catch{ }
                            // buttonState：单元(_x,_y)的类型索引
                            int buttonState = SymbolToState(chr);                      
                            Texture2D texture = null;
                            switch (buttonState){
                                //condition ? first_expression : second_expression;  
                                //若所选材质不为null，texture=该材质。否则texture=null
                                case 1:
                                    texture = (targetLevels.boxTexture != null) ? 
                                        targetLevels.boxTexture : null;
                                    break;
                                case 2:
                                    texture = (targetLevels.boxOnTargetTexture != null) ? 
                                        targetLevels.boxOnTargetTexture : null;
                                    break;
                                case 3:
                                    texture = (targetLevels.cellTexture != null) ? 
                                        targetLevels.cellTexture : null;
                                    break;
                                case 4:
                                    texture = (targetLevels.playerTexture != null) ? 
                                        targetLevels.playerTexture : null;
                                    break;
                                case 5:
                                    texture = (targetLevels.playerOnTargetTexture != null) ? 
                                        targetLevels.playerOnTargetTexture : null;
                                    break;
                                case 6:
                                    texture = (targetLevels.targetTexture != null) ? 
                                        targetLevels.targetTexture : null;
                                    break;
                                case 7:
                                    texture = (targetLevels.wallTexture != null) ? 
                                        targetLevels.wallTexture : null;
                                    break;
                                case 0:
                                    texture = (targetLevels.emptyTexture != null) ? 
                                        targetLevels.emptyTexture : null;
                                    break;
                            }
                            if (GUILayout.Button(texture, skin.button)){
                                buttonState = (int)tileTypes;
                            }
                            lvl.levelDef[index] = StateToSymbol(buttonState).ToString();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUI.indentLevel--;
                EditorUtility.SetDirty(lvl);
                i++;
            }
            //创建新关卡
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            bool addButton = GUILayout.Button("Add New Stage", GUILayout.Width(97.0f));
            if (addButton){
                levels.Add((Level)ScriptableObject.CreateInstance("Level"));
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        EditorUtility.SetDirty(targetLevels);

        //清空场景
        EditorGUILayout.Separator();
        EditorGUILayout.BeginHorizontal();
        bool clearButton = GUILayout.Button("Clear Stage", GUILayout.Width(80.0f));
        if (clearButton)
            ClearStage();
        EditorGUILayout.EndHorizontal();
    }

    //在坐标(x,y,z)实例化物体
    public GameObject Instantiate(GameObject obj, int x,float y ,int z) {
        return (GameObject)GameObject.Instantiate(obj, new Vector3(x,y,z), Quaternion.identity);
    }

    /// <summary>
    ///  输入符号 返回该符号的索引
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    int SymbolToState(char symbol) {
        int i = 0;
        foreach (char c in Symbols.symbols)
        {
            if (c == symbol)
                return i;
            i++;
        }
         return 0;
    }
    /// <summary>
    /// 输入索引值 返回对应的符号
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    char StateToSymbol(int state) {
        return Symbols.symbols[state];
    }

    public void ClearStage() {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player"))
            DestroyImmediate(obj);
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Box"))
            DestroyImmediate(obj);
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Cell"))
            DestroyImmediate(obj);
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Wall"))
            DestroyImmediate(obj);
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Destination"))
            DestroyImmediate(obj);
    }
   
}
