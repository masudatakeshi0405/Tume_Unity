using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScript : MonoBehaviour
{

    public bool isMove5  = true;
    public bool isMove6  = true;

    // Start is called before the first frame update
    void Start()
    {
        // MeshRenderer mr;
    }

    // Update is called once per frame
    void Update()
    {
        Transform pos = this.transform;
        Vector3 pos2 = pos.position;
        if(isMove5 == false){
            pos2.x += -20.0f;
            pos.position = pos2;
            isMove5 = true;
        }

        if(isMove6 == false){
            pos2.x += 20.0f;
            pos.position = pos2;
            isMove6 = true;
        }
    }
}
