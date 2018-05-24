using UnityEngine;
using System.Collections;
using System;

public class Box : MonoBehaviour {

    //箱子状态机
    public enum BoxState { Idle, Moving };
    public LittleFsm<BoxState> bFsm = new LittleFsm<BoxState>();
    //移动目的坐标
    private Vector3 endpos;

    // Use this for initialization
    void Start () {
        bFsm.RegistState(BoxState.Idle, OnIdleEnter, null, OnIdleExit);
        bFsm.RegistState(BoxState.Moving, OnMovingEnter, OnMovingUpdate, OnMovingExit);
        bFsm.SetState(BoxState.Idle);	
	}
	
	// Update is called once per frame
	void Update () {
        bFsm.UpdateState(3 * Time.deltaTime);
    }
    void OnIdleEnter(object param) { Debug.Log("Box enter Idle"); }
    void OnIdleExit() { }

    void OnMovingEnter(object param)
    {
        Debug.Log("Box enter Moving");
        switch (param.ToString())
        {
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
    void OnMovingUpdate(float delta)
    {
        //移动时间设置，时间增量为delta
        if (bFsm.m_StateTimer < 0.8f)
            this.transform.position = Vector3.Lerp(this.transform.position, endpos, bFsm.m_StateTimer);
        else bFsm.SetState(BoxState.Idle);
    }
    void OnMovingExit() {
        int X= (int)Math.Round(this.transform.position.x, MidpointRounding.AwayFromZero);
        int Z= (int)Math.Round(this.transform.position.z, MidpointRounding.AwayFromZero);
        //调整坐标为整数
        transform.position = new Vector3(X, this.transform.position.y, Z);
    }
}
