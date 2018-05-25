using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIcontroller : MonoBehaviour {
    GameObject UItext;
    GameObject[] AllBox;
    GameObject[] AllTarget;
    // 箱子数量 目标数量
    int BoxNum;
    int TargetNum;
    // 分数 移动步数
    public int score;
    public int moves;
    // Use this for initialization
    void Start () {
        AllBox = GameObject.FindGameObjectsWithTag("Box");
        AllTarget = GameObject.FindGameObjectsWithTag("Destination");
        UItext = GameObject.Find("Text1");
        BoxNum = AllBox.Length;
        TargetNum = AllTarget.Length;
        moves = 0;
    }
	
	// Update is called once per frame
	void Update () {
        JudegeWin();
	}

    void JudegeWin() {
        score = 0;
        foreach (GameObject b in AllBox)
            foreach (GameObject d in AllTarget)
            {
                if (b.transform.position.x > d.transform.position.x - 0.5
                  && b.transform.position.x < d.transform.position.x + 0.5
                  && b.transform.position.z > d.transform.position.z - 0.5
                  && b.transform.position.z < d.transform.position.z + 0.5)
                    score++;
            }
        UItext.GetComponent<Text>().text = "Moves Count:  " + moves + "\nBox Num:  " + AllBox.Length + "\nScore:  " + score;
        if (score == AllBox.Length)
            UItext.GetComponent<Text>().text = UItext.GetComponent<Text>().text + "\n (￣3￣) YOU WIN";
    }

}
