using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public GameObject stage;
    GameObject player;
    GameObject[] boxGroup;
    GameObject[] destGroup;
    GameObject boxToMove;
    GameObject textUI;
    bool[,] boxPos;
    int score;
    int chessLength;
    int chessWidth;

	// Use this for initialization
	void Start () {
        //设置关卡
        stage.GetComponent<GameModel>().setStage();
        //箱子坐标
        boxPos = stage.GetComponent<GameModel>().BoxPos;
        chessLength = stage.GetComponent<GameModel>().chessLength;
        chessWidth = stage.GetComponent<GameModel>().chessWidth;

        //获取玩家、箱子、目标点对象
        player = GameObject.FindGameObjectWithTag("Player");
        boxGroup = GameObject.FindGameObjectsWithTag("box");
        destGroup = GameObject.FindGameObjectsWithTag("Destination");
        textUI = GameObject.Find("Text");
        boxToMove = null;
        
        //判断
        InvokeRepeating("judgeWin", 1, 1);
    }

	// Update is called once per frame
	void Update () {
        //玩家状态机update()
        player.GetComponent<Player>().pFsm.UpdateState(Time.deltaTime);
        //箱子状态机update()
        if (boxToMove != null)
            boxToMove.GetComponent<Box>().bFsm.UpdateState(Time.deltaTime);

        getInput();
    }
    //输入控制
    void getInput()
    {
        string msg=null;

        if (Input.GetKeyDown(KeyCode.W)) {
            msg = "W";
            //判断边界      
            if (player.transform.position.z + 1 < chessWidth) {
                //判断该方向上是否与箱子相邻
                if (boxPos[(int)(chessWidth - player.transform.position.z - 2), (int)player.transform.position.x])
                {
                    //相邻，判断箱子是否能推动
                    if (player.transform.position.z + 2 < chessWidth)
                        if (!(boxPos[(int)(chessWidth - player.transform.position.z - 3), (int)player.transform.position.x]))
                        {
                            //可推动
                            RaycastHit hit;
                            if (Physics.Raycast(player.transform.position, Vector3.forward, out hit, 1))
                            {
                                boxToMove = hit.collider.gameObject;
                                player.GetComponent<Player>().pFsm.SetState(Player.PlayerState.Walking, msg);
                                boxToMove.GetComponent<Box>().bFsm.SetState(Box.BoxState.Moving,msg);
                                boxPos[(int)(chessWidth - player.transform.position.z - 2), (int)player.transform.position.x] = false;
                                boxPos[(int)(chessWidth - player.transform.position.z - 3), (int)player.transform.position.x] = true;
                            }                               
                        }                   
                }
                else {
                    //不相邻，直接移动
                    player.GetComponent<Player>().pFsm.SetState(Player.PlayerState.Walking, msg);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            msg = "S";
            //判断边界      
            if (player.transform.position.z - 1 >= 0)
            {
                //判断该方向上是否与箱子相邻
                if (boxPos[(int)(chessWidth - player.transform.position.z), (int)player.transform.position.x])
                {
                    //相邻，判断箱子是否能推动
                    if (player.transform.position.z - 2 >= 0)
                        if (!(boxPos[(int)(chessWidth - player.transform.position.z + 1), (int)player.transform.position.x]))
                        {
                            //可推动
                            RaycastHit hit;
                            if (Physics.Raycast(player.transform.position, Vector3.back, out hit, 1))
                            {
                                boxToMove = hit.collider.gameObject;
                                player.GetComponent<Player>().pFsm.SetState(Player.PlayerState.Walking, msg);
                                boxToMove.GetComponent<Box>().bFsm.SetState(Box.BoxState.Moving, msg);
                                boxPos[(int)(chessWidth - player.transform.position.z ), (int)player.transform.position.x] = false;
                                boxPos[(int)(chessWidth - player.transform.position.z +1), (int)player.transform.position.x] = true;
                            }
                        }
                }
                else
                {
                    //不相邻，直接移动
                    player.GetComponent<Player>().pFsm.SetState(Player.PlayerState.Walking, msg);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            msg = "A";
            if (player.transform.position.x - 1 >= 0)
            {
                //判断该方向上是否与箱子相邻
                if (boxPos[(int)(chessWidth - 1 - player.transform.position.z), (int)(player.transform.position.x - 1 )])
                {
                    //相邻，判断箱子是否能推动
                    if (player.transform.position.x - 2 >= 0)
                        if (!(boxPos[(int)(chessWidth - player.transform.position.z - 1), (int)(player.transform.position.x - 2 )]))
                        {
                            //可推动
                            RaycastHit hit;
                            if (Physics.Raycast(player.transform.position, Vector3.left, out hit, 1))
                            {
                                boxToMove = hit.collider.gameObject;
                                player.GetComponent<Player>().pFsm.SetState(Player.PlayerState.Walking, msg);
                                boxToMove.GetComponent<Box>().bFsm.SetState(Box.BoxState.Moving, msg);
                                boxPos[(int)(chessWidth - player.transform.position.z - 1), (int)(player.transform.position.x - 1 )] = false;
                                boxPos[(int)(chessWidth - player.transform.position.z - 1), (int)(player.transform.position.x - 2 )] = true;
                            }
                        }
                }
                else
                {
                    //不相邻，直接移动
                    player.GetComponent<Player>().pFsm.SetState(Player.PlayerState.Walking, msg);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            msg = "D";
            if (player.transform.position.x + 1 < chessLength)
            {
                //判断该方向上是否与箱子相邻
                if (boxPos[(int)(chessWidth - 1 - player.transform.position.z), (int)(player.transform.position.x + 1)])
                {
                    //相邻，判断箱子是否能推动
                    if (player.transform.position.x + 2 < chessLength)
                        if (!(boxPos[(int)(chessWidth - player.transform.position.z - 1), (int)(player.transform.position.x + 2)]))
                        {
                            //可推动
                            RaycastHit hit;
                            if (Physics.Raycast(player.transform.position, Vector3.right, out hit, 1))
                            {
                                boxToMove = hit.collider.gameObject;
                                player.GetComponent<Player>().pFsm.SetState(Player.PlayerState.Walking, msg);
                                boxToMove.GetComponent<Box>().bFsm.SetState(Box.BoxState.Moving, msg);
                                boxPos[(int)(chessWidth - player.transform.position.z - 1), (int)(player.transform.position.x + 1)] = false;
                                boxPos[(int)(chessWidth - player.transform.position.z - 1), (int)(player.transform.position.x + 2)] = true;
                            }
                        }
                }
                else
                {
                    //不相邻，直接移动
                    player.GetComponent<Player>().pFsm.SetState(Player.PlayerState.Walking, msg);
                }
            }
        }
    }
   
    //得分统计
    void judgeWin() {
        score = 0;
        foreach (GameObject b in boxGroup)        
            foreach (GameObject d in destGroup)
            {
                if (b.transform.position.x > d.transform.position.x - 0.5
                  && b.transform.position.x < d.transform.position.x + 0.5
                  && b.transform.position.z > d.transform.position.z - 0.5
                  && b.transform.position.z < d.transform.position.z + 0.5)
                    score++;
            }
        //  Debug.Log("score: " + score);
        textUI.GetComponent<Text>().text = "Score: "+score;
        if (score == boxGroup.Length) textUI.GetComponent<Text>().text= "Score: " + score+"\n YOU WIN";
    }
}
