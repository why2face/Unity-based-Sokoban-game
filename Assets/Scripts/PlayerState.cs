using UnityEngine;
using System.Collections;
using System;

public enum PlayerStateType
{
    Idle,
    Moving
}
public class PlayerState : MonoBehaviour {
    public PlayerStateType nowState = PlayerStateType.Idle;
    //移动方向
    Vector3 startPosition;
    Vector3 endPosition;
    //移动时间
    private float moveTime;
    //移动速度
    int moveSpeed = 3;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (nowState == PlayerStateType.Moving)
            StartCoroutine(moveAnimation());
	}

    /// <summary>
    /// 开始移动 direction= W/A/S/D
    /// </summary>
    /// <param name="direction"></param>
    public void move(string direction)
    {
        nowState = PlayerStateType.Moving;
        Transform t = this.transform;
        switch (direction)
        {
            case "W":
                endPosition = new Vector3(t.position.x, t.position.y, t.position.z + 1);
                break;
            case "S":
                endPosition = new Vector3(t.position.x, t.position.y, t.position.z - 1);
                break;
            case "A":
                endPosition = new Vector3(t.position.x - 1, t.position.y, t.position.z);
                break;
            case "D":
                endPosition = new Vector3(t.position.x + 1, t.position.y, t.position.z);
                break;
        }
    }

    /// <summary>
    /// 移动动画 协程
    /// </summary>
    /// <returns></returns>
    public IEnumerator moveAnimation() {
        Transform t = this.transform;
        startPosition = t.position;
        moveTime = 0;
        while (moveTime < 1f){
            moveTime += Time.deltaTime * moveSpeed;
            t.position = Vector3.Lerp(startPosition, endPosition, moveTime);
            yield return null;
        }
        //移动结束 状态变为idle
        nowState = PlayerStateType.Idle;
        setInt();
        yield return 0;
    }

    /// <summary>
    /// 重置坐标为整数值
    /// </summary>
    void setInt() {
        int _x = (int)Math.Round(this.transform.position.x, MidpointRounding.AwayFromZero);
        int _z = (int)Math.Round(this.transform.position.z, MidpointRounding.AwayFromZero);
        transform.position = new Vector3(_x, this.transform.position.y, _z); 
    }
}
