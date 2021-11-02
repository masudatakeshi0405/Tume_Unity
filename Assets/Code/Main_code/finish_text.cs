using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//この宣言が必要

public class finish_text : MonoBehaviour
{

    Text myText;

    GameObject Ball;


    // Start is called before the first frame update
    void Start()
    {
        myText = GetComponentInChildren<Text>();//UIのテキストの取得の仕方
        Ball = GameObject.Find("Ball3");
    }

    // Update is called once per frame
    void Update()
    {
        myText.text = ("3問中" + Ball.GetComponent<BallControl_BMC>().issue_ok_cou + "問正解！");//テキストの変更
    }
}
