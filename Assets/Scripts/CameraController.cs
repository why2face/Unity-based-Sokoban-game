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
        //目标物体要到达的目标位置 = 当前物体的位置 + 当前摄像机的位置  
        Vector3 targetPos = playerTransform.position + new Vector3(0, 6, -4);
        //使用线性插值计算让摄像机用smoothing * Time.deltaTime时间从当前位置到移动到目标位置  
        this.transform.position = Vector3.Lerp(this.transform.position, targetPos, movespeed * Time.deltaTime);
    }
}
