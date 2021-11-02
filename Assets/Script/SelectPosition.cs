using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPosition : MonoBehaviour
{
    GameObject Ball2;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        Ball2 = GameObject.Find("Ball3");
    }

    void Update()
    {
        // gameObject.SetActive(Ball2.GetComponent<RptateTest2>().rate);
        // if(Ball2.GetComponent<RptateTest2>().rate == true){
        //     gameObject.SetActive(true);
        // }
    }
}
