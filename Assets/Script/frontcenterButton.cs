using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frontcenterButton : MonoBehaviour
{
    GameObject frontcenterpos;
    GameObject Ball;

    public void OnClick()
    {
        Debug.Log("押された！"); //
        frontcenterpos.SetActive(true);
        Ball.GetComponent<RptateTest2>().fcenBt = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        frontcenterpos = GameObject.Find("frontcenter");
        Ball = GameObject.Find("Ball3");
        frontcenterpos.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
