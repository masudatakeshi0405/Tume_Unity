using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class BallShot : MonoBehaviour
{
    public float thrust;
    public Rigidbody rb;
    public bool ballshot = false;
    
    void Start () {
        //Rigidbodyを取得する
        rb = GetComponent<Rigidbody>();
    }
	
	void Update () {
        // transformを取得
        Transform myTransform = this.transform;
        if(ballshot == true){
            rb.isKinematic = false;
            rb.useGravity = true;
            transform.rotation = Quaternion.Euler(-15, 0, 0);
            rb.AddForce(transform.forward * thrust); //前方に向けて発射
            ballshot = false;
        }
        // if(transform)
    }
}