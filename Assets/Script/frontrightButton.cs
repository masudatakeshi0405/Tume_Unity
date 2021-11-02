using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frontrightButton : MonoBehaviour
{
    GameObject frontrightpos;
    GameObject Ball;

    public void OnClick()
    {
        Debug.Log("押された！"); //
        frontrightpos.SetActive(true);
        Ball.GetComponent<RptateTest2>().frigBt = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        frontrightpos = GameObject.Find("frontright");
        frontrightpos.SetActive(false);
        Ball = GameObject.Find("Ball3");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
