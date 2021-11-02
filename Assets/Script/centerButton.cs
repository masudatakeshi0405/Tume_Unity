using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class centerButton : MonoBehaviour
{
    GameObject centerpos;
    GameObject Ball;

    public void OnClick()
    {
        Debug.Log("押された！"); //
        Ball.GetComponent<RptateTest2>().cenBt = true;
        centerpos.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        centerpos = GameObject.Find("center");
        Ball = GameObject.Find("Ball3");
        centerpos.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
