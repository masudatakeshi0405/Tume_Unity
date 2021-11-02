using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hightlight_Style_First : MonoBehaviour
{

    GameObject ss01;
    GameObject ss02;
    GameObject ss03;
    GameObject ss04;
    GameObject ss05;
    GameObject ss06;
    GameObject ss07;
    GameObject ss01_c;
    GameObject ss02_c;
    GameObject ss03_c;
    GameObject ss04_c;
    GameObject ss05_c;
    GameObject ss06_c;
    GameObject ss07_c;
    GameObject Ball;

    // Start is called before the first frame update
    void Start()
    {
        ss01_c = GameObject.Find("ss01_arrow_c");
        ss02_c = GameObject.Find("ss02_arrow_c");
        ss03_c = GameObject.Find("ss03_arrow_c");
        ss04_c = GameObject.Find("ss04_arrow_c");
        ss05_c = GameObject.Find("ss05_arrow_c");
        ss06_c = GameObject.Find("ss06_arrow_c");
        ss07_c = GameObject.Find("ss07_arrow_c");
        ss01 = GameObject.Find("ss01_box");
        ss02 = GameObject.Find("ss02_box");
        ss03 = GameObject.Find("ss03_box");
        ss04 = GameObject.Find("ss04_box");
        ss05 = GameObject.Find("ss05_box");
        ss06 = GameObject.Find("ss06_box");
        ss07 = GameObject.Find("ss07_box");
        Ball = GameObject.Find("Ball3");
        ss01_c.SetActive(false);
        ss02_c.SetActive(false);
        ss03_c.SetActive(false);
        ss04_c.SetActive(false);
        ss05_c.SetActive(false);
        ss06_c.SetActive(false);
        ss07_c.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // if(Ball.GetComponent<BallControl_First>().s_style==0){
        //     switch(Ball.GetComponent<BallControl_First>().issue_num)
        //     {
        //         case 1:
        //             Hightlight_false_box();
        //             ss01.SetActive(true);
        //             ss02.SetActive(true);
        //             ss07.SetActive(true);
        //             break;

        //         case 2:
        //             Hightlight_false_box();
        //             ss01.SetActive(true);
        //             ss02.SetActive(true);
        //             ss03.SetActive(true);
        //             ss04.SetActive(true);
        //             break;

        //         case 3:
        //             Hightlight_false_box();
        //             ss02.SetActive(true);
        //             ss03.SetActive(true);
        //             ss04.SetActive(true);
        //             ss07.SetActive(true);
        //             break;

        //         case 4:
        //             Hightlight_false_box();
        //             ss01.SetActive(true);
        //             ss02.SetActive(true);
        //             ss07.SetActive(true);
        //             break;
                
        //         case 5:
        //             Hightlight_false_box();
        //             ss01.SetActive(true);
        //             ss02.SetActive(true);
        //             ss07.SetActive(true);
        //             break;
        //     }
        // }
        

        switch(Ball.GetComponent<BallControl_First>().s_style)
        {
            case 1:
                Hightlight_false();
                ss01_c.SetActive(true);
                break;

            case 2:
                Hightlight_false();
                ss02_c.SetActive(true);
                break;

            case 3:
                Hightlight_false();
                ss03_c.SetActive(true);
                break;
            
            case 4:
                Hightlight_false();
                ss04_c.SetActive(true);
                break;
            
            case 5:
                Hightlight_false();
                ss05_c.SetActive(true);
                break;

            case 6:
                Hightlight_false();
                ss06_c.SetActive(true);
                break;

            case 7:
                Hightlight_false();
                ss07_c.SetActive(true);
                break;
        }
    }

    void Hightlight_false()
    {
        ss01_c.SetActive(false);
        ss02_c.SetActive(false);
        ss03_c.SetActive(false);
        ss04_c.SetActive(false);
        ss05_c.SetActive(false);
        ss06_c.SetActive(false);
        ss07_c.SetActive(false);
    }

    void Hightlight_false_box()
    {
        ss01.SetActive(false);
        ss02.SetActive(false);
        ss03.SetActive(false);
        ss04.SetActive(false);
        ss05.SetActive(false);
        ss06.SetActive(false);
        ss07.SetActive(false);
    }
}
