using System;
using UnityEngine;

public class Player : MonoBehaviour {
    
    //玩家状态机
    public enum PlayerState { Idle, Walking };
    public LittleFsm<PlayerState> pFsm = new LittleFsm<PlayerState>();
    //移动目的坐标
    private Vector3 endpos;

    // Use this for initialization
    void Start () {
        pFsm.RegistState(PlayerState.Idle, OnIdleEnter, null, OnIdleExit);
        pFsm.RegistState(PlayerState.Walking, OnWalkingEnter, OnWalkingUpdate, OnWalkingExit);
        //默认状态:空闲
        pFsm.SetState(PlayerState.Idle);
    }
	
	// Update is called once per frame
	void Update () {
        pFsm.UpdateState(3 * Time.deltaTime);
    }
    void OnIdleEnter(object param) { Debug.Log("Player enter Idle"); }
    void OnIdleExit() { }

    void OnWalkingEnter(object param)
    {
        Debug.Log("Player enter Walking");
        switch (param.ToString()) {
            case "W":
                endpos = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z + 1);
                break;
            case "S":
                endpos = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 1);
                break;
            case "A":
                endpos = new Vector3(this.transform.position.x - 1, this.transform.position.y, this.transform.position.z);
                break;
            case "D":
                endpos = new Vector3(this.transform.position.x + 1, this.transform.position.y, this.transform.position.z);
                break;
            default:
                break;
        }
    }
    void OnWalkingUpdate(float delta)
    {
        //移动时间设置，时间增量为delta
        if (pFsm.m_StateTimer < 0.8f) 
             this.transform.position = Vector3.Lerp(this.transform.position, endpos, pFsm.m_StateTimer);
        else pFsm.SetState(PlayerState.Idle);
    }
    void OnWalkingExit() {
       //调整坐标为整数
       this.GetComponentInParent<GameModel>().playerX = (int)Math.Round(this.transform.position.x, MidpointRounding.AwayFromZero);
       this.GetComponentInParent<GameModel>().playerZ = (int)Math.Round(this.transform.position.z, MidpointRounding.AwayFromZero);
       transform.position = new Vector3(this.GetComponentInParent<GameModel>().playerX,
       this.transform.position.y,this.GetComponentInParent<GameModel>().playerZ);
    }
}
