using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC04_Btm_S01 : MonoBehaviour
{

    GameObject Ball;

    public void OnClick()
    {
        Debug.Log("ナックルサービス");
        Ball.GetComponent<BallControl_BMC_S01>().s_course = 4;
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
