using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class leftButton : MonoBehaviour
{
    GameObject leftpos;
    GameObject Ball;

    public void OnClick()
    {
        Debug.Log("押された！"); //
        leftpos.SetActive(true);
        Ball.GetComponent<RptateTest2>().lefBt = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        leftpos = GameObject.Find("left");
        Ball = GameObject.Find("Ball3");
        leftpos.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
