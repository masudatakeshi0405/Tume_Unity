using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frontleftButton2 : MonoBehaviour
{
    GameObject coatpos;
    GameObject Ball;

    public void OnClick()
    {
        Debug.Log("押された！");
        coatpos.SetActive(true);
        Ball.GetComponent<BallControl>().flefBt = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        coatpos = GameObject.Find("frontleft");
        Ball = GameObject.Find("Ball3");
        coatpos.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
