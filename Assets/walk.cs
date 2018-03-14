using UnityEngine;
using System.Collections;

public class walk : MonoBehaviour {

    public GameObject player;
    GameObject[] box;
    public static float speed = 0.4F;
    private Transform pTransform;

    //bool 是否可移动
    bool ifLeft;
    bool ifRight;
    bool ifForward;
    bool ifBack;
    Vector3 L=Vector3.left;
    Vector3 R=Vector3.right;
    Vector3 F=Vector3.forward;
    Vector3 B=Vector3.back;

    private Transform leftCheck;
    private Transform rightCheck;
    private Transform forwardCheck;
    private Transform backCheck;

    // Use this for initialization
    void Start () {
        pTransform = gameObject.GetComponent<Transform>();
        box = GameObject.FindGameObjectsWithTag("box");
	}

// Update is called once per frame
void Update() {
          if (Input.GetKey(KeyCode.W)) { pTransform.Translate(Vector3.forward * 0.1f, Space.Self); }
          if (Input.GetKey(KeyCode.A)) { pTransform.Translate(Vector3.left * 0.1f, Space.Self); }
          if (Input.GetKey(KeyCode.S)) { pTransform.Translate(Vector3.back * 0.1f, Space.Self); }
          if (Input.GetKey(KeyCode.D)) { pTransform.Translate(Vector3.right * 0.1f, Space.Self); }
    }

    public void WalkLeft()
    {
        //判断能否移动
        ifLeft= Physics2D.Linecast(transform.position, leftCheck.position, 1 << LayerMask.NameToLayer("Wall"));
        if (!ifLeft)
        {
            player.transform.Translate(L * speed);
        }
    }
}
