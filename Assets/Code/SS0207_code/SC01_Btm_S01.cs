using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC01_Btm_S01 : MonoBehaviour
{

    GameObject Ball;

    public void OnClick()
    {
        Debug.Log("ナックルサービス");
        Ball.GetComponent<BallControl_BMC_S01>().s_course = 1;
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
