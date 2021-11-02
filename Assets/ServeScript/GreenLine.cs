using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenLine : MonoBehaviour
{

GameObject Ball;

    // Start is called before the first frame update
    void Start()
    {
        Ball = GameObject.Find("Ball3");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Ball.transform.position;
    }
}
