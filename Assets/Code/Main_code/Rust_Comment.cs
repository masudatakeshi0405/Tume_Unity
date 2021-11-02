using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//この宣言が必要

public class Rust_Comment : MonoBehaviour
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
        if(Ball.GetComponent<BallControl_BMC>().judge==true){
            myText.text = "ナイスサービス！！";
        }else{
            myText.text = "選んだサービスは失敗です";
        }
    }
}
