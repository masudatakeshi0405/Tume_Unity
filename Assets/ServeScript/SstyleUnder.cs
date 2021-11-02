using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SstyleUnder : MonoBehaviour
{

    GameObject Ball;

    public void OnClick()
    {
        Debug.Log("押された！");
        Ball.GetComponent<BallControl>().s_style = 2;
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
