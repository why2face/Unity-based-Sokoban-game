using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public Transform playerTransform;//玩家坐标  
    public float movespeed = 3;//摄像头速度  

    void Start () {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
	}

    void LateUpdate()
    {
        //摄像机的目标位置 = 目标物体的坐标 + 摄像机的固定坐标  
        Vector3 targetPos = playerTransform.position + new Vector3(0, 6, -4);
        //线性插值 移动到目标位置  
        this.transform.position = Vector3.Lerp(this.transform.position, targetPos, movespeed * Time.deltaTime);
    }
}
