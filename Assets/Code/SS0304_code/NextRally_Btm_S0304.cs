using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextRally_Btm_S0304 : MonoBehaviour
{

    GameObject Ball;

    public void OnClick()
    {
        Ball.GetComponent<BallControl_S0304>().turncou++;
        Debug.Log(Ball.GetComponent<BallControl_S0304>().turncou);
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
