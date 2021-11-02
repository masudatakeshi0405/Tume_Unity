using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript6 : MonoBehaviour
{
    GameObject Ball;

    public void OnClick()
    {
        Debug.Log("押された！"); //
        Ball.GetComponent<ObjectScript>().isMove6 = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        Ball = GameObject.Find("GameObject");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
