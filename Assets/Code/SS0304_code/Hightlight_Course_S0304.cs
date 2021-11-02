using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hightlight_Course_S0304 : MonoBehaviour
{

    GameObject pos01;
    GameObject pos02;
    GameObject pos03;
    GameObject pos04;
    GameObject pos05;
    GameObject pos06;
    GameObject Ball;

    // Start is called before the first frame update
    void Start()
    {
        pos01 = GameObject.Find("left");
        pos02 = GameObject.Find("center");
        pos03 = GameObject.Find("right");
        pos04 = GameObject.Find("frontleft");
        pos05 = GameObject.Find("frontcenter");
        pos06 = GameObject.Find("frontright");
        Ball = GameObject.Find("Ball3");
        pos01.SetActive(false);
        pos02.SetActive(false);
        pos03.SetActive(false);
        pos04.SetActive(false);
        pos05.SetActive(false);
        pos06.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch(Ball.GetComponent<BallControl_S0304>().s_course)
        {
            case 1:
                Hightlight_false();
                pos01.SetActive(true);
                break;

            case 2:
                Hightlight_false();
                pos02.SetActive(true);
                break;

            case 3:
                Hightlight_false();
                pos03.SetActive(true);
                break;
            
            case 4:
                Hightlight_false();
                pos04.SetActive(true);
                break;
            
            case 5:
                Hightlight_false();
                pos05.SetActive(true);
                break;

            case 6:
                Hightlight_false();
                pos06.SetActive(true);
                break;
        }
    }

    void Hightlight_false()
    {
        pos01.SetActive(false);
        pos02.SetActive(false);
        pos03.SetActive(false);
        pos04.SetActive(false);
        pos05.SetActive(false);
        pos06.SetActive(false);
    }
}
