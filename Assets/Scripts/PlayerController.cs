using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    GameObject player;
    GameObject boxToMove;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
    }
	
	// Update is called once per frame
	void Update () {
        if (player.GetComponent<PlayerState>().nowState == PlayerStateType.Idle)
            GetInput();
    }

    void GetInput() {
        if (Input.GetKeyDown(KeyCode.W))
        {
            RaycastHit hit;
            if (Physics.Raycast(player.transform.position, Vector3.forward, out hit, 1))
            {
                if (hit.collider.gameObject.tag == "Wall")
                {
                    //撞墙 返回
                    Debug.Log("colliding Wall");
                    return;
                }                    
                else if (hit.collider.gameObject.tag == "Box") {
                    Debug.Log("colliding Box");
                    boxToMove = hit.collider.gameObject;
                    RaycastHit[] temp = Physics.RaycastAll(player.transform.position, Vector3.forward, 2);
                    //撞箱子 判断是否可推
                    if (temp.Length == 1)
                    {
                        player.GetComponent<PlayerState>().move("W");
                        boxToMove.GetComponent<BoxState>().move("W");
                        GameObject.Find("UI").GetComponent<UIcontroller>().moves++;
                    }
                    else return;            
                }          
            }
            else
            {
                //无碰撞 
                player.GetComponent<PlayerState>().move("W");
                GameObject.Find("UI").GetComponent<UIcontroller>().moves++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            RaycastHit hit;
            if (Physics.Raycast(player.transform.position, Vector3.back, out hit, 1))
            {
                if (hit.collider.gameObject.tag == "Wall")
                {
                    Debug.Log("colliding Wall");
                    return;
                }
                else if (hit.collider.gameObject.tag == "Box")
                {
                    Debug.Log("colliding Box");
                    boxToMove = hit.collider.gameObject;
                    RaycastHit[] temp = Physics.RaycastAll(player.transform.position, Vector3.back, 2);
                    if (temp.Length == 1)
                    {
                        player.GetComponent<PlayerState>().move("S");
                        boxToMove.GetComponent<BoxState>().move("S");
                        GameObject.Find("UI").GetComponent<UIcontroller>().moves++;
                    }
                    else return;
                }
            }
            else
            {
                player.GetComponent<PlayerState>().move("S");
                GameObject.Find("UI").GetComponent<UIcontroller>().moves++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            RaycastHit hit;
            if (Physics.Raycast(player.transform.position, Vector3.left, out hit, 1))
            {
                if (hit.collider.gameObject.tag == "Wall")
                {
                    Debug.Log("colliding Wall");
                    return;
                }
                else if (hit.collider.gameObject.tag == "Box")
                {
                    Debug.Log("colliding Box");
                    boxToMove = hit.collider.gameObject;
                    RaycastHit[] temp = Physics.RaycastAll(player.transform.position, Vector3.left, 2);
                    if (temp.Length == 1)
                    {
                        player.GetComponent<PlayerState>().move("A");
                        boxToMove.GetComponent<BoxState>().move("A");
                        GameObject.Find("UI").GetComponent<UIcontroller>().moves++;
                    }
                    else return;
                }
            }
            else
            {
                player.GetComponent<PlayerState>().move("A");
                GameObject.Find("UI").GetComponent<UIcontroller>().moves++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            RaycastHit hit;
            if (Physics.Raycast(player.transform.position, Vector3.right, out hit, 1))
            {
                if (hit.collider.gameObject.tag == "Wall")
                {
                    Debug.Log("colliding Wall");
                    return;
                }
                else if (hit.collider.gameObject.tag == "Box")
                {
                    Debug.Log("colliding Box");
                    boxToMove = hit.collider.gameObject;
                    RaycastHit[] temp = Physics.RaycastAll(player.transform.position, Vector3.right, 2);
                    if (temp.Length == 1)
                    {
                        player.GetComponent<PlayerState>().move("D");
                        boxToMove.GetComponent<BoxState>().move("D");
                        GameObject.Find("UI").GetComponent<UIcontroller>().moves++;
                    }
                    else return;
                }
            }
            else
            {
                player.GetComponent<PlayerState>().move("D");
                GameObject.Find("UI").GetComponent<UIcontroller>().moves++;
            }
        }
    }

}
