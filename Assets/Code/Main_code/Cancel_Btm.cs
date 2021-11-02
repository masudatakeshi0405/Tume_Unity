using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cancel_Btm : MonoBehaviour
{

    GameObject Ball;
    
    public void OnClick()
    {
        Debug.Log("ナックルサービス");
        Ball.GetComponent<BallControl_BMC>().s_style = 0;
        Ball.GetComponent<BallControl_BMC>().s_course = 0;
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
