using UnityEngine;
using System.Collections;

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
	
	}
    void OnIdleEnter(object param) { Debug.Log("Player enter Idle"); }
    void OnIdleExit() { Debug.Log("Player exit Idle "); }

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
        //移动时间设置为1，时间增量为delta
        if (pFsm.m_StateTimer < 1)
            this.transform.position = Vector3.Lerp(this.transform.position, endpos, pFsm.m_StateTimer);
        else pFsm.SetState(PlayerState.Idle);
    }
    void OnWalkingExit() { Debug.Log("Player Exit Walking"); }
}
