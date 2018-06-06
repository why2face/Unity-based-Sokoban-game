using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using SokobanSolver.Sokoban;
using SokobanSolver.Engine;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class UIcontroller : MonoBehaviour {
    GameObject UItext;
    GameObject SolveText;
    GameObject[] AllBox;
    GameObject[] AllTarget;
    // 箱子数量 目标数量
    int BoxNum;
    int TargetNum;
    GameObject player;
    GameObject boxToMove;
    // 分数 移动步数
    public int score;
    public int moves;
    // 解题路径
    string path = "";

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        AllBox = GameObject.FindGameObjectsWithTag("Box");
        AllTarget = GameObject.FindGameObjectsWithTag("Destination");
        UItext = GameObject.Find("Text1");
        SolveText = GameObject.Find("Text2");
        BoxNum = AllBox.Length;
        TargetNum = AllTarget.Length;
        moves = 0;
        UItext.GetComponent<Text>().text = "Moves Count:  " + moves + "\nBox Num:  " + AllBox.Length + "\nScore:  " + score;
        //按钮事件 解题
        Button solveBtn = GameObject.Find("UI/Button1").GetComponent<Button>();
        solveBtn.onClick.AddListener(SolveListener);
        Button refreshBtn = GameObject.Find("UI/Button3").GetComponent<Button>();
        refreshBtn.onClick.AddListener(RefreshListener);
    }
	
	// Update is called once per frame
	void Update () {
        if(player.GetComponent<PlayerState>().nowState==PlayerStateType.Moving)
            JudegeWin();
    }
    void SolveListener()
    {
        //读取地图符号
        List<Level> stagelist = GameObject.Find("Level").GetComponent<Levels>().levels;
        int index = GameObject.Find("Level").GetComponent<Levels>().CurrentLevel - 1;
        Level currentStage = stagelist[index];
        string stageDef = "";
        for (int _y = 0; _y < currentStage.SizeY; _y++)
            for (int _x = 0; _x < currentStage.SizeX; _x++)
            {
                char s = currentStage.levelDef[_x * currentStage.SizeY + _y][0];
                stageDef += s;
                if (_x == currentStage.SizeX - 1)
                    stageDef += Environment.NewLine;
            }
        Debug.Log(stageDef);
        FileStream stream = File.Open("temp.txt", FileMode.OpenOrCreate, FileAccess.Write);
        stream.Seek(0, SeekOrigin.Begin);
        stream.SetLength(0);
        stream.Close();
        StreamWriter sw = new StreamWriter("temp.txt", true, System.Text.Encoding.GetEncoding("gb2312"));
        sw.WriteLine(stageDef);
        sw.Flush();
        sw.Close();
        //算出路径
        var position = SokobanPosition.LoadFromFile("temp.txt");
        var solver = new Solver<SokobanPosition>();
        Solution<SokobanPosition> solution = solver.AStar(position);
        if (solution.FinalPosition != null)
        {
            path = SokobanUtil.GetFullPath(solution.GetPath().ToArray());
            SolveText.GetComponent<Text>().text = "Solution: \n" + path;
        }
        else {
            SolveText.GetComponent<Text>().text = "Solution not found.";
        }
        //按钮 播放动画 ↑↓←→
        Button btn2 = GameObject.Find("UI/Button2").GetComponent<Button>();
        btn2.interactable = true;
        btn2.onClick.AddListener(StartAnimate);

    }
    public void StartAnimate() {
        StartCoroutine(Animate());
    }
    IEnumerator Animate() {
        for (int i = 0; i < path.Length; i++) {
            if (path[i] == '↑')
            {
                Debug.Log('↑');
                RaycastHit hit;
                if (Physics.Raycast(player.transform.position, Vector3.forward, out hit, 1))
                {
                    if (hit.collider.gameObject.tag == "Wall")
                    {  //撞墙 返回
                        yield return null;
                    }
                    else if (hit.collider.gameObject.tag == "Box")
                    {
                        boxToMove = hit.collider.gameObject;
                        RaycastHit[] temp = Physics.RaycastAll(player.transform.position, Vector3.forward, 2);
                        //撞箱子 判断是否可推
                        if (temp.Length == 1)
                        {
                            player.GetComponent<PlayerState>().move("W");
                            boxToMove.GetComponent<BoxState>().move("W");
                            moves++;
                            yield return null;
                        }
                        else yield return null;
                    }
                }
                else
                {   //无碰撞 
                    player.GetComponent<PlayerState>().move("W");
                    moves++;
                    yield return null;
                }
            }
            else if (path[i] == '↓')
            {
                Debug.Log('↓');
                RaycastHit hit;
                if (Physics.Raycast(player.transform.position, Vector3.back, out hit, 1))
                {
                    if (hit.collider.gameObject.tag == "Wall")
                    {
                        yield return null;
                    }
                    else if (hit.collider.gameObject.tag == "Box")
                    {
                        boxToMove = hit.collider.gameObject;
                        RaycastHit[] temp = Physics.RaycastAll(player.transform.position, Vector3.back, 2);
                        if (temp.Length == 1)
                        {
                            player.GetComponent<PlayerState>().move("S");
                            boxToMove.GetComponent<BoxState>().move("S");
                            moves++;
                            yield return null;
                        }
                        else yield return null;
                    }
                }
                else
                {
                    player.GetComponent<PlayerState>().move("S");
                    moves++;
                    yield return null;
                }
            }
            else if (path[i] == '←')
            {
                Debug.Log('←');
                RaycastHit hit;
                if (Physics.Raycast(player.transform.position, Vector3.left, out hit, 1))
                {
                    if (hit.collider.gameObject.tag == "Wall")
                    {   yield return null;
                    }
                    else if (hit.collider.gameObject.tag == "Box")
                    {
                        boxToMove = hit.collider.gameObject;
                        RaycastHit[] temp = Physics.RaycastAll(player.transform.position, Vector3.left, 2);
                        if (temp.Length == 1)
                        {
                            player.GetComponent<PlayerState>().move("A");
                            boxToMove.GetComponent<BoxState>().move("A");
                            moves++;
                            yield return null;
                        }
                        else yield return null;
                    }
                }
                else
                {
                    player.GetComponent<PlayerState>().move("A");
                    moves++;
                    yield return null;
                }
            }
            else if (path[i] == '→') {
                Debug.Log('→');
                RaycastHit hit;
                if (Physics.Raycast(player.transform.position, Vector3.right, out hit, 1))
                {
                    if (hit.collider.gameObject.tag == "Wall")
                    {   yield return null;
                    }
                    else if (hit.collider.gameObject.tag == "Box")
                    {
                        boxToMove = hit.collider.gameObject;
                        RaycastHit[] temp = Physics.RaycastAll(player.transform.position, Vector3.right, 2);
                        if (temp.Length == 1)
                        {
                            player.GetComponent<PlayerState>().move("D");
                            boxToMove.GetComponent<BoxState>().move("D");
                            moves++;
                            yield return null;
                        }
                        else yield return null;
                    }
                }
                else
                {
                    player.GetComponent<PlayerState>().move("D");
                    moves++;
                    yield return null;
                }
            }
            //暂停一秒后继续执行for循环
            yield return new WaitForSeconds(0.5f) ;
        }
    }

    void RefreshListener() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    //得分判断
    void JudegeWin() {
        score = 0;
        foreach (GameObject b in AllBox)
            foreach (GameObject d in AllTarget)
            {
                if (b.transform.position.x > d.transform.position.x - 0.5
                  && b.transform.position.x < d.transform.position.x + 0.5
                  && b.transform.position.z > d.transform.position.z - 0.5
                  && b.transform.position.z < d.transform.position.z + 0.5)
                    score++;
            }
        UItext.GetComponent<Text>().text = "Moves Count:  " + moves + "\nBox Num:  " + AllBox.Length + "\nScore:  " + score;
        if (score == AllBox.Length)
            UItext.GetComponent<Text>().text = UItext.GetComponent<Text>().text + "\n (￣3￣) YOU WIN";
    }

}
