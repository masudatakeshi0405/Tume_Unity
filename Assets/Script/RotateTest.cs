using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTest : MonoBehaviour {
    public Vector3 axis = Vector3.up;

    public float rotSpeed = 0;
    public float power = 0.1f;
    public float friction = 0.98f;

    public float stickPower = 10;

    public float timer = 0;

    public bool isMove  = true;
    public bool isMove2  = true;
    public bool isMove3  = true;
    public bool isMove4  = true;

    Vector3 prevPos;
    
    Vector3 rotVec = Vector3.zero;  // 回転ベクトル

    Vector3 rotKeyVec = Vector3.zero;

    Vector3 rotat = Vector3.zero;

    // Vector3 pos2 = transform.position;

    float startTime;

    MeshRenderer mr;

    void Start()
    {
        Vector3 pos = transform.position;
        // this.transform.localScale = new Vector3(5, 5, 5);
        // mr = GetComponent<MeshRenderer>();
        // mr.material.color = mr.material.color - new Color32(0,0,0,60);
    }

    // Update is called once per frame
    void Update () {

        // if(this.transform.localScale.x <= 3.0){
        //     this.transform.localScale = new Vector3(10, 10, 10);
        //     timer = 0;
        // }else{
        //     this.transform.localScale = new Vector3(10 - timer * 0.1f, 10 - timer * 0.1f, 10 - timer * 0.1f);
        // }
        if (isMove == false) //上回転
        {            
            // ButtonTrue();
            // isMove = false;
            //rotat.x = 0.9f;
            rotat.y = -0.9f;
            rotat = rotat.normalized * stickPower;
            AddRotateVector(rotat);
        }

        if (isMove2 == false) //下回転
        {
            // ButtonTrue();
            // isMove2 = false;
            //rotat.x = 0.9f;
            rotat.y = 0.9f;
            rotat = rotat.normalized * stickPower;
            AddRotateVector(rotat);
        }

        if (isMove3 == false) //逆横
        {
            // ButtonTrue();
            // isMove3 = false;
            rotat.x = 0.9f;
            //rotat.y = 0.9f;
            rotat = rotat.normalized * stickPower;
            AddRotateVector(rotat);
        }

        if (isMove4 == false) //順横
        {
            // ButtonTrue();
            // isMove4 = false;
            rotat.x = -0.9f;
            //rotat.y = 0.9f;
            rotat = rotat.normalized * stickPower;
            AddRotateVector(rotat);
        }


        // マウスフリックで操作
        if (Input.GetMouseButtonDown(0))
        {
            prevPos = Input.mousePosition;
            startTime = Time.realtimeSinceStartup;
            ButtonTrue();
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector3 pos = Input.mousePosition;
            Vector3 diff = pos - prevPos;
            AddRotateVector(diff * 0.1f / (Time.realtimeSinceStartup - startTime));
            // if(timer>100)
            // {
            //     isMove = true;
            // }
        }

        // 十字キーで操作
        rotKeyVec.x = Input.GetAxis("Horizontal");
        rotKeyVec.y = Input.GetAxis("Vertical");
        rotKeyVec = rotKeyVec.normalized * stickPower;

        AddRotateVector(rotKeyVec);

        // 摩擦
        rotVec *= friction;
        rotSpeed = rotVec.magnitude;

        // transform.position.x = 1.0f; 
        transform.rotation = Quaternion.AngleAxis(rotSpeed * Time.deltaTime, axis) * transform.rotation;

        timer++;
    }

    /// <summary>
    /// 回転ベクトル加算
    /// </summary>
    /// <param name="vec"></param>
    private void AddRotateVector(Vector3 vec)
    {
        Vector3 diff2 = transform.position - Camera.main.transform.position;
        diff2.Normalize();

        float p = vec.magnitude * power * 20;    // 回転速度計算

        vec.Normalize();
        
        rotVec += vec * p;  // 足す

        rotSpeed = rotVec.magnitude;    // 新しい回転速度計算
        axis = Vector3.Cross(rotVec.normalized, diff2); // 新しい回転軸計算
    }

    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos, pos + axis * 10f);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pos, pos + rotVec * 10f);

    }

    private void ButtonTrue()
    {
        isMove = true;
        isMove2 = true;
        isMove3 = true;
        isMove4 = true;
    }
}
