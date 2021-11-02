using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    GameObject Ball;
    GameObject Ball2;

    public void OnClick()
    {
        Debug.Log("押された！"); //
        //Ball.GetComponent<RotateTest>().isMove = false; //BallのScriptのRotateTestの変数のisMoveをfalseにする
        Ball2.GetComponent<RptateTest2>().isMove = false;
        Ball2.GetComponent<RptateTest2>().ballshot = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Ball = GameObject.Find("Ball"); //BallはGameObjectのBallのことです
        Ball2 = GameObject.Find("Ball3");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
