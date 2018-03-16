using UnityEngine;
using System.Collections;
using System;

public enum PlayerState { Idle, Walking, Pushing };

public class PlayerMove : MonoBehaviour {

    private Vector3 endpos;
    private Transform t;

    private CharacterController cc;
    public float speed = 4;

    public int score;
    public int boxNum;
    public int destNum;
    GameObject[] box;
    GameObject[] destination;

    public enum PlayerState { Idle, Walking, Pushing };
    LittleFsm<PlayerState> pFsm = new LittleFsm<PlayerState>();

    // Use this for initialization
    void Start () {
        t = GetComponent<Transform>();
        cc = GetComponent<CharacterController>();
        box = GameObject.FindGameObjectsWithTag("box");
        destination = GameObject.FindGameObjectsWithTag("Finish");
        boxNum = box.Length;
        destNum = destination.Length;
        score = 0;
        //判断
        InvokeRepeating("judgeWin", 2, 2);

        pFsm.RegistState(PlayerState.Idle, OnIdleEnter, OnIdleUpdate, OnIdleExit);
        pFsm.RegistState(PlayerState.Walking, OnWalkingEnter, OnWalkingUpdate, OnWalkingExit);
        pFsm.RegistState(PlayerState.Pushing, OnPushingEnter, OnPushingUpdate, OnPushingExit);
        //默认状态
        pFsm.SetState(PlayerState.Idle);
    }



    // Update is called once per frame
    void Update () {

        pFsm.UpdateState(Time.deltaTime);
        if (cc.collisionFlags != 0) {
            pFsm.SetState(PlayerState.Pushing);
        }
        else if (Input.GetKeyDown(KeyCode.W) ||
                 Input.GetKeyDown(KeyCode.S) ||
                 Input.GetKeyDown(KeyCode.A) ||
                 Input.GetKeyDown(KeyCode.D) )
            {
              pFsm.SetState(PlayerState.Walking);     
            }

    }

    //控制碰撞
    public float pushPower = 2.0F;
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("Colliding !");
    }

    //得分判断
    public void judgeWin() {
        score = 0;
        foreach (GameObject b in box) {
            foreach (GameObject d in destination) {
                if (b.transform.position.x > d.transform.position.x - 0.5
                  && b.transform.position.x < d.transform.position.x + 0.5
                  && b.transform.position.z > d.transform.position.z - 0.5
                  && b.transform.position.z < d.transform.position.z + 0.5)
                      score++;             
            }          
        }
        Debug.Log("score: " + score);
        if (score == boxNum) Debug.Log("you win");
    }
    
    void OnIdleEnter(object param) { Debug.Log("Player enter Idle"); }
    void OnIdleUpdate(float delta) {  }
    void OnIdleExit() { Debug.Log("Player exit Idle "); }

    void OnWalkingEnter(object param) {
        Debug.Log("Player enter Walking");
        if (Input.GetKeyDown(KeyCode.W))
        {
                endpos = new Vector3(t.transform.position.x, t.transform.position.y, t.transform.position.z + 1);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
                endpos = new Vector3(t.transform.position.x, t.transform.position.y, t.transform.position.z - 1);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
                endpos = new Vector3(t.transform.position.x + 1, t.transform.position.y, t.transform.position.z);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
                endpos = new Vector3(t.transform.position.x - 1, t.transform.position.y, t.transform.position.z);
        }
    }
    void OnWalkingUpdate(float delta) {
       Debug.Log("Player update walking");
        if (pFsm.m_StateTimer < 1)
            t.transform.position = Vector3.Lerp(t.transform.position, endpos, pFsm.m_StateTimer);
        else pFsm.SetState(PlayerState.Idle);
    }
    void OnWalkingExit() { Debug.Log("Player Exit Walking"); }

    void OnPushingEnter(object param) { Debug.Log("Player Enter Pushing"); }
    void OnPushingUpdate(float delta) { }
    void OnPushingExit() { }


}
