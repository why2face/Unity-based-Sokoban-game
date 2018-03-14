using UnityEngine;
using System.Collections;

public class BoxMove : MonoBehaviour {
    private Transform boxTrans;
	// Use this for initialization
	void Start () {
        boxTrans = gameObject.GetComponent<Transform>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
   // void OnCollisionEnter(Collision c) {
  //      if(c.gameObject.tag == "Player"){
  //      Vector3 dir = c.contacts[0].point - transform.position;
  //      dir = -dir.normalized;
  //      boxTrans.Translate(dir * 0.1f, Space.Self);
  //      }       
  //  }
}
