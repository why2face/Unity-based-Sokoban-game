using UnityEngine;
using System.Collections;

public enum PlayerState { Idle, Walking, Pushing };

public class PlayerMove : MonoBehaviour {

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
        cc = GetComponent<CharacterController>();
        box = GameObject.FindGameObjectsWithTag("box");
        destination = GameObject.FindGameObjectsWithTag("Finish");
        boxNum = box.Length;
        destNum = destination.Length;
        score = 0;
        //判断
        if (score < destNum)
            InvokeRepeating("judgeWin",2,2);
        //移动
      //  this.StartCoroutine(moveStep());

        pFsm.RegistState(PlayerState.Idle, OnIdleEnter, OnIdleUpdate, OnIdleExit);
        pFsm.RegistState(PlayerState.Walking, OnWalkingEnter, OnWalkingUpdate, OnWalkingExit);
        pFsm.RegistState(PlayerState.Pushing, OnPushingEnter, OnPushingUpdate, OnPushingExit);
        //默认状态
        pFsm.SetState(PlayerState.Idle);
    }



    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.W)||
            Input.GetKeyDown(KeyCode.S)||
            Input.GetKeyDown(KeyCode.A)||
            Input.GetKeyDown(KeyCode.D)) {
            pFsm.SetState(PlayerState.Walking);
        }
        if (Input.GetKeyUp(KeyCode.W) ||
            Input.GetKeyUp(KeyCode.S) ||
            Input.GetKeyUp(KeyCode.A) ||
            Input.GetKeyUp(KeyCode.D))
        {
            pFsm.SetState(PlayerState.Idle);
        }
        if (cc.collisionFlags != 0) {
            pFsm.SetState(PlayerState.Pushing);
        }
    }

    IEnumerator moveStep() {
        while (true)
        {
        //step1     
        
        //获取位移方向
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1)
        {
            Vector3 targetDir = new Vector3(h, 0, v);
            //让目标旋转到获取到的方向
            transform.LookAt(targetDir + transform.position);
            //移动
            cc.SimpleMove(targetDir * speed);

        }
        yield return null;
        }
        
    }

    //控制碰撞
    public float pushPower = 2.0F;
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
           return;
        if (hit.moveDirection.y < -0.3F)
            return;
      //push
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
         body.velocity = pushDir * pushPower;
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
        if (score == destNum) Debug.Log("you win");
    }
    
    void OnIdleEnter(object param) { Debug.Log("Player enter Idle"); }
    void OnIdleUpdate(float delta) { }
    void OnIdleExit() { Debug.Log("Player exit Idle "); }

    void OnWalkingEnter(object param) {
        Debug.Log("Player enter Walking");
        //获取位移方向
         Vector3 targetDir = new Vector3(0, 0, 1);
        //让目标旋转到获取到的方向
         transform.LookAt(targetDir + transform.position);
        //移动
         cc.SimpleMove(targetDir * speed);
        
    }
    void OnWalkingUpdate(float delta) { }
    void OnWalkingExit() { }

    void OnPushingEnter(object param) { }
    void OnPushingUpdate(float delta) { }
    void OnPushingExit() { }


}
