using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decision_Btm_First : MonoBehaviour
{

    GameObject Ball;

    public void OnClick()
    {
        Ball.GetComponent<BallControl_First>().turncou++;
        Debug.Log(Ball.GetComponent<BallControl_First>().turncou);
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
