using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repeat_Btm : MonoBehaviour
{
    GameObject Ball;

    public void OnClick()
    {
        Debug.Log("ナックルサービス");
        Ball.GetComponent<BallControl_BMC>().reset = true;
        Ball.GetComponent<BallControl_BMC>().repeat = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Ball = GameObject.Find("Ball3");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
