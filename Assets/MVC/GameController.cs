using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
    public GameObject stage;
    GameObject player;
    GameObject[] boxGroup;
    GameObject[] destGroup;
    int score;
	// Use this for initialization
	void Start () {
        //设置关卡
        stage.GetComponent<GameModel>().setStage();
        //获取玩家、箱子、目标点对象
        player = GameObject.FindGameObjectWithTag("Player");
        boxGroup = GameObject.FindGameObjectsWithTag("box");
        destGroup = GameObject.FindGameObjectsWithTag("Destination");
	}
	
	// Update is called once per frame
	void Update () {
        getInput();	
	}
    //输入控制
    void getInput()
    {
        string msg=null;
        if (Input.GetKeyDown(KeyCode.W)) {
            msg = "W";
            player.GetComponent<Player>().pFsm.SetState(Player.PlayerState.Walking, msg);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            msg = "S";
            player.GetComponent<Player>().pFsm.SetState(Player.PlayerState.Walking, msg);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            msg = "A";
            player.GetComponent<Player>().pFsm.SetState(Player.PlayerState.Walking, msg);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            msg = "D";
            player.GetComponent<Player>().pFsm.SetState(Player.PlayerState.Walking, msg);
        }
        //玩家状态机update()
        player.GetComponent<Player>().pFsm.UpdateState(Time.deltaTime);

    }
   
    //得分统计
    void judgeWin() {
        score = 0;
        foreach (GameObject b in boxGroup)
        {
            foreach (GameObject d in destGroup)
            {
                if (b.transform.position.x > d.transform.position.x - 0.5
                  && b.transform.position.x < d.transform.position.x + 0.5
                  && b.transform.position.z > d.transform.position.z - 0.5
                  && b.transform.position.z < d.transform.position.z + 0.5)
                    score++;
            }
        }
        //  Debug.Log("score: " + score);
        if (score == boxGroup.Length) Debug.Log("you win");
    }
}
