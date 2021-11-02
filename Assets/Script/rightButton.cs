using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rightButton : MonoBehaviour
{
    GameObject rightpos;
    GameObject Ball;
    

    public void OnClick()
    {
        Debug.Log("押された！"); //
        Ball.GetComponent<RptateTest2>().rigBt = true;
        rightpos.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        rightpos = GameObject.Find("right");
        Ball = GameObject.Find("Ball3");
        rightpos.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
