using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameModel : MonoBehaviour {

    public GameObject prefab_player;
    public GameObject prefab_cell;
    public GameObject prefab_box;

    private GameObject[,] chessGrid;
    public bool[,] isTarget;
    public bool[,] BoxPos;
    
    public  int chessLength; public  int chessWidth;
    public int playerX; public int playerZ;

    void Awake() {
        chessLength = 5; chessWidth = 5;
        chessGrid = new GameObject[chessLength,chessWidth];
    }
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
    /// <summary>
    /// 加载箱子、目标点坐标
    /// </summary>
    void loadPosition() {
        isTarget = new bool[, ]
        { { false,false,true,false,false},
          { false, false, false, false, false},
          { false,false,false,false,true},
          { false, false, false, false, false},
          { false,false,true,false,true }  };
        BoxPos=new bool[,]
        { {false,false,false,false,false},
          {false,false,true,false,false},
          {false,true,false,false,false},
          {false,false,false,true,false },
          {false,false,false,false,false}};
    }
    /// <summary>
    /// 初始化关卡
    /// 数组[i,j]对应坐标：( j, chessWidth-1-i )
    /// 坐标(x,z)对应数组：[ chessWidth-1-z , x ]
    /// </summary>
    public void setStage() {

        loadPosition();
        //设置地板 
        for (int i = 0; i < chessLength; i++)
            for (int j = 0; j < chessWidth; j++)
            {                     
                chessGrid[i, j] = Instantiate(prefab_cell, new Vector3(i, 0, j), Quaternion.identity) as GameObject;
                chessGrid[i, j].transform.parent = this.transform;
            }
        //设置目标点和箱子
        for (int i = 0; i < chessLength; i++)
            for (int j = 0; j < chessWidth; j++)
            {
                if (isTarget[i, j]) {
                    chessGrid[j, chessWidth - i - 1].GetComponent<MeshRenderer>().material.color = Color.blue;
                    chessGrid[j, chessWidth - i - 1].tag = "Destination";
                }
                if (BoxPos[i, j]) {
                    GameObject box;
                    box = Instantiate(prefab_box, new Vector3(j, 0.5f, chessWidth-i-1), Quaternion.identity) as GameObject;
                    box.transform.parent = this.transform;
                }
            }
                
       
        //设置玩家
        GameObject player;
        player = Instantiate(prefab_player, new Vector3(playerX, 1, playerZ),Quaternion.identity)as GameObject;
        player.transform.parent = this.transform;
    }
}

