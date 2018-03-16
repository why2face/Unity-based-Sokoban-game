using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {
    private bool isMoving = false;
    private Vector3 endpos;
    private float process = 0;
    private Transform t;
    private CharacterController cc;
	// Use this for initialization
	void Start () {
        t = GetComponent<Transform>();
        cc = GetComponent<CharacterController>();
	}

    // Update is called once per frame

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!isMoving)
            {   endpos = new Vector3(t.transform.position.x, t.transform.position.y, t.transform.position.z+1);
                process = 0;  isMoving = true;
            }
        }else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!isMoving)
            {
                endpos = new Vector3(t.transform.position.x , t.transform.position.y, t.transform.position.z-1);
                process = 0;  isMoving = true;
            }
        }else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (!isMoving)
            {
                endpos = new Vector3(t.transform.position.x+1, t.transform.position.y, t.transform.position.z);
                process = 0;  isMoving = true;
            }
        }else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (!isMoving)
            {
                endpos = new Vector3(t.transform.position.x-1, t.transform.position.y, t.transform.position.z);
                process = 0;  isMoving = true;
            }
        }
        if (isMoving)
        {
           process += Time.deltaTime ;
            if (process < 1)
               t.transform.position = Vector3.Lerp(t.transform.position, endpos, process);
            else
                isMoving = false;      
        }
    }
}
