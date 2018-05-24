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
        GetInput();
	}

    void GetInput() {
        if (Input.GetKeyDown(KeyCode.W))
        {
            RaycastHit hit;
            if (Physics.Raycast(player.transform.position, Vector3.forward, out hit, 1))
            {
                Debug.Log(hit.collider.gameObject.name);
                player.GetComponent<Player>().pFsm.SetState(Player.PlayerState.Walking, "W");
            }
            else
            {
                player.GetComponent<Player>().pFsm.SetState(Player.PlayerState.Walking, "W");
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            RaycastHit hit;
            if (Physics.Raycast(player.transform.position, Vector3.back, out hit, 1))
            {
                Debug.Log(hit.collider.gameObject.name);
                player.GetComponent<Player>().pFsm.SetState(Player.PlayerState.Walking, "S");
            }
            else
            {
                player.GetComponent<Player>().pFsm.SetState(Player.PlayerState.Walking, "S");
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            RaycastHit hit;
            if (Physics.Raycast(player.transform.position, Vector3.left, out hit, 1))
            {
                Debug.Log(hit.collider.gameObject.name);
                player.GetComponent<Player>().pFsm.SetState(Player.PlayerState.Walking, "A");
            }
            else
            {
                player.GetComponent<Player>().pFsm.SetState(Player.PlayerState.Walking, "A");
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            RaycastHit hit;
            if (Physics.Raycast(player.transform.position, Vector3.right, out hit, 1))
            {
                Debug.Log(hit.collider.gameObject.name);
                player.GetComponent<Player>().pFsm.SetState(Player.PlayerState.Walking, "D");
            }
            else
            {
                player.GetComponent<Player>().pFsm.SetState(Player.PlayerState.Walking, "D");
            }
        }
    }
}
