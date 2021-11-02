using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frontleftButton : MonoBehaviour
{
    GameObject frontleftpos;
    GameObject Ball;

    public void OnClick()
    {
        Debug.Log("押された！"); //
        frontleftpos.SetActive(true);
        Ball.GetComponent<RptateTest2>().flefBt = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        frontleftpos = GameObject.Find("frontleft");
        frontleftpos.SetActive(false);
        Ball = GameObject.Find("Ball3");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
