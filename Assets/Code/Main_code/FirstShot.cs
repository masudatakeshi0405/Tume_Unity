using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//この宣言が必要

public class FirstShot : MonoBehaviour
{

    Text myText;
    
    GameObject Ball;

    string[] ss_name = new string[] {"","上回転","下回転","順横下回転","逆横下回転","順横回転","逆横回転","ナックル"};
    string[] sc_name = new string[] {"","フォア","ミドル","バック","前フォア","前ミドル","前バック"};

    // Start is called before the first frame update
    void Start()
    {
        myText = GetComponentInChildren<Text>();//UIのテキストの取得の仕方
        Ball = GameObject.Find("Ball3");
    }

    // Update is called once per frame
    void Update()
    {
        myText.text = string.Format("{0} : {1}", ss_name[Ball.GetComponent<BallControl_BMC>().s_style], sc_name[Ball.GetComponent<BallControl_BMC>().s_course]);
    }
}
