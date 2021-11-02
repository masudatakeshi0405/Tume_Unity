using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//この宣言が必要

public class ShotResult : MonoBehaviour
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
            myText.text = "成功";
        }else{
            myText.text = "失敗";
        }
    }
}
