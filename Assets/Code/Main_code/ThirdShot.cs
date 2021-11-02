using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//この宣言が必要

public class ThirdShot : MonoBehaviour
{
    Text myText;
    
    GameObject Ball;

    string[] ts_name = new string[] {"","ドライブ","ツッツキ","ストップ","フリック","チキータ","ネット"};
    string[] tc_name = new string[] {"","フォア","ミドル","バック","前フォア","前ミドル","前バック"};

    // Start is called before the first frame update
    void Start()
    {
        myText = GetComponentInChildren<Text>();//UIのテキストの取得の仕方
        Ball = GameObject.Find("Ball3");
    }

    // Update is called once per frame
    void Update()
    {
        myText.text = string.Format("{0} : {1}", ts_name[Ball.GetComponent<BallControl_BMC>().t_style], tc_name[Ball.GetComponent<BallControl_BMC>().t_course]);
    }
}
