using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneHit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //当たった瞬間に呼ばれる関数
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name); // ログを表示する
        // if(collision.gameObject.name == "Cube" || collision.gameObject.name == "TT_Table_1" || collision.gameObject.name == "TT_Table_2"){
        //     Debug.Log("Hit"); // ログを表示する
        //     OnCollision = true;
        // }
    }
}
